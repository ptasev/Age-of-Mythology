using AoMEngineLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AoMEngineLibrary.Data.XmbFile;

public static class XmbFile
{
    // spells l33t in ASCII
    private static ReadOnlySpan<byte> CompressedTag => new byte[] {0x6C, 0x33, 0x33, 0x74};
    private const string XmbTag = "X1";
    private const string RootTag = "XR";
    private const string NodeTag = "XN";

    public static XmlWriterSettings XmlWriterSettings => new XmlWriterSettings()
    {
        Indent = true,
        IndentChars = "\t",
        Encoding = new UTF8Encoding(false)
    };

    public static XDocument Load(Stream stream)
    {
        var input = DecompressIfNecessary(stream, out var createdNewStream);
        using (var br = new BinaryReader(input, Encoding.UTF8, !createdNewStream))
        using (var reader = new ChunkReader(br))
        {
            reader.ReadExpectedTag(XmbTag);
            var version = reader.ReadTaggedUInt32(RootTag);

            string[]? nodeNames = null;
            if (version >= 2)
            {
                var numNames = reader.ReadInt32();
                nodeNames = new string[numNames];
                for (var i = 0; i < numNames; ++i)
                {
                    var name = reader.ReadUnicodeString();
                    nodeNames[i] = name;
                }
            }

            string[]? attrNames = null;
            if (version >= 3)
            {
                var numNames = reader.ReadInt32();
                attrNames = new string[numNames];
                for (var i = 0; i < numNames; ++i)
                {
                    var name = reader.ReadUnicodeString();
                    attrNames[i] = name;
                }
            }

            var xml = new XDocument(new XDeclaration("1.0", null, null));
            var root = ReadNode(reader, version, nodeNames, attrNames);
            xml.Add(root);

            // Validate read
            reader.ValidateChunkRead(XmbTag);

            return xml;
        }

        static Stream DecompressIfNecessary(Stream input, out bool createdNewStream)
        {
            Span<byte> tag = stackalloc byte[4];
            input.ReadExactly(tag);

            if (tag.SequenceEqual(CompressedTag))
            {
                // This is a compressed xmb, decompress first
                createdNewStream = true;

                // Next 4 bytes is original file length
                input.ReadExactly(tag);
                var originalFileLength = BitConverter.ToInt32(tag);

                var output = new MemoryStream(originalFileLength);
                using (var ds = new ZLibStream(input, CompressionMode.Decompress, true))
                {
                    ds.CopyTo(output);
                }

                output.Seek(0, SeekOrigin.Begin);
                return output;
            }
            else
            {
                input.Seek(-tag.Length, SeekOrigin.Current);
                createdNewStream = false;
                return input;
            }
        }
    }

    private static XElement ReadNode(ChunkReader reader, uint version,
        string[]? nodeNames, string[]? attrNames)
    {
        reader.ReadExpectedTag(NodeTag);

        var valueText = reader.ReadUnicodeString();

        // Read node text value
        string name;
        if (version == 0)
        {
            name = reader.ReadUnicodeString();
        }
        else
        {
            if (nodeNames is null)
                throw new FileFormatException("File is missing node name data.");

            var nameId = reader.ReadInt32();
            name = nodeNames[nameId];
        }

        var node = new XElement(name) {Value = valueText};

        if (version == 0)
        {
            // unknown data
            reader.ReadInt32();
        }

        if (version >= 8)
        {
            var lineNumber = reader.ReadInt32();
            node.AddAnnotation(new XmlLineInfoAnnotation(lineNumber));
        }

        ReadAttributes(reader, version, attrNames, node);

        // Read child nodes
        var childCount = reader.ReadInt32();
        for (var i = 0; i < childCount; ++i)
        {
            var childNode = ReadNode(reader, version, nodeNames, attrNames);
            node.Add(childNode);
        }

        reader.ValidateChunkRead(NodeTag);

        return node;

        static void ReadAttributes(ChunkReader reader, uint version, string[]? attrNames,
            XElement node)
        {
            var attributeCount = reader.ReadInt32();
            for (var i = 0; i < attributeCount; ++i)
            {
                string name;
                if (version == 0)
                {
                    name = reader.ReadUnicodeString();
                }
                else
                {
                    if (attrNames is null)
                        throw new FileFormatException("File is missing attribute name data.");

                    var nameId = reader.ReadInt32();
                    name = attrNames[nameId];
                }

                var valueText = reader.ReadUnicodeString();

                node.SetAttributeValue(name, valueText);
            }
        }
    }

