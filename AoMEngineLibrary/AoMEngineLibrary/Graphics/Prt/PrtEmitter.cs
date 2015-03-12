namespace AoMEngineLibrary.Graphics.Prt
{
    using System;

    public class PrtEmitter
    {
        public bool TiedToEmitter {get;set;}
        public bool IgnoreRotation { get; set; }
        public bool EmitByMotion {get;set;}
        public bool Loop { get; set; }
        public bool InheritVelocity { get; set; }
        public bool UseMinVelocity { get; set; }
        public bool UseMaxVelocity { get; set; }
        public bool AlwaysActive { get; set; }
        public bool SyncWithAttackAnim { get; set; }
        public Int32 MaxParticles { get; set; }
        public Int32 AppearanceType { get; set; }
        public float UpdateRadius { get; set; }
        public float MaxParticlesVar { get; set; }
        public float ParticleLife { get; set; }
        public float ParticleLifeVar { get; set; }
        public float GlobalFadeIn { get; set; }
        public float GlobalFadeInVar { get; set; }
        public float GlobalFadeOut { get; set; }
        public float GlobalFadeOutVar { get; set; }
        public float EmitDistance { get; set; }
        public float EmitDistanceVar { get; set; }
        public float EmissionRate { get; set; }
        public float EmissionRateVar { get; set; }
        public float InitialDormancy { get; set; }
        public float InitialDormancyVar { get; set; }
        public float InitialUpdate { get; set; }
        public float InitialUpdateVar { get; set; }
        public float EmissionTime { get; set; }
        public float EmissionTimeVar { get; set; }
        public float DormantTime { get; set; }
        public float DormantTimeVar { get; set; }
        public float InitialDistance { get; set; }
        public float InitialDistanceVar { get; set; }
        public float InitialVelocity { get; set; }
        public float InitialVelocityVar { get; set; }
        public float Acceleration { get; set; }
        public float AccelerationVar { get; set; }
        public float InheritInfluence { get; set; }
        public float InheritInfluenceVar { get; set; }
        public float MinVelocity { get; set; }
        public float MinVelocityVar { get; set; }
        public float MaxVelocity { get; set; }
        public float MaxVelocityVar { get; set; }

        private PrtEmitter()
        {

        }
        public PrtEmitter(PrtBinaryReader reader)
        {
            this.TiedToEmitter = reader.ReadBoolean();
            this.IgnoreRotation = reader.ReadBoolean();
            this.EmitByMotion = reader.ReadBoolean();
            this.Loop = reader.ReadBoolean();
            this.InheritVelocity = reader.ReadBoolean();
            this.UseMinVelocity = reader.ReadBoolean();
            this.UseMaxVelocity = reader.ReadBoolean();
            this.AlwaysActive = reader.ReadBoolean();
            this.SyncWithAttackAnim = reader.ReadBoolean();
            reader.ReadBytes(3); // 32 bit padding

            this.MaxParticles = reader.ReadInt32();
            this.AppearanceType = reader.ReadInt32();
            this.UpdateRadius = reader.ReadSingle();
            this.MaxParticlesVar = reader.ReadSingle();
            this.ParticleLife = reader.ReadSingle();
            this.ParticleLifeVar = reader.ReadSingle();
            this.GlobalFadeIn = reader.ReadSingle();
            this.GlobalFadeInVar = reader.ReadSingle();
            this.GlobalFadeOut = reader.ReadSingle();
            this.GlobalFadeOutVar = reader.ReadSingle();
            this.EmitDistance = reader.ReadSingle();
            this.EmitDistanceVar = reader.ReadSingle();
            this.EmissionRate = reader.ReadSingle();
            this.EmissionRateVar = reader.ReadSingle();
            this.InitialDormancy = reader.ReadSingle();
            this.InitialDormancyVar = reader.ReadSingle();
            this.InitialUpdate = reader.ReadSingle();
            this.InitialUpdateVar = reader.ReadSingle();
            this.EmissionTime = reader.ReadSingle();
            this.EmissionTimeVar = reader.ReadSingle();
            this.DormantTime = reader.ReadSingle();
            this.DormantTimeVar = reader.ReadSingle();
            this.InitialDistance = reader.ReadSingle();
            this.InitialDistanceVar = reader.ReadSingle();
            this.InitialVelocity = reader.ReadSingle();
            this.InitialVelocityVar = reader.ReadSingle();
            this.Acceleration = reader.ReadSingle();
            this.AccelerationVar = reader.ReadSingle();
            this.InheritInfluence = reader.ReadSingle();
            this.InheritInfluenceVar = reader.ReadSingle();
            this.MinVelocity = reader.ReadSingle();
            this.MinVelocityVar = reader.ReadSingle();
            this.MaxVelocity = reader.ReadSingle();
            this.MaxVelocityVar = reader.ReadSingle();
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.TiedToEmitter);
            writer.Write(this.IgnoreRotation);
            writer.Write(this.EmitByMotion);
            writer.Write(this.Loop);
            writer.Write(this.InheritVelocity);
            writer.Write(this.UseMinVelocity);
            writer.Write(this.UseMaxVelocity);
            writer.Write(this.AlwaysActive);
            writer.Write(this.SyncWithAttackAnim);
            writer.Write(new byte[3]); // 32 bit padding

            writer.Write(this.MaxParticles);
            writer.Write(this.AppearanceType);
            writer.Write(this.UpdateRadius);
            writer.Write(this.MaxParticlesVar);
            writer.Write(this.ParticleLife);
            writer.Write(this.ParticleLifeVar);
            writer.Write(this.GlobalFadeIn);
            writer.Write(this.GlobalFadeInVar);
            writer.Write(this.GlobalFadeOut);
            writer.Write(this.GlobalFadeOutVar);
            writer.Write(this.EmitDistance);
            writer.Write(this.EmitDistanceVar);
            writer.Write(this.EmissionRate);
            writer.Write(this.EmissionRateVar);
            writer.Write(this.InitialDormancy);
            writer.Write(this.InitialDormancyVar);
            writer.Write(this.InitialUpdate);
            writer.Write(this.InitialUpdateVar);
            writer.Write(this.EmissionTime);
            writer.Write(this.EmissionTimeVar);
            writer.Write(this.DormantTime);
            writer.Write(this.DormantTimeVar);
            writer.Write(this.InitialDistance);
            writer.Write(this.InitialDistanceVar);
            writer.Write(this.InitialVelocity);
            writer.Write(this.InitialVelocityVar);
            writer.Write(this.Acceleration);
            writer.Write(this.AccelerationVar);
            writer.Write(this.InheritInfluence);
            writer.Write(this.InheritInfluenceVar);
            writer.Write(this.MinVelocity);
            writer.Write(this.MinVelocityVar);
            writer.Write(this.MaxVelocity);
            writer.Write(this.MaxVelocityVar);
        }
    }
}
