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
        public int SplineSegments = 6;
        [SerializeField] private bool startOnLeftSoundway = true;
        public Soundway leftSoundway;
        public Soundway rightSoundway;
        public HoverboardController hoverboardController;
        
        private float swapTime;
        private Soundway queuedSoundway;
        private Soundway currentSoundway;
        private int currentSoundwayIndex = 0;

        public static SoundwayManager Instance;
        public float RoadWidth = 5f;

        public Spline GetCurrentSpline()
        {
            return hoverboardController.GetCurrentSpline();    
        }
        
        public float GetPlayerT()
        {
            return hoverboardController.GetNormalizedT();
        }

        public bool IsRightmostSoundway()
        {
            return currentSoundway == rightSoundway ? true : false;
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

            if (startOnLeftSoundway)
            {
                currentSoundway = leftSoundway;
            }
            else
            {
                currentSoundway = rightSoundway;
            }
            soundways = new List<Soundway> { leftSoundway, rightSoundway };
        }

        private void Start()
        {
            AudioManager.Instance.OnSoundwaySwitch += OnSoundwaySwap;
        }

        public void QueueSoundwaySwap(Soundway soundway)
        {
            AudioManager.Instance.QueueSoundwaySwitch(soundway.SoundwayState);
        }
        private void OnSoundwaySwap()
        {
            currentSoundway = queuedSoundway;    
            queuedSoundway = null;
        }
        public bool CanSwapSoundways(bool isRight, out Soundway soundway)
        {
            soundway = isRight ? rightSoundway : leftSoundway;
            return IsValidTimingForSoundwaySwap();
        }

        public bool IsValidTimingForSoundwaySwap()
        {
            // Beats 7, 15, 23, ...
            return AudioManager.Instance.CurrentMeasure % 8 == 7;
        }

        public Soundway GetCurrentSoundway()
        {
            return currentSoundway;
        }
    }
}