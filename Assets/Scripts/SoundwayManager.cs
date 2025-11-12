using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace DefaultNamespace
{
    public class SoundwayManager : MonoBehaviour
    {
        private List<Soundway> soundways;
        [SerializeField] private SplineContainer mainSplineContainer;
        public HoverboardController hoverboardController;
        
        public SongData songData;
        private float swapTime;
        private Soundway currentSoundway;
        private int currentSoundwayIndex = 0;

        private int lastBeat = -1;
        
        public event Action<int> OnBeatChangedEvent;
        

        public static SoundwayManager Instance;

        public bool IsRightmostSoundway()
        {
            return currentSoundwayIndex == soundways.Count - 1;
        }
        
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
            
            foreach (var spline in mainSplineContainer.Splines)
            {
                Soundway soundway = new Soundway();
                soundway.Spline = spline;
                soundways.Add(soundway);
            }
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
                    return IsValidTimingForSoundwaySwap();
                }
                return false;
            }
            if (currentSoundwayIndex > 0)
            {
                currentSoundway = soundways[--currentSoundwayIndex];
                soundway = currentSoundway;
                return IsValidTimingForSoundwaySwap();
            }
            return false;
        }

        public bool IsValidTimingForSoundwaySwap()
        {
            // Beats 7, 15, 23, ...
            return GetCurrentMeasure() % 8 == 7;
        }

        public void Update()
        {
            int currentBeat = GetCurrentBeat();
            if (currentBeat != lastBeat)
            {
                lastBeat = currentBeat;
                OnBeatChanged(currentBeat);
            }
        }

        public void OnBeatChanged(int currentBeat)
        {
            
            OnBeatChangedEvent?.Invoke(currentBeat);
        }
        
        public int GetCurrentBeat()
        {
            int totalMeasures = 0;
            totalMeasures = songData.totalMeasures;

            int totalBeats = Math.Max(1, totalMeasures * 4);
            float t = Mathf.Clamp01(hoverboardController.GetSplineT());
            int beat = Mathf.FloorToInt(t * totalBeats);
            if (beat >= totalBeats) beat = totalBeats - 1; // handle t == 1

            return beat; // zero-based beat index in range [0, totalBeats-1]
        }

        public int GetCurrentMeasure()
        {
            return GetCurrentBeat() / 4;
        }

        public Soundway GetCurrentSoundway()
        {
            return currentSoundway;
        }
        
    }
}