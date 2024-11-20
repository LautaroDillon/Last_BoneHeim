using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioClip throwClip;
    public Transform cam;
    public Transform attackPoint;
    public Transform curvePoint;
    public GameObject objectToThrow;
    public Rigidbody arm;
    Guns guns;
    public GameObject armPrefab;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;
    public float recoverArmTime;
    public float recoverArmMaxTime = 15;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;

    private bool isReturning = false;
    private Vector3 oldPos;
    private float time = 0.0f;

    bool readyToThrow;

    private void Start()
    {
        recoverArmTime = recoverArmMaxTime;
        guns = GameObject.Find("Gun").GetComponent<Guns>();
        readyToThrow = true;
    }

    private void Update()
    {
        if (totalThrows >= 1)
            totalThrows = 1;
        if(Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw();
            Invoke("RestoreThrow", recoverArmTime);
            SoundManager.instance.PlaySound(throwClip, transform, 1f, false);
            if(guns.isSkeleton == true)
            {
                guns.magazineSize = guns.magazineSize / 2;
            }
            if (guns.isInvoker == true)
            {
                guns.magazineSize = guns.magazineSize / 2;
            }
            if (guns.isKnuckle == true)
            {
                guns.magazineSize = guns.magazineSize / 2;
            }
            Debug.Log("Throw!");
        }
    }

    private void Throw()
    {
        readyToThrow = false;
        arm.isKinematic = false;

        // instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // calculate direction
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (attackPoint.position - hit.point).normalized;
        }

        // add force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;

        // implement throwCooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }
    private void ReturnThrow()
    {
        oldPos = arm.position;
        isReturning = true;
        arm.velocity = Vector3.zero;
        arm.isKinematic = true;
    }

    Vector3 BQCPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }
    public void RestoreThrow()
    {
        totalThrows++;
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}