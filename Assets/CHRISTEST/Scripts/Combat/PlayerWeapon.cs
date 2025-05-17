using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EZCameraShake;
public class PlayerWeapon : MonoBehaviour
{
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
    //public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public TextMeshProUGUI ammoText;

    [Header("UI References")]
    public TextMeshProUGUI fireModeText;
    public float fireModeDisplayTime = 2f;

    [Header("Bullet Display")]
    public List<GameObject> bulletDisplay = new List<GameObject>();

    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    private bool isFiringHeld = false;

    void Start()
    {
        currentAmmo = magazineSize;
    }

    public void Update()
    {
        if (PauseManager.isPaused)
            return;
        else
        {
            Reloading();
            Shooting();
            UpdateAmmoUI();
            HandleFireModeSwitching();
        }
        if (currentAmmo > magazineSize)
            currentAmmo = magazineSize;
    }

    void Reloading()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0 || Input.GetKeyDown(reloadButton))
        {
            StartCoroutine(Reload());
            return;
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

    void Shoot()
    {
        if (isReloading || currentAmmo <= 0)
            return;

        CameraShake.Instance.ShakeOnce(1f, 1f, 0.1f, 1f);
        PlayerMovement.instance.animator.SetTrigger("Atack");
        PlayerMovement.instance.animator.SetBool("Idle", false);
        StartCoroutine(ResetIdle());

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }

        Vector3 aimDirection = (targetPoint - firePoint.position).normalized;

        float maxAngle = 60f; // clamp angle from firePoint.forward
        if (Vector3.Angle(firePoint.forward, aimDirection) > maxAngle)
        {
            aimDirection = firePoint.forward;
        }

        GameObject visualBone = GetBulletDisplayObject();
        if (visualBone == null) return;

        Vector3 shootPosition = visualBone.transform.position;
        Vector3 shootDirection = (targetPoint - shootPosition).normalized;

        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = shootPosition;
        bullet.transform.rotation = Quaternion.LookRotation(shootDirection);

        if (bullet.TryGetComponent(out PlayerBullet bulletScript))
        {
            bulletScript.SetDamage(damage);
            bulletScript.ResetBullet();
        }

        if (bullet.TryGetComponent(out Rigidbody rb))
            rb.velocity = shootDirection * bulletSpeed;

        currentAmmo--;
        UpdateBulletDisplay();
    }

    void ShootShotgun()
    {
        if (currentAmmo <= 0 || isReloading)
            return;

        PlayerMovement.instance.animator.SetTrigger("Atack");
        PlayerMovement.instance.animator.SetBool("Idle", false);
        StartCoroutine(ResetIdle());

        CameraShake.Instance.ShakeOnce(1.5f, 1.5f, 0.1f, 1f);

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(1000);

        float baseDistance = Vector3.Distance(firePoint.position, targetPoint);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            // Calculate spread per pellet
            Vector3 direction = (targetPoint - firePoint.position).normalized;
            direction = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * direction;

            if (currentAmmo <= 0 || currentAmmo > bulletDisplay.Count)
                return;

            GameObject visualBullet = GetBulletDisplayObject();
            if (visualBullet == null) return;

            Vector3 spawnPos = visualBullet.transform.position;
            Quaternion spawnRot = firePoint.rotation;

            GameObject bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = spawnPos;
            bullet.transform.rotation = spawnRot;

            float falloffMultiplier = 1f;
            if (baseDistance > falloffStartDistance)
            {
                float excessDistance = baseDistance - falloffStartDistance;
                falloffMultiplier = Mathf.Lerp(1f, minDamageMultiplier, excessDistance / 20f); // 20f = falloff range
            }

            float pelletDamage = (shotgunDamage / pelletsPerShot) * falloffMultiplier;

            if (bullet.TryGetComponent(out PlayerBullet bulletScript))
            {
                bulletScript.ResetBullet();
                bulletScript.SetDamage(Mathf.RoundToInt(pelletDamage));
            }

            if (bullet.TryGetComponent(out Rigidbody rb))
                rb.velocity = direction * bulletSpeed;
        }

        currentAmmo--;
        UpdateBulletDisplay();
        Debug.Log("Shotgun fired! Ammo left: " + currentAmmo);
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

    GameObject GetBulletDisplayObject()
    {
        if (currentAmmo <= 0 || currentAmmo > bulletDisplay.Count)
            return null;

        GameObject bone = bulletDisplay[currentAmmo - 1];
        bone.SetActive(false); // Deactivate bone as visual feedback
        return bone;
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
        StopAllCoroutines(); // Optional: avoids overlapping texts
        StartCoroutine(DisplayFireModeText(message));
    }

    IEnumerator DisplayFireModeText(string message)
    {
        fireModeText.text = message;
        fireModeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(fireModeDisplayTime);
        fireModeText.gameObject.SetActive(false);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        UpdateBulletDisplay();
        Debug.Log("Reloaded!");
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
        PlayerMovement.instance.animator.SetBool("Idle", true);
    }
}
