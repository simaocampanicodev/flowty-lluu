using UnityEngine;

public class PaperPlane : MonoBehaviour
{
    private float speed = 4f;
    private PaperPlaneSpawner spawner;
    private Camera mainCamera;
    private Vector2 screenBounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (transform.position.y > screenBounds.y + 1f)
        {
            Destroy(gameObject);
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
            DestroyPlane();
        }
    }
}
