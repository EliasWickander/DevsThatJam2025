using System;
using CustomToolkit.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour
{
    [Scene]
    [SerializeField]
    private string m_levelScene;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(m_levelScene);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
