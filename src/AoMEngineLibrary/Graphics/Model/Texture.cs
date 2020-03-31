namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Texture
    {
        public virtual string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Texture()
        {
            this.Name = "Texture";
            this.Width = 0;
            this.Height = 0;
        }
    }
}
