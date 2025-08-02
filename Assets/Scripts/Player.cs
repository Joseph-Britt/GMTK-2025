using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Grid wallGrid;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private List<PathNode> path;
    private Vector2Int from;
    private Vector2Int to;
    private bool hasTarget;
    private float moveTimer;

    public enum Direction {
        IDLE,
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator.SetBool("Walking", false);
        animator.SetInteger("Direction", (int) Direction.SOUTH);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            GridManager.Instance.ResetTestVisual();
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            List<PathNode> newPath = GridManager.Instance.FindPath(to, GridManager.Instance.GetGridPosition(mousePosition));
            if (newPath != null) {
                newPath.RemoveAt(0);
                path = newPath;
            } else {
                Debug.Log("No path found");
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            GridManager.Instance.ResetTestVisual();
        }

        // Follow path
        if (hasTarget) {
            // Move to target
            animator.SetBool("Walking", true);
            Vector2 position = Vector2.Lerp(from, to, moveTimer);
            transform.position = new Vector3(position.x, position.y, transform.position.z);
            moveTimer += moveSpeed * Time.deltaTime;
            if (moveTimer >= 1f) {
                transform.position = new Vector3(to.x, to.y, transform.position.z);
                hasTarget = false;
                animator.SetBool("Walking", false);
            }
        } else if (path != null && path.Count > 0) {
            // Set target
            from = GridManager.Instance.GetGridPosition(transform.position);
            to = path[0].cell;
            path.RemoveAt(0);
            hasTarget = true;
            moveTimer = 0f;

            if (from.y < to.y)
            {
                animator.SetInteger("Direction", (int) Direction.NORTH);
            }
            else if (from.x < to.x)
            {
                animator.SetInteger("Direction", (int) Direction.EAST);
            }
            else if(from.y > to.y)
            {
                animator.SetInteger("Direction", (int) Direction.SOUTH);
            } else
            {
                animator.SetInteger("Direction", (int) Direction.WEST);
            }

            // Update sorting order of tiles
            GridManager.Instance.UpdateTileSortingOrder(to, from);
        }
    }

}
