using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class FracturedObj : MonoBehaviour
    {
        public float shrinkDelay = 3f;
        public float shrinkDuration = 1f;
        public GameObject IntactObjReference;
        public GameObject FracturedObjPrefab;
        public float explodeForce = 500f;
        public void Explode()
        {
            IntactObjReference.SetActive(false);
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(explodeForce, transform.position, 5f);
                }
            }

            StartCoroutine(DestroyAfterDelay());
        }

        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(shrinkDelay);
            transform.DOScale(0.0f, shrinkDuration).SetEase(Ease.InBack);
            Destroy(gameObject, shrinkDuration);
        }
    }
}