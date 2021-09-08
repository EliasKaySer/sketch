using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour {
    public static Texture2D GetTexture
        (Color[] colourMap, int width, int length) {
        Texture2D texture = new Texture2D(width, length);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D GetTexture(Color[] colourMap, int chunkSize) {
        return GetTexture(colourMap, chunkSize, chunkSize);
    }

    public static Texture2D GetTexture(float[,] heightMap) {
        int width = heightMap.GetLength(0);
        int length = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * length];
        for (int z = 0; z < length; z++) {
            for (int x = 0; x < width; x++) {
                colourMap[z * width + x] = Color
                    .Lerp(Color.black, Color.white, heightMap[x, z]);
            }
        }

        return GetTexture(colourMap, width, length);
    }
}
