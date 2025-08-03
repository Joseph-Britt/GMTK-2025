using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private List<PathNode> path;
    private Vector2Int from;
    private Vector2Int to;
    private bool hasTarget;
    private float moveTimer;

    public enum Direction {
        NONE,
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError("There is more than one player: " + this);
        }

        rb = GetComponent<Rigidbody2D>();
        animator.SetBool("Walking", false);
        animator.SetInteger("Direction", (int) Direction.SOUTH);
    }

    private void Update() {
        // Find path
        if (Input.GetMouseButton(0)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GridManager.Instance.ResetTestVisual();
            Vector2Int goal = GridManager.Instance.GetGridPosition(mousePosition);
            List<PathNode> newPath = GridManager.Instance.FindPath(to, goal);
            if (newPath != null) {
                newPath.RemoveAt(0);
                path = newPath;
            } else if ((path == null || path.Count == 0) && !hasTarget) {
                SetDirection(goal.x - to.x, goal.y - to.y);
            }
        }

        // Handle interaction 
        if (Input.GetMouseButtonDown(1)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int target = GridManager.Instance.GetGridPosition(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Door door)) {
                door.TryInteract(GridManager.Instance.GetGridPosition(transform.position));
            }
            SetDirection(target.x - to.x, target.y - to.y);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            GridManager.Instance.ResetTestVisual();
        }

        // Follow path
        if (hasTarget) {
            // Move to target
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

            SetDirection(to.x - from.x, to.y - from.y);
            animator.SetBool("Walking", true);

            // Update sorting order of tiles
            GridManager.Instance.UpdateTileSortingOrder(to, from);
        }
    }

    private void SetDirection(int dx, int dy) {
        if (Mathf.Abs(dy) >= Mathf.Abs(dx)) {
            if (dy > 0) {
                animator.SetInteger("Direction", (int)Direction.NORTH);
            } else {
                animator.SetInteger("Direction", (int)Direction.SOUTH);
            }
        } else {
            if (dx > 0) {
                animator.SetInteger("Direction", (int)Direction.EAST);
            } else {
                animator.SetInteger("Direction", (int)Direction.WEST);
            }
        }
    }

}
