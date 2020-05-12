using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        Destroy(AudioManager.instance.gameObject);
        SceneManager.LoadScene("Menu");
    }

}
