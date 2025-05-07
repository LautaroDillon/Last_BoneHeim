using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerWeapon : MonoBehaviour
{
    public enum FireMode { SemiAuto, Burst, FullAuto }
    public FireMode fireMode = FireMode.SemiAuto;

    [Header("Keybinds")]
    public KeyCode shootButton;
    public KeyCode reloadButton;

    [Header("Gun Settings")]
    public float fireRate = 0.2f;
    public int burstCount = 3;
    public int damage = 10;
    public int magazineSize = 30;
    public float reloadTime = 2f;

    [Header("Recoil Settings")]
    public Transform recoilTransform;
    public Vector3 recoilRotation = new Vector3(-5f, 0f, 0f);
    public float recoilReturnSpeed = 5f;
    public float recoilSnappiness = 10f;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public TextMeshProUGUI ammoText;

    [Header("Bullet Display")]
    public List<GameObject> bulletDisplay = new List<GameObject>();

    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    private bool isFiringHeld = false;

    void Start()
    {
        currentAmmo = magazineSize;
    }

    void Update()
    {
        if (PauseManager.isPaused)
            return;
        else
        {
            Reloading();
            Shooting();
            Recoil();
            UpdateAmmoUI();
        }
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
        }
    }

    void Recoil()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoilSnappiness * Time.deltaTime);
        recoilTransform.localRotation = Quaternion.Euler(currentRotation);
    }

    void Shoot()
    {
        if (currentAmmo <= 0 || isReloading)
            return;

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

        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

        if (bullet.TryGetComponent(out PlayerBullet bulletScript))
            bulletScript.SetDamage(damage);

        if (bullet.TryGetComponent(out Rigidbody rb))
            rb.velocity = direction * bulletSpeed;

        currentAmmo--;
        UpdateBulletDisplay();
        Debug.Log("Shot fired! Ammo left: " + currentAmmo);
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
