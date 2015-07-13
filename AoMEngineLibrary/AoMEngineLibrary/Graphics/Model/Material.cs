namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Material
    {
        public Color3D DiffuseColor
        {
            get
            {
                return diffuseColor;
            }
            set
            {
                diffuseColor = value;
            }
        }
        private Color3D diffuseColor;
        public Color3D AmbientColor
        {
            get
            {
                return ambientColor;
            }
            set
            {
                ambientColor = value;
            }
        }
        private Color3D ambientColor;
        public Color3D SpecularColor
        {
            get
            {
                return specularColor;
            }
            set
            {
                specularColor = value;
            }
        }
        private Color3D specularColor;
        public Color3D EmissiveColor
        {
            get
            {
                return emissiveColor;
            }
            set
            {
                emissiveColor = value;
            }
        }
        private Color3D emissiveColor;

        public float Opacity { get; set; }
        public float SpecularExponent { get; set; }
        public bool FaceMap { get; set; }
        public bool TwoSided { get; set; }

        public Material()
        {
            this.diffuseColor = new Color3D(1f);
            this.ambientColor = new Color3D(1f);
            this.specularColor = new Color3D(0f);
            this.emissiveColor = new Color3D(0f);

            this.Opacity = 1f;
            this.SpecularExponent = 0f;
            this.FaceMap = false;
            this.TwoSided = false;
        }
    }
}
