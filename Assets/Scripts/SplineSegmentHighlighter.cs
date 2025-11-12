#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class SplineSegmentHighlighter : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private int segments = 48;
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color highlightColor = Color.cyan;
    [SerializeField] private bool drawInEditor = true;
    [SerializeField, Min(0.1f), Tooltip("Line width in pixels (Editor only).")] private float lineWidth = 4f;

    private void OnValidate()
    {
        if (segments < 6) segments = 6;
        if (lineWidth < 0.1f) lineWidth = 0.1f;
    }

    private void OnDrawGizmos()
    {
        if (!drawInEditor || splineContainer == null) return;

        var splines = GetSplinesList(splineContainer);
        if (splines == null || splines.Count == 0) return;

        int maxSplines = Math.Min(6, splines.Count);
        int perSplineSegments = Mathf.Max(1, segments / 6);

        for (int s = 0; s < maxSplines; s++)
        {
            var spline = splines[s];
            // draw perSplineSegments segments for this spline
            for (int i = 0; i < perSplineSegments; i++)
            {
                float t0 = i / (float)perSplineSegments;
                float t1 = (i + 1) / (float)perSplineSegments;

                Vector3 a = spline.EvaluatePosition(t0);
                Vector3 b = spline.EvaluatePosition(t1);

                bool isHighlight = (i == perSplineSegments - 1); // highlight last subsegment of each spline
#if UNITY_EDITOR
                Handles.color = isHighlight ? highlightColor : normalColor;
                Handles.DrawAAPolyLine(lineWidth, a, b);
#else
                Gizmos.color = isHighlight ? highlightColor : normalColor;
                Gizmos.DrawLine(a, b);
#endif
            }
        }
    }

    // Try to handle both SplineContainer.Splines and SplineContainer.Spline APIs
    private static List<Spline> GetSplinesList(SplineContainer container)
    {
        if (container == null) return null;

        try
        {
            var list = container.Splines;
            if (list != null) return new List<Spline>(list);
        }
        catch { /* property not present in this package version */ }

        try
        {
            var single = container.Spline;
            return new List<Spline> { single };
        }
        catch { }

        return null;
    }
}