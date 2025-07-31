using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Vector2 moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        transform.position += moveSpeed * Time.deltaTime * (Vector3) moveDirection;
    }

}
