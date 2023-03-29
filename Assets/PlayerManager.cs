using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
   private void Update()
   {
      if (Input.GetKey(KeyCode.Escape))
         SceneManager.LoadScene("menu");

   }
}
