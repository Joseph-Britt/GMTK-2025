using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour {

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Grid wallGrid;
    [SerializeField] private Tilemap wallTilemap;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Vector2 moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        transform.position += moveSpeed * Time.deltaTime * (Vector3)moveDirection;
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int position = new Vector2Int((int) Mathf.Floor(mousePosition.x), (int) Mathf.Floor(mousePosition.y));
            Debug.Log(position);
            Debug.Log(wallTilemap.HasTile((Vector3Int) position));
        }
    }

}
