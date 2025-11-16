using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class Pulser : MonoBehaviour
    {
        public float pulseSize = 1.3f;
        public float pulseDuration = 0.25f;
        
        private void Start()
        {
            AudioManager.Instance.OnBeat += OnBeatChanged;
        }

        private void OnBeatChanged()
        {
            transform.DOScale(pulseSize, pulseDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo);
        }
    }
}