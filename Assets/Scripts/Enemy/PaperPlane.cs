using UnityEngine;

public class PaperPlane : MonoBehaviour
{
    private float speed = 4f;
    private PaperPlaneSpawner spawner;
    private Camera mainCamera;
    private Vector2 screenBounds;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);

            if (transform.position.y > screenBounds.y + 2f)
            {
                DestroyPlane();
            }
        }
    }

    public void Initialize(PaperPlaneSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void DestroyPlane()
    {
        if (spawner != null)
        {
            spawner.RemovePlane(gameObject);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
            DestroyPlane();
        }
    }
}