using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaperPlaneSpawner : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject paperPlanePrefab;
    public float baseSpawnRate = 2f;
    public float minSpawnRate = 0.3f;
    public float planeSpeed = 4f;
    public float maxPlaneSpeed = 8f;

    private Camera mainCamera;
    private Vector2 screenBounds;
    private bool isSpawning = false;
    private List<GameObject> activePlanes = new List<GameObject>();
    private Coroutine spawnCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    public void StartSpawning()
    {
        isSpawning = true;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnPlanes());
    }
    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    private IEnumerator SpawnPlanes()
    {
        while (isSpawning)
        {
            if (activePlanes.Count < 5)
            {
                SpawnPlane();
            }
            float currentSpawnRate = Mathf.Max(minSpawnRate, baseSpawnRate - (GameManager.Instance.CurrentScore * 0.05f));
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }
    private void SpawnPlane()
    {
        float spawnX = Random.Range(-screenBounds.x, screenBounds.x);
        Vector3 spawnPosition = new Vector3(spawnX, -screenBounds.y - 1f, 0);

        GameObject plane = Instantiate(paperPlanePrefab, spawnPosition, Quaternion.identity);
        plane.GetComponent<PaperPlane>().Initialize(this);
        activePlanes.Add(plane);
    }
    public void UpdateDifficulty(int score)
    {
        float speedMultiplier = 1f + (score * 0.1f);
        float newSpeed = Mathf.Min(maxPlaneSpeed, planeSpeed * speedMultiplier);

        foreach (GameObject plane in activePlanes)
        {
            if (plane != null)
            {
                plane.GetComponent<PaperPlane>().SetSpeed(newSpeed);
            }
        }
    }

    public void RemovePlane(GameObject plane)
    {
        activePlanes.Remove(plane);
    }

    public void ClearAllPlanes()
    {
        foreach (GameObject plane in activePlanes)
        {
            if (plane != null)
            {
                Destroy(plane);
            }
        }
        activePlanes.Clear();
    }
}
