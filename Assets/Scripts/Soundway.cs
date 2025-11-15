using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace DefaultNamespace
{
    public class Soundway : MonoBehaviour
    {
        public SplineContainer SplineContainer;
        public List<Spline> Splines;
        public bool IsLeftSoundway = false;
        [SerializeField] public AK.Wwise.State SoundwayState;
    }
}