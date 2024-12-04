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

    public GameObject closed;
    public GameObject open;

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
            }
            else
            {
                ActivateCursedChest();
            }
        }
    }

    private void OpenNormalChest()
    {
        Debug.Log("Cofre abierto: recompensa obtenida");
        closed.SetActive(false);
        open.SetActive(true);
        Instantiate(rewardPrefab, rewardSpawnPoint.position, Quaternion.identity);
    }

    private void ActivateCursedChest()
    {
        Debug.Log("¡Cofre maldito activado!");

        // Mostrar efecto de maldición
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

        closed.SetActive(false);
        open.SetActive(true);
    }

    private void InvokeEnemies()
    {
        Debug.Log("Invocando enemigos...");
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

    private void TriggerExplosion()
    {
        Debug.Log("Explosión activada");
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.1f);
        // Añadir lógica para una explosión (ejemplo: daño en área)
    }

    /*private void ApplyCurseToPlayer()
    {
        Debug.Log("Maldición aplicada al jugador");
        // Reduce temporalmente las habilidades del jugador
        PlayerStats player = GameManager.instance.thisIsPlayer.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.ApplyCurse(curseEffectDuration);
        }
    }*/
}
