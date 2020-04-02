using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AoMProtoEditor
{
    public class ProtoFile
    {
        public Dictionary<string, ProtoFormat> Format;
        public Dictionary<int, ProtoPropertyDefinition> Definition;
        public BiDictionaryOneToOne<string, int> String;
        public List<ProtoProperty> Unit;

        public ProtoFile(FileStream schema, FileStream stream)
        {
            Format = new Dictionary<string, ProtoFormat>();
            Definition = new Dictionary<int, ProtoPropertyDefinition>();
            String = new BiDictionaryOneToOne<string, int>();
            Unit = new List<ProtoProperty>();
            XmlDocument reader = new XmlDocument();
            using (schema)
            {
                reader.Load(schema);

                ProtoFormat pFormat;
                ProtoPropertyDefinition pDef;
                foreach (XmlElement element in reader.DocumentElement)
                {
                    if (element.Name == "formatList")
                    {
                        foreach (XmlElement format in element.ChildNodes)
                        {
                            pFormat = new ProtoFormat(format, this);
                            Format.Add(pFormat.Name, pFormat);
                        }
                    }
                    else if (element.Name == "definitionList")
                    {
                        foreach (XmlElement definition in element.ChildNodes)
                        {
                            pDef = new ProtoPropertyDefinition(definition, this);
                            Definition.Add(pDef.NameID, pDef);
                        }
                    }
                }

                reader = null;
            }

            reader = new XmlDocument();
            using (stream)
            {
                reader.Load(stream);

                ProtoUnit pUnit;
                foreach (XmlElement unit in reader.DocumentElement)
                {
                    //pUnit = new ProtoUnit(unit, this);
                    //Unit.Add(pUnit);
                    Unit.Add(new ProtoProperty(unit, this));
                }

                reader = null;
            }
        }

        public void Write(FileStream stream)
        {
            XmlDocument writer = new XmlDocument();
            using (stream)
            {
                writer.AppendChild(writer.CreateXmlDeclaration("1.0", "UTF-8", null));
                writer.AppendChild(writer.CreateElement("proto"));
                writer.LastChild.Attributes.Append(writer.CreateAttribute("version"));
                writer.LastChild.Attributes["version"].Value = "4";

                foreach (ProtoProperty property in Unit)
                {
                    property.Write(writer, writer.LastChild as XmlElement);
                }

                writer.Save(stream);
                writer = null;
            }
        }

        public ProtoFormat GetProtoFormat(int nameID, int attrNameID)
        {
            ProtoFormat fMat;
            foreach (KeyValuePair<string, ProtoFormat> pair in Format)
            {
                fMat = pair.Value;
                if (!fMat.Predefined && fMat.PropertyNameID == nameID && fMat.AttributeNameID == attrNameID)
                {
                    return fMat;
                }
            }

            return null;
        }
    }

    public class ProtoUnit
    {
        public ProtoFile File;
        public int ID;
        public string Name;
        public List<ProtoProperty> Property;

        public ProtoUnit(XmlElement unit, ProtoFile file)
        {
            File = file;
            ID = Convert.ToInt32(unit.GetAttribute("id"));
            Name = unit.GetAttribute("name");

            Property = new List<ProtoProperty>(unit.ChildNodes.Count);
            foreach (XmlElement property in unit.ChildNodes)
            {
                Property.Add(new ProtoProperty(property, File));
            }
        }
    }

    public class ProtoProperty
    {
        public ProtoFile File;
        public int NameID;
        public string Name
        {
            get { return File.String.GetBySecond(NameID); }
        }
        public object Value;
        private List<ProtoAttribute> Attribute;
        public List<ProtoProperty> ChildProperty;
        public ProtoPropertyDefinition Definition
        {
            get { return File.Definition[NameID]; }
        }

        public ProtoProperty(XmlElement property, ProtoFile _file)
        {
            File = _file;
            File.String.TryAdd(property.Name, File.String.Count);
            NameID = File.String.GetByFirst(property.Name);

            if (!File.Definition.ContainsKey(NameID))
            {
                File.Definition.Add(NameID, new ProtoPropertyDefinition(File, NameID, ProtoType.InferType(property.InnerText)));
            }
            FillProtoFormat();

            Attribute = new List<ProtoAttribute>(property.Attributes.Count);
            foreach (XmlAttribute attribute in property.Attributes)
            {
                Attribute.Add(new ProtoAttribute(attribute, File, this));
            }

            Value = ProtoType.ToObject(property.InnerText, Definition.DefaultFormat);
            ChildProperty = new List<ProtoProperty>(property.ChildNodes.Count);
            foreach (XmlNode childProperty in property.ChildNodes)
            {
                if (childProperty is XmlElement)
                    ChildProperty.Add(new ProtoProperty((XmlElement)childProperty, File));
            }
        }

        public void Write(XmlDocument writer, XmlElement element)
        {
            element.AppendChild(writer.CreateElement(Name));

            foreach (ProtoAttribute attribute in Attribute)
            {
                attribute.Write(writer, element.LastChild as XmlElement);
            }

            if (ChildProperty.Count > 0)
            {
                foreach (ProtoProperty childProperty in ChildProperty)
                {
                    childProperty.Write(writer, element.LastChild as XmlElement);
                }
            }
            else
            {
                string format = ProtoType.ToString(Value, Definition.DefaultFormat);
                //if (!string.IsNullOrEmpty(format))
                    element.LastChild.InnerText = format;
            }
        }

        public ProtoAttribute this[string attrName]
        {
            get
            {
                foreach (ProtoAttribute attrib in Attribute)
                {
                    if (attrib.Name == attrName)
                        return attrib;
                }

                return null;
            }
        }

        private void FillProtoFormat()
        {
            ProtoFormat fMat = File.GetProtoFormat(NameID, -1);
            if (fMat != null)
            {
                fMat.Add(Value as string);
                Definition.Format.Add(fMat.Name); // disable with noUpdate in formatList
            }
        }
    }

    public class ProtoAttribute
    {
        public ProtoFile File;
        public string Name
        {
            get { return File.String.GetBySecond(nameID); }
        }
        public object Value;
        public ProtoProperty ParentProperty
        {
            get { return parentProperty; }
        }
        public ProtoAttributeDefinition Definition
        {
            get { return ParentProperty.Definition[nameID]; }
        }

        private int nameID;
        private ProtoProperty parentProperty;

        public ProtoAttribute(XmlAttribute attribute, ProtoFile _file, ProtoProperty _parentProperty)
        {
            File = _file;
            File.String.TryAdd(attribute.Name, File.String.Count);
            nameID = File.String.GetByFirst(attribute.Name);

            parentProperty = _parentProperty;
            if (!parentProperty.Definition.Predefined && !parentProperty.Definition.HasAttribute(nameID))
            {
                parentProperty.Definition.Attribute.Add(new ProtoAttributeDefinition(File, nameID, ProtoType.InferType(attribute.InnerText)));
            }
            FillProtoFormat();

            Value = ProtoType.ToObject(attribute.InnerText, Definition.DefaultFormat);
        }

        public void Write(XmlDocument writer, XmlElement element)
        {
            element.Attributes.Append(writer.CreateAttribute(Name));
            string format = Definition.Format.Count > 0 ? Definition.Format.First() : "STRING";
            element.Attributes[Name].Value = ProtoType.ToString(Value, Definition.DefaultFormat);
        }

        private void FillProtoFormat()
        {
            ProtoFormat fMat = File.GetProtoFormat(ParentProperty.NameID, nameID);
            if (fMat != null)
            {
                fMat.Add(Value as string);
                Definition.Format.Add(fMat.Name);
            }
        }
    }

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

    public class ProtoPropertyDefinition
    {
        public ProtoFile File;
        public int NameID;
        public bool Predefined
        {
            get { return predefined; }
        }
        public HashSet<string> Format;
        public string DefaultFormat
        {
            get
            {
                if (Format.Count > 0)
                    return Format.First();
                else
                    return ProtoType.DefaultFormat;
            }
        }
        public List<ProtoAttributeDefinition> Attribute;

        private bool predefined;

        public ProtoPropertyDefinition(XmlElement definition, ProtoFile _file)
        {
            File = _file;
            string eName = definition.GetAttribute("element");
            File.String.TryAdd(eName, File.String.Count);
            NameID = File.String.GetByFirst(eName);
            predefined = definition.HasAttribute("predefined");

            Format = new HashSet<string>();
            Attribute = new List<ProtoAttributeDefinition>();
            foreach (XmlElement element in definition.ChildNodes)
            {
                if (element.Name == "format")
                {
                    Format.Add(element.InnerText);
                }
                else if (element.Name == "attribute")
                {
                    Attribute.Add(new ProtoAttributeDefinition(element, File));
                }
            }
        }
        public ProtoPropertyDefinition(ProtoFile _file, int nameID, string format)
        {
            File = _file;
            NameID = nameID;
            predefined = false;

            Format = new HashSet<string>();
            Format.Add(format);

            Attribute = new List<ProtoAttributeDefinition>();
        }

        public bool HasAttribute(int name)
        {
            foreach (ProtoAttributeDefinition attr in Attribute)
            {
                if (attr.NameID == name)
                    return true;
            }

            return false;
        }
        public ProtoAttributeDefinition this[int name]
        {
            get
            {
                foreach (ProtoAttributeDefinition attr in Attribute)
                {
                    if (attr.NameID == name)
                        return attr;
                }

                throw new Exception("NameID out of range for AttribDefinition list!");
            }
        }
    }

    public class ProtoAttributeDefinition
    {
        public ProtoFile File;
        public int NameID;
        public HashSet<string> Format;
        public string DefaultFormat
        {
            get
            {
                if (Format.Count > 0)
                    return Format.First();
                else
                    return ProtoType.DefaultFormat;
            }
        }

        public ProtoAttributeDefinition(XmlElement attribute, ProtoFile _file)
        {
            File = _file;
            string aName = attribute.GetAttribute("name");
            File.String.TryAdd(aName, File.String.Count);
            NameID = File.String.GetByFirst(aName);

            Format = new HashSet<string>();
            foreach (XmlElement format in attribute.ChildNodes)
            {
                Format.Add(format.InnerText);
            }
        }
        public ProtoAttributeDefinition(ProtoFile _file, int nameID, string format)
        {
            File = _file;
            NameID = nameID;

            Format = new HashSet<string>();
            Format.Add(format);
        }
    }

    public static class ProtoType
    {
        public static string DefaultFormat = "VOID";

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
                return "VOID";
            if (Int32.TryParse(value, out i) && !value.Contains('.'))
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
                    return null;
                default:
                    return "{0}";
            }
        }
        public static string ToString(object value, string type)
        {
            return String.Format(GetFormatSpecifier(type), value);
        }

        public static object ToObject(string value, string type)
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
