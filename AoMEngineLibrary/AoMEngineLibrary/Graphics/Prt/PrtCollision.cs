namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;

    public class PrtCollision
    {
        public Int32 NumTypes { get; set; }
        public Int32 TerrainInteractionType { get; set; }
        public float TerrainHeight { get; set; }
        public float TerrainHeightVar { get; set; }

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
            this.TerrainInteractionType = reader.ReadInt32();
            this.TerrainHeight = reader.ReadSingle();
            this.TerrainHeightVar = reader.ReadSingle();

            this.CollisionTypes = new List<PrtCollisionType>(this.NumTypes);
        }

        public void Write(PrtBinaryWriter writer)
        {
            this.NumTypes = this.CollisionTypes.Count;
            writer.Write(this.NumTypes);
            writer.Write(this.TerrainInteractionType);
            writer.Write(this.TerrainHeight);
            writer.Write(this.TerrainHeightVar);
        }
    }
}
