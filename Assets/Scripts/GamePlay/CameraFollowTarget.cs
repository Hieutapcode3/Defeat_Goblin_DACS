using UnityEngine;

public class CameraFollowTouch : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isSwiping = false;
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
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        currentTouchPosition = touch.position;
                        Vector2 swipeDelta = (currentTouchPosition - startTouchPosition).normalized;

                        Vector3 moveDirection = new Vector3(-swipeDelta.x, -swipeDelta.y, 0);
                        //transform.position += moveDirection * moveSpeed * Time.deltaTime;
                        targetPosition = rb.position + (Vector2)moveDirection * moveSpeed * Time.deltaTime;
                    }
                    break;

                case TouchPhase.Ended:
                    isSwiping = false;
                    break;
            }
        }
    }
    void FixedUpdate()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }
}
