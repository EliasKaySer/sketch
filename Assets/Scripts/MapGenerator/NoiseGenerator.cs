using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator {
    public static float[,] GetNoiseMap(int seed,
        int mapChunkSize, float scale,
        int octaves, float persistance, float lacunarity,
        Vector2 offset, NormalizeMode normalizeMode) {
        return GetNoiseMap(seed, mapChunkSize, mapChunkSize,
            scale, octaves, persistance, lacunarity, offset, normalizeMode);
    }
    public static float[,] GetNoiseMap(int seed,
        int mapWidth, int mapLength, float scale,
        int octaves, float persistance, float lacunarity,
        Vector2 offset, NormalizeMode normalizeMode) {
        float[,] noiseMap = new float[mapWidth, mapLength];

        System.Random random = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++) {
            float offsetX = random.Next(-100000, 100000) + offset.x;
            float offsetZ = random.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfLenght = mapLength / 2f;

        for (int z = 0; z < mapLength; z++) {
            for (int x = 0; x < mapWidth; x++) {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleZ = (z - halfLenght + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf
                        .PerlinNoise(sampleX, sampleZ) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                } else if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, z] = noiseHeight;
            }
        }
        for (int z = 0; z < mapLength; z++) {
            for (int x = 0; x < mapWidth; x++) {
                if (normalizeMode.Equals(NormalizeMode.Local)) {
                    float normalizeHeight = Mathf
                        .InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, z]);
                    noiseMap[x, z] = normalizeHeight;
                } else {
                    float normalizeHeight = (noiseMap[x, z] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, z] = Mathf.Clamp(normalizeHeight, 0f, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}
