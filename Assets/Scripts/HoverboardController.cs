using System;
using AK.Wwise;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class HoverboardController : MonoBehaviour
{
    [SerializeField] private GameObject mesh;

    [Header("Spline Movement Settings")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float maxRollAngle = 60f; // Maximum roll angle in degrees
    
    [Tooltip("Normalized speed along the spline (0..1 per second).")]
    [SerializeField] private bool followSpline = true;
    [SerializeField] private bool loopSpline = true;
    
    [Header("Steer Settings")]
    [SerializeField] private float steerSpeed = 3f;
    [SerializeField] private float steerMaxPositionOffset = 2f;

    [Header("Lane Switch Settings")]
    [SerializeField] private float swapCooldown = 0.5f;
    [SerializeField] private float swapSpeed = 0.2f;

    private bool swapRight = true;
    private float swapCooldownTimer;
    private float steerValue = 0f;
    private float splineT = 0f; // normalized position along spline
    public float GetSplineT() => splineT;
    private Soundway currentSoundway;
    private float timePerSegment;
    
    private Tween swapTween = null;
    
    private Vector2 steerInput = Vector2.zero;

    private Soundway queuedSoundway = null;

    public bool IsEnabled = false;
    public void OnSteer(InputValue value)
    {
        steerInput = value.Get<Vector2>();
        float horizontalInput = steerInput.x;
        float angle = maxRollAngle * horizontalInput;
        mesh.transform.localRotation = Quaternion.Euler(0f, 0f, -angle);
        if (horizontalInput > 0)
        {
            swapRight = true;
        }
        else if (horizontalInput < 0)
        {
            swapRight = false;
        }
    }

    public void OnSwap()
    {
        bool canSwap = SoundwayManager.Instance.CanSwapSoundways(swapRight, out var newSoundway);
        if (canSwap)
        {
            SoundwayManager.Instance.QueueSoundwaySwap(newSoundway);
            queuedSoundway = newSoundway;
        }
    }
    
    public void OnSoundwaySwap()
    {
        currentSoundway = queuedSoundway;
        queuedSoundway = null;
    }

    private void OnMusicStart()
    {
        IsEnabled = true;
    }
    public void Start()
    {
        AudioManager.Instance.OnMusicStart += OnMusicStart;
        AudioManager.Instance.OnSoundwaySwitch += OnSoundwaySwap;
        currentSoundway = SoundwayManager.Instance.GetCurrentSoundway();
        var songData = AudioManager.Instance.CurrentSongData;
        timePerSegment = (songData.totalMeasures * songData.BeatsPerMeasure * 60.0f) / (songData.BPM * SoundwayManager.Instance.SplineSegments);
    }
    
    public void Update()    
    {
        if (!IsEnabled) return;
        if (currentSoundway != null)
        {
            var splineContainer = currentSoundway.SplineContainer;
            
            //Right soundway is one spline shorter
            int splineIndex = Mathf.Clamp((int)(splineT), 0, splineContainer.Splines.Count - 1);
            if (currentSoundway == SoundwayManager.Instance.rightSoundway)
            {
                splineIndex--;
            }
            var spline = splineContainer.Splines[splineIndex];
            float splineNormalizedSpeed = 1.0f / timePerSegment;
            
            // Advance normalized parameter
            splineT += splineNormalizedSpeed * Time.deltaTime;

            float normalizedT = splineT - Mathf.Floor(splineT);
            
            

            // Sample spline (position + rotation)
            var sample = spline.Evaluate(normalizedT, out var position,  out var tangent, out var up);
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(tangent, up);
            
            // Optionally apply lateral offset along the spline's right vector (strafe)
            if (Mathf.Abs(steerInput.x) > Mathf.Epsilon)
            {
                Vector3 right = transform.rotation * Vector3.right;
                transform.position += right * (steerSpeed * steerInput.x * Time.deltaTime);
            }
        }
        if (queuedSoundway != null && swapTween == null)
        {
            // Vector3 startPos = transform.position;
            // Vector3 endPos = queuedSoundway.SplineContainer.Splines[0].;
            // swapTween = transform.DOMove(endPos, swapSpeed).SetEase(Ease.InOutSine).OnComplete(() =>
            // {
            //     swapTween = null;
            //     OnSoundwaySwap();
            // });
        }
    }
}
    