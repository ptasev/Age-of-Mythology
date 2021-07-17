using System;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgAsetHeader
    {
        public uint NumFrames { get; set; }

        public float InvFrames { get; set; }

        public float AnimTime { get; set; }

        public float Frequency { get; set; }

        public float Spf { get; set; }

        public float Fps { get; set; }

        public uint Reserved { get; set; }

        public BrgAsetHeader()
        {
        }
        public BrgAsetHeader(BrgBinaryReader reader)
        {
            NumFrames = reader.ReadUInt32();
            InvFrames = reader.ReadSingle();
            AnimTime = reader.ReadSingle();
            Frequency = reader.ReadSingle();
            Spf = reader.ReadSingle();
            Fps = reader.ReadSingle();
            Reserved = reader.ReadUInt32();
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(NumFrames);
            writer.Write(InvFrames);
            writer.Write(AnimTime);
            writer.Write(Frequency);
            writer.Write(Spf);
            writer.Write(Fps);
            writer.Write(Reserved);
        }
    }
}
