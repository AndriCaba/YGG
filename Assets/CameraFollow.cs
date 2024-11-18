using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // The object the camera will follow
    public float smoothSpeed = 0.125f; // Smooth movement speed
    public Vector3 offset;          // Offset position relative to the target

    void FixedUpdate()
    {
        // Target position with offset
        Vector3 desiredPosition = target.position + offset;

        // Keep Z position unchanged for 2D
        desiredPosition.z = transform.position.z;

        // Smoothly interpolate the camera's position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
