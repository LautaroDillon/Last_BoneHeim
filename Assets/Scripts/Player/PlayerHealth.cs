using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, Idamagable
{
    public static PlayerHealth instance;
    [SerializeField] float life;
    public float _maxlife;
    public Image healthBar;
    public bool isInReviveState = false;
    public float reviveTime = 10f;
    private float reviveTimer;
    private bool isDead = false;
    private bool enemyKilled = false;
    [SerializeField] private AudioClip painClip;

    private void Awake()
    {
        _maxlife = FlyweightPointer.Player.maxLife;
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
            StartReviveCountdown();
        }

        // Si el jugador est� en el estado de revivible, cuenta el tiempo
        if (isInReviveState)
        {
            reviveTimer -= Time.deltaTime;
            PlayerMovementAdvanced.instance.walkSpeed = 40;
            PlayerMovementAdvanced.instance.sprintSpeed = 60;

            // Si el jugador mata a un enemigo en el tiempo l�mite
            if (enemyKilled)
            {
                RevivePlayer();
                life = _maxlife;
            }

            // Si se agota el tiempo y no ha matado a ning�n enemigo
            if (reviveTimer <= 0 && !enemyKilled)
            {
                GameOver();
            }
        }
    }

    void StartReviveCountdown()
    {
        isInReviveState = true;
        reviveTimer = reviveTime;
        isDead = true;
        Debug.Log("�Est�s en estado de revivible! Tienes " + reviveTime + " segundos para matar a un enemigo.");
    }

    void RevivePlayer()
    {
        life += _maxlife;
        PlayerMovementAdvanced.instance.walkSpeed = 12;
        PlayerMovementAdvanced.instance.sprintSpeed = 12;
        isInReviveState = false;
    }

    void GameOver()
    {
        isInReviveState = false;
        SceneManager.LoadScene(0);
        Debug.Log("�Has muerto definitivamente! Fin del juego.");

    }

    // Llamar a este m�todo cuando el jugador mate a un enemigo
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
        }
    }
}
