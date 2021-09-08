using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {

    const float
        scale = 2.5f,
        viewerMoveThresholdForChunkUpdate = 5f,
        sqrtViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    [SerializeField] private LODInfo[] detailLevels = new LODInfo[] {
        new LODInfo { lod = 0, visibleDistanseThreshold = 200 },
        new LODInfo { lod = 1, visibleDistanseThreshold = 400 },
        new LODInfo { lod = 2, visibleDistanseThreshold = 600 },
        new LODInfo { lod = 3, visibleDistanseThreshold = 800 },
    };
    [SerializeField] private Transform viewer;
    [SerializeField] private Material material;

    private static float maxViewDistanse;


    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    int chunkSize;
    int chunkVisibleInViewDistanse;

    static MapGenerator mapGenerator;
    Transform parent;

    static List<TerrainChunk> terrainChunkList = new List<TerrainChunk>();

    private void Start() {
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDistanse = detailLevels.Last().visibleDistanseThreshold;
        parent = new GameObject("MapChunks").transform;
        //parent.SetParent(GameObject.Find("Map").transform);
        parent.SetParent(mapGenerator.gameObject.transform);
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunkVisibleInViewDistanse = Mathf.RoundToInt(maxViewDistanse / chunkSize);
        UpdateVisibleChunks();
    }

    private void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;
        if ((viewerPositionOld - viewerPosition)
            .sqrMagnitude > sqrtViewerMoveThresholdForChunkUpdate) {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    private void UpdateVisibleChunks() {
        List<Vector2> currentChunksPosition = new List<Vector2>();
        int chunkCoordinateX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int chunkCoordinateZ = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        for (int zOffset = -chunkVisibleInViewDistanse; zOffset <= chunkVisibleInViewDistanse; zOffset++) {
            for (int xOffset = -chunkVisibleInViewDistanse; xOffset <= chunkVisibleInViewDistanse; xOffset++) {
                Vector2 viewChunkCoordinate =
                    new Vector2(chunkCoordinateX + xOffset, chunkCoordinateZ + zOffset);
                var terrainChunk = terrainChunkList
                    .FirstOrDefault(e => e.Position.Equals(viewChunkCoordinate * chunkSize));
                if (terrainChunk == null) {
                    terrainChunk = new TerrainChunk(viewChunkCoordinate,
                        chunkSize, detailLevels, parent, material);
                    terrainChunk.Active = true;
                } else {
                    terrainChunk.UpdateTerrainChunk();
                }
                currentChunksPosition.Add(terrainChunk.Position);
            }
        }
        foreach (var chunk in terrainChunkList.Where(e => e.Active && !currentChunksPosition.Contains(e.Position))) {
            chunk.Active = false;
        }
    }

    class TerrainChunk {
        GameObject meshObject;

        Vector2 position;
        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public bool Active {
            get { return meshObject?.activeSelf ?? false; }
            set { meshObject?.SetActive(value); }
        }

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        MapData mapData;
        bool mapDataReceive;
        int previosLODId = -1;
        public TerrainChunk(Vector2 coordinate, int size, LODInfo[] detailLevels, Transform parent, Material material) {
            this.detailLevels = detailLevels;
            position = coordinate * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject(string.Format("chunk[{0},{1},{2}]", coordinate.x, 0, coordinate.y));
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.SetParent(parent);
            meshObject.transform.localScale = Vector3.one * scale;

            this.Active = false;

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++) {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
            terrainChunkList.Add(this);
        }

        private void OnMapDataReceived(MapData mapData) {
            this.mapData = mapData;
            this.mapDataReceive = true;

            Texture2D texture = TextureGenerator
                .GetTexture(mapData.colourMap, mapData.heightMap.GetLength(0), mapData.heightMap.GetLength(1));
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk() {
            if (mapDataReceive) {
                float viewerDistanseFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistanseFromNearestEdge <= maxViewDistanse;

                if (visible) {
                    int lodId = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++) {
                        if (viewerDistanseFromNearestEdge > detailLevels[i].visibleDistanseThreshold) {
                            lodId = i + 1;
                        } else {
                            break;
                        }
                    }

                    if (lodId != previosLODId) {
                        LODMesh lodMesh = lodMeshes[lodId];
                        if (lodMesh.hasMesh) {
                            previosLODId = lodId;
                            meshFilter.mesh = lodMesh.mesh;
                        } else if (!lodMesh.hasRequestedMesh) {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                    if (mapGenerator.AddingCollider) {
                        MeshCollider meshCollider = null;
                        meshObject.TryGetComponent<MeshCollider>(out meshCollider);
                        if (meshCollider != null) {
                            Destroy(meshCollider);
                        }
                        meshObject.AddComponent<MeshCollider>();
                    }
                }
                this.Active = visible;
            }
        }
    }

    class LODMesh {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback) {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData) {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData) {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [Serializable]
    struct LODInfo {
        public int lod;
        public float visibleDistanseThreshold;
    }
}
