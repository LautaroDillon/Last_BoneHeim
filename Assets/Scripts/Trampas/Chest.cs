using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum ChestType { Normal, Cursed }
    public ChestType chestType;

    [Header("Rewards")]
    public GameObject rewardPrefab;
    public Transform rewardSpawnPoint;

    [Header("Cursed Effects")]
    public GameObject cursedEffectPrefab;
    public GameObject enemyPrefab;
    public GameObject explosion;
    public Transform spawnPoint;

    [Header("Sounds")]
    [SerializeField] private AudioClip chestOpeningClip;
    [SerializeField] private AudioClip cursedOpeningClip;

    public GameObject open;
    public GameObject closed;

    //public float curseEffectDuration = 5f;
    private bool isOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isOpened)
        {
            isOpened = true;
            if (chestType == ChestType.Normal)
            {
                OpenNormalChest();
                SoundManager.instance.PlaySound(chestOpeningClip, transform, 1f, false);
            }
            else
            {
                ActivateCursedChest();
                SoundManager.instance.PlaySound(cursedOpeningClip, transform, 1f, false);
            }
        }
    }

    private void OpenNormalChest()
    {
        Debug.Log("Cofre abierto: recompensa obtenida");
        Instantiate(rewardPrefab, rewardSpawnPoint.position, Quaternion.identity);
        Destroy(gameObject); // Elimina el cofre tras abrirlo
    }

    private void ActivateCursedChest()
    {
        Debug.Log("�Cofre maldito activado!");

        // Mostrar efecto de maldici�n
        if (cursedEffectPrefab != null)
        {
            Instantiate(cursedEffectPrefab, transform.position, Quaternion.identity);
        }

        // Activar un efecto maldito aleatorio
        int effect = Random.Range(0, 2);
        switch (effect)
        {
            case 0:
                InvokeEnemies();
                break;
            case 1:
                TriggerExplosion();
                break;
            case 2:
               // ApplyCurseToPlayer();
                break;
        }

        Destroy(gameObject, 2f); // Elimina el cofre despu�s de un tiempo
    }

    private void InvokeEnemies()
    {
        Debug.Log("Invocando enemigos...");
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

    private void TriggerExplosion()
    {
        Debug.Log("Explosi�n activada");
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.1f);
        // A�adir l�gica para una explosi�n (ejemplo: da�o en �rea)
    }

    public void Change()
    {
        open.SetActive(false);
        closed.SetActive(true);
    }


    /*private void ApplyCurseToPlayer()
    {
        Debug.Log("Maldici�n aplicada al jugador");
        // Reduce temporalmente las habilidades del jugador
        PlayerStats player = GameManager.instance.thisIsPlayer.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.ApplyCurse(curseEffectDuration);
        }
    }*/
}
