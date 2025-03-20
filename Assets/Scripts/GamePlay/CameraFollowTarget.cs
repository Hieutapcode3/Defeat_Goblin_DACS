using UnityEngine;

public class CameraFollowMouse : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 startMousePosition;
    private Vector2 currentMousePosition;
    private bool isDragging = false;
    private Rigidbody2D rb;
    private Vector2 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        targetPosition = this.transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDelta = (currentMousePosition - startMousePosition).normalized;

            Vector3 moveDirection = new Vector3(-mouseDelta.x, -mouseDelta.y, 0);
            targetPosition = rb.position + (Vector2)moveDirection * moveSpeed * Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }
}