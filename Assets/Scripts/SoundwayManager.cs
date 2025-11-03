using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class SoundwayManager : MonoBehaviour
    {
        [SerializeField] private List<Soundway> soundways;
        private float swapTime;
        private Soundway currentSoundway;
        private int currentSoundwayIndex = 0;

        public static SoundwayManager Instance;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            if (soundways == null) Debug.LogError("SoundwayManager is empty!");
            currentSoundway = soundways[currentSoundwayIndex];
        }
        
        public Soundway GetCurrentSoundway()
        {
            return currentSoundway;
        }

        public bool SwapSoundways(bool isRight, out Soundway soundway)
        {
            soundway = null;
            if (isRight)
            {
                if (currentSoundwayIndex < soundways.Count - 1)
                {
                    currentSoundway = soundways[++currentSoundwayIndex];
                    soundway = currentSoundway;
                    return true;
                }
                return false;
            }
            if (currentSoundwayIndex > 0)
            {
                currentSoundway = soundways[--currentSoundwayIndex];
                soundway = currentSoundway;
                return true;
            }
            return false;
        }
        
    }
}