namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgAsetHeader
    {
        public int numFrames;
        public float frameStep;
        public float animTime;
        public float frequency;
        public float spf;
        public float fps;
        public int space;

        public BrgAsetHeader()
        {

        }
        public BrgAsetHeader(BrgBinaryReader reader)
        {
            this.numFrames = reader.ReadInt32();
            this.frameStep = reader.ReadSingle();
            this.animTime = reader.ReadSingle();
            this.frequency = reader.ReadSingle();
            this.spf = reader.ReadSingle();
            this.fps = reader.ReadSingle();
            this.space = reader.ReadInt32();
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(this.numFrames);
            writer.Write(this.frameStep);
            writer.Write(this.animTime);
            writer.Write(this.frequency);
            writer.Write(this.spf);
            writer.Write(this.fps);
            writer.Write(this.space);
        }
    }
}
