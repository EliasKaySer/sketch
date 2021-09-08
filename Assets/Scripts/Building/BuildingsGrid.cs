using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsGrid : MonoBehaviour
{
    public Vector2Int GridSize = new Vector2Int(10, 10);

    private Buildings[,] grid;
    private Buildings flyingBuilding;
    private Buildings lastBuilding;
    private Camera mainCamera;
    private bool copyMode = false;

    private void Awake()
    {
        grid = new Buildings[GridSize.x, GridSize.y];

        mainCamera = Camera.main;
    }

    public void StarPlacingBuilding(Buildings buildingPrefab)
    {
        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding.gameObject);
        }
        lastBuilding = buildingPrefab;
        flyingBuilding = Instantiate(buildingPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            copyMode = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            copyMode = false;
        }

        if (flyingBuilding != null && Input.GetMouseButtonDown(1))
        {
            copyMode = false;
            Destroy(flyingBuilding.gameObject);
            lastBuilding = null;
        }

        if (flyingBuilding != null)
        {

            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosiotion = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosiotion.x);
                int y = Mathf.RoundToInt(worldPosiotion.z);

                bool avalible = IsPlaceTaken(x, y);

                flyingBuilding.transform.position = new Vector3(x, 0, y);
                flyingBuilding.SetTransparent(avalible);

                if (avalible && Input.GetMouseButtonDown(0))
                {
                    PlaceFlyingBuilding(x, y);
                }
            }
        }
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        if (placeX < 0 || placeX > (GridSize.x - flyingBuilding.Size.x)) return false;
        if (placeY < 0 || placeY > (GridSize.x - flyingBuilding.Size.x)) return false;
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
            {
                if (grid[placeX + x, placeY + y] != null) return false;
            }
        }
        return true;
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        Debug.Log("Size.x = " + flyingBuilding.Size.x + "\n"
            + "Size.y = " + flyingBuilding.Size.y + "\n"
            + "placeX = " + placeX + "\n"
            + "placeY = " + placeY + "\n"
            );
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
            {
                grid[placeX + x, placeY + y] = flyingBuilding;
            }
        }
        flyingBuilding.SetNormal();
        flyingBuilding = null;
        if (copyMode)
        {
            flyingBuilding = Instantiate(lastBuilding);
        }
    }
}
