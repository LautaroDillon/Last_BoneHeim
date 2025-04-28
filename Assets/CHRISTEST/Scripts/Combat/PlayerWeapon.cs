using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerWeapon : MonoBehaviour
{
    public KeyCode shootKey;
    //bullet 
    public GameObject bullet;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
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

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //bug fixing :D
    public bool allowInvoke = true;

    public List<GameObject> bulletDisplay = new List<GameObject>();

    private void Awake()
    {
        //make sure magazine is full
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
        else
        {
            MyInput();

            if (ammunitionDisplay != null)
                ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);

            if (bulletsLeft > magazineSize)
                bulletsLeft = magazineSize;
        }
    }

    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else 
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
            Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            PlayerMovement.instance.animator.SetTrigger("Atack");
            PlayerMovement.instance.animator.SetBool("Idle", false);
            bulletsShot = 0;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        //if (muzzleFlash != null)
          //  Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;

        if (bulletsLeft < bulletDisplay.Count)
        {
            bulletDisplay[bulletsLeft].SetActive(false);
        }

        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

        PlayerMovement.instance.animator.SetBool("Idle", true);

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
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
    }
    private void ReloadFinished()
    {
        //Fill magazine
        bulletsLeft = magazineSize;
        foreach (GameObject bulletObj in bulletDisplay)
        {
            bulletObj.SetActive(true);
        }
        reloading = false;
    }
}
