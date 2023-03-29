using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private PlayerLook player;
    public void SetSensitivity (float x)
    {
        player.xSensitivity =100f;
        //ySensitivity *= y.value;

    }
}
