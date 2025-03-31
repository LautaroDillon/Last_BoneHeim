using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EToxic : EnemisBehaivor, Idamagable
{
    public GameObject acidPrefab;           
    public float dropInterval = 2f;
    public float acidLifetime = 5f;
    public GameObject pointAcid;

    private float dropTimer;

    public Transform[] points;
    public int PointNumber;
    public float proximityThreshold = 0.1f;

    void Awake()
    {
        currentlife = FlyweightPointer.Ehealer.maxLife;

        dropTimer = dropInterval;
    }

    void Update()
    {
        // inicia el taimer para soltar el acido
        dropTimer -= Time.deltaTime;
        //pregunta si puede soltar el acido
        if (dropTimer <= 0)
        {
            DropAcid();

            dropTimer = dropInterval;
        }
        if (currentlife >= 0)
        {
            Movement();
        }
    }

    void Movement()
    {

        if (points.Length == 0) return;


        if (Vector3.Distance(transform.position, points[PointNumber].position) < proximityThreshold)
        {

            PointNumber = (PointNumber + 1) % points.Length;
        }

        transform.position = Vector3.MoveTowards(transform.position, points[PointNumber].position, FlyweightPointer.Eboomer.speed * Time.deltaTime);
    }
    void DropAcid()
    {
        GameObject acid = Instantiate(acidPrefab, pointAcid.transform.position, Quaternion.identity);

        Destroy(acid, acidLifetime);
    }
}
