using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Grn;

public static class GrnMeshOptimizer
{
    public static void Optimize(GrnMesh input)
    {
        Dictionary<(Vector3, GrnVertexWeight), int> positionWeightMap = new();
        Dictionary<Vector3, int> normalMap = new();
        Dictionary<Vector3, int> texCoordMap = new();

        HashSet<(int a, int b, int c)> indices = new();
        HashSet<(int a, int b, int c)> normalIndices = new();
        HashSet<(int a, int b, int c)> texCoordIndices = new();
        List<GrnFace> faces = new();

        foreach (var face in input.Faces)
        {
            var newFace = new GrnFace { MaterialIndex = face.MaterialIndex };

            var a = face.Indices[0];
            var b = face.Indices[1];
            var c = face.Indices[2];
            a = GetPosIndex((input.Vertices[a], input.VertexWeights[a]));
            b = GetPosIndex((input.Vertices[b], input.VertexWeights[b]));
            c = GetPosIndex((input.Vertices[c], input.VertexWeights[c]));
            bool triAdded1 = indices.Add((a, b, c));
            newFace.Indices = new List<int>() { a, b, c };

            a = face.NormalIndices[0];
            b = face.NormalIndices[1];
            c = face.NormalIndices[2];
            a = GetNormIndex(input.Normals[a]);
            b = GetNormIndex(input.Normals[b]);
            c = GetNormIndex(input.Normals[c]);
            bool triAdded2 = normalIndices.Add((a, b, c));
            newFace.NormalIndices = new List<int>() { a, b, c };

            a = face.TextureIndices[0];
            b = face.TextureIndices[1];
            c = face.TextureIndices[2];
            a = GetTexCoordIndex(input.TextureCoordinates[a]);
            b = GetTexCoordIndex(input.TextureCoordinates[b]);
            c = GetTexCoordIndex(input.TextureCoordinates[c]);
            bool triAdded3 = texCoordIndices.Add((a, b, c));
            newFace.TextureIndices = new List<int>() { a, b, c };

            // if the triangle was unique in any of the 3 cases, then the whole face is unique, add
            if (triAdded1 || triAdded2 || triAdded3)
            {
                faces.Add(newFace);
            }
        }

        var posWeightMapSorted = positionWeightMap.OrderBy(kv => kv.Value);
        input.Vertices = posWeightMapSorted.Select(kv => kv.Key.Item1).ToList();
        input.VertexWeights = posWeightMapSorted.Select(kv => kv.Key.Item2).ToList();
        input.Normals = normalMap.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();
        input.TextureCoordinates = texCoordMap.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();
        input.Faces = faces;

        int GetPosIndex((Vector3, GrnVertexWeight) pos)
        {
            if (positionWeightMap.TryGetValue(pos, out int index))
            {
                return index;
            }
            else
            {
                index = positionWeightMap.Count;
                positionWeightMap.Add(pos, index);
                return index;
            }
        }

        int GetNormIndex(Vector3 normal) => GetMapIndex(normalMap, normal);
        int GetTexCoordIndex(Vector3 texCoord) => GetMapIndex(texCoordMap, texCoord);
    }

    private static int GetMapIndex(Dictionary<Vector3, int> map, Vector3 key)
    {
        if (map.TryGetValue(key, out int index))
        {
            return index;
        }
        else
        {
            index = map.Count;
            map.Add(key, index);
            return index;
        }
    }
}