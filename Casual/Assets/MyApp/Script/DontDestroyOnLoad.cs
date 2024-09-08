using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        Resolution currentRes = Screen.currentResolution;
        Debug.Log($"Current resolution: {currentRes.width} x {currentRes.height}");
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        Application.targetFrameRate = 60;
        Debug.Log(Application.targetFrameRate);
    }
}
