using AoMEngineLibrary.Graphics.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AoMModelViewer.Graphics
{
    public class Shader
    {
        IReadOnlyList<Vector3> vertices;
        IReadOnlyList<Vector3> normals;
        IReadOnlyList<Vector3> texCoords;
        IReadOnlyList<Face> faces;
        Vector3 lightDir;
        Matrix4x4 modelView;
        Matrix4x4 projection;
        Matrix4x4 viewport;

        Matrix4x4 normalMat; // written by vertex shader, read by fragment shader
        Matrix4x4 uvMat;
        Image<Rgb24> image;

        public Shader(IReadOnlyList<Vector3> vertices, IReadOnlyList<Vector3> normals, IReadOnlyList<Vector3> texCoords, IReadOnlyList<Face> faces, Vector3 lightDirection,
            Matrix4x4 modelView, Matrix4x4 projection, Matrix4x4 viewport)
        {
            this.vertices = vertices;
            this.normals = normals;
            this.texCoords = texCoords;
            this.faces = faces;
            this.lightDir = -lightDirection;
            this.modelView = modelView;
            this.projection = projection;
            this.viewport = viewport;
            normalMat = new Matrix4x4();
            uvMat = new Matrix4x4();
            image = Image.Load<Rgb24>(@"C:\Games\Steam\steamapps\common\Age of Mythology\textures\infantry g hoplite standard.bmp");
        }

        public Vector4 Vertex(int iface, int nthvert)
        {
            Vector3 glVertex = vertices[faces[iface].Indices[nthvert]]; // read the vertex
            Vector4 output = Vector4.Transform(glVertex, modelView); // transform to view space
            output = Vector4.Transform(output, projection); // transform it to clip space

            var normal = normals[faces[iface].Indices[nthvert]];
            normal = Vector3.TransformNormal(normal, modelView);
            normal = Vector3.Normalize(normal);

            var texCoord = texCoords[faces[iface].Indices[nthvert]];

            switch (nthvert)
            {
                case 0:
                    normalMat.M11 = normal.X;
                    normalMat.M12 = normal.Y;
                    normalMat.M13 = normal.Z;

                    uvMat.M11 = texCoord.X;
                    uvMat.M12 = (1 - texCoord.Y);
                    uvMat.M13 = texCoord.Z;
                    break;
                case 1:
                    normalMat.M21 = normal.X;
                    normalMat.M22 = normal.Y;
                    normalMat.M23 = normal.Z;

                    uvMat.M21 = texCoord.X;
                    uvMat.M22 = (1 - texCoord.Y);
                    uvMat.M23 = texCoord.Z;
                    break;
                case 2:
                    normalMat.M31 = normal.X;
                    normalMat.M32 = normal.Y;
                    normalMat.M33 = normal.Z;

                    uvMat.M31 = texCoord.X;
                    uvMat.M32 = (1 - texCoord.Y);
                    uvMat.M33 = texCoord.Z;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
            return output;
        }

        public bool Fragment(Vector3 bar, out Color color)
        {
            Vector3 normal = Vector3.Transform(bar, normalMat);
            float ival = Math.Clamp(Vector3.Dot(normal, lightDir), 0, 1); // get diffuse lighting intensity

            Vector3 uv = Vector3.Transform(bar, uvMat);
            uv.X *= image.Width;
            uv.Y *= image.Height;
            uv.X = Math.Clamp(uv.X, 0, image.Width - 1);
            uv.Y = Math.Clamp(uv.Y, 0, image.Height - 1);
            var col = image.GetPixelRowSpan((int)uv.Y)[(int)uv.X];

            byte val = (byte)(255 * ival);
            //ival = 1.0f;
            color = new Color(255, (byte)(col.R * ival), (byte)(col.G * ival), (byte)(col.B * ival)); // well duh
            return ival <= 0; // no, we do not discard this pixel
        }
    }
}
