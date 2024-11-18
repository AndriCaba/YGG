using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionOnCollision : MonoBehaviour
{
    // Name of the scene to load. Can be set in the Unity Inspector.
    [SerializeField]
    private string sceneName;

    // Update is called once per frame
    private void Update()
    {
        // Check if the "P" key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P key pressed. Transitioning to next scene.");
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("Scene name is not set. Please assign a scene name in the Inspector.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision with Player detected!");
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger detected with: {other.gameObject.name}");
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
