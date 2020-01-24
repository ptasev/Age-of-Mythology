namespace AoMEngineLibrary.Graphics.Model
{
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Material
    {
        private Color3D diffuseColor;
        private Color3D ambientColor;
        private Color3D specularColor;
        private Color3D emissiveColor;

        #region Properties
        public virtual string Name { get; set; }
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

        public float Opacity { get; set; }
        public float SpecularExponent { get; set; }
        public bool FaceMap { get; set; }
        public bool TwoSided { get; set; }
        #endregion

        public Material()
        {
            this.Name = "Material";
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
