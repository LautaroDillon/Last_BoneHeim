using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BossTrigger : MonoBehaviour
{
    public static BossTrigger instance;
    [SerializeField] protected AudioClip bossTriggerClip;
    public Canvas bossHealth;
    public Canvas defeatText;
    private void Start()
    {
        bossHealth.gameObject.SetActive(false);
        defeatText.gameObject.SetActive(false);
        instance = this;
    }

   /* private void Update()
    {
        if(ENecro.instanse.necroLife <= 0)
        {
            StartCoroutine("DeathEnd", 1);
            defeatText.gameObject.SetActive(true);
            Debug.Log("Bitch is dead!");
        }
    }*/

    public IEnumerator DeathEnd(int triggerTime)
    {

        FullscreenShader.instance.acidShaderEnabled = false;
        FullscreenShader.instance.armShaderEnabled = false;
        FullscreenShader.instance.blazingShaderEnabled = false;
        FullscreenShader.instance.cursedShaderEnabled = false;
        FullscreenShader.instance.normalShaderEnabled = false;
        FullscreenShader.instance.speedShaderEnabled = false;
        FullscreenShader.instance.vengefulShaderEnabled = false;
        FullscreenShader.instance.blessedShaderEnabled = false;

        Debug.Log("Started Boss End!");
        yield return new WaitForSeconds(triggerTime);
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Started Boss End!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            bossHealth.gameObject.SetActive(true);
            SoundManager.instance.PlaySound(bossTriggerClip, transform, 0.5f, false);
            transform.position += Vector3.up * 50f;
        }
    }
}
