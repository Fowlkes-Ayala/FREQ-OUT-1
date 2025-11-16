using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Got coin");
            AudioManager.Instance.GetCoin();
            Destroy(gameObject);
        }
    }
}
