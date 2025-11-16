using UnityEngine;
using UnityEngine.VFX;

public class Coin : MonoBehaviour
{
    [SerializeField] private GameObject vfxPrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            PlayVFX();
            AudioManager.Instance.GetCoin();
            Destroy(transform.parent.gameObject);
        }
    }

    private void PlayVFX()
    {
        // Instantiate the effect
        GameObject vfxInstance = Instantiate(vfxPrefab, transform.position, Quaternion.identity);

        // Destroy when done (if your VFX has a fixed duration)
        Destroy(vfxInstance, 2f);
    }
}
