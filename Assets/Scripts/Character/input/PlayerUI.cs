using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerMotor playerMotor;
    public Image healthBar;
    public Image staminaBar;

    

    void Update()
    {
        healthBar.fillAmount = playerMotor.currentHealth / playerMotor.maxHealth;
        staminaBar.fillAmount = playerMotor.currentStamina / playerMotor.maxStamina;
    }
}