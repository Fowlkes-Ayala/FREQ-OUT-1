using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
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
            AudioManager.Instance.OnQueueSoundwaySwitch += OnQueueSoundwaySwitch;
            AudioManager.Instance.OnSoundwaySwitch += OnSoundwaySwitch;
        }
        
        private void OnSoundwaySwitch()
        {
            _soundwayIndicatorImage.color = Color.white;
        }
        
        private void OnQueueSoundwaySwitch()
        {
            _soundwayIndicatorImage.color = Color.green;
        }

        private void OnBeatChanged()
        {
            _soundwayIndicatorImage.rectTransform.DOScale(pulseSize, pulseDuration)
                .SetEase(Ease.Linear)
                .SetLoops(1, LoopType.Yoyo);
            if (_soundwayManager.IsValidTimingForSoundwaySwap())
            {
                _soundwayIndicatorImage.enabled = true;
                _soundwayManager.CanSwapSoundways(_soundwayManager.IsRightmostSoundway(), out var newSoundway);
                // newSoundway.GetSpline(0).Evaluate(0.0f, out var position, out var tangent, out var up);
                // if (Camera.main != null)
                //     _soundwayIndicatorImage.rectTransform.LookAt(Camera.main.transform, Vector3.up);
            }
            else
            {
                _soundwayIndicatorImage.enabled = false;
            }
        }
        
    }
}