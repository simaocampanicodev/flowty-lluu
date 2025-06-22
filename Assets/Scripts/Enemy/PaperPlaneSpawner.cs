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
    public int maxActivePlanes = 8;

    private Camera mainCamera;
    private Vector2 screenBounds;
    private bool isSpawning = false;
    private List<GameObject> activePlanes = new List<GameObject>();
    private Coroutine spawnCoroutine;

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
            CleanupNullReferences();


            if (activePlanes.Count < maxActivePlanes)
            {
                SpawnPlane();
            }

            float currentSpawnRate = Mathf.Max(minSpawnRate, baseSpawnRate - (GameManager.Instance.CurrentScore * 0.05f));

            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    private void CleanupNullReferences()
    {
        activePlanes.RemoveAll(plane => plane == null);
    }

    private void SpawnPlane()
    {
        if (paperPlanePrefab == null)
        {
            return;
        }

        float spawnX = Random.Range(-screenBounds.x, screenBounds.x);
        Vector3 spawnPosition = new Vector3(spawnX, -screenBounds.y - 1f, 0);

        GameObject plane = Instantiate(paperPlanePrefab, spawnPosition, Quaternion.identity);

        PaperPlane planeScript = plane.GetComponent<PaperPlane>();
        if (planeScript != null)
        {
            planeScript.Initialize(this);

            float speedMultiplier = 1f + (GameManager.Instance.CurrentScore * 0.1f);
            float newSpeed = Mathf.Min(maxPlaneSpeed, planeSpeed * speedMultiplier);
            planeScript.SetSpeed(newSpeed);
        }

        activePlanes.Add(plane);
    }

    public void UpdateDifficulty(int score)
    {
        float speedMultiplier = 1f + (score * 0.1f);
        float newSpeed = Mathf.Min(maxPlaneSpeed, planeSpeed * speedMultiplier);

        CleanupNullReferences();

        foreach (GameObject plane in activePlanes)
        {
            if (plane != null)
            {
                PaperPlane planeScript = plane.GetComponent<PaperPlane>();
                if (planeScript != null)
                {
                    planeScript.SetSpeed(newSpeed);
                }
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