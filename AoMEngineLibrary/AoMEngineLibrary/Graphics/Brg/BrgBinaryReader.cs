namespace AoMEngineLibrary.Graphics.Brg
{
    using MiscUtil.Conversion;
    using MiscUtil.IO;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class BrgBinaryReader : EndianBinaryReader
    {
        public BrgBinaryReader(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
        {
        }

        public void ReadHeader(ref BrgHeader header)
        {
            header.unknown01 = this.ReadInt32();
            header.numMaterials = this.ReadInt32();
            header.unknown02 = this.ReadInt32();
            header.numMeshes = this.ReadInt32();
            header.space = this.ReadInt32();
            header.unknown03 = this.ReadInt32();
        }
        public void ReadAsetHeader(ref BrgAsetHeader header)
        {
            header.numFrames = this.ReadInt32();
            header.frameStep = this.ReadSingle();
            header.animTime = this.ReadSingle();
            header.frequency = this.ReadSingle();
            header.spf = this.ReadSingle();
            header.fps = this.ReadSingle();
            header.space = this.ReadInt32();
        }
        public void ReadMeshHeader(ref BrgMeshHeader header)
        {
            header.version = this.ReadInt16();
            header.format = (BrgMeshFormat)this.ReadInt16();
            header.numVertices = this.ReadInt16();
            header.numFaces = this.ReadInt16();
            header.interpolationType = this.ReadByte();
            header.properties = (BrgMeshAnimType)this.ReadByte();
            header.userDataEntryCount = this.ReadInt16();
            this.ReadVector3(out header.centerPos, true, false);
            header.centerRadius = this.ReadSingle();
            this.ReadVector3(out header.position, true, false);
            this.ReadVector3(out header.groundPos, true, false);
            header.extendedHeaderSize = this.ReadInt16();
            header.flags = (BrgMeshFlag)this.ReadInt16();
            this.ReadVector3(out header.boundingBoxMin, true);
            this.ReadVector3(out header.boundingBoxMax, true);
        }
        public void ReadMeshExtendedHeader(ref BrgMeshExtendedHeader header, int extendedHeaderSize)
        {
            header.numIndex = this.ReadInt16();
            header.numMatrix = this.ReadInt16();
            header.nameLength = this.ReadInt16();
            if (extendedHeaderSize > 6)
            {
                header.pointMaterial = this.ReadInt16();
                header.pointRadius = this.ReadSingle();
            }
            if (extendedHeaderSize > 12)
            {
                header.materialCount = this.ReadByte();
                header.shadowNameLength0 = this.ReadByte();
                header.shadowNameLength1 = this.ReadByte();
                header.shadowNameLength2 = this.ReadByte();
            }
            if (extendedHeaderSize > 16)
            {
                header.animTime = this.ReadSingle();
            }
            if (extendedHeaderSize > 20)
            {
                header.materialLibraryTimestamp = this.ReadInt32();
            }
            if (extendedHeaderSize > 24)
            {
                //this.ReadInt16(); //09a checkSpace
                header.unknown09e = this.ReadSingle();
            }
            if (extendedHeaderSize > 28)
            {
                header.exportedScaleFactor = this.ReadSingle();
            }
            if (extendedHeaderSize > 32)
            {
                header.nonUniformKeyCount = this.ReadInt32(); //09c
            }
            if (extendedHeaderSize > 36)
            {
                header.uniqueMaterialCount = this.ReadInt32();
            }

            //animTime = 0f;
            //materialLibraryTimestamp = 0;
            //unknown09e = 0f;
            //exportedScaleFactor = 1f;
            //lenSpace = 0;
            //uniqueMaterialCount = 0;
        }
        public BrgUserDataEntry ReadUserDataEntry(bool isParticle)
        {
            BrgUserDataEntry dataEntry;

            dataEntry.dataNameLength = this.ReadInt32();
            dataEntry.dataType = this.ReadInt32();
            switch (dataEntry.dataType)
            {
                case 1:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength + (int)dataEntry.data);
                    break;
                case 2:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
                case 3:
                    dataEntry.data = this.ReadSingle();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
                default:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
            }

            return dataEntry;
        }
        public BrgMatSFX ReadMaterialSFX()
        {
            BrgMatSFX sfx;

            sfx.Id = this.ReadByte();
            sfx.Name = this.ReadString(this.ReadInt16());

            return sfx;
        }

        #region ReadVector3
        public void ReadVector3(out Vector3<float> v, bool isAom = true, bool isHalf = false)
        {
            if (isAom)
            {
                if (!isHalf)
                {
                    v.X = this.ReadSingle();
                    v.Y = this.ReadSingle();
                    v.Z = this.ReadSingle();
                }
                else
                {
                    v.X = this.ReadHalf();
                    v.Y = this.ReadHalf();
                    v.Z = this.ReadHalf();
                }
            }
            else
            {
                if (!isHalf)
                {
                    v.X = -this.ReadSingle();
                    v.Z = -this.ReadSingle();
                    v.Y = this.ReadSingle();
                }
                else
                {
                    v.X = -this.ReadHalf();
                    v.Z = -this.ReadHalf();
                    v.Y = this.ReadHalf();
                }
            }
        }
        public void ReadVector3(out Vector3<Int16> v, bool isAom = true)
        {
            if (isAom)
            {
                v.X = this.ReadInt16();
                v.Y = this.ReadInt16();
                v.Z = this.ReadInt16();
            }
            else
            {
                v.X = (Int16)(-this.ReadInt16());
                v.Z = (Int16)(-this.ReadInt16());
                v.Y = this.ReadInt16();
            }
        }
        #endregion

        #region ReadVector2
        public void ReadVector2(out Vector2<float> v, bool isHalf = false)
        {
            if (!isHalf)
            {
                v.X = this.ReadSingle();
                v.Y = this.ReadSingle();
            }
            else
            {
                v.X = this.ReadHalf();
                v.Y = this.ReadHalf();
            }
        }
        #endregion

        public float ReadHalf()
        {
            byte[] f = new byte[4];
            byte[] h = this.ReadBytes(2);
            f[2] = h[0];
            f[3] = h[1];
            return EndianBitConverter.Little.ToSingle(f, 0);
        }
        public void ReadVertexColor(out VertexColor color)
        {
            color.R = this.ReadByte();
            color.G = this.ReadByte();
            color.B = this.ReadByte();
            color.A = this.ReadByte();
        }
        public string ReadString(byte terminator = 0x0)
        {
            string filename = "";
            List<byte> fnBytes = new List<byte>();
            byte filenameByte = this.ReadByte();
            while (filenameByte != terminator)
            {
                filename += (char)filenameByte;
                fnBytes.Add(filenameByte);
                filenameByte = this.ReadByte();
            }
            return Encoding.UTF8.GetString(fnBytes.ToArray());
        }
        public string ReadString(int length)
        {
            return Encoding.UTF8.GetString(this.ReadBytes(length));
        }
    }
}
