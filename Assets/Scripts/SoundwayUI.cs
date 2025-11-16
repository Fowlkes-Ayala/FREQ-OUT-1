using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SoundwayUI : MonoBehaviour
    {
        public float pulseSize = 1.3f;
        public float pulseDuration = 0.3f;
        private SoundwayManager _soundwayManager;
        public Image _soundwayIndicatorImage;
        
        private void Start()
        {
            _soundwayManager = SoundwayManager.Instance;
            _soundwayIndicatorImage.enabled = false;
            AudioManager.Instance.OnBeat += OnBeatChanged;
        }

        private void OnBeatChanged()
        {
            _soundwayIndicatorImage.rectTransform.DOScale(pulseSize, pulseDuration)
                .SetEase(Ease.Linear)
                .SetLoops(1, LoopType.Yoyo);
            if (_soundwayManager.IsValidTimingForSoundwaySwap())
            {
                _soundwayIndicatorImage.enabled = true;
            }
            else
            {
                _soundwayIndicatorImage.enabled = false;
            }
        }
        
    }
}