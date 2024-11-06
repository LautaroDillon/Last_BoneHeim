using UnityEngine;

public class B_fire : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();


        if (other.gameObject.layer == 11 && damagableInterface != null)
        {
            Destroy(gameObject);
            damagableInterface.TakeDamage(FlyweightPointer.Eshoot.Damage);
        }
        else if (other.gameObject.layer == 6 || other.gameObject.layer == 7)
        {
            Destroy(gameObject);
        }
    }
}
