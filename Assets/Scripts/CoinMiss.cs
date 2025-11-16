using System.Collections;
using UnityEngine;

public class CoinMiss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.MissCoin();
            StartCoroutine(DestroySelf());
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(transform.parent.gameObject);
    }
}
