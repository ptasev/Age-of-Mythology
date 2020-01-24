namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Grn.Nodes;
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnFile : ModelFile<GrnMesh, GrnMaterial, GrnAnimation>
    {
        public List<Dictionary<string, string>> DataExtensions { get; set; }
        public List<GrnBone> Bones { get; set; }

        public List<GrnTexture> Textures { get; set; }

        public GrnFile()
            : base()
        {
            this.DataExtensions = new List<Dictionary<string, string>>();
            this.Bones = new List<GrnBone>();
            this.Textures = new List<GrnTexture>();
        }

        public void DumpData(System.IO.Stream stream, string folderPath)
        {
            using (GrnBinaryReader reader = new GrnBinaryReader(stream))
            {
                Int64 magic = reader.ReadInt64();
                if (magic != 7380350958317416490)
                {
                    throw new Exception("This is not a GRN file!");
                }
                reader.ReadBytes(56);
                GrnNodeType nodeType = (GrnNodeType)reader.ReadInt32(); // should be FileDirectory
                GrnNode mainNode = GrnNode.ReadByNodeType(reader, null, nodeType);
                mainNode.CreateFolder(folderPath, 0);
            }
        }

        public void Read(System.IO.Stream stream)
        {
            using (GrnBinaryReader reader = new GrnBinaryReader(stream))
            {
                Int64 magic = reader.ReadInt64();
                if (magic != 7380350958317416490)
                {
                    throw new Exception("This is not a GRN file!");
                }
                reader.ReadBytes(56);
                GrnNodeType nodeType = (GrnNodeType)reader.ReadInt32(); // should be FileDirectory
                GrnNode mainNode = GrnNode.ReadByNodeType(reader, null, nodeType);
                //mainNode.CreateFolder(@"C:\Users\Petar\Desktop\Nieuwe map (3)\Output", 0);

                GrnSectionNode? dirNode = mainNode.FindNode<GrnSectionNode>(GrnNodeType.StandardFrameDirectory);
                if (dirNode == null) throw new InvalidDataException("Grn file has no standard frame directory node");
                uint directoryOffset = dirNode.Offset;

                // 0StringTable
                List<string>? strings = dirNode.FindNode<GrnStringTableNode>(GrnNodeType.StringTable)?.Strings;
                if (strings == null) throw new InvalidDataException("Grn file has no string table.");

                // 1DataExtension
                this.ReadDataExtension(strings, dirNode.FindNodes<GrnNode>(GrnNodeType.DataExtension));

                // 2VectorChannel

                // 3TransformChannel
                List<int> transformChannels = this.ReadTransformChannel(dirNode.ChildNodes[3].FindNodes<GrnNode>(GrnNodeType.TransformChannel));

                // 4Mesh
                List<GrnNode> meshes = dirNode.ChildNodes[4].FindNodes<GrnNode>(GrnNodeType.Mesh);
                for (int i = 0; i < meshes.Count; ++i)
                {
                    this.Meshes.Add(new GrnMesh(this));
                    this.Meshes[i].Read(meshes[i]);
                }

                // 5Skeleton
                List<GrnBoneNode> bones = dirNode.ChildNodes[5].FindNodes<GrnBoneNode>(GrnNodeType.Bone);
                for (int i = 0; i < bones.Count; ++i)
                {
                    this.Bones.Add(new GrnBone(this));
                    this.Bones[i].Read(bones[i]);
                }

                // 6Texture
                List<GrnNode> textureMaps = dirNode.ChildNodes[6].FindNodes<GrnNode>(GrnNodeType.TextureMap);
                for (int i = 0; i < textureMaps.Count; ++i)
                {
                    this.Textures.Add(new GrnTexture(this));
                    this.Textures[i].Read(textureMaps[i]);
                }

                // 7Material
                List<GrnNode> materials = dirNode.ChildNodes[7].FindNodes<GrnNode>(GrnNodeType.Material);
                for (int i = 0; i < materials.Count; ++i)
                {
                    this.Materials.Add(new GrnMaterial(this));
                    this.Materials[i].Read(materials[i]);
                }

                // 8Form
                GrnFormBoneChannelsNode? formBoneChannels = dirNode.ChildNodes[8].FindNode<GrnFormBoneChannelsNode>(GrnNodeType.FormBoneChannels);
                if (formBoneChannels == null) throw new InvalidDataException("Form node has no bone channels node.");
                this.ReadFormBoneChannel(transformChannels, formBoneChannels);
                List<GrnFormMeshNode> formMeshes = dirNode.ChildNodes[8].
                    FindNodes<GrnFormMeshNode>(GrnNodeType.FormMesh);
                List<int> meshLinks = new List<int>(formMeshes.Count);
                for (int i = 0; i < formMeshes.Count; ++i)
                {
                    meshLinks.Add(formMeshes[i].MeshIndex - 1);
                    this.Meshes[formMeshes[i].MeshIndex - 1].ReadFormMeshBones(formMeshes[i].
                        FindNodes<GrnFormMeshBoneNode>(GrnNodeType.FormMeshBone));
                }

                // 9Model
                List<GrnRenderPassNode> renderPass = 
                    dirNode.ChildNodes[9].FindNodes<GrnRenderPassNode>(GrnNodeType.RenderPass);
                for (int i = 0; i < renderPass.Count; ++i)
                {
                    int meshIndex = meshLinks[renderPass[i].FormMeshIndex];
                    int matIndex = renderPass[i].MaterialIndex - 1;
                    GrnRenderPassTrianglesNode? triNode =
                        renderPass[i].FindNode<GrnRenderPassTrianglesNode>(GrnNodeType.RenderPassTriangles);
                    if (triNode == null)
                        throw new InvalidDataException("Render pass node has no triangles node");
                    this.Meshes[meshIndex].ReadRenderPassTriangles(matIndex, triNode);
                }

                // 10Animation
                List<GrnAnimationTransformTrackKeysNode> animTransTrackKeys = 
                    dirNode.ChildNodes[10].FindNodes<GrnAnimationTransformTrackKeysNode>(
                    GrnNodeType.AnimationTransformTrackKeys);
                this.Animation.BoneTracks = new List<GrnBoneTrack>(animTransTrackKeys.Count);
                for (int i = 0; i < animTransTrackKeys.Count; i++)
                {
                    GrnAnimationTransformTrackKeysNode? prevSibling = ((GrnAnimationTransformTrackKeysNode?)animTransTrackKeys[i].PreviousSibling);
                    if (prevSibling != null && prevSibling.TransformChannelIndex == animTransTrackKeys[i].TransformChannelIndex)
                    {
                        animTransTrackKeys.RemoveAt(i);
                        --i;
                        continue;
                    }
                    this.Animation.BoneTracks.Add(new GrnBoneTrack());
                    this.Animation.BoneTracks[i].Read(transformChannels, animTransTrackKeys[i]);
                }
                this.CalculateAnimationDuration();
            }
        }
        private void ReadDataExtension(List<string> strings, List<GrnNode> dataExtensions)
        {
            for (int i = 0; i < dataExtensions.Count; ++i)
            {
                this.DataExtensions.Add(new Dictionary<string, string>());
                List<GrnDataExtensionPropertyNode> dataExtensionProperties = 
                    dataExtensions[i].FindNodes<GrnDataExtensionPropertyNode>(
                    GrnNodeType.DataExtensionProperty);
                foreach (GrnDataExtensionPropertyNode dataExtProp in dataExtensionProperties)
                {
                    GrnDataExtensionPropertyValueNode? dataExtensionPropertyValue = 
                        dataExtProp.FindNode<GrnDataExtensionPropertyValueNode>(
                        GrnNodeType.DataExtensionPropertyValue);
                    if (dataExtensionPropertyValue == null)
                        throw new InvalidDataException($"Data extension property has no value.");

                    this.DataExtensions[i].Add(strings[dataExtProp.StringTableIndex], 
                        strings[dataExtensionPropertyValue.StringTableIndex]);
                }
            }
        }
        private List<int> ReadTransformChannel(List<GrnNode> transformChannelNodes)
        {
            List<int> transformChannels = new List<int>(transformChannelNodes.Count);
            for (int i = 0; i < transformChannelNodes.Count; ++i)
            {
                var refNode = transformChannelNodes[i].FirstChild as GrnDataExtensionReferenceNode;
                if (refNode == null)
                    throw new InvalidDataException("Transform channel node has no data extension reference nodes.");
                transformChannels.Add(refNode.DataExtensionIndex - 1);
            }
            return transformChannels;
        }
        private void ReadFormBoneChannel(List<int> transformChannels, GrnFormBoneChannelsNode formBoneChannels)
        {
            for (int i = 0; i < this.Bones.Count; ++i)
            {
                this.Bones[i].DataExtensionIndex = 
                    transformChannels[formBoneChannels.TransformChannelIndices[i] - 1];
            }
        }
        private void CalculateAnimationDuration()
        {
            this.Animation.Duration = 0;
            this.Animation.TimeStep = 1f/30f;
            for (int i = 0; i < this.Animation.BoneTracks.Count; ++i)
            {
                //this.Animation.Duration = Math.Max(this.Animation.Duration, this.Animation.BoneTracks[i].PositionKeys.Last());
                if (this.Animation.BoneTracks[i].PositionKeys.LastOrDefault() > this.Animation.Duration)
                {
                    this.Animation.Duration = this.Animation.BoneTracks[i].PositionKeys.LastOrDefault();
                }
                if (this.Animation.BoneTracks[i].RotationKeys.LastOrDefault() > this.Animation.Duration)
                {
                    this.Animation.Duration = this.Animation.BoneTracks[i].RotationKeys.LastOrDefault();
                }
                if (this.Animation.BoneTracks[i].ScaleKeys.LastOrDefault() > this.Animation.Duration)
                {
                    this.Animation.Duration = this.Animation.BoneTracks[i].ScaleKeys.LastOrDefault();
                }
            }
        }

        public void Write(Stream stream)
        {
            using (GrnBinaryWriter writer = new GrnBinaryWriter(stream))
            {
                this.WriteHeader(writer);

                GrnMainNode mainNode = new GrnMainNode();
                mainNode.NumTotalChildNodes = 3;
                this.CreateVerFrameDir(mainNode);

                GrnSectionNode staFrameDir = new GrnSectionNode(mainNode, GrnNodeType.StandardFrameDirectory);
                staFrameDir.Offset = 440;
                mainNode.AppendChild(staFrameDir);

                // 0StringTable
                OrderedDictionary stringMap = this.CreateStringMap();
                GrnStringTableNode strTableNode = new GrnStringTableNode(staFrameDir);
                strTableNode.Strings = stringMap.Keys.Cast<string>().ToList();
                staFrameDir.AppendChild(strTableNode);

                // 1DataExtension
                GrnNode dataExtSecNode = new GrnNode(staFrameDir, GrnNodeType.DataExtensionSection);
                staFrameDir.AppendChild(dataExtSecNode);
                this.WriteDataExtensions(dataExtSecNode, stringMap);

                // 2VectorChannel
                GrnNode vecChanSecNode = new GrnNode(staFrameDir, GrnNodeType.VectorChannelSection);
                staFrameDir.AppendChild(vecChanSecNode);

                // 3TransformChannel
                GrnNode traChanSecNode = new GrnNode(staFrameDir, GrnNodeType.TransformChannelSection);
                staFrameDir.AppendChild(traChanSecNode);
                foreach (GrnBone bone in this.Bones)
                {
                    GrnNode traChanNode = new GrnNode(traChanSecNode, GrnNodeType.TransformChannel);
                    traChanSecNode.AppendChild(traChanNode);

                    GrnDataExtensionReferenceNode refNode = new GrnDataExtensionReferenceNode(traChanNode);
                    refNode.DataExtensionIndex = bone.DataExtensionIndex + 1;
                    traChanNode.AppendChild(refNode);
                }

                // 4Mesh
                GrnNode meshSecNode = new GrnNode(staFrameDir, GrnNodeType.MeshSection);
                staFrameDir.AppendChild(meshSecNode);
                foreach (GrnMesh mesh in this.Meshes)
                {
                    mesh.Write(meshSecNode);
                }

                // 5Skeleton
                GrnNode skelSecNode = new GrnNode(staFrameDir, GrnNodeType.SkeletonSection);
                staFrameDir.AppendChild(skelSecNode);
                GrnNode skelNode = new GrnNode(skelSecNode, GrnNodeType.Skeleton);
                skelSecNode.AppendChild(skelNode);
                GrnNode boneSecNode = new GrnNode(skelNode, GrnNodeType.BoneSection);
                skelNode.AppendChild(boneSecNode);
                foreach (GrnBone bone in this.Bones)
                {
                    bone.Write(boneSecNode);
                }

                // 6Texture
                GrnNode texSecNode = new GrnNode(staFrameDir, GrnNodeType.TextureSection);
                staFrameDir.AppendChild(texSecNode);
                foreach (GrnTexture tex in this.Textures)
                {
                    tex.Write(texSecNode);
                }

                // 7Material
                GrnNode matSecNode = new GrnNode(staFrameDir, GrnNodeType.MaterialSection);
                staFrameDir.AppendChild(matSecNode);
                foreach (GrnMaterial mat in this.Materials)
                {
                    mat.Write(matSecNode);
                }

                // 8Form
                GrnNode formSecNode = new GrnNode(staFrameDir, GrnNodeType.FormSection);
                staFrameDir.AppendChild(formSecNode);
                this.WriteForm(formSecNode);

                // 9Model
                GrnNode modelSecNode = new GrnNode(staFrameDir, GrnNodeType.ModelSection);
                staFrameDir.AppendChild(modelSecNode);
                GrnNode modelNode = new GrnNode(modelSecNode, GrnNodeType.Model);
                modelNode.Data = new byte[] { 0x01, 0x00, 0x00, 0x00 };
                modelSecNode.AppendChild(modelNode);
                GrnNode rendPassSecNode = new GrnNode(modelNode, GrnNodeType.RenderPassSection);
                modelNode.AppendChild(rendPassSecNode);
                for (int i = 0; i < this.Meshes.Count; ++i)
                {
                    this.Meshes[i].WriteRenderPass(rendPassSecNode, i);
                }

                // 10Animation
                GrnNode animSecNode = new GrnNode(staFrameDir, GrnNodeType.AnimationSection);
                staFrameDir.AppendChild(animSecNode);
                GrnNode animNode = new GrnNode(animSecNode, GrnNodeType.Animation);
                animSecNode.AppendChild(animNode);
                GrnNode animVecTraSecNode = new GrnNode(animNode, GrnNodeType.AnimationVectorTrackSection);
                animNode.AppendChild(animVecTraSecNode);
                GrnNode animTraTraSecNode = new GrnNode(animNode, GrnNodeType.AnimationTransformTrackSection);
                animNode.AppendChild(animTraTraSecNode);
                for (int i = 0; i < this.Animation.BoneTracks.Count; ++i)
                {
                    this.Animation.BoneTracks[i].Write(animTraTraSecNode, i);
                }

                // 11NullTerminator
                GrnNode nullTermNode = new GrnNode(staFrameDir, GrnNodeType.NullTerminator);
                staFrameDir.AppendChild(nullTermNode);

                GrnSectionNode nullFrameDir = new GrnSectionNode(mainNode, GrnNodeType.NullFrameDirectory);
                mainNode.AppendChild(nullFrameDir);
                nullTermNode = new GrnNode(nullFrameDir, GrnNodeType.NullTerminator);
                nullFrameDir.AppendChild(nullTermNode);

                mainNode.Write(writer);
            }
        }
        private void WriteHeader(GrnBinaryWriter writer)
        {
            writer.Write(7380350958317416490);
            writer.Write(8845441965100526989);
            writer.Write(3613614802009591920);
            writer.Write(2261131331893869605);
            writer.Write(2261131331893869605);
            writer.Write(7053895580311513156);
            writer.Write(2138146631217928280);
            writer.Write(4032492294667058554);
        }
        private void CreateVerFrameDir(GrnNode mainNode)
        {
            GrnSectionNode verFrameDir = new GrnSectionNode(null, GrnNodeType.VersionFrameDirectory);
            verFrameDir.Offset = 156;
            verFrameDir.NumTotalChildNodes = 6;
            mainNode.AppendChild(verFrameDir);

            GrnStringTableNode strNode = new GrnStringTableNode(verFrameDir);
            strNode.Offset = 88;
            strNode.Strings.Add(string.Empty);
            strNode.Strings.Add("RAD 3D Studio MAX 4.x");
            strNode.Strings.Add("1.2b");
            strNode.Strings.Add("10-4-2000");
            strNode.Strings.Add("win32");
            strNode.Strings.Add("(C) Copyright 1999-2000 RAD Game Tools, Inc.  All Rights Reserved.");
            verFrameDir.AppendChild(strNode);

            GrnNode verSect = new GrnNode(verFrameDir, GrnNodeType.VersionSection);
            verSect.Offset = 208;
            verSect.NumTotalChildNodes = 3;
            verFrameDir.AppendChild(verSect);

            GrnNode expVer = new GrnNode(verSect, GrnNodeType.ExporterVersion);
            expVer.Offset = 208;
            expVer.NumTotalChildNodes = 1;
            expVer.Data = new byte[] { 0x01, 0x00, 0x00, 0x00, 
                    0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00 };
            verSect.AppendChild(expVer);

            GrnNode maxSys = new GrnNode(expVer, GrnNodeType.ModelerAxisSystem);
            maxSys.Offset = 220;
            maxSys.Data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0xBF,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00};
            expVer.AppendChild(maxSys);

            GrnNode runVer = new GrnNode(verSect, GrnNodeType.RuntimeVersion);
            runVer.Offset = 268;
            runVer.Data = new byte[] { 0x02, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
                    0x03, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00};
            verSect.AppendChild(runVer);

            GrnNode nullTer = new GrnNode(verFrameDir, GrnNodeType.NullTerminator);
            nullTer.Offset = 284;
            verFrameDir.AppendChild(nullTer);
        }
        private OrderedDictionary CreateStringMap()
        {
            OrderedDictionary stringMap = new OrderedDictionary();

            stringMap.Add(string.Empty, 0);
            stringMap.Add("__Standard", 1);
            foreach (Dictionary<string, string> dataExt in this.DataExtensions)
            {
                foreach (KeyValuePair<string, string> dataExtProp in dataExt)
                {
                    if (!stringMap.Contains(dataExtProp.Key))
                    {
                        stringMap.Add(dataExtProp.Key, stringMap.Count);
                    }

                    if (!stringMap.Contains(dataExtProp.Value))
                    {
                        stringMap.Add(dataExtProp.Value, stringMap.Count);
                    }
                }
            }

            return stringMap;
        }
        private void WriteDataExtensions(GrnNode dataExtSecNode, OrderedDictionary stringMap)
        {
            foreach (Dictionary<string, string> dataExt in this.DataExtensions)
            {
                GrnNode dataExtNode = new GrnNode(dataExtSecNode, GrnNodeType.DataExtension);
                dataExtNode.Data = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
                dataExtSecNode.AppendChild(dataExtNode);

                GrnNode datExtPropSecNode = new GrnNode(dataExtNode, GrnNodeType.DataExtensionPropertySection);
                dataExtNode.AppendChild(datExtPropSecNode);

                foreach (KeyValuePair<string, string> deProp in dataExt)
                {
                    GrnDataExtensionPropertyNode dePropNode = new GrnDataExtensionPropertyNode(datExtPropSecNode);
                    dePropNode.StringTableIndex = (int)stringMap[deProp.Key];
                    datExtPropSecNode.AppendChild(dePropNode);

                    GrnNode deValSection = new GrnNode(dePropNode, GrnNodeType.DataExtensionValueSection);
                    dePropNode.AppendChild(deValSection);

                    GrnDataExtensionPropertyValueNode dePValNode = new GrnDataExtensionPropertyValueNode(deValSection);
                    dePValNode.StringTableIndex = (int)stringMap[deProp.Value];
                    deValSection.AppendChild(dePValNode);
                }
            }
        }
        private List<int> CreateFormBoneChannels()
        {
            List<int> fBC = new List<int>(this.Bones.Count);
            for (int i = 1; i <= this.Bones.Count; ++i)
            {
                fBC.Add(i);
            }
            return fBC;
        }
        private void WriteForm(GrnNode formSecNode)
        {
            GrnNode formNode = new GrnNode(formSecNode, GrnNodeType.Form);
            formSecNode.AppendChild(formNode);

            GrnNode formSkelSecNode = new GrnNode(formNode, GrnNodeType.FormSkeletonSection);
            formNode.AppendChild(formSkelSecNode);
            GrnNode formSkelNode = new GrnNode(formSkelSecNode, GrnNodeType.FormSkeleton);
            formSkelNode.Data = new byte[] { 0x01, 0x00, 0x00, 0x00 };
            formSkelSecNode.AppendChild(formSkelNode);
            GrnFormBoneChannelsNode fBoneChaNode = new GrnFormBoneChannelsNode(formSkelNode);
            fBoneChaNode.TransformChannelIndices = this.CreateFormBoneChannels();
            formSkelNode.AppendChild(fBoneChaNode);
            GrnNode fPoseWeightsNode = new GrnNode(formSkelNode, GrnNodeType.FormPoseWeights);
            formSkelNode.AppendChild(fPoseWeightsNode);

            GrnNode formMeshSecNode = new GrnNode(formNode, GrnNodeType.FormMeshSection);
            formNode.AppendChild(formMeshSecNode);
            for (int i = 0; i < this.Meshes.Count; ++i)
            {
                GrnFormMeshNode fMeshNode = new GrnFormMeshNode(formMeshSecNode);
                fMeshNode.MeshIndex = i + 1;
                formMeshSecNode.AppendChild(fMeshNode);
                GrnNode fVertSetWeiNode = new GrnNode(fMeshNode, GrnNodeType.FormVertexSetWeights);
                fMeshNode.AppendChild(fVertSetWeiNode);
                GrnNode fMeshBoneSecNode = new GrnNode(fMeshNode, GrnNodeType.FormMeshBoneSection);
                fMeshNode.AppendChild(fMeshBoneSecNode);
                this.Meshes[i].WriteFormMeshBones(fMeshBoneSecNode);
            }
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
            string propValue;
            if (this.DataExtensions[dataExtensionIndex].TryGetValue(property, out propValue))
            {
                return propValue;
            }
            else
            {
                return string.Empty;
            }
        }
        public void SetDataExtensionFileName(int dataExtensionIndex, string value)
        {
            this.SetDataExtensionProperty(dataExtensionIndex, "__FileName", value);
        }
        public void SetDataExtensionProperty(int dataExtensionIndex, string property, string value)
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
