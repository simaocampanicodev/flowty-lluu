using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float chaseSpeed = 3f;
    public float rotationSmoothness = 0.1f;

    private Transform player;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float targetRotation = 0f;
    private float currentRotationVelocity = 0f;
    private bool isChasing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            ChasePlayer();
            HandleRotation();
        }
    }
    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = angle + 90f;
    }

    private void HandleRotation()
    {
        float currentRotation = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation, ref currentRotationVelocity, rotationSmoothness);
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    public void StartChasing()
    {
        isChasing = true;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    public void StopChasing()
    {
        isChasing = false;
    }
}