using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {
    [SerializeField] Renderer textureRander;
    [SerializeField] MeshRenderer meshRenderer;

    public void DrawMap(Texture2D texture) {
        textureRander.sharedMaterial.mainTexture = texture;
        textureRander.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMap(Texture2D texture, MeshData meshData) {
        meshRenderer.GetComponentInParent<MeshFilter>().sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}