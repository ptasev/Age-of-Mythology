using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Brg;

public static class BrgMeshOptimizer
{
    public static void Optimize(IReadOnlyList<BrgMesh> frames)
    {
        if (frames.Count <= 0) return;
        var indexMap = new Dictionary<int, int>();

        // Pos, Norm, TexCoord, Color
        var frameVertices = Enumerable.Repeat(false, frames.Count)
            .Select(_ => new List<BrgVertex>())
            .ToArray();

        // Optimize vertices
        var baseMesh = frames.First();
        var vertexCount = baseMesh.Vertices.Count;
        for (int i = 0; i < vertexCount; ++i)
        {
            int existingIndex = GetExistingIndex(frames, i, frameVertices);

            if (existingIndex < 0)
            {
                // vertex is not duplicate add to each frame
                indexMap.Add(i, frameVertices[0].Count);
                for (int f = 0; f < frames.Count; ++f)
                {
                    var frame = frames[f];
                    var frameVerts = frameVertices[f];

                    var vertex = GetVertex(frame, i);
                    frameVerts.Add(vertex);
                }
            }
            else
            {
                // vertex is a duplicate, just add to index map
                indexMap.Add(i, existingIndex);
            }
        }

        HashSet<(int a, int b, int c)> indices = new();
        List<BrgFace> faces = new();

        foreach (var face in baseMesh.Faces)
        {
            var a = indexMap[face.A];
            var b = indexMap[face.B];
            var c = indexMap[face.C];
            var triAdded = indices.Add((a, b, c));

            var newFace = new BrgFace((ushort)a, (ushort)b, (ushort)c) { MaterialIndex = face.MaterialIndex };

            // if the triangle was unique then add
            if (triAdded)
            {
                faces.Add(newFace);
            }
        }

        // Update the data in the frames
        for (int f = 0; f < frames.Count; ++f)
        {
            var frame = frames[f];
            var verts = frameVertices[f];

            frame.Vertices = verts.Select(v => v.Position).ToList();
            frame.Normals = verts.Select(v => v.Normal).ToList();

            if ((!frame.Header.Flags.HasFlag(BrgMeshFlag.Secondary) ||
                 frame.Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords) ||
                 frame.Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem)) &&
                frame.Header.Flags.HasFlag(BrgMeshFlag.Texture1))
            {
                frame.TextureCoordinates = verts.Select(v => v.TexCoord).ToList();
            }

            if (((frame.Header.Flags.HasFlag(BrgMeshFlag.AlphaChannel) ||
                  frame.Header.Flags.HasFlag(BrgMeshFlag.ColorChannel)) &&
                 !frame.Header.Flags.HasFlag(BrgMeshFlag.Secondary)) ||
                frame.Header.Flags.HasFlag(BrgMeshFlag.AnimVertexColor))
            {
                frame.Colors = verts.Select(v => v.Color).ToList();
            }

            frame.Header.NumVertices = (ushort)frame.Vertices.Count;
            frame.Header.NumFaces = (ushort)faces.Count;
        }

        // Update base mesh faces
        baseMesh.Faces = faces;
        baseMesh.VertexMaterials = Enumerable.Repeat((short)0, baseMesh.Vertices.Count).ToList();
        foreach (var face in baseMesh.Faces)
        {
            baseMesh.VertexMaterials[face.A] = face.MaterialIndex;
            baseMesh.VertexMaterials[face.B] = face.MaterialIndex;
            baseMesh.VertexMaterials[face.C] = face.MaterialIndex;
        }
    }

    private static int GetExistingIndex(IReadOnlyList<BrgMesh> frames, int vertIndex, List<BrgVertex>[] frameVertices)
    {
        int existingIndex = -1;
        for (int i = 0; i < frames.Count; ++i)
        {
            var frame = frames[i];
            var vertexMap = frameVertices[i];

            var vertex = GetVertex(frame, vertIndex);
            int frameVertIndex = vertexMap.IndexOf(vertex);
            if (frameVertIndex >= 0)
            {
                // Vertex is not unique in this frame
                if (existingIndex == -1)
                {
                    // set the existing index for first time
                    existingIndex = frameVertIndex;
                }
                else if (existingIndex != frameVertIndex)
                {
                    // this frame's vert index does not match other frame's existing index
                    // this means the vert is not a duplicate across all frames
                    return -1;
                }
            }
            else
            {
                // if a single frame can't find an existing index, then the vert is not a duplicate
                return -1;
            }
        }

        return existingIndex;
    }

    private static BrgVertex GetVertex(BrgMesh frame, int vertexIndex)
    {
        var pos = frame.Vertices[vertexIndex];
        var norm = frame.Normals[vertexIndex];
        var tex = Vector2.Zero;
        var color = Vector4.Zero;

        if ((!frame.Header.Flags.HasFlag(BrgMeshFlag.Secondary) ||
             frame.Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords) ||
             frame.Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem)) &&
            frame.Header.Flags.HasFlag(BrgMeshFlag.Texture1))
        {
            tex = frame.TextureCoordinates[vertexIndex];
        }

        if (((frame.Header.Flags.HasFlag(BrgMeshFlag.AlphaChannel) ||
              frame.Header.Flags.HasFlag(BrgMeshFlag.ColorChannel)) &&
             !frame.Header.Flags.HasFlag(BrgMeshFlag.Secondary)) ||
            frame.Header.Flags.HasFlag(BrgMeshFlag.AnimVertexColor))
        {
            color = frame.Colors[vertexIndex];
        }

        return new BrgVertex(pos, norm, tex, color);
    }

    private record struct BrgVertex(Vector3 Position, Vector3 Normal, Vector2 TexCoord, Vector4 Color);
}
