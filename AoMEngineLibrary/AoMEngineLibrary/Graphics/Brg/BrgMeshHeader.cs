namespace AoMEngineLibrary.Graphics.Brg
{
    using System;

    public struct BrgMeshHeader
    {
        public Int16 version;
        public BrgMeshFormat format;
        public Int16 numVertices;
        public Int16 numFaces;
        public byte interpolationType;
        public BrgMeshAnimType properties;
        public Int16 userDataEntryCount;
        public Vector3<float> centerPos;
        public float centerRadius;
        public Vector3<float> position;
        public Vector3<float> groundPos;
        public Int16 extendedHeaderSize;
        public BrgMeshFlag flags;
        public Vector3<float> boundingBoxMin;
        public Vector3<float> boundingBoxMax;
    }
}
