using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.WSA;

public class GridManager : MonoBehaviour {

    public static GridManager Instance { get; private set; }

    public enum CellType {
        EMPTY,
        FLOOR,
        WALL
    }

    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap collisionOverlayTilemap;

    private Vector2Int to;
    private Vector2Int from;
    private TileBase toTile;
    private TileBase fromTile;
    bool changedToTile;
    bool changedFromTile;

    [SerializeField] private Transform testPrefab;
    private List<Transform> testVisual;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError("There is more than one GridManager: " + this);
        }
        
        testVisual = new List<Transform>();
    }

    public void ResetTestVisual() {
        foreach (Transform visual in testVisual) {
            Destroy(visual.gameObject);
        }
        testVisual = new List<Transform>();
    }

    public List<PathNode> FindPath(Vector2Int start, Vector2Int goal) {
        List<Vector2Int> visited = new List<Vector2Int>();
        List<PathNode> pq = new List<PathNode>();
        pq.Add(new PathNode(start, 0, Vector2.Distance(start, goal), null));

        int loops = 0;
        while (pq.Count > 0) {
            PathNode curr = pq[0];
            float fMin = curr.g + curr.h;
            for (int i = 1; i < pq.Count; i++) {
                float f = pq[i].g + pq[i].h;
                if (f < fMin) {
                    curr = pq[i];
                    fMin = f;
                }
            }
            pq.Remove(curr);
            Transform testTransform = Instantiate(testPrefab);
            testVisual.Add(testTransform);
            testTransform.GetComponent<TestData>().SetData(loops);
            testTransform.position = new Vector3(curr.cell.x, curr.cell.y, -3);
            if (curr.cell == goal) {
                List<PathNode> path = new List<PathNode>();
                while (curr != null) {
                    testTransform = Instantiate(testPrefab);
                    testVisual.Add(testTransform);
                    testTransform.GetComponent<TestData>().SetData(-5);
                    testTransform.position = new Vector3(curr.cell.x, curr.cell.y, -4);
                    testTransform.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                    path.Insert(0, curr);
                    curr = curr.previous;
                }
                return path;
            }
            Vector2Int[] neighbors = {
                new Vector2Int(curr.cell.x - 1, curr.cell.y), // Left
                new Vector2Int(curr.cell.x + 1, curr.cell.y), // Right
                new Vector2Int(curr.cell.x, curr.cell.y + 1), // Up
                new Vector2Int(curr.cell.x, curr.cell.y - 1) // Down
            };
            foreach (Vector2Int neighbor in neighbors) {
                if (!visited.Contains(neighbor) && CanMove(curr.cell, neighbor)) {
                    pq.Add(new PathNode(neighbor, curr.g + 1, Vector2.Distance(neighbor, goal), curr));
                    visited.Add(neighbor);
                }
            }

            if (loops++ > 10000) {
                Debug.LogError("Finding path required too many iterations (> 10,000).");
            }
        }

        return null;
    }

    public bool CanMove(Vector2Int from, Vector2Int to) {
        Vector2Int difference = to - from;
        if (difference.x == 0 && difference.y == 1) {
            // Moving up
            return IsCellWalkableInFront(to);
        } else if (difference.x == 0 && difference.y == -1 && IsWall(from) && IsFloor(to)) {
            // Moving down from behind wall to in front of wall - not allowed
            return false;
        } else {
            return IsCellWalkableBehind(to);
        }
    }

    public bool IsCellWalkableInFront(Vector2Int cellPosition) {
        return GetCellType(cellPosition) == CellType.FLOOR;
    }

    public bool IsCellWalkableBehind(Vector2Int cellPosition) {
        int x = cellPosition.x;
        int y = cellPosition.y;
        CellType type = GetCellType(x, y);
        if (IsFloor(type)) {
            return true;
        }
        if (IsWall(type) && IsFloor(GetCellType(x, y + 1))) {
            return true;
        }
        return false;
    }

    public bool IsWall(Vector2Int cellPosition) {
        return IsWall(GetCellType(cellPosition));
    }

    public bool IsWall(CellType type) {
        return type == CellType.WALL;
    }

    public bool IsFloor(Vector2Int cellPosition) {
        return IsFloor(GetCellType(cellPosition));
    }

    public bool IsFloor(CellType type) {
        return type == CellType.FLOOR;
    }

    public CellType GetCellType(int x, int y) {
        return GetCellType(new Vector2Int(x, y));
    }

    public CellType GetCellType(Vector2Int cellPosition) {
        Vector3Int position = new Vector3Int(cellPosition.x, cellPosition.y, 0);
        if (wallTilemap.HasTile(position) || collisionOverlayTilemap.HasTile(position)) {
            return CellType.WALL;
        }
        if (floorTilemap.HasTile(position)) {
            return CellType.FLOOR;
        }
        return CellType.EMPTY;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        return new Vector2Int((int)Mathf.Floor(worldPosition.x), (int)Mathf.Floor(worldPosition.y));
    }

    public void UpdateTileSortingOrder(Vector2Int to, Vector2Int from) {
        RevertTileSortingOrder();

        changedToTile = false;
        if (wallTilemap.HasTile((Vector3Int)to)) {
            this.to = to;
            toTile = wallTilemap.GetTile((Vector3Int)to);
            collisionOverlayTilemap.SetTile((Vector3Int)to, toTile);
            wallTilemap.SetTile((Vector3Int)to, null);
            changedToTile = true;
        }

        changedFromTile = false;
        if (wallTilemap.HasTile((Vector3Int)from)) {
            this.from = from;
            fromTile = wallTilemap.GetTile((Vector3Int)from);
            collisionOverlayTilemap.SetTile((Vector3Int)from, fromTile);
            wallTilemap.SetTile((Vector3Int)from, null);
            changedFromTile = true;
        }
    }

    public void RevertTileSortingOrder() {
        if (changedToTile) {
            collisionOverlayTilemap.SetTile((Vector3Int)to, null);
            wallTilemap.SetTile((Vector3Int)to, toTile);
        }

        if (changedFromTile) {
            collisionOverlayTilemap.SetTile((Vector3Int)from, null);
            wallTilemap.SetTile((Vector3Int)from, fromTile);
        }
    }

}
