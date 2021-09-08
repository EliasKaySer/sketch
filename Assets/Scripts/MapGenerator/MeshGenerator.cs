using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {
    public static MeshData GetMeshData(float[,] heightMap,
        float heightMultiplayer, AnimationCurve heightCurve, int levelOfDetail) {
        AnimationCurve meshHeightCurve = new AnimationCurve(heightCurve.keys);

        int width = heightMap.GetLength(0);
        int length = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (length - 1) / 2f;

        int meshSpecificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int verticesPerline = (width - 1) / meshSpecificationIncrement + 1;

        MeshData meshData = new MeshData(width, length);
        int vertexId = 0;

        for (int z = 0; z < length; z += meshSpecificationIncrement) {
            for (int x = 0; x < width; x += meshSpecificationIncrement) {
                meshData.vertices[vertexId] =
                    new Vector3(topLeftX + x, meshHeightCurve.Evaluate(heightMap[x, z]) * heightMultiplayer, topLeftZ - z);
                meshData.uvs[vertexId] =
                    new Vector2(x / (float)width, z / (float)length);

                if (x < width - 1 && z < length - 1) {
                    meshData.AddTriangle(vertexId, vertexId + verticesPerline + 1, vertexId + verticesPerline);
                    meshData.AddTriangle(vertexId + verticesPerline + 1, vertexId, vertexId + 1);
                }

                vertexId++;
            }
        }

        return meshData;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public int triangleId;

    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleId] = a;
        triangles[triangleId + 1] = b;
        triangles[triangleId + 2] = c;
        triangleId += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