    /// <summary>
    /// Saves the text xml from the input stream to the output stream in XMB format.
    /// </summary>
    public static void Save(Stream input, Stream output, CompressionLevel compressionLevel)
    {
        var xml = XDocument.Load(input, LoadOptions.SetLineInfo);
        Save(xml, output, compressionLevel);
    }
    public static void Save(XDocument xml, Stream output, CompressionLevel compressionLevel)
    {
        const uint version = 8;
        var compress = compressionLevel != CompressionLevel.NoCompression;
        using (var bw = new BinaryWriter(compress ? new MemoryStream() : output, Encoding.UTF8, !compress))
        using (var writer = new ChunkWriter(bw))
        {
            var startPosition = bw.BaseStream.Position;
            writer.WriteTagPostSized(XmbTag, out var fileSizePosition);
            writer.WriteTagged(RootTag, version);

            // Generate node and attribute name maps
            var nodeNameMap = new Dictionary<string, int>();
            var attrNameMap = new Dictionary<string, int>();
            if (xml.Root is not null)
            {
                GenerateNameMaps(xml.Root, nodeNameMap, attrNameMap);

                var nodeNames = nodeNameMap.OrderBy(x => x.Value).Select(x => x.Key);
                writer.Write(nodeNameMap.Count);
                foreach (var name in nodeNames)
                {
                    writer.WriteUnicodeString(name);
                }

                var attrNames = attrNameMap.OrderBy(x => x.Value).Select(x => x.Key);
                writer.Write(attrNameMap.Count);
                foreach (var name in attrNames)
                {
                    writer.WriteUnicodeString(name);
                }

                WriteNode(writer, xml.Root, nodeNameMap, attrNameMap);
            }

            writer.WriteSize(XmbTag, fileSizePosition);
            bw.Flush();
            var endPosition = bw.BaseStream.Position;

            if (compress)
            {
                output.Write(CompressedTag);
                Span<byte> sizeBytes = stackalloc byte[sizeof(int)];
                var fileSize = Convert.ToInt32(endPosition - startPosition);
                BitConverter.TryWriteBytes(sizeBytes, fileSize);
                output.Write(sizeBytes);

                using (var zs = new ZLibStream(output, compressionLevel, true))
                {
                    bw.BaseStream.Seek(0, SeekOrigin.Begin);
                    bw.BaseStream.CopyTo(zs);
                }
            }
        }

        static void GenerateNameMaps(XElement element, Dictionary<string, int> nodeNameMap,
            Dictionary<string, int> attrNameMap)
        {
            nodeNameMap.TryAdd(element.Name.LocalName, nodeNameMap.Count);

            foreach (var attr in element.Attributes())
            {
                attrNameMap.TryAdd(attr.Name.LocalName, attrNameMap.Count);
            }

            foreach (var childElement in element.Elements())
            {
                GenerateNameMaps(childElement, nodeNameMap, attrNameMap);
            }
        }
    }

    private static void WriteNode(ChunkWriter writer, XElement node, Dictionary<string, int> nodeNameMap,
        Dictionary<string, int> attrNameMap)
    {
        writer.WriteTagPostSized(NodeTag, out var nodeSizePosition);
        writer.WriteUnicodeString(string.Concat(node.Nodes().OfType<XText>().Select(x => x.Value)));

        writer.Write(nodeNameMap[node.Name.LocalName]);
        writer.Write(GetXElementLineNumber(node));

        // Write attributes
        writer.Write(node.Attributes().Count());
        foreach (var attr in node.Attributes())
        {
            writer.Write(attrNameMap[attr.Name.LocalName]);
            writer.WriteUnicodeString(attr.Value);
        }

        // Write child nodes
        writer.Write(node.Elements().Count());
        foreach (var childElement in node.Elements())
        {
            WriteNode(writer, childElement, nodeNameMap, attrNameMap);
        }

        writer.WriteSize(NodeTag, nodeSizePosition);
    }

    private static int GetXElementLineNumber(XElement element)
    {
        var li = (IXmlLineInfo)element;
        return li.HasLineInfo() ? li.LineNumber : element.Annotation<XmlLineInfoAnnotation>()?.LineNumber ?? 1;
    }

    private record XmlLineInfoAnnotation(int LineNumber);
}
