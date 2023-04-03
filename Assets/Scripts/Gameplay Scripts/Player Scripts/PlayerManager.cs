using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuUI;
   public static bool isPaused;

   private void Update()
   {
      if (Input.GetKeyDown("escape"))
      {
         if (isPaused)
            Resume();
         else
         {
            Pause();
         }
      }
      
   }

    public void Pause()
   {
      PauseMenuUI.SetActive(true);
      Time.timeScale = 0f;
      isPaused = true;
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
   }

   public void Resume()
   {
      
      PauseMenuUI.SetActive(false);
      Time.timeScale = 1f;
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      isPaused = false;
      
}
}
