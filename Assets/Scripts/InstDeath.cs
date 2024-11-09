using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InstDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();

        if (other.gameObject.layer == 11 && damagableInterface != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(2);
        }
    }
}
