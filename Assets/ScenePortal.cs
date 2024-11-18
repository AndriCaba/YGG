using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collision detected with: {other.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered the portal. Loading scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("Collision object is not tagged as 'Player'.");
        }
    }
}
