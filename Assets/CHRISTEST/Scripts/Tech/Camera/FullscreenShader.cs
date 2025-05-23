using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class FullscreenShader : MonoBehaviour
{
    public static FullscreenShader instance;

    void Start()
    {
        instance = this;
        fullscreenSpeedShader.SetActive(false);
        fullscreenHitShader.SetActive(false);
    }

    [Header("Speed Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenSpeedShader;
    public Shader speedShader; 
    private Material speedMaterial;
    public bool speedShaderEnabled = false;

    [Header("Hit Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenHitShader;
    public Shader hitShader;
    private Material hitMaterial;
    public bool hitShaderEnabled = false;
    
    /*
    [Header("Arm Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenArmShader;
    public Shader armShader;
    private Material armMaterial;
    public bool armShaderEnabled = false;

    [Header("Acid Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenAcidShader;
    public Shader acidShader;
    private Material acidMaterial;
    public bool acidShaderEnabled = false;

    [Header("Normals Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenNormalShader;
    public Shader normalShader;
    private Material normalMaterial;
    public bool normalShaderEnabled = false;

    [Header("Cursed Organ Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenCursedShader;
    public Shader cursedShader;
    private Material cursedMaterial;
    public bool cursedShaderEnabled = false;

    [Header("Vengeful Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenVengefulShader;
    public Shader vengefulShader;
    private Material vengefulMaterial;
    public bool vengefulShaderEnabled = false;

    [Header("Blessed Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenBlessedShader;
    public Shader blessedShader;
    private Material blessedMaterial;
    public bool blessedShaderEnabled = false;

    [Header("Blazing Shader")]
    [SerializeField] private ScriptableRendererFeature fullscreenBlazingShader;
    public Shader blazingShader;
    private Material blazingMaterial;
    public bool blazingShaderEnabled = false;
    */


    void Update()
    {
        SpeedShader();
        HitShader();
    }

    void SpeedShader()
    {
        if (speedShaderEnabled)
        {
            if (speedMaterial != null)
            {
                speedMaterial.shader = speedShader;
            }
            fullscreenSpeedShader.SetActive(true);
        }
        else
        {
            fullscreenSpeedShader.SetActive(false);
        }
    }

    void HitShader()
    {
        if (hitShaderEnabled)
        {
            if (hitMaterial != null)
            {
                hitMaterial.shader = hitShader;
            }
            fullscreenHitShader.SetActive(true);
            StartCoroutine("HitShaderTrigger", 1f);
            
        }
        else
        {
            fullscreenHitShader.SetActive(false);
        }
    }
    /*
    void ArmRecoveryShader()
    {
        if (armShaderEnabled)
        {
            if (armMaterial != null)
            {
                armMaterial.shader = armShader;
            }
            fullscreenArmShader.SetActive(true);
            StartCoroutine("ArmRecoveryTrigger", 1.5f);
        }
        else
        {
            fullscreenArmShader.SetActive(false);
        }
    }

    void AcidShader()
    {
        if (acidShaderEnabled)
        {
            if (acidMaterial != null)
            {
                acidMaterial.shader = acidShader;
            }
            fullscreenAcidShader.SetActive(true);
            StartCoroutine("AcidTrigger", 1.5f);
        }
        else
        {
            fullscreenAcidShader.SetActive(false);
        }
    }

    #region Organ Shaders
    void NormalOrganShader()
    {
        if (normalShaderEnabled)
        {
            if (normalMaterial != null)
            {
                normalMaterial.shader = normalShader;
            }
            fullscreenNormalShader.SetActive(true);
            StartCoroutine("NormalOrganTrigger", 1.5f);
        }
        else
        {
            fullscreenNormalShader.SetActive(false);
        }
    }

    void CursedOrganShader()
    {
        if (cursedShaderEnabled)
        {
            if (cursedMaterial != null)
            {
                cursedMaterial.shader = cursedShader;
            }
            fullscreenCursedShader.SetActive(true);
            StartCoroutine("CursedOrganTrigger", 1.5f);
        }
        else
        {
            fullscreenCursedShader.SetActive(false);
        }
    }

    void VengefulOrganShader()
    {
        if (vengefulShaderEnabled)
        {
            if (vengefulMaterial != null)
            {
                vengefulMaterial.shader = vengefulShader;
            }
            fullscreenVengefulShader.SetActive(true);
            StartCoroutine("VengefulOrganTrigger", 1.5f);
        }
        else
        {
            fullscreenVengefulShader.SetActive(false);
        }
    }

    void BlazingOrganShader()
    {
        if (blazingShaderEnabled)
        {
            if (blazingMaterial != null)
            {
                blazingMaterial.shader = blazingShader;
            }
            fullscreenBlazingShader.SetActive(true);
            StartCoroutine("BlazingOrganTrigger", 1.5f);
        }
        else
        {
            fullscreenBlazingShader.SetActive(false);
        }
    }

    void BlessedOrganShader()
    {
        if (blessedShaderEnabled)
        {
            if (blessedMaterial != null)
            {
                blessedMaterial.shader = blessedShader;
            }
            fullscreenBlessedShader.SetActive(true);
            StartCoroutine("BlessedOrganTrigger", 1.5f);
        }
        else
        {
            fullscreenBlessedShader.SetActive(false);
        }
    }
    #endregion
    */
    public IEnumerator HitShaderTrigger(float triggerTime)
    {
        hitShaderEnabled = true;
        yield return new WaitForSeconds(triggerTime);
        hitShaderEnabled = false;
    }
    /*
    #region IEnumerators
    public IEnumerator BlessedOrganTrigger(float triggerTime)
    {
        blessedShaderEnabled = true;
        yield return new WaitForSeconds(triggerTime);
        blessedShaderEnabled = false;
    }
    public IEnumerator NormalOrganTrigger(float triggerTime)
    {
        normalShaderEnabled = true;
        yield return new WaitForSeconds(triggerTime);
        normalShaderEnabled = false;
    }
    public IEnumerator VengefulOrganTrigger(float triggerTime)
    {
        vengefulShaderEnabled = true;
        yield return new WaitForSeconds(triggerTime);
        vengefulShaderEnabled = false;
    }
    public IEnumerator CursedOrganTrigger(float triggerTime)
    {
        cursedShaderEnabled = true;
        yield return new WaitForSeconds(triggerTime);
        cursedShaderEnabled = false;
    }

    public IEnumerator BlazingOrganTrigger(float triggerTime)
    {
        blazingShaderEnabled = true;
        yield return new WaitForSeconds(triggerTime);
        blazingShaderEnabled = false;
    }

    public IEnumerator ArmRecoveryTrigger(float triggerTime)
    {
        armShaderEnabled = true;
        yield return new WaitForSeconds(triggerTime);
        armShaderEnabled = false;
    }

    public IEnumerator AcidTrigger(float triggerTime)
    {
        acidShaderEnabled = true;
        yield return new WaitForSeconds(triggerTime);
        acidShaderEnabled = false;
    }
    #endregion*/
}
