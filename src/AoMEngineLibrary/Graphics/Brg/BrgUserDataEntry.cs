namespace AoMEngineLibrary.Graphics.Brg
{
    using Extensions;
    using Newtonsoft.Json;
    using System;

    public struct BrgUserDataEntry
    {
        public int dataNameLength;
        public int dataType;
        public object data;
        public string dataName;
    }
}
