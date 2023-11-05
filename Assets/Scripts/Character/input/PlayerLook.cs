using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity { get; private set; }
    public float ySensitivity { get; private set; }

    void Start()
    {
        UpdateSensitivity(GameSettingsManager.Instance.MouseSensitivity);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Subscribe to sensitivity changes
        GameSettingsManager.Instance.OnSensitivityChanged += UpdateSensitivity;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if(GameSettingsManager.Instance != null)
        {
            GameSettingsManager.Instance.OnSensitivityChanged -= UpdateSensitivity;
        }
    }

    public void UpdateSensitivity(float sensitivity)
    {
        xSensitivity = sensitivity;
        ySensitivity = sensitivity;
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
}
