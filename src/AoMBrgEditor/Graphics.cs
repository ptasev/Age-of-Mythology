using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using MiscUtil.IO;
using MiscUtil.Conversion;

namespace AoMBrgEditor
{
    #region PSSG
    public class PssgFile
    {
        public string magic;
        public PssgNodeInfo[] nodeInfo;
        public PssgAttributeInfo[] attributeInfo;
        public PssgNode rootNode;

        public PssgFile(System.IO.Stream fileStream)
        {
            using (PssgBinaryReader reader = new PssgBinaryReader(new BigEndianBitConverter(), fileStream))
            {
                magic = reader.ReadPSSGString(4);
                if (magic != "PSSG")
                {
                    throw new Exception("This is not a PSSG file!");
                }
                int size = reader.ReadInt32();
                int attributeInfoCount = reader.ReadInt32();
                int nodeInfoCount = reader.ReadInt32();

                attributeInfo = new PssgAttributeInfo[attributeInfoCount];
                nodeInfo = new PssgNodeInfo[nodeInfoCount];

                for (int i = 0; i < nodeInfoCount; i++)
                {
                    nodeInfo[i] = new PssgNodeInfo(reader, this);
                }
                long positionAfterInfo = reader.BaseStream.Position;

                rootNode = new PssgNode(reader, this, null, true);
                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    reader.BaseStream.Position = positionAfterInfo;
                    rootNode = new PssgNode(reader, this, null, false);
                    if (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        throw new Exception("This file is improperly saved and not supported by this version of the PSSG editor." + Environment.NewLine + Environment.NewLine +
                            "Get an older version of the program if you wish to take out its contents, but put it back together using this program and the original version of the pssg file.");
                    }
                }
            }
        }
        public PssgFile()
        {
            magic = "PSSG";
            nodeInfo = new PssgNodeInfo[0];
            attributeInfo = new PssgAttributeInfo[0];
        }

        public void Write(System.IO.Stream fileStream)
        {
            using (PssgBinaryWriter writer = new PssgBinaryWriter(new BigEndianBitConverter(), fileStream))
            {
                writer.Write(Encoding.ASCII.GetBytes(magic));
                writer.Write(0);
                writer.Write(attributeInfo.Length);
                writer.Write(nodeInfo.Length);

                for (int i = 0; i < nodeInfo.Length; i++)
                {
                    nodeInfo[i].Write(writer);
                }

                if (rootNode != null)
                {
                    rootNode.UpdateSize();
                    rootNode.Write(writer);
                }
                writer.BaseStream.Position = 4;
                writer.Write((int)writer.BaseStream.Length - 8);
            }
        }
        public void WriteAsModel(System.IO.Stream fileStream)
        {
            XmlDocument pssg = new XmlDocument();
            pssg.AppendChild(pssg.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
            pssg.AppendChild(pssg.CreateElement("COLLADA", "http://www.collada.org/2008/03/COLLADASchema"));
            pssg.DocumentElement.AppendChild(pssg.CreateAttribute("version"));
            pssg.DocumentElement.Attributes["version"].InnerText = "1.5.0";

            if (rootNode.HasAttributes)
            {
                XmlElement asset = pssg.CreateElement("asset");
                if (rootNode.HasAttribute("creator"))
                {
                    asset.AppendChild(pssg.CreateElement("contributor"));
                    asset.LastChild.AppendChild(pssg.CreateElement("author"));
                    asset.LastChild.LastChild.InnerText = rootNode["creator"].ToString();
                }
                // TODO: unit meter 1, created, up axis, scale?, creatorMachine


            }
        }

        public TreeNode CreateTreeViewNode(PssgNode node)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = node.Name;
            treeNode.Tag = node;
            if (node.subNodes != null)
            {
                foreach (PssgNode subNode in node.subNodes)
                {
                    treeNode.Nodes.Add(CreateTreeViewNode(subNode));
                }
            }
            node.TreeNode = treeNode;
            return treeNode;
        }
        public void CreateSpecificTreeViewNode(TreeView tv, string nodeName)
        {
            List<PssgNode> textureNodes = FindNodes(nodeName);
            TreeNode treeNode = new TreeNode();
            foreach (PssgNode texture in textureNodes)
            {
                if (texture.attributes.ContainsKey("id") == false)
                {
                    continue;
                }
                treeNode.Text = texture.attributes["id"].ToString();
                treeNode.Tag = texture;
                tv.Nodes.Add(treeNode);
                treeNode = new TreeNode();
            }
        }
        public void CreateSpecificTreeViewNode(TreeView tv, string nodeName, string attributeName, string attributeValue)
        {
            List<PssgNode> textureNodes = FindNodes(nodeName, attributeName, attributeValue);
            TreeNode treeNode = new TreeNode();
            foreach (PssgNode texture in textureNodes)
            {
                treeNode.Text = texture.attributes["id"].ToString();
                treeNode.Tag = texture;
                tv.Nodes.Add(treeNode);
                treeNode = new TreeNode();
            }
        }

        public PssgNodeInfo[] GetNodeInfo(string nodeInfoName)
        {
            PssgNodeInfo[] query = nodeInfo.Where(x => x.name == nodeInfoName).ToArray();
            return query;
        }
        public PssgAttributeInfo[] GetAttributeInfo(string attributeInfoName)
        {
            PssgAttributeInfo[] query = attributeInfo.Where(x => x.name == attributeInfoName).ToArray();
            return query;
        }

        public List<PssgNode> FindNodes(string name, string attributeName = null, string attributeValue = null)
        {
            if (rootNode == null)
            {
                return new List<PssgNode>();
            }
            return rootNode.FindNodes(name, attributeName, attributeValue);
        }

