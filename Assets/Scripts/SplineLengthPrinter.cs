using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineLengthPrinter : MonoBehaviour
{
    [SerializeField] private SplineContainer container;

    [Header("Indices of Splines to be Measured")]
    [SerializeField] private List<int> splineIndices = new List<int>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PrintSelectedSplineLengths();
    }
    public void PrintSelectedSplineLengths()
    {
        if (container == null)
        {
            Debug.LogError("[SplineLengthPrinter] No SplineContainer assigned.");
            return;
        }

        var splines = container.Splines;  // Collection of splines inside the container

        // Use the container's transform as the space to measure the spline in world coordinates
        // The Splines API expects a 4x4 transform matrix; we provide the container's localToWorldMatrix.
        float4x4 worldMatrix = container.transform.localToWorldMatrix;

        foreach (int index in splineIndices)
        {
            if (index < 0 || index >= splines.Count)
            {
                Debug.LogWarning($"[SplineLengthPrinter] Spline index {index} is out of range.");
                continue;
            }

            var spline = splines[index];

            // Call the overload that takes a transform matrix so length is measured along the curve in world space
            float length = SplineUtility.CalculateLength(spline, worldMatrix);

            Debug.Log($"Spline {index} Length: {length:F3} units");
        }
    }
}
