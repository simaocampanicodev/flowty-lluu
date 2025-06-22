using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSmoothness = 0.1f;

    [Header("Screen")]
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    private Vector3 initialPosition;
    private float targetRotation = 0f;
    private float currentRotationVelocity = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        objectHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            HandleMovement();
            HandleRotation();
        }
    }
    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            movement.y += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            movement.y -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x -= 1f;
            targetRotation = 15f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movement.x += 1f;
            targetRotation = -15f;
        }

        if (movement.x == 0)
        {
            targetRotation = 0f;
        }

        Vector3 newPosition = transform.position + movement.normalized * moveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);

        transform.position = newPosition;
    }
    private void HandleRotation()
    {
        float currentRotation = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation, ref currentRotationVelocity, rotationSmoothness);
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        targetRotation = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("PaperPlane"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
