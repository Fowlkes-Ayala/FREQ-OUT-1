using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SoundwayUI : MonoBehaviour
    {
        public float pulseSize = 1.3f;
        public float pulseDuration = 0.2f;
        private SoundwayManager _soundwayManager;
        public Image _rightSoundwayImage;
        public Image _leftSoundwayImage;
        
        private void Start()
        {
            _soundwayManager = SoundwayManager.Instance;
            DisableBothSoundwayImages();
            AudioManager.Instance.OnBeat += OnBeatChanged;
        }
        
        private void DisableBothSoundwayImages()
        {
            _rightSoundwayImage.enabled = false;
            _leftSoundwayImage.enabled = false;
        }

        private void OnBeatChanged()
        {
            Pulse(_rightSoundwayImage.transform);
            Pulse(_leftSoundwayImage.transform);
            if (_soundwayManager.IsValidTimingForSoundwaySwap())
            {
                if (!_soundwayManager.IsRightmostSoundway())
                {
                    _rightSoundwayImage.enabled = true;
                }
                else
                {
                    _leftSoundwayImage.enabled = true;
                }
            }
            else
            {
                DisableBothSoundwayImages();
            }
        }

        private void Pulse(Transform transform)
        {
            transform.DOScale(new Vector3(pulseSize, pulseSize, pulseSize), pulseDuration / 2)
                .SetEase(Ease.Linear)
                .SetLoops(1, LoopType.Yoyo);
        }
        
    }
}