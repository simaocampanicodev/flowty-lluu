using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    [Header("Scale Settings")]
    public float scaleSpeed = 1f;
    public float scaleIntensity = 0.1f;

    [Header("Float Animation")]
    public float floatSpeed = 0.8f;
    public float floatIntensity = 10f;

    [Header("Rotation Animation")]
    public bool enableRotation = true;
    public float rotationSpeed = 0.5f;
    public float rotationIntensity = 2f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private float timeOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;

        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        AnimateLogo();
    }
    private void AnimateLogo()
    {
        float time = Time.time + timeOffset;

        float scaleMultiplier = 1f + Mathf.Sin(time * scaleSpeed) * scaleIntensity;
        transform.localScale = originalScale * scaleMultiplier;

        float floatOffset = Mathf.Sin(time * floatSpeed) * floatIntensity;
        Vector3 newPosition = originalPosition;
        newPosition.y += floatOffset;
        transform.localPosition = newPosition;

        if (enableRotation)
        {
            float rotationOffset = Mathf.Sin(time * rotationSpeed) * rotationIntensity;
            transform.localRotation = Quaternion.Euler(0, 0, rotationOffset);
        }
    }

    public void StopAnimation()
    {
        enabled = false;
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity;
    }

    public void StartAnimation()
    {
        enabled = true;
    }
}
