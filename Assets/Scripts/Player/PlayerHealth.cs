using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour, Idamagable
{
    [Header("References")]
    public Image healthBar;
    public Image berserkFillBar;
    public GameObject berserkBar;
    public FullscreenController fullscreenController;
    public static PlayerHealth instance;

    [Header("Variables")]
    [SerializeField] public float life;
    public float _maxlife;
    public float reviveTime = 5f;
    private float reviveTimer;

    [Header("Bools")]
    public bool isInReviveState = false;
    private bool isDead = false;
    public bool enemyKilled = false;

    [Header("Sounds")]
    [SerializeField] private AudioClip painClip;
    [SerializeField] private AudioClip berserkStartClip;
    [SerializeField] private AudioClip heartbeatClip;

    [Header("Damage Shader")]
    [SerializeField] private ScriptableRendererFeature _fullScreenDamage;
    [SerializeField] private Material _material;

    [Header("Shader Values")]
    [SerializeField] private float _damageDisplayTime = 1.5f;
    [SerializeField] private float _damageFadeoutTime = 0.5f;
    private const float VORONOI_INTENSITY_START_AMOUNT = 1f;
    private const float VIGNETTE_INTENSITY_START_AMOUNT = 1.2f;

    private int _vignetteIntensity = Shader.PropertyToID("_Intensity");
    private int _voronoiIntensity = Shader.PropertyToID("_VoronoiIntensity");

    [Header ("Berserk")]
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
        _fullScreenDamage.SetActive(false);
    }

    void Update()
    {
        if (life >= _maxlife)
            life = _maxlife;
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
        SoundManager.instance.PlaySound(heartbeatClip, transform, 1f, false);
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
        StartCoroutine(HurtShader());
        CameraShake.Shake(0.2f, 0.2f);
        SoundManager.instance.PlaySound(painClip, transform, 0.3f, false);

        if (life <= 0 && !isInReviveState)
        {
            StartReviveCountdown();
            SoundManager.instance.PlaySound(berserkStartClip, transform, 1f, false);
        }
    }

    public IEnumerator HurtShader()
    {
        _fullScreenDamage.SetActive(true);
        _material.SetFloat(_voronoiIntensity, VORONOI_INTENSITY_START_AMOUNT);
        _material.SetFloat(_vignetteIntensity, VIGNETTE_INTENSITY_START_AMOUNT);

        yield return new WaitForSeconds(_damageDisplayTime);

        float timeElapsed = 0f;
        while (timeElapsed < _damageFadeoutTime)
        {
            timeElapsed += Time.deltaTime;
            float lerpedVoronoi = Mathf.Lerp(VORONOI_INTENSITY_START_AMOUNT, 0f, (timeElapsed / _damageFadeoutTime));
            float lerpedVignette = Mathf.Lerp(VIGNETTE_INTENSITY_START_AMOUNT, 0f, (timeElapsed / _damageFadeoutTime));

            _material.SetFloat(_voronoiIntensity, lerpedVoronoi);
            _material.SetFloat(_vignetteIntensity, lerpedVignette);

            yield return null;
        }
        _fullScreenDamage.SetActive(false);
    }
}
