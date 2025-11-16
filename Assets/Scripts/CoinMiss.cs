using UnityEngine;

public class CoinMiss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Missed coin");
            AudioManager.Instance.MissCoin();
        }
    }
}
