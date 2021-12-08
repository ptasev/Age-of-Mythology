using System;
using System.Text;

namespace AoMEngineLibrary.Graphics.Brg
{
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
                CubeMapNameLength = Convert.ToByte(Encoding.UTF8.GetByteCount(value));
                _cubeMapName = value;
            }
        }

        private string _textureMapName;
        public string TextureMapName
        {
            get => _textureMapName;
            set
            {
                TextureMapNameLength = Convert.ToByte(Encoding.UTF8.GetByteCount(value));
                _textureMapName = value;
            }
        }

        public BrgCubeMapInfo()
        {
            Mode = 1;
            TextureFactor = 30;
            _cubeMapName = string.Empty;
            _textureMapName = string.Empty;
        }

        public bool Equals(BrgCubeMapInfo? other)
        {
            if (other == null)
            {
                return false;
            }

            var ret = Mode == other.Mode &&
                TextureFactor == other.TextureFactor &&
                CubeMapNameLength == other.CubeMapNameLength &&
                TextureMapNameLength == other.TextureMapNameLength &&
                CubeMapName == other.CubeMapName &&
                TextureMapName == other.TextureMapName;

            return ret;
        }

        public override bool Equals(object? obj) => Equals(obj as BrgCubeMapInfo);

        public override int GetHashCode() => HashCode.Combine(Mode, TextureFactor, CubeMapName, TextureMapName);
    }
}
