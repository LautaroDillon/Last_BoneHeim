using System.Collections;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public float healRadius = 10.0f;
    public int healingAmount = 5;
    public float healInterval = 0.5f;
    public float healDuration = 4f;


    private EHealer healer;

    void Start()
    {
        Debug.Log("Healing");
        StartCoroutine(HealOverTime());
    }

    public void SetHealer(EHealer healerInstance)
    {
        healer = healerInstance;
    }

    public IEnumerator HealOverTime()
    {
        float timer = 0f;

        while (timer < healDuration)
        {
            // Curar cada "healInterval"
            HealNearbyEntities();


            yield return new WaitForSeconds(healInterval);

            // Actualizar el temporizador
            timer += healInterval;
        }

        EnemisBehaivor.instance.healParticle.SetActive(false);
        Destroy(this.gameObject);
    }

    public void HealNearbyEntities()
    {
        var healPosition = transform.position;


        Collider[] colliders = Physics.OverlapSphere(healPosition, healRadius);

        // Recorrer todos los colisionadores dentro del área
        foreach (var col in colliders)
        {

            if (col.gameObject.layer == 10)
            {
                // Intentar obtener el componente EnemisBehaivor del objeto
                EnemisBehaivor entity = col.GetComponent<EnemisBehaivor>();

                if (entity != null)
                {
                    entity.Healing(healingAmount);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Dibujar una esfera en la posición del objeto con el radio de curación
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }
}
