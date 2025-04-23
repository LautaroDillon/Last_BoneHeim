using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityController : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX;
    public float sensY;

    [Header("References")]
    public Slider sensitivitySlider;

    public float startingSensitivity = 400f;

    [Header("Options")]
    public bool clearPrefsOnStart = false;

    private void Start()
    {
        if (clearPrefsOnStart)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs cleared.");
        }

        if (!PlayerPrefs.HasKey("Sensitivity"))
        {
            PlayerPrefs.SetFloat("Sensitivity", startingSensitivity);
            PlayerPrefs.Save();
            Debug.Log("No saved sensitivity found. Setting default: " + startingSensitivity);
        }

        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity");
        sensX = savedSensitivity;
        sensY = savedSensitivity;

        sensitivitySlider.value = savedSensitivity;
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);

        Debug.Log("Sensitivity Loaded: " + savedSensitivity);
    }

    public void SetSensitivity(float value)
    {
        sensX = value;
        sensY = value;

        PlayerPrefs.SetFloat("Sensitivity", value);
        PlayerPrefs.Save();

        Debug.Log("Sensitivity Set To: " + value);
    }
}
