namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using XmlCommentSerialization;

    public class PrtCollision : XmlAnnotate
    {
        public Int32 NumTypes { get; set; }
        [XmlComment]
        public PrtTerrainInteractionType TerrainInteractionType { get; set; }
        public float TerrainHeight { get; set; }
        public float TerrainHeightVar { get; set; }

        [XmlArrayItem("Type")]
        public List<PrtCollisionType> CollisionTypes
        {
            get;
            set;
        }

        private PrtCollision()
        {
            CollisionTypes = new List<PrtCollisionType>();
        }
        public PrtCollision(PrtBinaryReader reader)
        {
            this.NumTypes = reader.ReadInt32();
            this.TerrainInteractionType = (PrtTerrainInteractionType)reader.ReadInt32();
            this.TerrainHeight = reader.ReadSingle();
            this.TerrainHeightVar = reader.ReadSingle();

            this.CollisionTypes = new List<PrtCollisionType>(this.NumTypes);
        }

        public void Write(PrtBinaryWriter writer)
        {
            this.NumTypes = this.CollisionTypes.Count;
            writer.Write(this.NumTypes);
            writer.Write((Int32)this.TerrainInteractionType);
            writer.Write(this.TerrainHeight);
            writer.Write(this.TerrainHeightVar);
        }
    }
}
