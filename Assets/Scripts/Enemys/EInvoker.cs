using UnityEngine;

public class EInvoker : EnemisBehaivor
{
    [Header("invoker")]
    public GameObject[] enemyPrefabs;
    public Transform[] summonPoints;
    public float summonCooldown;
    public int maxSummoned;
    public bool isminiboos;


    public float summonTimer;
    int currentEnemisSumoned;


    void Awake()
    {
        if (true)
        {
            currentlife = FlyweightPointer.Ehealer.maxLife;
            speed = FlyweightPointer.Ehealer.speed;
        }

        summonTimer = summonCooldown;
    }

    private void Update()
    {
        if (currentlife >= 0)
        {
            EnemiMovement();
        }
    }

    public void EnemiMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);


        if (distanceToPlayer > ranged)
        {
            // anim.SetBool("run", false);
            if (!isminiboos)
            {

                cronometro += 1 * Time.deltaTime;
                if (cronometro >= 4)
                {
                    rutina = Random.Range(0, 2);
                    cronometro = 0;
                }
                switch (rutina)
                {
                    case 0:
                        break;
                    case 1:
                        grado = Random.Range(0, 360);
                        angulo = Quaternion.Euler(0, grado, 0);
                        rutina++;
                        break;
                    case 2:
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                        transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                        break;
                }
            }

        }
        else
        {

            var lookpos = transform.position - player.transform.position;

            lookpos.y = 0;

            var rotation = Quaternion.LookRotation(lookpos);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
            transform.Translate(Vector3.forward * 2 * Time.deltaTime);

            // Cuenta regresiva para invocar enemigos
            summonTimer -= Time.deltaTime;

            // Si es tiempo de invocar y no ha alcanzado el máximo de invocados
            if (summonTimer <= 0 && currentEnemisSumoned < maxSummoned)
            {

                SummonEnemy();
                summonTimer = summonCooldown;
            }
        }
    }

    void SummonEnemy()
    {
        // Escoge un punto de invocación aleatorio
        Vector3 spawnPosition;

        if (summonPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, summonPoints.Length);
            spawnPosition = summonPoints[randomIndex].position;
        }
        else
        {
            // Si no hay puntos invoca cerca de el
            spawnPosition = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        }

        // Escoge aleatoriamente qué tipo de enemigo invocar
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyToSummon = enemyPrefabs[randomEnemyIndex];

        GameObject newEnemy = Instantiate(enemyToSummon, spawnPosition, Quaternion.identity);

        currentEnemisSumoned++;

    }
}
