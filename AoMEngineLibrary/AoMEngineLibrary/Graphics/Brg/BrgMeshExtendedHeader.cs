namespace AoMEngineLibrary.Graphics.Brg
{
    using System;

    public struct BrgMeshExtendedHeader
    {
        public Int16 numIndex;
        public Int16 numMatrix;
        public Int16 nameLength; // unknown091
        public Int16 pointMaterial;
        public float pointRadius; // unknown09Unused
        public byte materialCount; // lastMaterialIndex
        public byte shadowNameLength0;
        public byte shadowNameLength1;
        public byte shadowNameLength2;
        public float animTime;
        public int materialLibraryTimestamp; // unknown09Const
        //public Int16 checkSpace; //09a
        public float unknown09e;
        public float exportedScaleFactor; // animTimeMult
        public int nonUniformKeyCount; //09c
        public int uniqueMaterialCount; // numMaterialsUsed
    }
}
