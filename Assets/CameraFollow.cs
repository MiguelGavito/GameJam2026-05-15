using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target; // Aquí arrastraremos a tu jugador

    [Header("Configuración de Suavidad")]
    [Range(0.01f, 1f)]
    public float smoothTime = 0.15f; // Tiempo que tarda en alcanzar al jugador

    [Header("Desplazamiento (Offset)")]
    public Vector3 offset = new Vector3(0f, 0f, -10f); // -10 en Z es vital para cámaras 2D

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // Si por alguna razón el jugador es destruido o no está asignado, evitamos errores
        if (target == null) return;

        // Calculamos cuál debería ser la posición ideal de la cámara
        Vector3 desiredPosition = target.position + offset;

        // Movemos la cámara de su posición actual hacia la posición deseada de forma fluida
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}