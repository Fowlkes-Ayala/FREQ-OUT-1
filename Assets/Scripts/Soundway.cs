using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace DefaultNamespace
{
    public class Soundway : MonoBehaviour
    {
        public List<SplineContainer> SplineContainers;
        public List<Spline> Splines;
        public int TotalSplines;
        public bool IsLeftSoundway = false;
        [SerializeField] public AK.Wwise.State SoundwayState;

        public void OnValidate()
        {
            foreach (var container in SplineContainers)
            {
                if (container != null)
                {
                    TotalSplines += SplineContainers.Count;
                }
            }
        }

        public Spline GetSpline(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of SplineContainers.");
            }

            int splineIndexCounter = 0;
            foreach (var container in SplineContainers)
            {
                if (container != null)
                {
                    splineIndexCounter += container.Splines.Count;
                    if (splineIndexCounter > index)
                    {
                        int localIndex = index - (splineIndexCounter - container.Splines.Count);
                        return container.Splines[localIndex];
                    }
                }
            }
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of SplineContainers.");
        }
    }
}