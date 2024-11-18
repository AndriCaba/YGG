using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class ButtonManagerScript : MonoBehaviour
{
   public void StartGame()
    {
        // Example action: Load a new scene
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        // Example action: Quit the application
        Application.Quit();
    }
    public void retry(){

        SceneManager.LoadScene("Arena");

    }

     public void toMenu(){

        SceneManager.LoadScene("Start");
        
    }
}
