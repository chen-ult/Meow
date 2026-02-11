using Unity.Cinemachine;
using UnityEngine;

public class CurrencyPickUp : MonoBehaviour
{
    [Header("Ê°È¡²ÎÊý")]
    public int pickupAmount = 1;
    public float destroyDelay = 0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            PickupCurrency();
        }
    }

    private void PickupCurrency()
    {
        CurrencyManager.instance?.AddCurrency(pickupAmount);
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, destroyDelay);
    }
}
