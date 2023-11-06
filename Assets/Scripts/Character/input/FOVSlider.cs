using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class FOVSlider : MonoBehaviour
{
    public Slider fovSlider;
    
    public TextMeshProUGUI fovText;



    private void Start()
    {
        fovSlider.value = GameSettingsManager.Instance.FOV;
        OnFOVSliderChanged(fovSlider.value);
        fovSlider.onValueChanged.AddListener(OnFOVSliderChanged);
    }

    public void OnFOVSliderChanged(float value)
    {
        GameSettingsManager.Instance.SetFOV(value);

        Camera playerCamera = Camera.main; // Assuming you want to change the main camera's FOV
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = value;
        }
        
        if (fovText != null)
        {
            fovText.text = $"{value.ToString("0")}";
        }
    }
}