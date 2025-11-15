using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SoundwayUI : MonoBehaviour
    {
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
        
        
    }
}