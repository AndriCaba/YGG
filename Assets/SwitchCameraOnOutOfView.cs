using UnityEngine;

public class SwitchCameraOnOutOfView : MonoBehaviour
{
    public Camera[] cameras; // Array of cameras to switch between
    private int currentCameraIndex = 0;
    private Renderer objectRenderer;

    private void Start()
    {
        if (cameras.Length == 0)
        {
            Debug.LogError("No cameras assigned to the script.");
            return;
        }

        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer found on the object.");
        }

        ActivateCamera(currentCameraIndex);
    }

    private void Update()
    {
        if (objectRenderer == null) return;

        if (!IsObjectVisible())
        {
            SwitchToNextCamera();
        }
    }

    private bool IsObjectVisible()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameras[currentCameraIndex]);
        return GeometryUtility.TestPlanesAABB(planes, objectRenderer.bounds);
    }

    private void SwitchToNextCamera()
    {
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
        ActivateCamera(currentCameraIndex);
    }

    private void ActivateCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = i == index;
        }

        Debug.Log($"Switched to camera: {cameras[index].name}");
    }
}
