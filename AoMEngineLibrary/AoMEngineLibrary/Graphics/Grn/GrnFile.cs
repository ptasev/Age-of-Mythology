namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnFile : ModelFile<GrnMesh, GrnMaterial, GrnAnimation>
    {
        public List<Dictionary<string, string>> DataExtensions { get; set; }
        public List<GrnBone> Bones { get; set; }

        public GrnFile()
            : base()
        {
            this.DataExtensions = new List<Dictionary<string, string>>();
            this.Bones = new List<GrnBone>();
        }

        public void DumpData(System.IO.Stream stream, string folderPath)
        {
            using (GrnBinaryReader reader = new GrnBinaryReader(stream))
            {
                reader.ReadBytes(64);
                GrnNodeType nodeType = (GrnNodeType)reader.ReadInt32(); // should be FileDirectory
                GrnNode mainNode = GrnNode.ReadByNodeType(reader, null, nodeType);
                mainNode.ReadData(reader, 0);
                mainNode.CreateFolder(folderPath, 0);
            }
        }

        public void Read(System.IO.Stream stream)
        {
            using (GrnBinaryReader reader = new GrnBinaryReader(stream))
            {
                reader.ReadBytes(64);
                GrnNodeType nodeType = (GrnNodeType)reader.ReadInt32(); // should be FileDirectory
                GrnNode mainNode = GrnNode.ReadByNodeType(reader, null, nodeType);
                mainNode.ReadData(reader, 0);
                mainNode.CreateFolder(@"C:\Users\Petar\Desktop\Nieuwe map (3)\Output", 0);

                GrnSectionNode dirNode = mainNode.FindNodes(GrnNodeType.StandardFrameDirectory)[0] as GrnSectionNode;
                uint directoryOffset = dirNode.Offset;

                // 0StringTable
                List<string> strings = this.ReadStringTable(reader, dirNode.FindNodes(GrnNodeType.StringTable)[0], directoryOffset);

                // 1DataExtension
                this.ReadDataExtension(reader, strings, dirNode.FindNodes(GrnNodeType.DataExtension), directoryOffset);

                // 2VectorChannel

                // 3TransformChannel
                List<int> transformChannels = this.ReadTransformChannel(reader, dirNode.ChildNodes[3].FindNodes(GrnNodeType.TransformChannel), directoryOffset);

                // 4Mesh
                List<GrnNode> meshes = dirNode.ChildNodes[4].FindNodes(GrnNodeType.Mesh);
                for (int i = 0; i < meshes.Count; ++i)
                {
                    this.Meshes.Add(new GrnMesh(this));
                    this.Meshes[i].Read(reader, meshes[i], directoryOffset);
                }

                // 5Skeleton
                List<GrnNode> bones = dirNode.ChildNodes[5].FindNodes(GrnNodeType.Bone);
                for (int i = 0; i < bones.Count; ++i)
                {
                    this.Bones.Add(new GrnBone(this));
                    this.Bones[i].Read(reader, bones[i], directoryOffset);
                }

                // 6Texture
                // -- Each TextureMap has width height, depth, and dataExtRef
                List<int> textureDataRefExts = new List<int>();
                List<GrnNode> textureMap = dirNode.ChildNodes[6].FindNodes(GrnNodeType.DataExtensionReference);
                for (int i = 0; i < textureMap.Count; ++i)
                {
                    reader.Seek((int)(textureMap[i].Offset + directoryOffset), SeekOrigin.Begin);
                    textureDataRefExts.Add(reader.ReadInt32() - 1);
                }

                // 7Material
                List<GrnNode> materials = dirNode.ChildNodes[7].FindNodes(GrnNodeType.Material);
                for (int i = 0; i < materials.Count; ++i)
                {
                    this.Materials.Add(new GrnMaterial(this));
                    this.Materials[i].Read(reader, textureDataRefExts, materials[i], directoryOffset);
                }

                // 8Form
                this.ReadFormBoneChannel(reader, transformChannels, dirNode.ChildNodes[8].FindNodes(GrnNodeType.FormBoneChannels)[0], directoryOffset);
                List<GrnNode> formMeshes = dirNode.ChildNodes[8].FindNodes(GrnNodeType.FormMesh);
                List<int> meshLinks = new List<int>(formMeshes.Count);
                for (int i = 0; i < formMeshes.Count; ++i)
                {
                    reader.Seek((int)(formMeshes[i].Offset + directoryOffset), SeekOrigin.Begin);
                    int meshIndex = reader.ReadInt32() - 1;
                    meshLinks.Add(meshIndex);
                    this.Meshes[meshIndex].ReadFormMeshBones(reader, formMeshes[i].FindNodes(GrnNodeType.FormMeshBone), directoryOffset);
                }

                // 9Model
                List<GrnNode> renderPass = dirNode.ChildNodes[9].FindNodes(GrnNodeType.RenderPass);
                for (int i = 0; i < renderPass.Count; ++i)
                {
                    reader.Seek((int)(renderPass[i].Offset + directoryOffset), SeekOrigin.Begin);
                    int meshIndex = meshLinks[reader.ReadInt32()];
                    int matIndex = reader.ReadInt32() - 1;
                    this.Meshes[meshIndex].ReadRenderPassTriangles(reader, matIndex, renderPass[i].FindNodes(GrnNodeType.RenderPassTriangles)[0], directoryOffset);
                }

                // 10Animation
                List<GrnNode> animTransTrackKeys = dirNode.ChildNodes[10].FindNodes(GrnNodeType.AnimationTransformTrackKeys);
                this.Animation.BoneTracks = new List<GrnBoneTrack>(animTransTrackKeys.Count);
                for (int i = 0; i < animTransTrackKeys.Count; i++)
                {
                    this.Animation.BoneTracks.Add(new GrnBoneTrack());
                    this.Animation.BoneTracks[i].Read(reader, transformChannels, animTransTrackKeys[i], directoryOffset);
                }
                this.CalculateAnimationDuration();
            }
        }
        private List<string> ReadStringTable(GrnBinaryReader reader, GrnNode stringTable, uint directoryOffset)
        {
            using (MemoryStream stream = new MemoryStream())
            reader.Seek((int)(stringTable.Offset + directoryOffset), SeekOrigin.Begin);
            Int32 numString = reader.ReadInt32();
            Int32 stringDataLength = reader.ReadInt32();

            List<string> strings = new List<string>(numString);
            for (int i = 0; i < numString; i++)
            {
                strings.Add(reader.ReadString());
            }

            //reader.ReadBytes((-stringDataLength) & 3); // padding
            return strings;
        }
        private void ReadDataExtension(GrnBinaryReader reader, List<string> strings, List<GrnNode> dataExtensions, uint directoryOffset)
        {
            for (int i = 0; i < dataExtensions.Count; ++i)
            {
                this.DataExtensions.Add(new Dictionary<string, string>());
                List<GrnNode> dataExtensionProperties = dataExtensions[i].FindNodes(GrnNodeType.DataExtensionProperty);
                foreach (GrnNode dataExtProp in dataExtensionProperties)
                {
                    GrnNode dataExtensionPropertyValue = dataExtProp.FindNodes(GrnNodeType.DataExtensionPropertyValue)[0];
                    reader.Seek((int)(dataExtProp.Offset + directoryOffset), SeekOrigin.Begin);
                    int key = reader.ReadInt32();
                    reader.Seek((int)(dataExtensionPropertyValue.Offset + directoryOffset), SeekOrigin.Begin);
                    reader.ReadInt32(); // skip 4
                    int value = reader.ReadInt32();
                    this.DataExtensions[i].Add(strings[key], strings[value]);
                }
            }
        }
        private List<int> ReadTransformChannel(GrnBinaryReader reader, List<GrnNode> transformChannelNodes, uint directoryOffset)
        {
            List<int> transformChannels = new List<int>(transformChannelNodes.Count);
            for (int i = 0; i < transformChannelNodes.Count; ++i)
            {
                reader.Seek((int)(transformChannelNodes[i].FirstChild.Offset + directoryOffset), SeekOrigin.Begin);
                transformChannels.Add(reader.ReadInt32() - 1);
            }
            return transformChannels;
        }
        private void ReadFormBoneChannel(GrnBinaryReader reader, List<int> transformChannels, GrnNode formBoneChannels, uint directoryOffset)
        {
            reader.Seek((int)(formBoneChannels.Offset + directoryOffset), SeekOrigin.Begin);
            for (int i = 0; i < formBoneChannels.GetDataLength() / 4; ++i)
            {
                this.Bones[i].DataExtensionIndex = transformChannels[reader.ReadInt32() - 1];
            }
        }
        private void CalculateAnimationDuration()
        {
            this.Animation.Duration = 0;
            this.Animation.TimeStep = 1;
            int maxKeys = 0;
            float minStep = 1;
            for (int i = 0; i < this.Animation.BoneTracks.Count; ++i)
            {
                if (this.Animation.BoneTracks[i].PositionKeys.Last() > this.Animation.Duration)
                {
                    this.Animation.Duration = this.Animation.BoneTracks[i].PositionKeys.Last();
                }
                if (this.Animation.BoneTracks[i].RotationKeys.Last() > this.Animation.Duration)
                {
                    this.Animation.Duration = this.Animation.BoneTracks[i].RotationKeys.Last();
                }
                if (this.Animation.BoneTracks[i].ScaleKeys.Last() > this.Animation.Duration)
                {
                    this.Animation.Duration = this.Animation.BoneTracks[i].ScaleKeys.Last();
                }

                if (this.Animation.BoneTracks[i].PositionKeys[1] < this.Animation.TimeStep)
                {
                    this.Animation.TimeStep = this.Animation.BoneTracks[i].PositionKeys[1];
                }
                if (this.Animation.BoneTracks[i].RotationKeys[1] < this.Animation.TimeStep)
                {
                    this.Animation.TimeStep = this.Animation.BoneTracks[i].RotationKeys[1];
                }
                if (this.Animation.BoneTracks[i].ScaleKeys[1] < this.Animation.TimeStep)
                {
                    this.Animation.TimeStep = this.Animation.BoneTracks[i].ScaleKeys[1];
                }

                maxKeys = Math.Max(maxKeys, this.Animation.BoneTracks[i].PositionKeys.Count);
                maxKeys = Math.Max(maxKeys, this.Animation.BoneTracks[i].RotationKeys.Count);
                maxKeys = Math.Max(maxKeys, this.Animation.BoneTracks[i].ScaleKeys.Count);

                for (int j = 1; j < this.Animation.BoneTracks[i].PositionKeys.Count; ++j)
                {
                    minStep = Math.Min(minStep, this.Animation.BoneTracks[i].PositionKeys[j] - this.Animation.BoneTracks[i].PositionKeys[j - 1]);
                }
                for (int j = 1; j < this.Animation.BoneTracks[i].RotationKeys.Count; ++j)
                {
                    minStep = Math.Min(minStep, this.Animation.BoneTracks[i].RotationKeys[j] - this.Animation.BoneTracks[i].RotationKeys[j - 1]);
                }
                for (int j = 1; j < this.Animation.BoneTracks[i].ScaleKeys.Count; ++j)
                {
                    minStep = Math.Min(minStep, this.Animation.BoneTracks[i].ScaleKeys[j] - this.Animation.BoneTracks[i].ScaleKeys[j - 1]);
                }
            }
            maxKeys = Convert.ToInt32(this.Animation.TimeStep * this.Animation.Duration);
        }

        public int AddDataExtension(string objectName)
        {
            //int result = this.GetDataExtensionIndex(objectName);
            //if (result > -1)
            //{
            //    return result;
            //}

            Dictionary<string, string> dataExtension = new Dictionary<string, string>();
            dataExtension.Add("__ObjectName", objectName);
            this.DataExtensions.Add(dataExtension);
            return this.DataExtensions.Count - 1;
        }
        //public int GetDataExtensionIndex(string objectName)
        //{
        //    for (int i = 0; i < this.DataExtensions.Count; ++i)
        //    {
        //        if (this.GetDataExtensionObjectName(i) == objectName)
        //        {
        //            return i;
        //        }
        //    }

        //    return -1;
        //}
        public string GetDataExtensionObjectName(int dataExtensionIndex)
        {
            return this.GetDataExtensionProperty(dataExtensionIndex, "__ObjectName");
        }
        public string GetDataExtensionFileName(int dataExtensionIndex)
        {
            return this.GetDataExtensionProperty(dataExtensionIndex, "__FileName");
        }
        public string GetDataExtensionProperty(int dataExtensionIndex, string property)
        {
            foreach (KeyValuePair<string, string> dataExtProp in this.DataExtensions[dataExtensionIndex])
            {
                if (dataExtProp.Key == property)
                {
                    return dataExtProp.Value;
                }
            }

            return string.Empty;
        }
        public void SetDataExtensionFileName(int dataExtensionIndex, string value)
        {
            this.SetDataExtensionProperty(dataExtensionIndex, "__FileName", value);
        }
        public void SetDataExtensionProperty(int dataExtensionIndex, string property, string value)
        {
            for (int i = 0; i < this.DataExtensions[dataExtensionIndex].Count; ++i)
            {
                if (this.DataExtensions[dataExtensionIndex].ContainsKey(property))
                {
                    this.DataExtensions[dataExtensionIndex][property] = value;
                }
                else
                {
                    this.DataExtensions[dataExtensionIndex].Add(property, value);
                }
            }
        }
    }
}
