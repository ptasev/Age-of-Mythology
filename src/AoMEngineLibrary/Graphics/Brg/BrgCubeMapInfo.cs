namespace AoMEngineLibrary.Graphics.Brg
{
    using System;
    using System.Text;

    public class BrgCubeMapInfo : IEquatable<BrgCubeMapInfo>
    {
        public byte Mode { get; set; }
        public byte TextureFactor { get; set; }
        public byte CubeMapNameLength { get; private set; }
        public byte TextureMapNameLength { get; private set; }

        private string _cubeMapName;
        public string CubeMapName 
        { 
            get => _cubeMapName;
            set
            {
                _cubeMapName = value;
                CubeMapNameLength = (byte)Encoding.ASCII.GetByteCount(value);
            }
        }

        private string _textureMapName;
        public string TextureMapName
        {
            get => _textureMapName;
            set
            {
                _textureMapName = value;
                TextureMapNameLength = (byte)Encoding.ASCII.GetByteCount(value);
            }
        }

        public BrgCubeMapInfo()
        {
            Mode = 1;
            TextureFactor = 40;
            _cubeMapName = string.Empty;
            _textureMapName = string.Empty;
        }

        public bool Equals(BrgCubeMapInfo other)
        {
            if (other == null)
            {
                return false;
            }

            bool ret = this.Mode == other.Mode &&
                this.TextureFactor == other.TextureFactor &&
                this.CubeMapNameLength == other.CubeMapNameLength &&
                this.TextureMapNameLength == other.TextureMapNameLength &&
                this.CubeMapName == other.CubeMapName &&
                this.TextureMapName == other.TextureMapName;

            return ret;
        }
    }
}
