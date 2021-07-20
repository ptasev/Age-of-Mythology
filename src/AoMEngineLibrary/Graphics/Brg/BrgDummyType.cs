using System;
using System.Collections.Generic;
using System.Linq;

namespace AoMEngineLibrary.Graphics.Brg
{
    public enum BrgDummyType
    {
        AttachPoint = 0,
        SmokePoint,
        Reserved,
        LeftHand,
        RightHand,
        TopOfHead,
        Forehead,
        Face,
        Chin,
        LeftEar,
        RightEar,
        Neck,
        LeftShoulder,
        RightShoulder,
        FrontChest,
        BackChest,
        FrontAbdomen,
        BackAbdomen,
        Pelvis,
        LeftThigh,
        RightThigh,
        LeftLeg,
        RightLeg,
        LeftFoot,
        RightFoot,
        LeftForearm,
        RightForearm,
        HitPointBar,
        GarrisonFlag,
        Smoke0,
        Smoke1,
        Smoke2,
        Smoke3,
        Smoke4,
        Smoke5,
        Smoke6,
        Smoke7,
        Smoke8,
        Smoke9,
        RESERVED0,
        RESERVED1,
        RESERVED2,
        RESERVED3,
        RESERVED4,
        RESERVED5,
        RESERVED6,
        RESERVED7,
        RESERVED8,
        RESERVED9,
        GatherPoint,
        Fire,
        Decal,
        Corpse,
        LaunchPoint,
        TargetPoint
    }

    public static class BrgDummyTypeExtensions
    {
        public static BrgDummyTypeInfo GetInfo(this BrgDummyType type)
        {
            var info = BrgDummyTypeInfo.Entries.FirstOrDefault(x => x.Type == type);
            return info is null ? throw new InvalidOperationException($"Cannot get info for dummy type {type}.") : info;
        }
    }

    public record BrgDummyTypeInfo(BrgDummyType Type, string Name, ushort Max)
    {
        public static IReadOnlyList<BrgDummyTypeInfo> Entries => new BrgDummyTypeInfo[]
        {
            new(BrgDummyType.AttachPoint, "attachpoint", byte.MaxValue),
            new(BrgDummyType.SmokePoint, "smokepoint", byte.MaxValue),
            new(BrgDummyType.Reserved, "reserved", byte.MaxValue),
            new(BrgDummyType.LeftHand, "lefthand", 1),
            new(BrgDummyType.RightHand, "righthand", 1),
            new(BrgDummyType.TopOfHead, "topofhead", 1),
            new(BrgDummyType.Forehead, "forehead", 1),
            new(BrgDummyType.Face, "face", 1),
            new(BrgDummyType.Chin, "chin", 1),
            new(BrgDummyType.LeftEar, "leftear", 1),
            new(BrgDummyType.RightEar, "rightear", 1),
            new(BrgDummyType.Neck, "neck", 1),
            new(BrgDummyType.LeftShoulder, "leftshoulder", 1),
            new(BrgDummyType.RightShoulder, "rightshoulder", 1),
            new(BrgDummyType.FrontChest, "frontchest", 1),
            new(BrgDummyType.BackChest, "backchest", 1),
            new(BrgDummyType.FrontAbdomen, "frontabdomen", 1),
            new(BrgDummyType.BackAbdomen, "backabdomen", 1),
            new(BrgDummyType.Pelvis, "pelvis", 1),
            new(BrgDummyType.LeftThigh, "leftthigh", 1),
            new(BrgDummyType.RightThigh, "rightthigh", 1),
            new(BrgDummyType.LeftLeg, "leftleg", 1),
            new(BrgDummyType.RightLeg, "rightleg", 1),
            new(BrgDummyType.LeftFoot, "leftfoot", 1),
            new(BrgDummyType.RightFoot, "rightfoot", 1),
            new(BrgDummyType.LeftForearm, "leftforearm", 1),
            new(BrgDummyType.RightForearm, "rightforearm", 1),
            new(BrgDummyType.HitPointBar, "hitpointbar", 1),
            new(BrgDummyType.GarrisonFlag, "garrisonflag", 1),
            new(BrgDummyType.Smoke0, "smoke0", 1),
            new(BrgDummyType.Smoke1, "smoke1", 1),
            new(BrgDummyType.Smoke2, "smoke2", 1),
            new(BrgDummyType.Smoke3, "smoke3", 1),
            new(BrgDummyType.Smoke4, "smoke4", 1),
            new(BrgDummyType.Smoke5, "smoke5", 1),
            new(BrgDummyType.Smoke6, "smoke6", 1),
            new(BrgDummyType.Smoke7, "smoke7", 1),
            new(BrgDummyType.Smoke8, "smoke8", 1),
            new(BrgDummyType.Smoke9, "smoke9", 1),
            new(BrgDummyType.RESERVED0, "reserved0", 1),
            new(BrgDummyType.RESERVED1, "reserved1", 1),
            new(BrgDummyType.RESERVED2, "reserved2", 1),
            new(BrgDummyType.RESERVED3, "reserved3", 1),
            new(BrgDummyType.RESERVED4, "reserved4", 1),
            new(BrgDummyType.RESERVED5, "reserved5", 1),
            new(BrgDummyType.RESERVED6, "reserved6", 1),
            new(BrgDummyType.RESERVED7, "reserved7", 1),
            new(BrgDummyType.RESERVED8, "reserved8", 1),
            new(BrgDummyType.RESERVED9, "reserved9", 1),
            new(BrgDummyType.GatherPoint, "gatherpoint", 1),
            new(BrgDummyType.Fire, "fire", byte.MaxValue),
            new(BrgDummyType.Decal, "decal", 1),
            new(BrgDummyType.Corpse, "corpse", 1),
            new(BrgDummyType.LaunchPoint, "launchpoint", byte.MaxValue),
            new(BrgDummyType.TargetPoint, "targetpoint", byte.MaxValue)
        };

        public static bool TryGetByName(string name, out BrgDummyTypeInfo info)
        {
            if (name.Contains("righthandtag", StringComparison.InvariantCultureIgnoreCase))
            {
                info = Entries[4];
                return true;
            }

            if (name.Contains("lefthandtag", StringComparison.InvariantCultureIgnoreCase))
            {
                info = Entries[3];
                return true;
            }

            for (var i = 0; i < Entries.Count; i++)
            {
                var typeInfo = Entries[i];
                if (typeInfo.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    info = typeInfo;
                    return true;
                }
            }

            info = Entries[0];
            return false;
        }

        public static BrgDummyTypeInfo GetByName(string name)
        {
            if (!TryGetByName(name, out var ret))
            {
                throw new ArgumentException($"Invalid dummy name: {name}");
            }
            return ret;
        }
    }
}
