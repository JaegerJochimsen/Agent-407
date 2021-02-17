using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Credit: https://www.youtube.com/watch?v=BLfNP4Sc_iA (basic health bar layour -- adapted for stamina and ammo)

public class healthBar : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        slider.value = MovePlayer.health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }
}
