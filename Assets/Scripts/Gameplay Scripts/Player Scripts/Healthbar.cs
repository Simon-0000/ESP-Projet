using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
  public Slider slider;
  public void updateHealthbar(float health) => slider.value = health;
  public void SetMaxHealth(float health) => slider.maxValue = health;
}
