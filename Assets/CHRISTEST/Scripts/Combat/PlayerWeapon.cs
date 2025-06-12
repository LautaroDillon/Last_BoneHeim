using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using TMPro;
using EZCameraShake;

public class PlayerWeapon : MonoBehaviour
{
    #region Variables
    public static PlayerWeapon Instance;

    public enum FireMode { SemiAuto, Burst, FullAuto, Shotgun }
    public FireMode fireMode = FireMode.SemiAuto;

    [Header("Keybinds")]
    public KeyCode shootButton;
    public KeyCode reloadButton;
    public KeyCode switchFireModeKey = KeyCode.V;

    [Header("Gun Settings")]
    public float fireRate = 0.2f;
    public int burstCount = 3;
    public int damage = 10;
    public int magazineSize = 30;
    public int currentAmmo;
    public float reloadTime = 2f;

    [Header("Shotgun Settings")]
    public int shotgunDamage = 60;
    public int pelletsPerShot = 6;
    public float spreadAngle = 10f;
    public float falloffStartDistance = 10f;
    public float minDamageMultiplier = 0.3f;

    [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public TextMeshProUGUI ammoText;
    public GameObject shootParticle;
    private Transform queuedFirePoint;

    [Header("UI References")]
    public TextMeshProUGUI fireModeText;
    public float fireModeDisplayTime = 2f;

    [Header("Bullet Display")]
    public List<GameObject> bulletDisplay = new List<GameObject>();
    [SerializeField] private List<Transform> firePoints;
    [SerializeField] private List<string> fireAnimations;

    [Header("Regeneration Settings")]
    public bool enableRegen = true;
    public float regenDelay = 2f;         // Time after last shot before regen starts
    public float regenInterval = 2f;    // Time between each bullet regen
    public int regenAmountPerInterval = 1;

    private float lastShotTime;
    private Coroutine regenCoroutine;

    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    int bulletIndex;
    #endregion

    #region Awake/Start/Update

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentAmmo = magazineSize;
        shootParticle.transform.position = firePoint.transform.position;
    }

    void Start()
    {
        if (fireModeText == null)
        {
            fireModeText = GameObject.Find("FireModeText")?.GetComponent<TextMeshProUGUI>();
        }
        UpdateBulletDisplay();
        UpdateAmmoUI();

        if (regenCoroutine == null && enableRegen)
            regenCoroutine = StartCoroutine(RegenerateAmmo());
    }

