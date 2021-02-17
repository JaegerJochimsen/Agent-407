using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class staminaBar : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        slider.value = MovePlayer.stamina;
    }

    public void SetStamina(float stamina)
    {
        slider.value = stamina;
    }
}
