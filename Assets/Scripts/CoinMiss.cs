using UnityEngine;

public class CoinMiss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.MissCoin();
        }
    }
}
