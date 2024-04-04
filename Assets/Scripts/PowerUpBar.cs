using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpBar : MonoBehaviour
{

    public Slider slider;

    public void Start()
    {
        slider.value = 0;
    }

    public void SetMaxValue(float PowerUpTime)
    {
        slider.maxValue = PowerUpTime;
    }

    public void SetCurrentValue(float PowerUpTimeRemaining)
    {
        slider.value = PowerUpTimeRemaining;
    }

}
