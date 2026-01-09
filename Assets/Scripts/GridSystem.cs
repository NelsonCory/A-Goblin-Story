using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 originPosition = Vector3.zero;
    
    [Header("Debug Visualization")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private bool showRuntimeDebug = true;
    [SerializeField] private Color debugColor = Color.red;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Material lineMaterial;

    private List<LineRenderer> gridLines = new List<LineRenderer>();

    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;
    public float CellSize => cellSize;
    public Vector3 OriginPosition => originPosition;

    /// <summary>
    /// Converts world position to grid coordinates
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - originPosition;
        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int y = Mathf.FloorToInt(localPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Converts grid coordinates to world position (center of cell)
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return GridToWorld(gridPosition.x, gridPosition.y);
    }

    /// <summary>
    /// Converts grid coordinates to world position (center of cell)
    /// </summary>
    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f, 0) + originPosition;
    }

    /// <summary>
    /// Converts grid coordinates to world position (bottom-left corner of cell)
    /// </summary>
    public Vector3 GridToWorldCorner(int x, int y)
    {
        return new Vector3(x * cellSize, y * cellSize, 0) + originPosition;
    }

    /// <summary>
    /// Checks if grid coordinates are within bounds
    /// </summary>
    public bool IsValidGridPosition(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    /// <summary>
    /// Checks if grid coordinates are within bounds
    /// </summary>
    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return IsValidGridPosition(gridPosition.x, gridPosition.y);
    }

    /// <summary>
    /// Gets the world position of the center of a grid cell
    /// </summary>
    public Vector3 GetCellCenter(int x, int y)
    {
        return GridToWorld(x, y);
    }

    /// <summary>
    /// Gets the world position of the center of a grid cell
    /// </summary>
    public Vector3 GetCellCenter(Vector2Int gridPosition)
    {
        return GetCellCenter(gridPosition.x, gridPosition.y);
    }

    private void Awake()
    {
        // Create a default material if none is assigned
        if (lineMaterial == null && showRuntimeDebug)
        {
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
            lineMaterial.color = debugColor;
        }
    }

    private void Start()
    {
        if (showRuntimeDebug)
        {
            DrawGridBorders();
        }
    }

    private void DrawGridBorders()
    {
        // Clear existing lines
        ClearGridLines();

        // Draw vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = GridToWorldCorner(x, 0);
            Vector3 end = GridToWorldCorner(x, gridHeight);
            CreateLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = GridToWorldCorner(0, y);
            Vector3 end = GridToWorldCorner(gridWidth, y);
            CreateLine(start, end);
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.color = debugColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.sortingOrder = 100; // Ensure lines render on top

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        gridLines.Add(lr);
    }

    private void ClearGridLines()
    {
        foreach (var line in gridLines)
        {
            if (line != null && line.gameObject != null)
            {
                Destroy(line.gameObject);
            }
        }
        gridLines.Clear();
    }

    /// <summary>
    /// Call this method to refresh the grid visualization when grid settings change
    /// </summary>
    public void RefreshVisualization()
    {
        if (showRuntimeDebug)
        {
            DrawGridBorders();
        }
    }

    private void OnValidate()
    {
        // Refresh visualization when values change in editor during play mode
        if (Application.isPlaying && showRuntimeDebug)
        {
            DrawGridBorders();
        }
    }

    // Editor visualization (Scene view)
    private void OnDrawGizmos()
    {
        if (!showDebug) return;

        Gizmos.color = debugColor;

        // Draw grid borders
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = GridToWorldCorner(x, 0);
            Vector3 end = GridToWorldCorner(x, gridHeight);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = GridToWorldCorner(0, y);
            Vector3 end = GridToWorldCorner(gridWidth, y);
            Gizmos.DrawLine(start, end);
        }
    }
}

