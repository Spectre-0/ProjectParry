using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraSettings : MonoBehaviour
{
    public Camera mainCamera;

    private void Start()
    {
        mainCamera.fieldOfView = GameSettingsManager.Instance.FOV;
    }


}
