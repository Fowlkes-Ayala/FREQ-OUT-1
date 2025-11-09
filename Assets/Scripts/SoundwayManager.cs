using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace DefaultNamespace
{
    public class SoundwayManager : MonoBehaviour
    {
        [SerializeField] private List<Soundway> soundways;
        [SerializeField] private SplineContainer mainSplineContainer;
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
            for (int i = 0; i < soundways.Count; i++)
            {
                soundways[i].Spline = mainSplineContainer.Splines[i];
            }
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

        public bool CheckSoundwayBeforeSwap(Soundway soundway)
        {
            // soundway.SplineContainer.KnotLinkCollection.GetKnotLinks(1);
            return false;
        }

        public bool CheckAvailableSoundwaySwaps(Soundway soundway)
        {
            return false;
        }
        
    }
}