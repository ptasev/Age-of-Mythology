using BrightIdeasSoftware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AoMProtoEditor
{
    // Set to store values as strings all the time, regardless of actual format
    // Format only matters when making changes in the front end
    public class ProtoFile
    {
        public string Name;

        public readonly StringTable ElementString;
        public readonly StringTable AttributeString;
        public readonly ProtoEditor Editor;

        public readonly ProtoSchema Schema;
        public readonly ProtoElement RootProperty;

        public ProtoFile(FileStream schema, FileStream stream)
        {
            Name = stream.Name;
            ElementString = new StringTable(StringComparer.Ordinal);
            AttributeString = new StringTable(StringComparer.Ordinal);
            Editor = new ProtoEditor();
            Schema = new ProtoSchema(schema, this);

            XmlDocument reader = new XmlDocument();
            using (XmlReader xReader = XmlReader.Create(stream, new XmlReaderSettings { IgnoreComments = true }))
            {
                reader.Load(xReader);

                if (reader.HasChildNodes)
                {
                    //XmlDeclaration xmlDeclaration = reader.FirstChild as XmlDeclaration;
                    //this.Schema.FileWriteSettings.Encoding = Encoding.GetEncoding(xmlDeclaration.Encoding);
                }

                RootProperty = new ProtoElement(reader.DocumentElement, this, null);

                reader = null;
            }
            
            //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        }
        public void Write(FileStream stream)
        {
            XmlDocument writer = new XmlDocument();
            using (XmlWriter xWriter = XmlWriter.Create(stream, Schema.FileWriteSettings))
            {
                xWriter.WriteStartDocument();
                //writer.AppendChild(writer.CreateXmlDeclaration("1.0", "UTF-8", null));

                RootProperty.Write(xWriter);
                //RootProperty.Write(writer, null);

                //writer.Save(xWriter);

                writer = null;
            }
        }

        public ProtoFormat GetProtoFormat(List<int> path, int attrNameID = -1)
        {
            ProtoFormat fMat;
            foreach (KeyValuePair<string, ProtoFormat> pair in this.Schema.Format)
            {
                fMat = pair.Value;
                if ((fMat.Absolute && path.Count != fMat.PropertyPath.Count)
                    || fMat.PropertyPath.Count == 0 || fMat.AttributeNameID != attrNameID)
                    continue;
                int j = 0, k;
                for (k = 0; k < path.Count; k++)
                {
                    if (j >= fMat.PropertyPath.Count || (fMat.Absolute && fMat.PropertyPath[j] != path[k]))
                        break;
                    if (fMat.PropertyPath[j] == path[k])
                    {
                        j++;
                    }
                }
                if (k >= path.Count && j >= fMat.PropertyPath.Count)
                {
                    fMat.PropertyPath = path;
                    fMat.Absolute = true;
                    return fMat;
                }
            }

            return null;
        }
    }

    public class ProtoSchema
    {
        public readonly ProtoFile File;
        public readonly XmlWriterSettings FileWriteSettings;
        public readonly bool FullEndTag;
        public bool WriteFormatValues;

        public readonly Dictionary<string, ProtoFormat> Format;
        public ProtoElementDefinition RootDefinition;

        public ProtoSchema(FileStream schema, ProtoFile _file)
        {
            this.File = _file;
            Format = new Dictionary<string, ProtoFormat>(StringComparer.Ordinal);
            FileWriteSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineHandling = NewLineHandling.Replace,
                NewLineChars = "\r\n",
                CloseOutput = true//,
                //Encoding = new UTF8Encoding(false)
            };

            XmlDocument reader = new XmlDocument();
            using (XmlReader xReader = XmlReader.Create(schema, new XmlReaderSettings { IgnoreComments = true, CloseInput = true }))
            {
                reader.Load(xReader);

                if (reader.DocumentElement.HasAttribute("indentChars"))
                    FileWriteSettings.IndentChars = reader.DocumentElement.GetAttribute("indentChars");
                if (reader.DocumentElement.HasAttribute("newLineChars"))
                    FileWriteSettings.NewLineChars = reader.DocumentElement.GetAttribute("newLineChars");
                if (reader.DocumentElement.HasAttribute("encoding"))
                    FileWriteSettings.Encoding = Encoding.GetEncoding(reader.DocumentElement.GetAttribute("encoding"));
                FullEndTag = reader.DocumentElement.HasAttribute("fullEndTag");

                ProtoFormat pFormat;
                foreach (XmlElement element in reader.DocumentElement)
                {
                    if (element.Name == "formatList")
                    {
                        WriteFormatValues = element.HasAttribute("writeFormatValues");
                        foreach (XmlElement format in element.ChildNodes)
                        {
                            pFormat = new ProtoFormat(format, File);
                            Format.Add(pFormat.Name, pFormat);
                        }
                    }
                    else if (element.Name == "element")
                    {
                        RootDefinition = new ProtoElementDefinition(element, File);
                    }
                }

                reader = null;
            }
        }

        public void Write(FileStream stream)
        {
            XmlWriterSettings schemaWriteSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineHandling = NewLineHandling.Replace,
                NewLineChars = "\r\n",
                CloseOutput = true
            };
            using (XmlWriter writer = XmlWriter.Create(stream, schemaWriteSettings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("schema");
                writer.WriteAttributeString("version", DateTime.Now.ToString("yyyy.MMdd"));
                writer.WriteAttributeString("indentChars", FileWriteSettings.IndentChars);
                writer.WriteAttributeString("newLineChars", FileWriteSettings.NewLineChars);
                writer.WriteAttributeString("encoding", FileWriteSettings.Encoding.WebName);
                if (FullEndTag)
                    writer.WriteAttributeString("fullEndTag", null);

                writer.WriteStartElement("formatList");
                foreach (KeyValuePair<string, ProtoFormat> pair in Format)
                {
                    pair.Value.Write(writer);
                }
                writer.WriteFullEndElement();

                RootDefinition.Write(writer);

                writer.WriteFullEndElement();
            }
        }
        public void WriteFull(FileStream stream)
        {
            // Save settings
            bool temp = WriteFormatValues;

            // Write with new settings
            WriteFormatValues = true;
            Write(stream);

            // Restore settings
            WriteFormatValues = temp;
        }
    }

    public abstract class ProtoProperty
    {
        public ProtoFile File;
        public ProtoElement ParentProperty
        {
            get;
            protected set;
        }
        public ProtoDefinition Definition
        {
            get { return GetDefinition(); }
        }
        protected abstract ProtoDefinition GetDefinition();

        public int NameID
        {
            get;
            protected set;
        }
        public abstract string Name
        {
            get;
        }
        public object Value;
        public string FormattedValue
        {
            get
            {
                return ProtoType.ToString(Value, Definition.DefaultFormat);
            }
        }

        public void UpdateParentProperty(ProtoElement parent)
        {
            this.ParentProperty = parent;
        }
    }

    public class ProtoElement : ProtoProperty, IEnumerable
    {
        public new ProtoElementDefinition Definition
        {
            get
            {
                return (ProtoElementDefinition)GetDefinition();
            }
        }
        protected override ProtoDefinition GetDefinition()
        {
            if (ParentProperty == null)
                return File.Schema.RootDefinition;
            else
                return ParentProperty.Definition[NameID];
        }
        public bool AllowTab
        {
            get { return Definition.ChildElement.Count > 0; }
        }

        public List<ProtoAttribute> Attribute;
        public List<ProtoElement> ChildElement;
        public List<int> Path
        {
            get
            {
                List<int> path;

                if (ParentProperty == null)
                    path = new List<int>();
                else
                    path = ParentProperty.Path;

                path.Add(NameID);
                return path;
            }
        }

        public override string Name
        {
            get { return File.ElementString.GetBySecond(NameID); }
        }
        public string DisplayName
        {
            get
            {
                return getDisplayName(1, true);
            }
        }
        public string DisplayAttribute
        {
            get
            {
                return getDisplayAttribute(true);
            }
        }
        private string getDisplayName(int backwardCount, bool withAttribute)
        {
            string ret = string.Empty;

            if (ParentProperty != null && backwardCount > 0)
            {
                ret += ParentProperty.getDisplayName(--backwardCount, false) + ".";
            }

            ret += Name;
            if (withAttribute)
                ret += getDisplayAttribute(false);

            return ret;
        }
        private string getDisplayAttribute(bool longVersion)
        {
            if (Attribute.Count > 0)
            {
                string[] attStr = new string[Attribute.Count];
                for (int i = 0; i < Attribute.Count; i++)
                {
                    attStr[i] = (longVersion ? Attribute[i].Name + "=" : string.Empty) + Attribute[i].FormattedValue;// ProtoType.ToString(Attribute[i].Value, Attribute[i].Definition.DefaultFormat);
                }
                return "(" + string.Join(", ", attStr) + ")";
            }

            return string.Empty;
        }

        public ProtoElement(XmlElement property, ProtoFile _file, ProtoElement parentProperty)
        {
            File = _file;
            ParentProperty = parentProperty;
            File.ElementString.TryAdd(property.Name, File.ElementString.Count);
            NameID = File.ElementString.GetByFirst(property.Name);

            if (File.Schema.RootDefinition == null || (ParentProperty == null && File.Schema.RootDefinition.NameID != NameID))
            {
                File.Schema.RootDefinition = new ProtoElementDefinition(File, NameID, ProtoType.InferType(property.InnerText));
            }
            else if (ParentProperty != null && !ParentProperty.Definition.HasProperty(NameID))
            {
                ProtoElementDefinition def = new ProtoElementDefinition(File, NameID, ProtoType.InferType(property.InnerText));
                ParentProperty.Definition.ChildElement.Add(def.NameID, def);
            }

            Attribute = new List<ProtoAttribute>(property.Attributes.Count);
            foreach (XmlAttribute attribute in property.Attributes)
            {
                Attribute.Add(new ProtoAttribute(attribute, File, this));
            }

            ChildElement = new List<ProtoElement>(property.ChildNodes.Count);
            foreach (XmlNode childProperty in property.ChildNodes)
            {
                if (childProperty is XmlElement)
                    ChildElement.Add(new ProtoElement((XmlElement)childProperty, File, this));
            }

            if (this.ChildElement.Count > 0 || string.CompareOrdinal(this.Definition.DefaultFormat, "VOID") == 0)
            {
                this.Value = string.Empty;
            }
            else
            {
                ProtoType.TryParse(property.InnerText, this.Definition, out this.Value, true);//.DefaultFormat);
                fillProtoFormat();
            }
        }
        public ProtoElement(ProtoFile _file, ProtoElement _parentProperty, int _nameID, object _value)
        {
            this.File = _file;
            this.ParentProperty = _parentProperty;
            this.NameID = _nameID;

            Attribute = new List<ProtoAttribute>(Definition.Attribute.Count);
            ChildElement = new List<ProtoElement>(Definition.ChildElement.Count);

            // Maybe include fillProtoFormat() here
            this.Value = _value;
        }
        public ProtoElement(ProtoElement elem)
        {
            this.File = elem.File;
            this.ParentProperty = elem.ParentProperty;
            this.NameID = elem.NameID;

            Attribute = new List<ProtoAttribute>(elem.Attribute.Count);
            ProtoProperty newProp;
            foreach (ProtoAttribute attr in elem.Attribute)
            {
                newProp = new ProtoAttribute(attr);
                Attribute.Add((ProtoAttribute)newProp);
                newProp.UpdateParentProperty(this);
            }
            ChildElement = new List<ProtoElement>(elem.ChildElement.Count);
            foreach (ProtoElement childElem in elem.ChildElement)
            {
                newProp = new ProtoElement(childElem);
                ChildElement.Add((ProtoElement)newProp);
                newProp.UpdateParentProperty(this);
            }

            this.Value = elem.Value;
        }
        private void fillProtoFormat()
        {
            ProtoFormat fMat;
            if (!File.Schema.Format.TryGetValue(Definition.DefaultFormat, out fMat))
                fMat = File.GetProtoFormat(Path, -1);
            //else if (fMat.PropertyPath.Count == 0)
             //   return; // if its predefined, don't add values to it
            if (fMat != null)
            {
                fMat.Add(Value as string);
                if (Definition.UpdateFormat)
                {
                    Definition.DefaultFormat = fMat.Name;
                    Definition.UpdateFormat = false;
                }
            }
        }
        public void Write(XmlDocument writer, XmlElement element)
        {
            if (element == null)
            {
                writer.AppendChild(writer.CreateElement(Name));
                element = (XmlElement)writer.LastChild;
            }
            else
            {
                element.AppendChild(writer.CreateElement(Name));
                element = (XmlElement)element.LastChild;
            }

            foreach (ProtoAttribute attribute in Attribute)
            {
                attribute.Write(writer, element as XmlElement);
            }

            if (ChildElement.Count > 0)
            {
                foreach (ProtoElement childProperty in ChildElement)
                {
                    childProperty.Write(writer, element as XmlElement);
                }
            }
            else
            {
                string format = ProtoType.ToString(Value, Definition.DefaultFormat);
                if (!string.IsNullOrEmpty(format))
                    element.InnerText = format;
            }
        }
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement(Name);

            foreach (ProtoAttribute attribute in Attribute)
            {
                writer.WriteAttributeString(attribute.Name, attribute.FormattedValue);//ProtoType.ToString(attribute.Value, attribute.Definition.DefaultFormat));
            }

            if (ChildElement.Count > 0)
            {
                foreach (ProtoElement childProperty in ChildElement)
                {
                    childProperty.Write(writer);
                }
            }
            else
            {
                //string format = ProtoType.ToString(Value, Definition.DefaultFormat);
                writer.WriteString(FormattedValue);
            }

            if (File.Schema.FullEndTag)
                writer.WriteFullEndElement();
            else
                writer.WriteEndElement();
        }

        public ProtoElement this[int index]
        {
            get
            {
                return ChildElement[index];
            }
        }
        public IEnumerator<ProtoElement> GetEnumerator()
        {
            foreach (ProtoElement prop in ChildElement)
            {
                yield return prop;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ProtoAttribute GetAttribute(string attrName)
        {
            foreach (ProtoAttribute attrib in Attribute)
            {
                if (attrib.Name == attrName)
                    return attrib;
            }

            return null;
        }
        public ProtoAttribute GetAttribute(int index)
        {
            if (index < Attribute.Count && index >= 0)
                return Attribute[index];

            return null;
        }
        public IEnumerable<ProtoAttribute> Attributes
        {
            get
            {
                foreach (ProtoAttribute attr in Attribute)
                {
                    yield return attr;
                }
            }
        }
    }

    public class ProtoAttribute : ProtoProperty
    {
        public new ProtoAttributeDefinition Definition
        {
            get
            {
                return (ProtoAttributeDefinition)GetDefinition();
            }
        }
        protected override ProtoDefinition GetDefinition()
        {
            return ParentProperty.Definition.GetAttribute(NameID);
        }
        public override string Name
        {
            get { return File.AttributeString.GetBySecond(NameID); }
        }

        public ProtoAttribute(XmlAttribute attribute, ProtoFile _file, ProtoElement _parentProperty)
        {
            File = _file;
            ParentProperty = _parentProperty;
            File.AttributeString.TryAdd(attribute.Name, File.AttributeString.Count);
            NameID = File.AttributeString.GetByFirst(attribute.Name);

            ProtoAttributeDefinition def;
            if (!ParentProperty.Definition.Predefined && !ParentProperty.Definition.HasAttribute(NameID))
            {
                def = new ProtoAttributeDefinition(File, NameID, ProtoType.InferType(attribute.InnerText));
                ParentProperty.Definition.Attribute.Add(def.NameID, def);
            }
            ProtoType.TryParse(attribute.InnerText, Definition, out Value, true);//.DefaultFormat);
            FillProtoFormat();
        }
        public ProtoAttribute(ProtoFile _file, ProtoElement _parentProperty, int _nameID, object _value)
        {
            this.File = _file;
            this.ParentProperty = _parentProperty;
            this.NameID = _nameID;

            // Maybe include fillProtoFormat() here
            this.Value = _value;
        }
        public ProtoAttribute(ProtoAttribute attr)
        {
            this.File = attr.File;
            this.ParentProperty = attr.ParentProperty;
            this.NameID = attr.NameID;
            this.Value = attr.Value;
        }
        public void Write(XmlDocument writer, XmlElement element)
        {
            element.Attributes.Append(writer.CreateAttribute(Name));
            element.Attributes[Name].Value = ProtoType.ToString(Value, Definition.DefaultFormat);
        }

        private void FillProtoFormat()
        {
            ProtoFormat fMat;
            if (!File.Schema.Format.TryGetValue(Definition.DefaultFormat, out fMat))
                fMat = File.GetProtoFormat(ParentProperty.Path, NameID);
            if (fMat != null)
            {
                fMat.Add(Value as string);
                if (Definition.UpdateFormat)
                {
                    Definition.DefaultFormat = fMat.Name;
                    Definition.UpdateFormat = false;
                }
            }
        }
    }

    public class ProtoFormat
    {
        public ProtoFile File;
        public string Name;
        public bool Absolute
        {
            get;
            internal set;
        }

        public List<int> PropertyPath;
        public int AttributeNameID;

        public HashSet<string> Value;
        public HashSet<string> Link;

        public ProtoFormat(XmlElement format, ProtoFile _file)
        {
            File = _file;
            Name = format.GetAttribute("name");
            Absolute = format.HasAttribute("absolute");

            PropertyPath = new List<int>(3);
            string pName = format.GetAttribute("element");
            if (string.IsNullOrEmpty(pName))
            {
                AttributeNameID = -1;
            }
            else
            {
                string[] pNames = pName.Split('.');
                for (int i = 0; i < pNames.Length; i++)
                {
                    if (!string.IsNullOrEmpty(pNames[i]))
                    {
                        File.ElementString.TryAdd(pNames[i], File.ElementString.Count);
                        PropertyPath.Add(File.ElementString.GetByFirst(pNames[i]));
                    }
                }
                pName = format.GetAttribute("attribute");
                if (string.IsNullOrEmpty(pName))
                {
                    AttributeNameID = -1;
                }
                else
                {
                    File.AttributeString.TryAdd(pName, File.AttributeString.Count);
                    AttributeNameID = File.AttributeString.GetByFirst(pName);
                }
            }

            Value = new HashSet<string>(StringComparer.Ordinal);
            Link = new HashSet<string>(StringComparer.Ordinal);
            foreach (XmlElement elem in format.ChildNodes)
            {
                if (elem.Name == "value")
                    Value.Add(elem.InnerText);
                else if (elem.Name == "link")
                    Link.Add(elem.InnerText);
            }
        }
        public void Write(XmlDocument writer, XmlElement elem, bool writeFormatValues)
        {
            elem.AppendChild(writer.CreateElement("format"));

            elem.LastChild.Attributes.Append(writer.CreateAttribute("name"));
            elem.LastChild.Attributes["name"].Value = Name;
            if (PropertyPath.Count > 0)
            {
                string[] pathString = new string[PropertyPath.Count];
                for (int i = 0; i < PropertyPath.Count; i++)
                {
                    pathString[i] = File.ElementString.GetBySecond(PropertyPath[i]);
                }
                elem.LastChild.Attributes.Append(writer.CreateAttribute("element"));
                elem.LastChild.Attributes["element"].Value = string.Join(".", pathString);
            }
            if (AttributeNameID != -1)
            {
                elem.LastChild.Attributes.Append(writer.CreateAttribute("attribute"));
                elem.LastChild.Attributes["attribute"].Value = File.AttributeString.GetBySecond(AttributeNameID);
            }
            if (Absolute)
                elem.LastChild.Attributes.Append(writer.CreateAttribute("absolute"));

            if (writeFormatValues || PropertyPath.Count == 0)
            {
                foreach (string s in Value)
                {
                    elem.LastChild.AppendChild(writer.CreateElement("value"));
                    elem.LastChild.LastChild.InnerText = s;
                }
            }

            foreach (string s in Link)
            {
                elem.LastChild.AppendChild(writer.CreateElement("link"));
                elem.LastChild.LastChild.InnerText = s;
            }
        }
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("format");

            writer.WriteAttributeString("name", Name);
            if (PropertyPath.Count > 0)
            {
                string[] pathString = new string[PropertyPath.Count];
                for (int i = 0; i < PropertyPath.Count; i++)
                {
                    pathString[i] = File.ElementString.GetBySecond(PropertyPath[i]);
                }
                writer.WriteAttributeString("element", string.Join(".", pathString));
            }
            if (AttributeNameID != -1)
            {
                writer.WriteAttributeString("attribute", File.AttributeString.GetBySecond(AttributeNameID));
            }
            if (Absolute)
                writer.WriteAttributeString("absolute", null);

            if (File.Schema.WriteFormatValues || PropertyPath.Count == 0)
            {
                foreach (string s in Value)
                {
                    writer.WriteStartElement("value");
                    writer.WriteString(s);
                    writer.WriteFullEndElement();
                }
            }

            foreach (string s in Link)
            {
                writer.WriteStartElement("link");
                writer.WriteString(s);
                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();
        }

        public void Add(string value)
        {
            if (!Value.Contains(value))
            {
                Value.Add(value);
            }
        }
    }

    public class ProtoDefinition
    {
        public readonly ProtoFile File;
        public readonly int NameID;
        public string Name
        {
            get { return stringTable.GetBySecond(NameID); }
        }
        public string DefaultFormat
        {
            get
            {
                if (!string.IsNullOrEmpty(format))
                    return format;
                else
                    return ProtoType.DefaultFormat;
            }
            internal set
            {
                format = value;
                //UpdateFormat = false;
            }
        }
        protected internal bool UpdateFormat
        {
            get;
            set;
        }

        protected string format;
        protected readonly StringTable stringTable;

        protected ProtoDefinition(XmlElement definition, ProtoFile _file, StringTable sTable)
        {
            this.File = _file;
            this.stringTable = sTable;
            string eName = definition.GetAttribute("name");
            sTable.TryAdd(eName, sTable.Count);
            this.NameID = sTable.GetByFirst(eName);

            this.format = definition.GetAttribute("format");
            if (string.IsNullOrEmpty(this.format))
            {
                this.format = ProtoType.DefaultFormat;
                this.UpdateFormat = true; // Implement XML attribute for this
            }
            else
            {
                this.UpdateFormat = false;
            }
        }
        protected ProtoDefinition(ProtoFile _file, StringTable sTable, int nameID, string _format, bool updateFormat = true)
        {
            this.File = _file;
            this.stringTable = sTable;
            this.NameID = nameID;

            this.format = _format;
            this.UpdateFormat = updateFormat;
        }

        protected void Write(XmlDocument writer, XmlElement elem, bool isAttribute)
        {
            elem.AppendChild(writer.CreateElement(isAttribute ? "attribute" : "element"));
            elem.LastChild.Attributes.Append(writer.CreateAttribute("name"));
            elem.LastChild.Attributes["name"].Value = stringTable.GetBySecond(NameID);
            elem.LastChild.Attributes.Append(writer.CreateAttribute("format"));
            elem.LastChild.Attributes["format"].Value = DefaultFormat;
        }
        protected void Write(XmlWriter writer, bool isAttribute)
        {
            writer.WriteStartElement(isAttribute ? "attribute" : "element");
            writer.WriteAttributeString("name", stringTable.GetBySecond(NameID));
            writer.WriteAttributeString("format", DefaultFormat);
        }
    }

    public class ProtoElementDefinition : ProtoDefinition, IEnumerable
    {
        public bool Predefined
        {
            get { return predefined; }
        }
        public Dictionary<int, ProtoAttributeDefinition> Attribute;
        public Dictionary<int, ProtoElementDefinition> ChildElement;

        private readonly bool predefined;

        public ProtoElementDefinition(XmlElement definition, ProtoFile _file)
            : base(definition, _file, _file.ElementString)
        {
            predefined = definition.HasAttribute("predefined");

            Attribute = new Dictionary<int, ProtoAttributeDefinition>();
            ChildElement = new Dictionary<int, ProtoElementDefinition>();
            ProtoElementDefinition def;
            ProtoAttributeDefinition def2;
            foreach (XmlElement element in definition.ChildNodes)
            {
                if (element.Name == "element")
                {
                    def = new ProtoElementDefinition(element, File);
                    ChildElement.Add(def.NameID, def);
                }
                else if (element.Name == "attribute")
                {
                    def2 = new ProtoAttributeDefinition(element, File);
                    Attribute.Add(def2.NameID, def2);
                }
            }
        }
        public ProtoElementDefinition(ProtoFile _file, int nameID, string _format, bool updateFormat = true)
            : base(_file, _file.ElementString, nameID, _format, updateFormat)
        {
            predefined = false;

            Attribute = new Dictionary<int, ProtoAttributeDefinition>();
            ChildElement = new Dictionary<int, ProtoElementDefinition>();
        }

        public ProtoElementDefinition this[int nameID]
        {
            get
            {
                return ChildElement[nameID];
            }
        }
        public IEnumerator<ProtoElementDefinition> GetEnumerator()
        {
            foreach (KeyValuePair<int, ProtoElementDefinition> prop in ChildElement)
            {
                yield return prop.Value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public ProtoElementDefinition TryGetProperty(int nameID)
        {
            ProtoElementDefinition def;
            ChildElement.TryGetValue(nameID, out def);

            return def;
        }
        public bool HasProperty(int nameID)
        {
            return ChildElement.ContainsKey(nameID);
        }

        public ProtoAttributeDefinition GetAttribute(int nameID)
        {
            return Attribute[nameID];
        }
        public IEnumerable<ProtoAttributeDefinition> Attributes
        {
            get
            {
                foreach (KeyValuePair<int, ProtoAttributeDefinition> attr in Attribute)
                {
                    yield return attr.Value;
                }
            }
        }
        public ProtoAttributeDefinition TryGetAttribute(int nameID)
        {
            ProtoAttributeDefinition def;
            Attribute.TryGetValue(nameID, out def);

            return def;
        }
        public bool HasAttribute(int name)
        {
            return Attribute.ContainsKey(name);
        }

        public void Write(XmlDocument writer, XmlElement elem)
        {
            base.Write(writer, elem, false);

            foreach (KeyValuePair<int, ProtoAttributeDefinition> pair in Attribute)
            {
                pair.Value.Write(writer, (XmlElement)elem.LastChild);
            }

            foreach (KeyValuePair<int, ProtoElementDefinition> pair in ChildElement)
            {
                pair.Value.Write(writer, (XmlElement)elem.LastChild);
            }
        }
        public void Write(XmlWriter writer)
        {
            base.Write(writer, false);

            foreach (KeyValuePair<int, ProtoAttributeDefinition> pair in Attribute)
            {
                pair.Value.Write(writer);
            }

            IOrderedEnumerable<KeyValuePair<int, ProtoElementDefinition>> ordered = ChildElement.OrderBy(def => def.Value.Name);
            foreach (KeyValuePair<int, ProtoElementDefinition> pair in ordered)
            {
                pair.Value.Write(writer);
            }

            writer.WriteFullEndElement();
        }
    }

    public class ProtoAttributeDefinition : ProtoDefinition
    {
        public ProtoAttributeDefinition(XmlElement attribute, ProtoFile _file)
            : base(attribute, _file, _file.AttributeString)
        {
        }
        public ProtoAttributeDefinition(ProtoFile _file, int nameID, string _format, bool updateFormat = true)
            : base(_file, _file.AttributeString, nameID, _format, updateFormat)
        {
        }

        public void Write(XmlDocument writer, XmlElement elem)
        {
            base.Write(writer, elem, true);

        }
        public void Write(XmlWriter writer)
        {
            base.Write(writer, true);
            writer.WriteEndElement();
        }
    }

    public static class ProtoType
    {
        public const string DefaultFormat = "STRING";

        public static object GetDefaultValue(string type)
        {
            switch (type)
            {
                case "INT":
                    return 0;
                case "FLOAT":
                case "FLOATF2":
                case "FLOATF4":
                    return 0.0F;
                default:
                    return string.Empty;
            }
        }

        public static Type GetType(string type)
        {
            switch (type)
            {
                case "INT":
                    return typeof(int);
                case "FLOAT":
                case "FLOATF4":
                case "FLOATF2":
                    return typeof(float);
                default:
                    return typeof(string);
            }
        }
        public static string InferType(string value)
        {
            int i;
            float f;

            if (string.IsNullOrEmpty(value))
                return "STRING";
            else if (Int32.TryParse(value, out i))
                return "INT";
            else if (Single.TryParse(value, out f))
                return "FLOAT";
            else
                return "STRING";
        }

        public static string GetFormatSpecifier(string type)
        {
            switch (type)
            {
                case "FLOATF2":
                    return "{0:F2}";
                case "FLOATF4":
                    return "{0:F4}";
                case "VOID":
                    return string.Empty;
                default:
                    return "{0}";
            }
        }
        public static string ToString(object value, string type)
        {
            return String.Format(GetFormatSpecifier(type), value);
        }

        [Obsolete]
        public static object Parse(string value, ProtoDefinition definition)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return value;
                else if (string.CompareOrdinal(definition.DefaultFormat, "STRING") == 0)
                {
                    return value;
                }
                else if (string.CompareOrdinal(definition.DefaultFormat, "INT") == 0)
                {
                    return Int32.Parse(value);
                }
                else if (string.CompareOrdinal(definition.DefaultFormat, "FLOAT") == 0
                  || string.CompareOrdinal(definition.DefaultFormat, "FLOATF2") == 0
                  || string.CompareOrdinal(definition.DefaultFormat, "FLOATF4") == 0)
                {
                    return Single.Parse(value);
                }
                else
                {
                    return value;
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Could not convert " + value + " to format " + definition.DefaultFormat
                    + " for definition " + definition.Name + "!" + Environment.NewLine + "The default value for this format has been set instead.");
                return GetDefaultValue(definition.DefaultFormat);
            }
        }
        public static bool TryParse(string value, ProtoDefinition definition, out object ret, bool forceStringReturn)
        {
            if (string.CompareOrdinal(definition.DefaultFormat, "STRING") == 0)
            {
                ret = value;
                return true;
            }

            if (string.CompareOrdinal(definition.DefaultFormat, "INT") == 0)
            {
                int i;
                if (Int32.TryParse(value, out i))
                {
                    ret = i;
                    return true;
                }

                if (definition.UpdateFormat)
                    definition.DefaultFormat = "FLOAT";
                else if (forceStringReturn)
                {
                    ret = value;
                    return false;
                }
                else
                {
                    ret = GetDefaultValue(definition.DefaultFormat);
                    return false;
                }
            }

            if (string.CompareOrdinal(definition.DefaultFormat, "FLOAT") == 0
              || string.CompareOrdinal(definition.DefaultFormat, "FLOATF2") == 0
              || string.CompareOrdinal(definition.DefaultFormat, "FLOATF4") == 0)
            {
                float f;
                if (Single.TryParse(value, out f))
                {
                    ret = f;
                    return true;
                }

                if (definition.UpdateFormat)
                    definition.DefaultFormat = "STRING";
                else if (forceStringReturn)
                {
                    ret = value;
                    return false;
                }
                else
                {
                    ret = GetDefaultValue(definition.DefaultFormat);
                    return false;
                }
            }

            ret = value;
            return true;
        }
        [Obsolete]
        public static object ToObject(string value, string type)
        {
            int i;
            float f;

            try
            {
                switch (type)
                {
                    case "INT":
                        if (Int32.TryParse(value, out i))
                            return i;
                        goto case "FLOAT";
                    case "FLOAT":
                    case "FLOATF4":
                    case "FLOATF2":
                        if (Single.TryParse(value, out f))
                            return f;
                        goto default;
                    default:
                        return value;
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(value + " " + type);
                return GetDefaultValue(type);
            }
        }      
        [Obsolete]
        public static object ToObjectOld(string value, string type)
        {
            object result = "";

            try
            {
                result = Convert.ChangeType(value, ProtoType.GetType(type));
            }
            catch (FormatException)
            {
                result = GetDefaultValue(type);
            }

            return result;
        }
    }
}

/*
    public class ProtoFormat
    {
        public ProtoFile File;
        public string Name;
        public HashSet<string> Value;
        public bool Predefined
        {
            get { return PropertyNameID == -1 && AttributeNameID == -1; }
        }
        public int PropertyNameID;
        public int AttributeNameID;

        public ProtoFormat(XmlElement format, ProtoFile _file)
        {
            File = _file;
            Name = format.GetAttribute("name");

            string pName = format.GetAttribute("element");
            if (string.IsNullOrEmpty(pName))
            {
                PropertyNameID = -1;
                AttributeNameID = -1;
            }
            else
            {
                File.String.TryAdd(pName, File.String.Count);
                PropertyNameID = File.String.GetByFirst(pName);
                pName = format.GetAttribute("attribute");
                if (string.IsNullOrEmpty(pName))
                {
                    AttributeNameID = -1;
                }
                else
                {
                    File.String.TryAdd(pName, File.String.Count);
                    AttributeNameID = File.String.GetByFirst(pName);
                }
            }

            Value = new HashSet<string>();
            foreach (XmlElement value in format.ChildNodes)
            {
                Value.Add(value.InnerText);
            }
        }

        public void Add(string value)
        {
            if (!Value.Contains(value))
            {
                Value.Add(value);
            }
        }
    }
*/