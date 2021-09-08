using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    #region Variables
    [Header("Режим генирации")]
    [SerializeField] private DrawMode drawMode;
    [Header("Нормализация высоты")]
    [SerializeField] private NormalizeMode normalizeMode;
    [Header("Детализация")]
    [SerializeField, Range(0, 6), ExecuteInEditMode] private int LODonEditor;
    [Header("Количество октав")]
    [Tooltip("Количество октав (повторение шума Перлина)")]
    [SerializeField, Min(0)] private int octaves;
    [Header("Масштаб")]
    [SerializeField] private float noiseScale;
    [Header("Постоянство")]
    [Tooltip("Контролирует изменение амплитуды (" +
        "максимальное значение смещения или изменения" +
        " переменной величины от среднего значения)")]
    [SerializeField, Range(0f, 1f)] private float persistance;
    [Header("Лакунарность")]
    [Tooltip("Контролирует изменение частоты (" +
        "характеристика периодического процесса, равна" +
        " количеству повторений или возникновения событий" +
        " (процессов) в единицу времени")]
    [SerializeField, Min(1f)] private float lacunarity;
    [Header("Зерно")]
    [SerializeField] private int seed;
    [Header("Смещение")]
    [SerializeField] private Vector2 offset;
    [Header("Использовать глубину")]
    [SerializeField] private bool useFalloff;
    [Header("Множитель высот")]
    [SerializeField] private float meshHeightMultiplayer;
    [Header("Изгиб высот")]
    [SerializeField] private AnimationCurve meshHeightCurve;
    [Header("Регионы")]
    [SerializeField] private TerrainType[] regions;
    [Header("Добавлять коллайдер")]
    [SerializeField] private bool addingCollider;
    public bool AddingCollider {
        get { return addingCollider; }
    }
    [Header("Авто")]
    [SerializeField] private bool autoUpdate;
    public bool AutoUpdate {
        get { return autoUpdate; }
    }

    [Header("Размер карты")]
    public const int mapChunkSize = 241;
    Queue<ThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<ThreadInfo<MapData>>();
    Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();

    float[,] falloffMap;
    #endregion Variables

    private void Awake() {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    private void Update() {
        RunQueue(ref mapDataThreadInfoQueue);
        RunQueue(ref meshDataThreadInfoQueue);
    }

    private void RunQueue<T>(ref Queue<ThreadInfo<T>> queue) {
        if (queue.Count > 0) {
            for (int i = 0; i < queue.Count; i++) {
                var threadInfo = queue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    #region MapDataThread
    public void RequestMapData(Vector2 centre, Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            MapDataThread(centre, callback);
        };
        new Thread(threadStart).Start();
    }
    void MapDataThread(Vector2 centre, Action<MapData> callback) {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue(
                new ThreadInfo<MapData>(callback, mapData)
                );
        }
    }
    #endregion MapDataThread

    #region MapDataThread
    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback) {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback) {
        MeshData meshData = MeshGenerator.GetMeshData(mapData.heightMap,
            meshHeightMultiplayer, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(
                new ThreadInfo<MeshData>(callback, meshData)
                );
        }
    }
    #endregion MapDataThread

    public void DrawMapInEditor() {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        switch (drawMode) {
            case DrawMode.FalloffMap:
                display.DrawMap(TextureGenerator
                    .GetTexture(falloffMap));
                break;
            case DrawMode.NoiseMap:
                display.DrawMap(TextureGenerator
                    .GetTexture(mapData.heightMap));
                break;
            case DrawMode.ColourMap:
                display.DrawMap(TextureGenerator
                    .GetTexture(mapData.colourMap, mapChunkSize));
                break;
            case DrawMode.Mesh:
                display.DrawMap(
                    TextureGenerator.GetTexture(mapData.colourMap, mapChunkSize),
                    MeshGenerator.GetMeshData(mapData.heightMap, meshHeightMultiplayer,
                        meshHeightCurve, LODonEditor));
                break;
            default: break;
        }
    }

    private MapData GenerateMapData(Vector2 centre) {
        float[,] heightMap = NoiseGenerator.GetNoiseMap(seed, mapChunkSize,
            noiseScale, octaves, persistance, lacunarity, centre + offset,
            normalizeMode);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        if ((regions?.Count() ?? 0) > 0) {
            for (int z = 0; z < mapChunkSize; z++) {
                for (int x = 0; x < mapChunkSize; x++) {
                    if (useFalloff) {
                        heightMap[x, z] = Mathf.Clamp01(heightMap[x, z] - falloffMap[x, z]);
                    }
                    colourMap[z * mapChunkSize + x] = regions
                        .LastOrDefault(e => e.Height <= heightMap[x, z])
                        .Colour;
                }
            }
        };

        return new MapData(heightMap, colourMap);
    }

    public void OptimizateValues() {
        LODonEditor = 1;
        seed = 23082018;
        octaves = 4;
        //noiseScale = 37.6f;
        noiseScale = 75.1f;
        persistance = 0.5f;
        lacunarity = 1.75f;
        offset = new Vector2(0, 0);
        regions = new TerrainType[] {
                    new TerrainType("Deep",         0.000f, new Color( 20/255f,  46/255f, 172/255f, 255/255f)),
                    new TerrainType("Lake",         0.100f, new Color(  0/255f,  77/255f, 182/255f, 200/255f)),
                    new TerrainType("Water",        0.150f, new Color(  0/255f, 149/255f, 182/255f, 200/255f)),
                    new TerrainType("Sand",         0.250f, new Color(240/255f, 219/255f, 125/255f, 255/255f)),
                    new TerrainType("Weet Earth",   0.300f, new Color(115/255f,  66/255f,  34/255f, 255/255f)),
                    new TerrainType("Earth",        0.350f, new Color(162/255f, 101/255f,  62/255f, 255/255f)),
                    new TerrainType("Grass",        0.400f, new Color( 65/255f, 156/255f,   3/255f, 255/255f)),
                    new TerrainType("Lowlands",     0.450f, new Color( 61/255f, 109/255f,  29/255f, 255/255f)),
                    new TerrainType("Hill",         0.550f, new Color(173/255f, 165/255f, 135/255f, 255/255f)),
                    new TerrainType("Mountain",     0.750f, new Color(212/255f, 208/255f, 193/255f, 255/255f)),
                    new TerrainType("Mountain cap", 1.000f, new Color(250/255f, 250/255f, 250/255f, 255/255f))
                };
        meshHeightMultiplayer = 30f;
        meshHeightCurve =
            new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.33f, 0f), new Keyframe(1f, 1f)) {
                postWrapMode = WrapMode.PingPong,
                preWrapMode = WrapMode.PingPong
            };
        DrawMapInEditor();
    }
}

