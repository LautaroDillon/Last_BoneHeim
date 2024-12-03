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
    public Image shieldFillBar;
    public Image berserkFillBar;
    public GameObject shieldBar;
    public GameObject berserkBar;
    public FullscreenController fullscreenController;
    public static PlayerHealth instance;

    [Header("Variables")]
    [SerializeField] public float life;
    public float _maxlife;
    public float reviveTime;
    [SerializeField] private float reviveTimer;
    public float lifeSteal = 0;
    public float shieldAmount = 0;
    public float shieldMax = 0;
    public float shieldRegenTime;

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
    public float berserkSpeedBuff;
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
        healthBar.fillAmount = life / 100;

        BerserkCheck();
        ReviveState();
        ShieldCheck();
    }

    void ShieldCheck()
    {
        shieldFillBar.fillAmount = shieldAmount / 100;
        if (shieldAmount > 0)
            shieldBar.gameObject.SetActive(true);
        else
            shieldBar.gameObject.SetActive(false);
    }

    void BerserkCheck()
    {
        if (life <= 0 && !isDead)
        {
            // Inicia el estado de revivible
            berserk.SetFloat("_Active", 1);
            StartReviveCountdown();
        }
    }

    void ReviveState()
    {
        if (isInReviveState)
        {
            reviveTimer -= Time.deltaTime;
            berserkFillBar.fillAmount = reviveTimer / reviveTime;

            PlayerMovementAdvanced.instance.walkSpeed += berserkSpeedBuff;
            PlayerMovementAdvanced.instance.sprintSpeed += berserkSpeedBuff;

            if (enemyKilled)
            {
                RevivePlayer();
                life = _maxlife;
                PlayerMovementAdvanced.instance.walkSpeed -= berserkSpeedBuff;
                PlayerMovementAdvanced.instance.sprintSpeed -= berserkSpeedBuff;
            }

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
        if(shieldAmount >= dmg)
        {
            shieldAmount -= dmg;
        }

        if(shieldAmount <= 0)
        {
            life -= dmg;
            healthBar.fillAmount = life / 100;
            StartCoroutine(HurtShader());
            CameraShake.Shake(0.2f, 0.2f);
            SoundManager.instance.PlaySound(painClip, transform, 0.3f, false);
            if(shieldMax > 0)
                StartCoroutine(ShieldRegen());
        }
        
        if (life <= 0 && !isInReviveState)
        {
            StartReviveCountdown();
            SoundManager.instance.PlaySound(berserkStartClip, transform, 1f, false);
        }
    }

    public IEnumerator ShieldRegen()
    {
        Debug.Log("Regenerating Shield!");
        yield return new WaitForSeconds(shieldRegenTime);
        shieldAmount = shieldMax;
        Debug.Log("Regenerated Shield!");
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
