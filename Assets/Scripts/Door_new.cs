using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.WSA;
using Unity.Mathematics;
using System.Linq;

public class Door_new : MonoBehaviour {

    [Serializable]
    public struct TilePair {
        public Vector2Int position { get; private set; }
        public TileBase tile { get; private set; }
    }

    [SerializeField] private List<Vector2Int> interactablePositions;
    [SerializeField] private List<TileBase> openTiles;
    [SerializeField] private List<Vector2Int> openPositions;
    [SerializeField] private List<Tilemap> openTilemaps;
    [SerializeField] private List<TileBase> closedTiles;
    [SerializeField] private List<Vector2Int> closedPositions;
    [SerializeField] private List<Tilemap> closedTilemaps;

    private Animator animator;
    private bool isOpen;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        DisplayTiles(closedTiles, closedPositions, closedTilemaps);
    }

    public bool TryInteract(Vector2Int gridPosition) {
        if (IsInteractable(gridPosition)) {
            Interact();
            return true;
        }
        return false;
    }

    public bool IsInteractable(Vector2Int gridPosition) {
        Vector2Int position = GridManager.Instance.GetGridPosition(transform.position);
        return interactablePositions.Any((Vector2Int offset) => gridPosition == position + offset);
    }

    public bool Interact() {
        isOpen = !isOpen;

        if (isOpen) {
            // Remove closed tiles
            ClearTiles(closedTiles, closedPositions, closedTilemaps);
            // Add open tiles
            //DisplayTiles(openTiles, openPositions);
            animator.SetBool("Open", true);
        } else {
            // Remove open tiles
            //ClearTiles(openTiles, openPositions);
            // Add closed tiles
            DisplayTiles(closedTiles, closedPositions, closedTilemaps);
            animator.SetBool("Open", false);
        }

        return isOpen;
    }

    private void DisplayTiles(List<TileBase> tiles, List<Vector2Int> positions, List<Tilemap> tilemaps) {
        for (int i = 0; i < tiles.Count; i++) {
            SetInteractableTile(GridManager.Instance.GetGridPosition(transform.position) + positions[i], tiles[i], tilemaps[i]);
        }
    }

    private void ClearTiles(List<TileBase> tiles, List<Vector2Int> positions, List<Tilemap> tilemaps) {
        for (int i = 0; i < tiles.Count; i++) {
            SetInteractableTile(GridManager.Instance.GetGridPosition(transform.position) + positions[i], null, tilemaps[i]);
        }
    }

    private void SetInteractableTile(Vector2Int gridPosition, TileBase tile, Tilemap tilemap) {
        tilemap.SetTile(new Vector3Int(gridPosition.x, gridPosition.y, 0), tile);
    }

}
