using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class MainMenu : MonoBehaviour

{

    public void LoadScene()
    {
        SceneManager.LoadScene("ProceduralMap");

    }
    public void QuitGame()
    { Application.Quit();
        EditorApplication.isPlaying = false;
    }
    private void Awake()
    {
        SetCursor();    
    }
    void SetCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


    }
}
