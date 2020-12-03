using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace AoMEngineLibrary.Tests.Graphics.Brg
{
    public class BrgMaterialTestObject
    {
        private readonly BrgMaterial _mat;

        public BrgMatFlag Flags { get; init; }

        public string DiffuseMapName { get; init; }

        public Vector3 AmbientColor { get; init; }
        public Vector3 DiffuseColor { get; init; }

        public BrgMaterialTestObject(BrgMaterial mat)
        {
            _mat = mat;
        }

        public void Validate()
        {
            Assert.Equal(Flags, _mat.Flags);

            Assert.Equal(DiffuseMapName, _mat.DiffuseMapName);

            Assert.Equal(AmbientColor, _mat.AmbientColor);
            Assert.Equal(DiffuseColor, _mat.DiffuseColor);
        }
    }
}
