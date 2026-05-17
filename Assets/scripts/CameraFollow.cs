using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance; 

    [Header("Objetivo")]
    public Transform target;

    [Header("Configuración de Suavidad")]
    [Range(0.01f, 1f)]
    public float smoothTime = 0.15f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private Vector3 velocity = Vector3.zero;

    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;

    void Awake()
    {
        instance = this;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 finalPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        if (shakeTimer > 0)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shakeMagnitude;
            
            finalPosition.x += randomOffset.x;
            finalPosition.y += randomOffset.y;

            shakeTimer -= Time.deltaTime;
        }

        transform.position = finalPosition;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}