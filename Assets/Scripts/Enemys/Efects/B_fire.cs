using UnityEngine;

public class B_fire : MonoBehaviour
{
    public GameObject acidPrefab;
    public float acidLifetime = 5f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();

        if (other.gameObject.layer == 11 && damagableInterface != null)
        {
            /*
            FullscreenShader.instance.acidShaderEnabled = true;
            Destroy(gameObject);
            damagableInterface.TakeDamage(FlyweightPointer.Eshoot.Damage);
            */
        }
        else if (other.gameObject.layer == 6 || other.gameObject.layer == 7)
        {
            DropAcid();
            Destroy(gameObject);
        }
    }

    void DropAcid()
    {
        GameObject acid = Instantiate(acidPrefab, this.transform.position, Quaternion.identity);
        Destroy(acid, acidLifetime);
    }
}
