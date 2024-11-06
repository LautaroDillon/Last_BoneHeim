using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, Idamagable
{
    public static PlayerHealth instance;
    [SerializeField] public float life;
    public float _maxlife;
    public Image healthBar;
    public Image berserkFillBar;
    public GameObject berserkBar;
    public bool isInReviveState = false;
    public float reviveTime = 5f;
    private float reviveTimer;
    private bool isDead = false;
    public bool enemyKilled = false;
    [SerializeField] private AudioClip painClip;
    [SerializeField] private AudioClip berserkStartClip;
    [SerializeField] private AudioClip heartbeatClip;

    [Header ("berserk")]
    [SerializeField] public Material berserk;
    public float turnOn;
    public float turnOof;

    private void Awake()
    {
        _maxlife = FlyweightPointer.Player.maxLife;
        berserkBar.gameObject.SetActive(false);
        berserk.SetFloat("_Active", 0);

    }

    void Start()
    {
        life = _maxlife;
        instance = this;
    }

    void Update()
    {
        if (life <= 0 && !isDead)
        {
            // Inicia el estado de revivible
            berserk.SetFloat("_Active", 1);
            StartReviveCountdown();
        }

        // Si el jugador está en el estado de revivible, cuenta el tiempo
        if (isInReviveState)
        {
            reviveTimer -= Time.deltaTime;
            PlayerMovementAdvanced.instance.walkSpeed = 14;
            PlayerMovementAdvanced.instance.sprintSpeed = 14;
            berserkFillBar.fillAmount = reviveTimer / 7;

            // Si el jugador mata a un enemigo en el tiempo límite
            if (enemyKilled)
            {
                RevivePlayer();
                life = _maxlife;
                PlayerMovementAdvanced.instance.walkSpeed = 10;
                PlayerMovementAdvanced.instance.sprintSpeed = 10;
            }

            // Si se agota el tiempo y no ha matado a ningún enemigo
            if (reviveTimer <= 0 && !enemyKilled)
            {
                GameOver();
            }
        }
    }

    void StartReviveCountdown()
    {
        isInReviveState = true;
        SoundManager.instance.PlaySound(heartbeatClip, transform, 1f);
        reviveTimer = reviveTime;
        berserk.SetFloat("_Active", turnOn);
        berserkBar.gameObject.SetActive(true);
        isDead = true;
        Debug.Log("¡Estás en estado de revivible! Tienes " + reviveTime + " segundos para matar a un enemigo.");
    }

    void RevivePlayer()
    {
        life += _maxlife;
        healthBar.fillAmount = life / 100;
        PlayerMovementAdvanced.instance.walkSpeed = 12;
        PlayerMovementAdvanced.instance.sprintSpeed = 12;
        berserk.SetFloat("_Active", 0);
        enemyKilled = false;
        isInReviveState = false;
    }

    void GameOver()
    {
        isInReviveState = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        berserk.SetFloat("_Active", 0);
        SceneManager.LoadScene(2);
        Debug.Log("¡Has muerto definitivamente! Fin del juego.");
    }

    // Llamar a este método cuando el jugador mate a un enemigo
    public void OnEnemyKilled()
    {
        if (isInReviveState)
        {
            enemyKilled = true;
        }
    }
    public void TakeDamage(float dmg)
    {
        life -= dmg;
        healthBar.fillAmount = life / 100;
        SoundManager.instance.PlaySound(painClip, transform, 0.3f);

        if (life <= 0)
        {
            StartReviveCountdown();
            SoundManager.instance.PlaySound(berserkStartClip, transform, 1f);
        }
    }
}
