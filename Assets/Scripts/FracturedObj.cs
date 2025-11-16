using System;
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
        public void Explode(Transform player)
        {
            IntactObjReference.SetActive(false);
            GameObject explode = Instantiate(FracturedObjPrefab, transform.position, Quaternion.identity, transform);
            foreach (Transform child in explode.transform)
            {
                if (child.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.AddExplosionForce(explodeForce, transform.position, 5f);
                    rb.AddForce(player.forward * explodeForce, ForceMode.Impulse);
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Explode(other.transform);
            }
        }
    }
}