using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerWeapon : MonoBehaviour
{
    public KeyCode shootKey;
    public GameObject bullet;
    public float shootForce, upwardForce;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public Vector3 kickbackAmount = new Vector3(-0.1f, 0.0f, 0.0f);
    public float returnSpeed = 10f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 currentOffset;
    private bool isKickedBack = false;
    bool shooting, readyToShoot, reloading;
    public Camera fpsCam;
    public Transform attackPoint;
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    public bool allowInvoke = true;

    public List<GameObject> bulletDisplay = new List<GameObject>();
    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        if (PauseManager.isPaused)
            return;

        MyInput();

        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);

        if (bulletsLeft > magazineSize)
            bulletsLeft = magazineSize;

        UpdateBulletDisplay();
    }

    private void MyInput()
    {
        if (allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
            Reload();

        // Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        bulletsLeft--;

        if (bulletsLeft < bulletDisplay.Count)
        {
            bulletDisplay[bulletsLeft].SetActive(false);
        }

        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    public void UpdateBulletDisplay()
    {
        for (int i = 0; i < bulletDisplay.Count; i++)
        {
            bulletDisplay[i].SetActive(i < bulletsLeft);
        }
    }

    public void GunKick()
    {
        if (isKickedBack)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * returnSpeed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * returnSpeed);

            if (Vector3.Distance(transform.localPosition, originalPosition) < 0.001f)
                isKickedBack = false;
        }
    }

    public void ApplyKickback()
    {
        transform.localPosition += kickbackAmount;
        isKickedBack = true;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        foreach (GameObject bulletObj in bulletDisplay)
        {
            bulletObj.SetActive(true);
        }
        reloading = false;
    }
}
