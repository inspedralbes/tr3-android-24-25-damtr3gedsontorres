using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Referencia al personaje
    public float smoothSpeed = 0.125f;  // Suavizado del movimiento
    public Vector3 offset;  // Offset para ajustar la posición de la cámara

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
