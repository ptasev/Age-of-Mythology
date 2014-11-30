using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AoMTextEditor
{
    public class AnimFile
    {
        public static void ConvertToXml(FileStream animFile, FileStream xmlFile)
        {
            using (TextReader reader = new StreamReader(animFile))
            {
                XDocument XDoc = new XDocument(new XElement("AnimFile"));
                XNode elem = XDoc.FirstNode;
                string line;
                bool nextLevel = false;

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    line = line.Trim();

                    if (line.StartsWith("//"))
                    {
                        if ((elem.Parent == null || nextLevel) && elem is XElement)
                        {
                            ((XElement)elem).Add(new XComment(line.Substring(2)));
                        }
                        else
                        {
                            elem.AddAfterSelf(new XComment(line.Substring(2)));
                            elem = elem.NextNode;
                        }
                    }
                    else if (line == "{")
                    {
                        nextLevel = true;
                    }
                    else if (line == "}")
                    {
                        elem = elem.Parent;
                    }
                    else
                    {
                        int spaceIndex = line.IndexOf(' ');
                        string function;
                        string data = string.Empty;
                        if (spaceIndex == -1)
                        {
                            function = line;
                        }
                        else
                        {
                            function = line.Substring(0, spaceIndex);
                            data = line.Substring(++spaceIndex);
                        }

                        if ((elem.Parent == null || nextLevel) && elem is XElement)
                        {
                            ((XElement)elem).Add(new XElement(function));
                            elem = ((XElement)elem).LastNode;
                            nextLevel = false;
                        }
                        else
                        {
                            elem.AddAfterSelf(new XElement(function));
                            elem = elem.NextNode;
                        }

                        if (spaceIndex == -1)
                        {
                            continue;
                        }

                        if (function.Equals("tag", StringComparison.InvariantCultureIgnoreCase) |
                            function.Equals("connect", StringComparison.InvariantCultureIgnoreCase) |
                            function.Equals("length", StringComparison.InvariantCultureIgnoreCase) |
                            function.Equals("replacetexture", StringComparison.InvariantCultureIgnoreCase) |
                            function.StartsWith("v", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ((XElement)elem).Value = data;
                        }
                        else
                        {
                            ((XElement)elem).Add(new XAttribute("data", data));
                        }
                    }
                }

                XDoc.Save(xmlFile);
            }
        }

        public static void ConvertToAnim(FileStream xmlFile, FileStream animFile)
        {
            using (TextWriter writer = new StreamWriter(animFile))
            {
                XDocument XDoc = XDocument.Load(xmlFile);
                Write(writer, ((XElement)XDoc.FirstNode).FirstNode, "");
            }
        }

        public static void Write(TextWriter writer, XNode node, string tabs)
        {
            if (node == null)
            {
                return;
            }

            if (node is XComment)
            {
                writer.Write(tabs);
                writer.WriteLine("//" + ((XComment)node).Value);
            }
            else if (node is XElement)
            {
                writer.Write(tabs);
                writer.Write(((XElement)node).Name);

                if (((XElement)node).HasAttributes)
                {
                    writer.Write(" " + ((XElement)node).Attribute("data").Value);
                }

                if (((XElement)node).HasElements)
                {
                    writer.WriteLine();
                    writer.WriteLine(tabs + "{");
                    Write(writer, ((XElement)node).FirstNode, tabs + "\t");
                    writer.WriteLine(tabs + "}");
                }
                else if (!string.IsNullOrEmpty(((XElement)node).Value) && !string.IsNullOrWhiteSpace(((XElement)node).Value))
                {
                    writer.Write(" " + ((XElement)node).Value);
                    writer.WriteLine();
                }
                else
                {
                    writer.WriteLine();
                }
            }
            else
            {
                throw new Exception("Could not recognize this type of xml compnonent!");
            }
            
            Write(writer, node.NextNode, tabs);
        }
    }
}
