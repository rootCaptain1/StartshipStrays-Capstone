using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject prefab; // The prefab of the object you want to place in the grid
    public int gridWidth = 5; // Number of columns in the grid
    public int gridHeight = 5; // Number of rows in the grid
    public float spacing = 1.0f; // Spacing between objects

    public void GenerateGrid()
    {
        Transform buttonParent = new GameObject("ButtonHolder").transform;
        buttonParent.SetParent(transform);

        // Find the center of the room or use a predefined center point
        Vector2 roomCenter = transform.position;

        // Calculate the starting position for the grid
        float startX = roomCenter.x - ((gridWidth - 1) * spacing) / 2;
        float startY = roomCenter.y - ((gridHeight - 1) * spacing) / 2;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 spawnPosition = new Vector3(startX + x * spacing, startY + y * spacing, 0);
                GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
                newObj.transform.SetParent(buttonParent); // Set the parent to keep the hierarchy organized
            }
        }
    }
}