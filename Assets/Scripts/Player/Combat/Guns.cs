
using UnityEngine;
using TMPro;

public class Guns : MonoBehaviour
{

    #region Variables
    [Header("References")]
    public static Guns instance;
    public GameObject bullet;
    public GameObject explosion;
    PhysicMaterial physics_mat;
    public Rigidbody rb;
    public Rigidbody playerRb;
    public Camera fpsCam;
    public Transform attackPoint;
    public LayerMask whatIsEnemies;
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    public CamShake camShake;

    [Header("Bullet Force")]
    public float shootForce, upwardForce;

    [Header("Stats")]
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;
    public float bulletsLeft, bulletsShot;
    public float killReward;
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = false;
    int collisions;
    public float recoilForce;

    [Header("Camera Shake")]
    public float camShakeMagnitude, camShakeDuration;

    [Header("Bools")]
    bool shooting, readyToShoot, reloading;
    public bool allowInvoke = true;
    public bool isSkeleton = false;
    public bool isTeeth = false;
    public bool isInvoker = false;
    public bool isKnuckle = false;

    [Header("Sounds")]
    [SerializeField] private AudioClip shootClip;

    #endregion

    #region Awake/Start/Update
    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        SkeletonHand();
        instance = this;
    }

    private void Update()
    {
        ShootInput();
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft + " / " + magazineSize);
        if (bulletsLeft >= magazineSize)
            bulletsLeft = magazineSize;
    }
    #endregion

    #region GunMethods
    private void ShootInput()
    {
        if(!UiControl._isPaused)
        {
            if(!UiControl._isInventory)
            {
                if (allowButtonHold)
                    shooting = Input.GetKey(KeyCode.Mouse0);
                else
                    shooting = Input.GetKeyDown(KeyCode.Mouse0);

                //Reloading 
                //if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
                  //  Reload();
                //if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
                  //  Reload();

                //Shooting
                if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
                {
                    bulletsShot = 0;
                    SoundManager.instance.PlaySound(shootClip, transform, 0.3f, false);
                    Shoot();
                }
            }
        }
        
    }
    private void Shoot()
    {
        readyToShoot = false;
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        camShake.Shake(camShakeDuration, camShakeMagnitude);

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

        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void BounceLifetime()
    {
        if (collisions > maxCollisions) Explode();

        //Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
        //Instantiate explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        //Check for enemies 
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            //Get component of enemy and call Take Damage

            //Just an example!
            ///enemies[i].GetComponent<ShootingAi>().TakeDamage(explosionDamage);

            //Add explosion force (if enemy has a rigidbody)
            if (enemies[i].GetComponent<Rigidbody>())
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        //Add a little delay, just to make sure everything works fine
        Invoke("Delay", 0.05f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Don't count collisions with other bullets
        if (collision.collider.CompareTag("Bullet")) return;

        //Count up collisions
        collisions++;

        //Explode if bullet hits an enemy directly and explodeOnTouch is activated
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch) Explode();
    }

    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
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
        reloading = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    #endregion

    #region Guns
    public void ResetGun()
    {
        shootForce = 0;
        timeBetweenShooting = 0;
        timeBetweenShots = 0;
        magazineSize = 0;
        bulletsLeft = 0;
        bulletsPerTap = 0;
        recoilForce = 0;
        spread = 0;
        bounciness = 0;
        maxCollisions = 0;
        maxLifetime = 0;
        explosionDamage = 0;
        explosionRange = 0;
        explosionForce = 0;
        allowButtonHold = false;
        explodeOnTouch = false;
        useGravity = false;
        isInvoker = false;
        isKnuckle = false;
        isSkeleton = false;
        isTeeth = false;
        FlyweightPointer.Player.Damage = 0;
    }

    public void SkeletonHand()
    {
        isSkeleton = true;
        shootForce += 65;
        timeBetweenShooting += 0.5f;
        magazineSize += 10;
        bulletsLeft = magazineSize;
        bulletsPerTap += 1;
        allowButtonHold = true;
        useGravity = false;
        FlyweightPointer.Player.Damage += 30;
    }
    public void KnuckleBuster()
    {
        isKnuckle = true;
        shootForce += 75;
        timeBetweenShooting += 0.1f;
        timeBetweenShots += 0.1f;
        magazineSize += 40;
        bulletsLeft = magazineSize;
        bulletsPerTap += 1;
        allowButtonHold = true;
        useGravity = false;
        FlyweightPointer.Player.Damage += 20;
    }

    public void InvokerHand()
    {
        isInvoker = true;
        shootForce += 60;
        timeBetweenShooting += 1f;
        magazineSize += 6;
        bulletsLeft = magazineSize;
        bulletsPerTap += 1;
        explosionRange = 4;
        explosionForce = 1200;
        explodeOnTouch = true;
        useGravity = true;
        allowButtonHold = false;
        FlyweightPointer.Player.Damage += 90;
    }

    public void TeethShot()
    {
        isTeeth = true;
        shootForce += 50;
        timeBetweenShooting += 1;
        magazineSize += 32;
        bulletsLeft = magazineSize;
        bulletsPerTap += 8;
        recoilForce += 1.5f;
        spread += 2.5f;
        useGravity = false;
        FlyweightPointer.Player.Damage += 35;
    }
    #endregion
}