        public PssgNode AddNode(PssgNode parentNode, int nodeID)
        {
            if (rootNode == null)
            {
                PssgNode newRootNode = new PssgNode(nodeID, this, null, nodeInfo[nodeID - 1].isDataNode);
                rootNode = newRootNode;
                return newRootNode;
            }
            if (parentNode.isDataNode == true)
            {
                MessageBox.Show("Adding sub nodes to a data node is not allowed!", "Add Node", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return null;
            }
            if (parentNode.subNodes != null)
            {
                Array.Resize(ref parentNode.subNodes, parentNode.subNodes.Length + 1);
            }
            else
            {
                parentNode.subNodes = new PssgNode[1];
            }
            PssgNode newNode = new PssgNode(nodeID, this, parentNode, nodeInfo[nodeID - 1].isDataNode);
            parentNode.subNodes[parentNode.subNodes.Length - 1] = newNode;
            return newNode;
        }
        public PssgAttribute AddAttribute(PssgNode parentNode, int attributeID, object data)
        {
            if (parentNode == null)
            {
                return null;
            }
            if (parentNode.attributes == null)
            {
                parentNode.attributes = new Dictionary<string, PssgAttribute>();
            }
            else if (parentNode.HasAttribute(attributeID))
            {
                parentNode[attributeID].data = data;
                return parentNode[attributeID];
            }
            else if (parentNode.attributes.ContainsKey(attributeInfo[attributeID - 1].name))
            {
                return null;
            }
            PssgAttribute newAttr = new PssgAttribute(attributeID, data, this, parentNode);
            parentNode.attributes.Add(newAttr.Name, newAttr);
            return newAttr;
        }
        public void RemoveNode(PssgNode node)
        {
            if (node.ParentNode == null)
            {
                rootNode = null;
            }
            else
            {
                List<PssgNode> subNodes = new List<PssgNode>(node.ParentNode.subNodes);
                subNodes.Remove(node);
                node.ParentNode.subNodes = subNodes.ToArray();
                node = null;
            }
        }
        public void RemoveAttribute(PssgNode node, string attributeName)
        {
            node.attributes.Remove(attributeName);
        }

        public PssgNodeInfo AddNodeInfo(string name)
        {
            if (GetNodeInfo(name).Length > 0)
            {
                return null;
            }
            if (nodeInfo == null)
            {
                nodeInfo = new PssgNodeInfo[1];
            }
            else
            {
                Array.Resize(ref nodeInfo, nodeInfo.Length + 1);
            }
            PssgNodeInfo nInfo = new PssgNodeInfo(nodeInfo.Length, name);
            nodeInfo[nInfo.id - 1] = nInfo;
            return nInfo;
        }
        public PssgAttributeInfo AddAttributeInfo(string name, PssgNodeInfo nodeInfo)
        {
            // For each attributeInfo in nodeInfo, the ids have to be consecutive
            if (GetAttributeInfo(name).Length > 0)
            {
                return null;
            }
            if (attributeInfo == null)
            {
                attributeInfo = new PssgAttributeInfo[1];
            }
            else
            {
                Array.Resize(ref attributeInfo, attributeInfo.Length + 1);
            }
            int newID = 0;
            List<int> currentKeys = new List<int>(nodeInfo.attributeInfo.Keys);
            if (currentKeys.Count > 0)
            {
                foreach (int k in currentKeys)
                {
                    newID = Math.Max(newID, k);
                }
                newID++;
            }
            else
            {
                newID = attributeInfo.Length;
            }
            PssgAttributeInfo attrInfo = new PssgAttributeInfo(newID, name);
            if (newID == attributeInfo.Length)
            {
                attributeInfo[attrInfo.id - 1] = attrInfo;
                this.nodeInfo[nodeInfo.id - 1].attributeInfo.Add(attrInfo.id, attrInfo);
            }
            else
            {
                for (int i = attributeInfo.Length - 2; i >= newID - 1; i--)
                {
                    attributeInfo[i + 1] = attributeInfo[i];
                    attributeInfo[i + 1].id = i + 2;
                }
                attributeInfo[attrInfo.id - 1] = attrInfo;
                this.nodeInfo[nodeInfo.id - 1].attributeInfo.Add(attrInfo.id, attrInfo);
                // Fix the NodeInfos
                foreach (PssgNodeInfo nInfo in this.nodeInfo)
                {
                    List<int> keys = new List<int>(nInfo.attributeInfo.Keys);
                    //keys.Sort();
                    if (nInfo != nodeInfo)
                    {
                        for (int i = keys.Count - 1; i >= 0; i--)
                        {
                            if (keys[i] >= newID)
                            {
                                PssgAttributeInfo aInfo = attributeInfo[keys[i]];
                                nInfo.attributeInfo.Remove(keys[i]);
                                nInfo.attributeInfo.Add(keys[i] + 1, aInfo);
                            }
                        }
                    }
                }
                // Edit CNode to fix CAttr.id
                rootNode.AddAttributeInfo(newID);
            }
            return attrInfo;
        }
        public void RemoveNodeInfo(int id)
        {
            // Remove all attributeInfos from nodeInfo
            List<int> attrKeys = new List<int>(nodeInfo[id - 1].attributeInfo.Keys);
            while (nodeInfo[id - 1].attributeInfo.Count > 0)
            {
                RemoveAttributeInfo(attrKeys[0]);
            }
            attrKeys = null;
            // Shift all succeeding nodeInfos and change their id
            for (int i = id - 1; i < nodeInfo.Length - 1; i++)
            {
                nodeInfo[i] = nodeInfo[i + 1];
                nodeInfo[i].id = i + 1;
            }
            Array.Resize(ref nodeInfo, nodeInfo.Length - 1);
            // Delete from CNode
            if (rootNode != null)
            {
                if (rootNode.id == id)
                {
                    rootNode = null;
                }
                else
                {
                    rootNode.RemoveNodeInfo(id);
                }
            }
        }
        public void RemoveAttributeInfo(int id)
        {
            // Shift all succeeding attributeInfos and change their id
            // 
            for (int i = id - 1; i < attributeInfo.Length - 1; i++)
            {
                attributeInfo[i] = attributeInfo[i + 1];
                attributeInfo[i].id--;
                //attributeInfo[i].id = i + 1;
            }
            Array.Resize(ref attributeInfo, attributeInfo.Length - 1);
            // Fix the NodeInfos
            foreach (PssgNodeInfo nInfo in nodeInfo)
            {
                List<int> keys = new List<int>(nInfo.attributeInfo.Keys);
                //keys.Sort();
                if (nInfo.attributeInfo.ContainsKey(id) == true)
                {
                    nInfo.attributeInfo.Remove(id);
                    for (int i = id + 1; nInfo.attributeInfo.ContainsKey(i); i++)
                    {
                        PssgAttributeInfo aInfo = attributeInfo[i - 2];
                        nInfo.attributeInfo.Remove(i);
                        nInfo.attributeInfo.Add(i - 1, aInfo);
                    }
                }
                else
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (keys[i] > id)
                        {
                            PssgAttributeInfo aInfo = attributeInfo[keys[i] - 2];
                            nInfo.attributeInfo.Remove(keys[i]);
                            nInfo.attributeInfo.Add(keys[i] - 1, aInfo);
                        }
                    }
                }
            }
            // Edit CNode to fix CAttr.id
            if (rootNode != null)
            {
                rootNode.RemoveAttributeInfo(id);
            }
        }
    }

    public class PssgNodeInfo
    {
        public int id;
        public string name;
        public SortedDictionary<int, PssgAttributeInfo> attributeInfo;
        public bool isDataNode = false;

        public PssgNodeInfo(PssgBinaryReader reader, PssgFile file)
        {
            attributeInfo = new SortedDictionary<int, PssgAttributeInfo>();

            id = reader.ReadInt32();
            name = reader.ReadPSSGString();
            int attributeInfoCount = reader.ReadInt32();
            PssgAttributeInfo ai;
            for (int i = 0; i < attributeInfoCount; i++)
            {
                ai = new PssgAttributeInfo(reader);
                try
                {
                    attributeInfo.Add(ai.id, ai);
                }
                catch (ArgumentException ex)
                {
                    if (MessageBox.Show("The attribute of id " + ai.id + ", named " + ai.name + ", already exists, and will not be saved. Continue?",
                        "PSSG Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        continue;
                    else
                        throw new Exception("The attribute of id " + ai.id + ", named " + ai.name + ", already exists.");
                }

                file.attributeInfo[ai.id - 1] = ai;
            }
        }
        public PssgNodeInfo(int id, string name)
        {
            this.id = id;
            this.name = name;
            attributeInfo = new SortedDictionary<int, PssgAttributeInfo>();
        }

        public void Write(PssgBinaryWriter writer)
        {
            writer.Write(id);
            writer.WritePSSGString(name);
            writer.Write(attributeInfo.Count);
            foreach (KeyValuePair<int, PssgAttributeInfo> info in attributeInfo)
            {
                writer.Write(info.Key);
                writer.WritePSSGString(info.Value.name);
            }
        }
    }

    public class PssgAttributeInfo
    {
        public int id;
        public string name;

        public PssgAttributeInfo(PssgBinaryReader reader)
        {
            id = reader.ReadInt32();
            name = reader.ReadPSSGString();
        }
        public PssgAttributeInfo(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public class PssgNode
    {
        public int id;
        private int size;
        private int attributeSize;
        public int Size
        {
            get
            {
                return size;
            }
        }
        public int AttributeSize
        {
            get
            {
                return attributeSize;
            }
        }
        public Dictionary<string, PssgAttribute> attributes;
        public PssgNode[] subNodes;
        public bool isDataNode = false;
        public byte[] data;
        public string Name
        {
            get
            {
                return file.nodeInfo[id - 1].name;
            }
        }

        private PssgFile file;
        public PssgNode ParentNode;
        public TreeNode TreeNode;

        public PssgNode(PssgBinaryReader reader, PssgFile file, PssgNode node, bool useDataNodeCheck)
        {
            this.file = file;
            ParentNode = node;

            id = reader.ReadInt32();
            size = reader.ReadInt32();
            long end = reader.BaseStream.Position + size;

            attributeSize = reader.ReadInt32();
            long attributeEnd = reader.BaseStream.Position + attributeSize;
            if (attributeEnd > reader.BaseStream.Length || end > reader.BaseStream.Length)
            {
                throw new Exception("This file is improperly saved and not supported by this version of the PSSG editor." + Environment.NewLine + Environment.NewLine +
                            "Get an older version of the program if you wish to take out its contents, but, put it back together using this program and a non-modded version of the pssg file.");
            }
            // Each attr is at least 8 bytes (id + size), so take a conservative guess
            attributes = new Dictionary<string, PssgAttribute>();
            PssgAttribute attr;
            while (reader.BaseStream.Position < attributeEnd)
            {
                attr = new PssgAttribute(reader, file, this);
                attributes.Add(attr.Name, attr);
            }

            switch (Name)
            {
                case "BOUNDINGBOX":
                case "DATA":
                case "DATABLOCKDATA":
                case "DATABLOCKBUFFERED":
                case "INDEXSOURCEDATA":
                case "INVERSEBINDMATRIX":
                case "MODIFIERNETWORKINSTANCEUNIQUEMODIFIERINPUT":
                case "NeAnimPacketData_B1":
                case "NeAnimPacketData_B4":
                case "RENDERINTERFACEBOUNDBUFFERED":
                case "SHADERINPUT":
                case "TEXTUREIMAGEBLOCKDATA":
                case "TRANSFORM":
                    isDataNode = true;
                    break;
            }
            if (isDataNode == false && useDataNodeCheck == true)
            {
                long currentPos = reader.BaseStream.Position;
                // Check if it has subnodes
                while (reader.BaseStream.Position < end)
                {
                    int tempID = reader.ReadInt32();
                    if (tempID > file.nodeInfo.Length || tempID < 0)
                    {
                        isDataNode = true;
                        break;
                    }
                    else
                    {
                        int tempSize = reader.ReadInt32();
                        if ((reader.BaseStream.Position + tempSize > end) || (tempSize == 0 && tempID == 0) || tempSize < 0)
                        {
                            isDataNode = true;
                            break;
                        }
                        else if (reader.BaseStream.Position + tempSize == end)
                        {
                            break;
                        }
                        else
                        {
                            reader.BaseStream.Position += tempSize;
                        }
                    }
                }
                reader.BaseStream.Position = currentPos;
            }

            if (isDataNode)
            {
                data = reader.ReadBytes((int)(end - reader.BaseStream.Position));
            }
            else
            {
                // Each node at least 12 bytes (id + size + arg size)
                subNodes = new PssgNode[(end - reader.BaseStream.Position) / 12];
                int nodeCount = 0;
                while (reader.BaseStream.Position < end)
                {
                    subNodes[nodeCount] = new PssgNode(reader, file, this, useDataNodeCheck);
                    nodeCount++;
                }
                Array.Resize(ref subNodes, nodeCount);
            }

            file.nodeInfo[id - 1].isDataNode = isDataNode;
        }
        public PssgNode(PssgNode nodeToCopy)
        {
            this.file = nodeToCopy.file;
            ParentNode = nodeToCopy.ParentNode;

            id = nodeToCopy.id;
            size = nodeToCopy.size;
            attributeSize = nodeToCopy.attributeSize;
            attributes = new Dictionary<string, PssgAttribute>();
            PssgAttribute attr;
            foreach (KeyValuePair<string, PssgAttribute> attrToCopy in nodeToCopy.attributes)
            {
                attr = new PssgAttribute(attrToCopy.Value);
                attributes.Add(attr.Name, attr);
            }

            isDataNode = nodeToCopy.isDataNode;

            if (isDataNode)
            {
                data = nodeToCopy.data;
            }
            else
            {
                // Each node at least 12 bytes (id + size + arg size)
                subNodes = new PssgNode[nodeToCopy.subNodes.Length];
                int nodeCount = 0;
                foreach (PssgNode subNodeToCopy in nodeToCopy.subNodes)
                {
                    subNodes[nodeCount] = new PssgNode(subNodeToCopy);
                    nodeCount++;
                }
                Array.Resize(ref subNodes, nodeCount);
            }
        }
        public PssgNode(int id, PssgFile file, PssgNode node, bool isDataNode)
        {
            this.id = id;
            this.file = file;
            this.ParentNode = node;
            this.isDataNode = isDataNode;
            attributes = new Dictionary<string, PssgAttribute>();
            if (isDataNode == true)
            {
                data = new byte[0];
            }
        }

        public void Write(PssgBinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(size);
            writer.Write(attributeSize);
            if (attributes != null)
            {
                foreach (KeyValuePair<string, PssgAttribute> attr in attributes)
                {
                    attr.Value.Write(writer);
                }
            }
            if (subNodes != null)
            {
                foreach (PssgNode node in subNodes)
                {
                    node.Write(writer);
                }
            }
            if (isDataNode)
            {
                writer.Write(data);
            }
        }
        public void UpdateSize()
        {
            attributeSize = 0;
            if (attributes != null)
            {
                foreach (KeyValuePair<string, PssgAttribute> attr in attributes)
                {
                    attr.Value.UpdateSize();
                    attributeSize += 8 + attr.Value.Size;
                }
            }
            size = 4 + attributeSize;
            if (subNodes != null)
            {
                foreach (PssgNode node in subNodes)
                {
                    node.UpdateSize();
                    size += 8 + node.Size;
                }
            }
            if (isDataNode)
            {
                size += data.Length;
            }
        }

        public List<PssgNode> FindNodes(string nodeName, string attributeName = null, string attributeValue = null)
        {
            List<PssgNode> ret = new List<PssgNode>();
            if (this.Name == nodeName)
            {
                if (attributeName != null && attributeValue != null)
                {
                    PssgAttribute attr;
                    if (attributes.TryGetValue(attributeName, out attr) && attr.ToString() == attributeValue)
                    {
                        ret.Add(this);
                    }
                }
                else if (attributeName != null)
                {
                    if (attributes.ContainsKey(attributeName) == true)
                    {
                        ret.Add(this);
                    }
                }
                else if (attributeValue != null)
                {
                    foreach (KeyValuePair<string, PssgAttribute> pair in attributes)
                    {
                        if (pair.Value.ToString() == attributeValue)
                        {
                            ret.Add(this);
                            break;
                        }
                    }
                }
                else
                {
                    ret.Add(this);
                }
            }
            if (subNodes != null)
            {
                foreach (PssgNode subNode in subNodes)
                {
                    ret.AddRange(subNode.FindNodes(nodeName, attributeName, attributeValue));
                }
            }
            return ret;
        }

        public void AddAttributeInfo(int id)
        {
            foreach (KeyValuePair<string, PssgAttribute> pair in attributes)
            {
                if (pair.Value.id >= id)
                {
                    pair.Value.id++;
                }
            }

            if (subNodes != null)
            {
                foreach (PssgNode subNode in subNodes)
                {
                    subNode.AddAttributeInfo(id);
                }
            }
        }
        public void RemoveNodeInfo(int id)
        {
            if (this.id > id)
            {
                this.id--;
            }

            if (subNodes != null)
            {
                List<PssgNode> newSubNodes = new List<PssgNode>();
                for (int i = 0; i < subNodes.Length; i++)
                {
                    if (subNodes[i].id != id)
                    {
                        subNodes[i].RemoveNodeInfo(id);
                        newSubNodes.Add(subNodes[i]);
                    }
                }
                subNodes = newSubNodes.ToArray();
            }
        }
        public void RemoveAttributeInfo(int id)
        {
            string toDelete = "";
            foreach (KeyValuePair<string, PssgAttribute> pair in attributes)
            {
                if (pair.Value.id == id)
                {
                    toDelete = pair.Key;
                }
                else if (pair.Value.id > id)
                {
                    pair.Value.id--;
                }
            }
            if (attributes.ContainsKey(toDelete) == true)
            {
                attributes.Remove(toDelete);
            }

            if (subNodes != null)
            {
                foreach (PssgNode subNode in subNodes)
                {
                    subNode.RemoveAttributeInfo(id);
                }
            }
        }

        public bool HasAttributes
        {
            get { return attributes.Count > 0; }
        }

        /// <summary>
        /// Determines whether the current node has an attribute with the specified id.
        /// </summary>
        /// <param name="attributeID">The id of the attribute to find.</param>
        public bool HasAttribute(int attributeID)
        {
            return attributes.Count(x => x.Value.id == attributeID) > 0;
        }
        public bool HasAttribute(string attributeName)
        {
            return attributes.ContainsKey(attributeName);
        }
        /// <summary>
        /// Gets or sets the node attribute associated with the specified attribute id.
        /// </summary>
        /// <param name="attributeID">The id of the attribute to get or set.</param>
        public PssgAttribute this[int attributeID]
        {
            get
            {
                List<string> keys = new List<string>(attributes.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    if (attributes[keys[i]].id == attributeID)
                    {
                        return attributes[keys[i]];
                    }
                }
                throw new ArgumentOutOfRangeException("attributeID");
                //return attributes.First(x => x.Value.id == id).Value;
            }
            set
            {
                this[attributeID] = value;
            }
        }
        public PssgAttribute this[string attributeName]
        {
            get
            {
                if (HasAttribute(attributeName))
                {
                    return attributes[attributeName];
                }
                throw new ArgumentOutOfRangeException("attributeName");
            }
            set
            {
                this[attributeName] = value;
            }
        }

        public override string ToString() { return Name; }
    }

    public class PssgAttribute
    {
        public int id;
        private int size;
        public int Size
        {
            get
            {
                return size;
            }
        }
        public object data;
        public object Value
        {
            get
            {
                object ret = "Byte Data - Do Not Edit";
                if (data is string)
                {
                    return (string)data;
                }
                else if ((data is byte[]) == false)
                {
                    return Convert.ChangeType(data, data.GetType());
                }
                else if (((byte[])data).Length == 4)
                {
                    ret = EndianBitConverter.Big.ToUInt32((byte[])data, 0);
                    if ((uint)ret > 1000000000)
                    {
                        ret = EndianBitConverter.Big.ToSingle((byte[])data, 0);
                    }
                    if (ParentNode.Name == "FETEXTLAYOUT")
                    {
                        if (Name == "height" ||
                            Name == "depth" ||
                            Name == "tracking")
                        {
                            ret = EndianBitConverter.Big.ToSingle((byte[])data, 0);
                        }
                    }
                    if (ParentNode.Name == "NEGLYPHMETRICS")
                    {
                        if (Name == "advanceWidth" ||
                            Name == "horizontalBearing" ||
                            Name == "verticalBearing" ||
                            Name == "physicalWidth" ||
                            Name == "physicalHeight")
                        {
                            ret = EndianBitConverter.Big.ToSingle((byte[])data, 0);
                        }
                        else if (Name == "codePoint")
                        {
                            //ret = EndianBitConverter.Big.ToInt32((byte[])data, 0);
                        }
                    }
                    if (ParentNode.Name == "SHADERGROUP")
                    {
                        if (Name == "defaultRenderSortPriority")
                        {
                            ret = EndianBitConverter.Big.ToSingle((byte[])data, 0);
                        }
                    }
                    if (ParentNode.Name == "FEATLASINFODATA")
                    {
                        if (Name == "u0" ||
                            Name == "v0" ||
                            Name == "u1" ||
                            Name == "v1")
                        {
                            ret = EndianBitConverter.Big.ToSingle((byte[])data, 0);
                        }
                    }
                }
                else if (((byte[])data).Length == 2)
                {
                    ret = EndianBitConverter.Big.ToUInt16((byte[])data, 0);
                }

                return ret;
            }
        }
        public string Name
        {
            get
            {
                return file.attributeInfo[id - 1].name;
            }
        }
        public override string ToString()
        {
            if (Value is string)
            {
                return (string)Value;
            }
            else if (Value is UInt16)
            {
                return ((UInt16)Value).ToString();
            }
            else if (Value is UInt32)
            {
                return ((uint)Value).ToString();
            }
            else if (Value is Int16)
            {
                return ((Int16)Value).ToString();
            }
            else if (Value is Int32)
            {
                return ((int)Value).ToString();
            }
            else if (Value is float)
            {
                return ((float)Value).ToString();
            }
            else if (Value is bool)
            {
                return ((bool)Value).ToString();
            }
            else
            {
                return "Byte Data - Do Not Edit";
            }
        }

        private PssgFile file;
        public PssgNode ParentNode;

        public PssgAttribute(int id, object data, PssgFile file, PssgNode ParentNode)
        {
            this.id = id;
            this.data = data;
            this.file = file;
            this.ParentNode = ParentNode;
        }
        public PssgAttribute(PssgBinaryReader reader, PssgFile file, PssgNode node)
        {
            this.file = file;
            ParentNode = node;

            id = reader.ReadInt32();
            size = reader.ReadInt32();
            if (size == 4)
            {
                data = reader.ReadBytes(size);
                return;
            }
            else if (size > 4)
            {
                int strlen = reader.ReadInt32();
                if (size - 4 == strlen)
                {
                    data = reader.ReadPSSGString(strlen);
                    return;
                }
                else
                {
                    reader.Seek(-4, System.IO.SeekOrigin.Current);
                }
            }
            data = reader.ReadBytes(size);
        }
        public PssgAttribute(PssgAttribute attrToCopy)
        {
            this.file = attrToCopy.file;
            ParentNode = attrToCopy.ParentNode;

            id = attrToCopy.id;
            size = attrToCopy.size;
            data = attrToCopy.data;
        }

        public void Write(PssgBinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(size);
            if (data is string)
            {
                writer.WritePSSGString((string)data);
            }
            else if (data is UInt16)
            {
                writer.Write((UInt16)data);
            }
            else if (data is UInt32)
            {
                writer.Write((UInt32)data);
            }
            else if (data is Int16)
            {
                writer.Write((Int16)data);
            }
            else if (data is Int32)
            {
                writer.Write((Int32)data);
            }
            else if (data is Single)
            {
                writer.Write((Single)data);
            }
            else if (data is bool)
            {
                writer.Write((bool)data);
            }
            else
            {
                writer.Write((byte[])data);
            }
        }

        public void UpdateSize()
        {
            if (data is string)
            {
                size = 4 + Encoding.UTF8.GetBytes((string)data).Length;
            }
            else if (data is UInt16)
            {
                size = EndianBitConverter.Big.GetBytes((UInt16)data).Length;
            }
            else if (data is UInt32)
            {
                size = EndianBitConverter.Big.GetBytes((UInt32)data).Length;
            }
            else if (data is Int16)
            {
                size = EndianBitConverter.Big.GetBytes((Int16)data).Length;
            }
            else if (data is Int32)
            {
                size = EndianBitConverter.Big.GetBytes((Int32)data).Length;
            }
            else if (data is Single)
            {
                size = EndianBitConverter.Big.GetBytes((Single)data).Length;
            }
            else if (data is bool)
            {
                size = EndianBitConverter.Big.GetBytes((bool)data).Length;
            }
            else
            {
                size = ((byte[])data).Length;
            }
        }
    }

    public class PssgBinaryReader : EndianBinaryReader
    {
        public PssgBinaryReader(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
        {
        }

        public string ReadPSSGString()
        {
            int length = this.ReadInt32();
            return this.ReadPSSGString(length);
        }
        public string ReadPSSGString(int length)
        {
            return Encoding.UTF8.GetString(this.ReadBytes(length));
        }
    }

    public class PssgBinaryWriter : EndianBinaryWriter
    {
        public PssgBinaryWriter(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
        {
        }

        public void WritePSSGString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            this.Write(bytes.Length);
            this.Write(bytes);
        }
    }
    #endregion

    #region DDS
    public struct DDS_HEADER
    {
        public uint size;
        public Flags flags;
        public uint height;
        public uint width;
        public uint pitchOrLinearSize;
        public uint depth;
        public uint mipMapCount;
        public uint[] reserved1; //  = new uint[11]
        public DDS_PIXELFORMAT ddspf;
        public Caps caps;
        public Caps2 caps2;
        public uint caps3;
        public uint caps4;
        public uint reserved2;

        public enum Flags
        {
            DDSD_CAPS = 0x1,
            DDSD_HEIGHT = 0x2,
            DDSD_WIDTH = 0x4,
            DDSD_PITCH = 0x8,
            DDSD_PIXELFORMAT = 0x1000,
            DDSD_MIPMAPCOUNT = 0x20000,
            DDSD_LINEARSIZE = 0x80000,
            DDSD_DEPTH = 0x800000
        }

        public enum Caps
        {
            DDSCAPS_COMPLEX = 0x8,
            DDSCAPS_MIPMAP = 0x400000,
            DDSCAPS_TEXTURE = 0x1000
        }

        public enum Caps2
        {
            DDSCAPS2_CUBEMAP = 0x200,
            DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,
            DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,
            DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,
            DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,
            DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,
            DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,
            DDSCAPS2_VOLUME = 0x200000
        }
    }

    public struct DDS_PIXELFORMAT
    {
        public uint size;
        public Flags flags;
        public uint fourCC;
        public uint rGBBitCount;
        public uint rBitMask;
        public uint gBitMask;
        public uint bBitMask;
        public uint aBitMask;

        public enum Flags
        {
            DDPF_ALPHAPIXELS = 0x1,
            DDPF_ALPHA = 0x2,
            DDPF_FOURCC = 0x4,
            DDPF_RGB = 0x40,
            DDPF_YUV = 0x200,
            DDPF_LUMINANCE = 0x20000
        }
    }

    public class Dds
    {
        uint magic;
        DDS_HEADER header;
        byte[] bdata;
        Dictionary<int, byte[]> bdata2;

        public Dds(DdtFile ddt)
        {
            magic = 0x20534444;
            header.size = 124;
            header.flags |= DDS_HEADER.Flags.DDSD_CAPS | DDS_HEADER.Flags.DDSD_HEIGHT | DDS_HEADER.Flags.DDSD_WIDTH | DDS_HEADER.Flags.DDSD_PIXELFORMAT;
            header.height = (uint)(ddt.Height);
            header.width = (uint)(ddt.Width);
            string texelFormat = ddt.GetTexelFormat();
            switch (texelFormat)
            {
                case "DXT1"://dtx1
                    header.flags |= DDS_HEADER.Flags.DDSD_LINEARSIZE;
                    header.pitchOrLinearSize = (uint)(Math.Max(1, (ddt.Width + 3) / 4) * 8);
                    header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_FOURCC;
                    header.ddspf.fourCC = BitConverter.ToUInt32(Encoding.UTF8.GetBytes(texelFormat), 0);
                    break;
                case "DXT2":
                case "DXT3":
                case "DXT4":
                case "DXT5":
                    header.flags |= DDS_HEADER.Flags.DDSD_LINEARSIZE;
                    header.pitchOrLinearSize = (uint)(Math.Max(1, (ddt.Width + 3) / 4) * 16);
                    header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_FOURCC;
                    header.ddspf.fourCC = BitConverter.ToUInt32(Encoding.UTF8.GetBytes(texelFormat), 0);
                    break;
                case "1555":
                    header.flags |= DDS_HEADER.Flags.DDSD_PITCH;
                    header.pitchOrLinearSize = (uint)((ddt.Width * 16 + 7) / 8);
                    header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_RGB | DDS_PIXELFORMAT.Flags.DDPF_ALPHAPIXELS;
                    header.ddspf.rGBBitCount = 16;
                    header.ddspf.rBitMask = 0x7C00;
                    header.ddspf.gBitMask = 0x3E0;
                    header.ddspf.bBitMask = 0x1F;
                    header.ddspf.aBitMask = 0x8000;
                    break;
                case "4444":
                    header.flags |= DDS_HEADER.Flags.DDSD_PITCH;
                    header.pitchOrLinearSize = (uint)((ddt.Width * 16 + 7) / 8);
                    header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_RGB | DDS_PIXELFORMAT.Flags.DDPF_ALPHAPIXELS;
                    header.ddspf.rGBBitCount = 16;
                    header.ddspf.rBitMask = 0xF00;
                    header.ddspf.gBitMask = 0xF0;
                    header.ddspf.bBitMask = 0xF;
                    header.ddspf.aBitMask = 0xF000;
                    break;
            }
            if (ddt.mipMap > 1)
            {
                header.flags |= DDS_HEADER.Flags.DDSD_MIPMAPCOUNT;
                header.mipMapCount = (uint)(ddt.mipMap + 1);
                header.caps |= DDS_HEADER.Caps.DDSCAPS_MIPMAP | DDS_HEADER.Caps.DDSCAPS_COMPLEX;
            }
            header.reserved1 = new uint[11];
            header.ddspf.size = 32;
            header.caps |= DDS_HEADER.Caps.DDSCAPS_TEXTURE;
            bdata = ddt.imageData;
        }
        public Dds(PssgNode node, bool cubePreview)
        {
            magic = 0x20534444;
            header.size = 124;
            header.flags |= DDS_HEADER.Flags.DDSD_CAPS | DDS_HEADER.Flags.DDSD_HEIGHT | DDS_HEADER.Flags.DDSD_WIDTH | DDS_HEADER.Flags.DDSD_PIXELFORMAT;
            header.height = (uint)(node.attributes["height"].Value);
            header.width = (uint)(node.attributes["width"].Value);
            switch ((string)node.attributes["texelFormat"].Value)
            {
                case "dxt1":
                    header.flags |= DDS_HEADER.Flags.DDSD_LINEARSIZE;
                    header.pitchOrLinearSize = (uint)(Math.Max(1, (((uint)node.attributes["width"].Value) + 3) / 4) * 8);
                    header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_FOURCC;
                    header.ddspf.fourCC = BitConverter.ToUInt32(Encoding.UTF8.GetBytes(((string)node.attributes["texelFormat"].Value).ToUpper()), 0);
                    break;
                case "dxt2":
                case "dxt3":
                case "dxt4":
                case "dxt5":
                    header.flags |= DDS_HEADER.Flags.DDSD_LINEARSIZE;
                    header.pitchOrLinearSize = (uint)(Math.Max(1, (((uint)node.attributes["width"].Value) + 3) / 4) * 16);
                    header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_FOURCC;
                    header.ddspf.fourCC = BitConverter.ToUInt32(Encoding.UTF8.GetBytes(((string)node.attributes["texelFormat"].Value).ToUpper()), 0);
                    break;
                case "ui8x4":
                    header.flags |= DDS_HEADER.Flags.DDSD_LINEARSIZE;
                    header.pitchOrLinearSize = (uint)(Math.Max(1, (((uint)node.attributes["width"].Value) + 3) / 4) * 16); // is this right?
                    header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_ALPHAPIXELS | DDS_PIXELFORMAT.Flags.DDPF_RGB;
                    header.ddspf.fourCC = 0;
                    header.ddspf.rGBBitCount = 32;
                    header.ddspf.rBitMask = 0xFF0000;
                    header.ddspf.gBitMask = 0xFF00;
                    header.ddspf.bBitMask = 0xFF;
                    header.ddspf.aBitMask = 0xFF000000;
                    break;
                case "u8":
                    header.flags |= DDS_HEADER.Flags.DDSD_LINEARSIZE;
                    header.pitchOrLinearSize = (uint)(Math.Max(1, (((uint)node.attributes["width"].Value) + 3) / 4) * 16); // is this right?
                    // Interchanging the commented values will both work, not sure which is better
                    header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_LUMINANCE;
                    //header.ddspf.flags |= DDS_PIXELFORMAT.Flags.DDPF_ALPHA;
                    header.ddspf.fourCC = 0;
                    header.ddspf.rGBBitCount = 8;
                    header.ddspf.rBitMask = 0xFF;
                    //header.ddspf.aBitMask = 0xFF;
                    break;
            }
            if (node.attributes.ContainsKey("automipmap") == true && node.attributes.ContainsKey("numberMipMapLevels") == true)
            {
                if ((uint)node.attributes["automipmap"].Value == 0 && (uint)node.attributes["numberMipMapLevels"].Value > 0)
                {
                    header.flags |= DDS_HEADER.Flags.DDSD_MIPMAPCOUNT;
                    header.mipMapCount = (uint)((uint)node.attributes["numberMipMapLevels"].Value + 1);
                    header.caps |= DDS_HEADER.Caps.DDSCAPS_MIPMAP | DDS_HEADER.Caps.DDSCAPS_COMPLEX;
                }
            }
            header.reserved1 = new uint[11];
            header.ddspf.size = 32;
            header.caps |= DDS_HEADER.Caps.DDSCAPS_TEXTURE;
            List<PssgNode> textureImageBlocks = node.FindNodes("TEXTUREIMAGEBLOCK");
            if ((uint)node.attributes["imageBlockCount"].Value > 1)
            {
                bdata2 = new Dictionary<int, byte[]>();
                for (int i = 0; i < textureImageBlocks.Count; i++)
                {
                    switch (textureImageBlocks[i].attributes["typename"].ToString())
                    {
                        case "Raw":
                            header.caps2 |= DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEX;
                            bdata2.Add(0, textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data);
                            break;
                        case "RawNegativeX":
                            header.caps2 |= DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEX;
                            bdata2.Add(1, textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data);
                            break;
                        case "RawPositiveY":
                            header.caps2 |= DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEY;
                            bdata2.Add(2, textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data);
                            break;
                        case "RawNegativeY":
                            header.caps2 |= DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEY;
                            bdata2.Add(3, textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data);
                            break;
                        case "RawPositiveZ":
                            header.caps2 |= DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEZ;
                            bdata2.Add(4, textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data);
                            break;
                        case "RawNegativeZ":
                            header.caps2 |= DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEZ;
                            bdata2.Add(5, textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data);
                            break;
                    }
                }
                if (cubePreview == true)
                {
                    header.caps2 = 0;
                }
                else if (bdata2.Count == (uint)node.attributes["imageBlockCount"].Value)
                {
                    header.caps2 |= DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP;
                    header.flags = header.flags ^ DDS_HEADER.Flags.DDSD_LINEARSIZE;
                    header.pitchOrLinearSize = 0;
                    header.caps |= DDS_HEADER.Caps.DDSCAPS_COMPLEX;
                }
                else
                {
                    throw new Exception("Loading cubemap failed because not all blocks were found. (Read)");
                }
            }
            else
            {
                bdata = textureImageBlocks[0].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data;
            }
        }
        public Dds(System.IO.Stream fileStream)
        {
            using (System.IO.BinaryReader b = new System.IO.BinaryReader(fileStream))
            {
                b.BaseStream.Position = 12;
                header.height = b.ReadUInt32();
                header.width = b.ReadUInt32();
                b.BaseStream.Position += 8;
                header.mipMapCount = b.ReadUInt32();
                b.BaseStream.Position += 52;
                header.ddspf.fourCC = b.ReadUInt32();
                header.ddspf.rGBBitCount = b.ReadUInt32();
                b.BaseStream.Position += 20;
                header.caps2 = (DDS_HEADER.Caps2)b.ReadUInt32();
                b.BaseStream.Position += 12;
                int count = 0;
                if ((uint)header.caps2 != 0)
                {
                    bdata2 = new Dictionary<int, byte[]>();
                    if ((header.caps2 & DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEX) == DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEX)
                    {
                        count++;
                        bdata2.Add(0, null);
                    }
                    else
                    {
                        bdata2.Add(-1, null);
                    }
                    if ((header.caps2 & DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEX) == DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEX)
                    {
                        count++;
                        bdata2.Add(1, null);
                    }
                    else
                    {
                        bdata2.Add(-2, null);
                    }
                    if ((header.caps2 & DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEY) == DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEY)
                    {
                        count++;
                        bdata2.Add(2, null);
                    }
                    else
                    {
                        bdata2.Add(-3, null);
                    }
                    if ((header.caps2 & DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEY) == DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEY)
                    {
                        count++;
                        bdata2.Add(3, null);
                    }
                    else
                    {
                        bdata2.Add(-4, null);
                    }
                    if ((header.caps2 & DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEZ) == DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_POSITIVEZ)
                    {
                        count++;
                        bdata2.Add(4, null);
                    }
                    else
                    {
                        bdata2.Add(-5, null);
                    }
                    if ((header.caps2 & DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEZ) == DDS_HEADER.Caps2.DDSCAPS2_CUBEMAP_NEGATIVEZ)
                    {
                        count++;
                        bdata2.Add(5, null);
                    }
                    else
                    {
                        bdata2.Add(-6, null);
                    }
                    if (count > 0)
                    {
                        int length = (int)((b.BaseStream.Length - (long)128) / (long)count);
                        //System.Windows.Forms.MessageBox.Show(count.ToString() + "  " + length.ToString());
                        for (int i = 0; i < bdata2.Count; i++)
                        {
                            if (bdata2.ContainsKey(i) == true)
                            {
                                bdata2[i] = b.ReadBytes(length);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Loading cubemap failed because not all blocks were found. (Read)");
                    }
                }
                else
                {
                    bdata = b.ReadBytes((int)(b.BaseStream.Length - (long)128));
                }
            }
        }

        public void Write(System.IO.Stream fileStream, int cubeIndex)
        {
            using (System.IO.BinaryWriter b = new System.IO.BinaryWriter(fileStream))
            {
                b.Write(magic);
                b.Write(header.size);
                b.Write((uint)header.flags);
                b.Write(header.height);
                b.Write(header.width);
                b.Write(header.pitchOrLinearSize);
                b.Write(header.depth);
                b.Write(header.mipMapCount);
                foreach (uint u in header.reserved1)
                {
                    b.Write(u);
                }
                b.Write(header.ddspf.size);
                b.Write((uint)header.ddspf.flags);
                b.Write(header.ddspf.fourCC);
                b.Write(header.ddspf.rGBBitCount);
                b.Write(header.ddspf.rBitMask);
                b.Write(header.ddspf.gBitMask);
                b.Write(header.ddspf.bBitMask);
                b.Write(header.ddspf.aBitMask);
                b.Write((uint)header.caps);
                b.Write((uint)header.caps2);
                b.Write(header.caps3);
                b.Write(header.caps4);
                b.Write(header.reserved2);
                if (cubeIndex != -1)
                {
                    b.Write(bdata2[cubeIndex]);
                }
                else if (bdata2 != null && bdata2.Count > 0)
                {
                    for (int i = 0; i < bdata2.Count; i++)
                    {
                        if (bdata2.ContainsKey(i) == true)
                        {
                            b.Write(bdata2[i]);
                        }
                    }
                }
                else
                {
                    b.Write(bdata);
                }
            }
        }
        public void Write(PssgNode node)
        {
            node.attributes["height"].data = MiscUtil.Conversion.EndianBitConverter.Big.GetBytes(header.height);
            node.attributes["width"].data = MiscUtil.Conversion.EndianBitConverter.Big.GetBytes(header.width);
            if (node.attributes.ContainsKey("numberMipMapLevels") == true)
            {
                if ((int)header.mipMapCount - 1 >= 0)
                {
                    node.attributes["numberMipMapLevels"].data = MiscUtil.Conversion.EndianBitConverter.Big.GetBytes(header.mipMapCount - 1);
                }
                else
                {
                    node.attributes["numberMipMapLevels"].data = MiscUtil.Conversion.EndianBitConverter.Big.GetBytes(0);
                }
            }
            if (header.ddspf.rGBBitCount == 32)
            {
                node.attributes["texelFormat"].data = "ui8x4";
            }
            else if (header.ddspf.rGBBitCount == 8)
            {
                node.attributes["texelFormat"].data = "u8";
            }
            else
            {
                node.attributes["texelFormat"].data = Encoding.UTF8.GetString(BitConverter.GetBytes(header.ddspf.fourCC)).ToLower();
            }
            List<PssgNode> textureImageBlocks = node.FindNodes("TEXTUREIMAGEBLOCK");
            if (bdata2 != null && bdata2.Count > 0)
            {
                for (int i = 0; i < textureImageBlocks.Count; i++)
                {
                    switch (textureImageBlocks[i].attributes["typename"].ToString())
                    {
                        case "Raw":
                            if (bdata2.ContainsKey(0) == true)
                            {
                                textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data = bdata2[0];
                                textureImageBlocks[i].attributes["size"].data = EndianBitConverter.Big.GetBytes(bdata2[0].Length);
                            }
                            else
                            {
                                throw new Exception("Loading cubemap failed because not all blocks were found. (Write)");
                            }
                            break;
                        case "RawNegativeX":
                            if (bdata2.ContainsKey(1) == true)
                            {
                                textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data = bdata2[1];
                                textureImageBlocks[i].attributes["size"].data = EndianBitConverter.Big.GetBytes(bdata2[1].Length);
                            }
                            else
                            {
                                throw new Exception("Loading cubemap failed because not all blocks were found. (Write)");
                            }
                            break;
                        case "RawPositiveY":
                            if (bdata2.ContainsKey(2) == true)
                            {
                                textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data = bdata2[2];
                                textureImageBlocks[i].attributes["size"].data = EndianBitConverter.Big.GetBytes(bdata2[2].Length);
                            }
                            else
                            {
                                throw new Exception("Loading cubemap failed because not all blocks were found. (Write)");
                            }
                            break;
                        case "RawNegativeY":
                            if (bdata2.ContainsKey(3) == true)
                            {
                                textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data = bdata2[3];
                                textureImageBlocks[i].attributes["size"].data = EndianBitConverter.Big.GetBytes(bdata2[3].Length);
                            }
                            else
                            {
                                throw new Exception("Loading cubemap failed because not all blocks were found. (Write)");
                            }
                            break;
                        case "RawPositiveZ":
                            if (bdata2.ContainsKey(4) == true)
                            {
                                textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data = bdata2[4];
                                textureImageBlocks[i].attributes["size"].data = EndianBitConverter.Big.GetBytes(bdata2[4].Length);
                            }
                            else
                            {
                                throw new Exception("Loading cubemap failed because not all blocks were found. (Write)");
                            }
                            break;
                        case "RawNegativeZ":
                            if (bdata2.ContainsKey(5) == true)
                            {
                                textureImageBlocks[i].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data = bdata2[5];
                                textureImageBlocks[i].attributes["size"].data = EndianBitConverter.Big.GetBytes(bdata2[5].Length);
                            }
                            else
                            {
                                throw new Exception("Loading cubemap failed because not all blocks were found. (Write)");
                            }
                            break;
                    }
                }
            }
            else
            {
                if ((uint)node.attributes["imageBlockCount"].Value > 1)
                {
                    throw new Exception("Loading cubemap failed because not all blocks were found. (Write)");
                }
                textureImageBlocks[0].FindNodes("TEXTUREIMAGEBLOCKDATA")[0].data = bdata;
                textureImageBlocks[0].attributes["size"].data = EndianBitConverter.Big.GetBytes(bdata.Length);
            }
        }
    }
    #endregion
}