namespace AoMEngineLibrary.Graphics.Prt
{
    public class PrtForces
    {
        public bool RandomOrientation { get; set; }
        public bool Tumble { get; set; }
        public bool TumbleBothDirections { get; set; }
        public bool RandomAxis { get; set; }
        public float InternalGravity { get; set; }
        public float InternalGravityVar { get; set; }
        public float InternalWindDirection { get; set; }
        public float InternalWindDirectionVar { get; set; }
        public float InternalWindSpeed { get; set; }
        public float InternalWindSpeedVar { get; set; }
        public float InternalWindDelay { get; set; }
        public float InternalWindDelayVar { get; set; }
        public float ExternalWindInfluence { get; set; }
        public float ExternalWindInfluenceVar { get; set; }
        public float ExternalWindDelay { get; set; }
        public float ExternalWindDelayVar { get; set; }
        public float MinAngularVelocity { get; set; }
        public float MaxAngularVelocity { get; set; }
        public float XAxis { get; set; }
        public float XAxisVar { get; set; }
        public float YAxis { get; set; }
        public float YAxisVar { get; set; }
        public float ZAxis { get; set; }
        public float ZAxisVar { get; set; }

        public PrtForces()
        {

        }
        public PrtForces(PrtBinaryReader reader)
        {
            this.RandomOrientation = reader.ReadBoolean();
            this.Tumble = reader.ReadBoolean();
            this.TumbleBothDirections = reader.ReadBoolean();
            this.RandomAxis = reader.ReadBoolean();

            this.InternalGravity = reader.ReadSingle();
            this.InternalGravityVar = reader.ReadSingle();
            this.InternalWindDirection = reader.ReadSingle();
            this.InternalWindDirectionVar = reader.ReadSingle();
            this.InternalWindSpeed = reader.ReadSingle();
            this.InternalWindSpeedVar = reader.ReadSingle();
            this.InternalWindDelay = reader.ReadSingle();
            this.InternalWindDelayVar = reader.ReadSingle();
            this.ExternalWindInfluence = reader.ReadSingle();
            this.ExternalWindInfluenceVar = reader.ReadSingle();
            this.ExternalWindDelay = reader.ReadSingle();
            this.ExternalWindDelayVar = reader.ReadSingle();
            this.MinAngularVelocity = reader.ReadSingle();
            this.MaxAngularVelocity = reader.ReadSingle();
            this.XAxis = reader.ReadSingle();
            this.XAxisVar = reader.ReadSingle();
            this.YAxis = reader.ReadSingle();
            this.YAxisVar = reader.ReadSingle();
            this.ZAxis = reader.ReadSingle();
            this.ZAxisVar = reader.ReadSingle();
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.RandomOrientation);
            writer.Write(this.Tumble);
            writer.Write(this.TumbleBothDirections);
            writer.Write(this.RandomAxis);

            writer.Write(this.InternalGravity);
            writer.Write(this.InternalGravityVar);
            writer.Write(this.InternalWindDirection);
            writer.Write(this.InternalWindDirectionVar);
            writer.Write(this.InternalWindSpeed);
            writer.Write(this.InternalWindSpeedVar);
            writer.Write(this.InternalWindDelay);
            writer.Write(this.InternalWindDelayVar);
            writer.Write(this.ExternalWindInfluence);
            writer.Write(this.ExternalWindInfluenceVar);
            writer.Write(this.ExternalWindDelay);
            writer.Write(this.ExternalWindDelayVar);
            writer.Write(this.MinAngularVelocity);
            writer.Write(this.MaxAngularVelocity);
            writer.Write(this.XAxis);
            writer.Write(this.XAxisVar);
            writer.Write(this.YAxis);
            writer.Write(this.YAxisVar);
            writer.Write(this.ZAxis);
            writer.Write(this.ZAxisVar);
        }
    }
}