#region Lists
public enum DrawMode {
    [Header("Карта глубин")] FalloffMap,
    [Header("Шум")] NoiseMap,
    [Header("Цвет")] ColourMap,
    [Header("Меш")] Mesh,
}
public enum NormalizeMode {
    [Header("Локальная")] Local,
    [Header("Глобальная")] Global
}
[Serializable]
public struct TerrainType {
    [Header("Название")]
    [SerializeField] string name;
    public string Name {
        get { return name; }
        set { name = value; }
    }
    [Header("Высота")]
    [SerializeField] float height;
    public float Height {
        get { return height; }
        set { height = value; }
    }
    [Header("Цвет")]
    [SerializeField] Color colour;
    public Color Colour {
        get { return colour; }
        set { colour = value; }
    }
    public TerrainType(string name, float height, Color colour) {
        this.name = name;
        this.height = height;
        this.colour = colour;
    }
}
[Serializable]
public struct MapData {
    [SerializeField] public readonly float[,] heightMap;
    [SerializeField] public readonly Color[] colourMap;
    public MapData(float[,] heightMap, Color[] colourMap) {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}
struct ThreadInfo<T> {
    public readonly Action<T> callback;
    public readonly T parameter;

    public ThreadInfo(Action<T> callback, T parameter) {
        this.callback = callback;
        this.parameter = parameter;
    }
}
#endregion Lists