    public void Update()
    {
        if (PauseManager.isPaused)
            return;

       // Reloading();
        Shooting();
        UpdateAmmoUI();
        HandleFireModeSwitching();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Mouse0 pressed");
            if (PlayerMovement.instance != null)
                Debug.Log("PlayerMovement exists");
            else
                Debug.LogError("PlayerMovement.instance is null");

            if (PlayerMovement.instance.handAnimator != null)
            {
                Debug.Log("Hand Animator exists");
                PlayerMovement.instance.handAnimator.SetTrigger("TestFire");
            }
            else
                Debug.LogError("Hand Animator is null");
        }
        if (magazineSize <= currentAmmo)
        {
            currentAmmo = magazineSize;
        }
    }

    #endregion

    #region Reload/Shooting Mode Inputs
    void Reloading()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0 || Input.GetKeyDown(reloadButton))
        {
            StartCoroutine(Reload());
        }
    }

    void Shooting()
    {
        switch (fireMode)
        {
            case FireMode.SemiAuto:
                if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + fireRate;
                    Shoot();
                }
                break;

            case FireMode.Burst:
                if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire)
                {
                    StartCoroutine(FireBurst());
                    nextTimeToFire = Time.time + fireRate * burstCount;
                }
                break;

            case FireMode.FullAuto:
                if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + fireRate;
                    Shoot();
                }
                break;

            case FireMode.Shotgun:
                if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + fireRate;
                    ShootShotgun();
                }
                break;
        }
    }
    #endregion

    #region Shoot Methods
    public void Shoot()
    {
        if (isReloading || currentAmmo <= 0)
            return;
        CameraShake.Instance.ShakeOnce(1f, 1f, 0.1f, 1f);
        PlayerMovement.instance.handAnimator.SetBool("Idle", false);
        StartCoroutine(ResetIdle());

        int bulletIndex = currentAmmo - 1;

        // Set queued fire point to be used in the animation event
        queuedFirePoint = (bulletIndex >= 0 && bulletIndex < firePoints.Count)
            ? firePoints[bulletIndex]
            : firePoint;

        // Trigger correct animation
        if (bulletIndex >= 0 && bulletIndex < fireAnimations.Count)
        {
            string animTrigger = fireAnimations[bulletIndex];
            PlayerMovement.instance.handAnimator.SetTrigger(animTrigger);
        }

        currentAmmo--;
        lastShotTime = Time.time;
        //RestartRegen();
        UpdateBulletDisplay();
        UpdateAmmoUI();

        Debug.Log("Animation triggered. Waiting for animation event to fire bullet...");
    }

    void ShootShotgun()
    {
        if (currentAmmo <= 0 || isReloading)
            return;

        int bulletIndex = currentAmmo - 1;

        if (bulletIndex < 0 || bulletIndex >= firePoints.Count)
        {
            Debug.LogWarning("Invalid fire point index for shotgun: " + bulletIndex);
            return;
        }

        Transform selectedFirePoint = firePoints[bulletIndex];

        if (bulletIndex >= 0 && bulletIndex < fireAnimations.Count)
        {
            string animTrigger = fireAnimations[bulletIndex];
            if (!string.IsNullOrEmpty(animTrigger))
                PlayerMovement.instance.handAnimator.SetTrigger(animTrigger);
        }

        PlayerMovement.instance.handAnimator.SetBool("Idle", false);
        StartCoroutine(ResetIdle());

        CameraShake.Instance.ShakeOnce(1.5f, 1.5f, 0.1f, 1f);

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Vector3 targetPoint = ray.GetPoint(1000f);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }

        float baseDistance = Vector3.Distance(selectedFirePoint.position, targetPoint);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 directionToCenter = (targetPoint - selectedFirePoint.position).normalized;

            Vector3 spreadDirection = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * directionToCenter;

            GameObject bullet = Instantiate(bulletPrefab, selectedFirePoint.position, Quaternion.LookRotation(spreadDirection));


            float falloffMultiplier = 1f;
            if (baseDistance > falloffStartDistance)
            {
                float excessDistance = baseDistance - falloffStartDistance;
                falloffMultiplier = Mathf.Lerp(1f, minDamageMultiplier, excessDistance / 20f);
            }

            float pelletDamage = (shotgunDamage / pelletsPerShot) * falloffMultiplier;

            if (bullet.TryGetComponent(out PlayerBullet bulletScript))
                bulletScript.SetDamage(Mathf.RoundToInt(pelletDamage));

            if (bullet.TryGetComponent(out Rigidbody rb))
                rb.velocity = spreadDirection * bulletSpeed;
        }

        currentAmmo--;
        lastShotTime = Time.time;
        //RestartRegen();
        UpdateBulletDisplay();
        Debug.Log("Shotgun fired from finger " + bulletIndex + "! Ammo left: " + currentAmmo);
    }
    #endregion

    #region Animation/UI Methods

    [Preserve]
    public void FireBulletFromAnimation()
    {
        Debug.Log("FireBulletFromAnimation() called");

        if (queuedFirePoint == null)
        {
            Debug.LogWarning("No fire point queued when animation event triggered.");
            return;
        }

        Debug.Log("Spawning bullet at: " + queuedFirePoint.name);

        Transform selectedFirePoint = queuedFirePoint;
        queuedFirePoint = null; // Clear after use

        // Get world position of center screen point
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        Vector3 targetPoint = ray.GetPoint(1000f);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }

        // Calculate direction from fire point to screen center target point
        Vector3 directionToCenter = (targetPoint - selectedFirePoint.position).normalized;

        Debug.DrawRay(selectedFirePoint.position, directionToCenter * 5f, Color.red, 2f);

        GameObject bullet = Instantiate(bulletPrefab, selectedFirePoint.position, Quaternion.LookRotation(directionToCenter));
        GameObject particle = Instantiate(shootParticle, selectedFirePoint.position, selectedFirePoint.rotation);
        Destroy(particle, 1.5f);

        if (bullet.TryGetComponent(out PlayerBullet bulletScript))
            bulletScript.SetDamage(damage);

        if (bullet.TryGetComponent(out Rigidbody rb))
            rb.velocity = directionToCenter * bulletSpeed;

        Debug.Log("Bullet fired from animation event!");
    }

    void UpdateAmmoUI()
    {
        ammoText.text = currentAmmo + " / " + magazineSize;
    }

    void UpdateBulletDisplay()
    {
        for (int i = 0; i < bulletDisplay.Count; i++)
        {
            bulletDisplay[i].SetActive(i < currentAmmo);
        }
    }

    void HandleFireModeSwitching()
    {
        if (Input.GetKeyDown(switchFireModeKey))
        {
            fireMode = (FireMode)(((int)fireMode + 1) % System.Enum.GetValues(typeof(FireMode)).Length);
            Debug.Log("Switched to: " + fireMode);
            ShowFireModeText("Fire Mode: " + fireMode.ToString());
        }
    }

    void ShowFireModeText(string message)
    {
        if (fireModeText == null)
        {
            Debug.LogWarning("FireModeText is not assigned!");
            return;
        }
        StopAllCoroutines();
        StartCoroutine(DisplayFireModeText(message));
    }

    IEnumerator DisplayFireModeText(string message)
    {
        if (fireModeText == null)
            yield break;

        fireModeText.text = message;
        fireModeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(fireModeDisplayTime);
        fireModeText.gameObject.SetActive(false);
    }
    #endregion

    IEnumerator Reload()
    {
        isReloading = true;
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        UpdateBulletDisplay();
        Debug.Log("Reloaded!");
    }

    private IEnumerator RegenerateAmmo()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);

            if (PauseManager.isPaused || isReloading || !enableRegen)
                continue;

            if (currentAmmo < magazineSize)
            {
                currentAmmo += regenAmountPerInterval;
                currentAmmo = Mathf.Min(currentAmmo, magazineSize);
                UpdateAmmoUI();
                UpdateBulletDisplay();
            }
        }
    }

    IEnumerator FireBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (currentAmmo <= 0)
                break;

            Shoot();
            yield return new WaitForSeconds(fireRate);
        }
    }

    IEnumerator ResetIdle()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerMovement.instance.handAnimator.SetBool("Idle", true);
    }
}
