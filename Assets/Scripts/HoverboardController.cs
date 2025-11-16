using System;
using AK.Wwise;
using DefaultNamespace;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    [SerializeField] private float steerAcceleration = 15f; // How quickly steering responds
    [SerializeField] private float steerDeceleration = 20f; // How quickly steering returns to center
    [SerializeField] private AnimationCurve steerResponseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float steerOffset = 0f;
    private float targetSteerOffset = 0f;
    private float currentSteerVelocity = 0f;
    [SerializeField] private float steerMaxPositionOffset = 2f;

    [Header("Roll Settings")]
    [SerializeField] private float rollSmoothing = 12f; // Smooth rotation for roll angle
    private float currentRollAngle = 0f;
    private float targetRollAngle = 0f;

    [Header("Lane Switch Settings")]
    [SerializeField] private float swapCooldown = 0.5f;
    [SerializeField] private float swapSpeed = 0.2f;

    private float swapCooldownTimer;
    private float steerValue = 0f;
    private float splineT = 0f; // normalized position along spline
    public float GetSplineT() => splineT;
    private Soundway currentSoundway;
    private float timePerSegment;
    
    private Tween swapTween = null;
    
    private Vector2 steerInput = Vector2.zero;
    
    [Header("Movement Smoothing")]
    [SerializeField] private float positionSmoothing = 15f; // Smooth position updates
    [SerializeField] private float rotationSmoothing = 12f; // Smooth rotation updates
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private Soundway queuedSoundway = null;
    private float normalizedT = 0f;

    public bool IsEnabled = false;
    public void OnSteer(InputValue value)
    {
        steerInput = value.Get<Vector2>();
        // Target roll angle based on input
        float horizontalInput = steerInput.x;
        targetRollAngle = maxRollAngle * horizontalInput;
        
        // Calculate target steer offset with response curve for better feel
        float inputMagnitude = Mathf.Abs(horizontalInput);
        float curvedInput = steerResponseCurve.Evaluate(inputMagnitude) * Mathf.Sign(horizontalInput);
        targetSteerOffset = curvedInput * steerMaxPositionOffset;
    }

    public void OnSwap()
    {
        bool swapRight;
        if (currentSoundway.IsLeftSoundway) swapRight = true;
        else swapRight = false;
        bool canSwap = SoundwayManager.Instance.CanSwapSoundways(swapRight, out var newSoundway);
        if (canSwap)
        {
            SoundwayManager.Instance.QueueSoundwaySwap(newSoundway);
            queuedSoundway = newSoundway;
        }
    }

    public void OnRestart()
    {
        AkUnitySoundEngine.StopAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
    
    public Spline GetCurrentSpline()
    {
        if (currentSoundway != null)
        {
            int splineIndex = Mathf.Clamp((int)(splineT), 0, SoundwayManager.Instance.SplineSegments-1);
            if (currentSoundway == SoundwayManager.Instance.rightSoundway && splineIndex > 0)
            {
                splineIndex--;
            }
            return currentSoundway.GetSpline(splineIndex);
        }
        return null;
    }
    
    public float GetNormalizedT()
    {
        return normalizedT;
    }

    public void Update()
    {
        if (!IsEnabled) return;
        
        float deltaTime = Time.deltaTime;
        
        if (currentSoundway != null)
        {
            var spline = GetCurrentSpline();
            float splineNormalizedSpeed = 1.0f / timePerSegment;
            
            // Advance normalized parameter
            splineT += splineNormalizedSpeed * deltaTime;
            normalizedT = splineT - Mathf.Floor(splineT);

            // Sample spline (position + rotation)
            spline.Evaluate(normalizedT, out var position, out var tangent, out var up);
            tangent = math.normalize(tangent);
            up = math.normalize(up);
            
            // Calculate right vector for lateral offset
            Vector3 right = Vector3.Cross(up, tangent).normalized;
            
            // Set target position and rotation
            targetPosition = position;
            targetRotation = Quaternion.LookRotation(tangent, up);
            
            // Smooth steering with acceleration/deceleration for responsive feel
            float steerInputX = steerInput.x;
            if (Mathf.Abs(steerInputX) > Mathf.Epsilon)
            {
                // Accelerate towards target velocity based on input
                float targetVelocity = steerSpeed * steerInputX;
                currentSteerVelocity = Mathf.MoveTowards(currentSteerVelocity, 
                    targetVelocity, steerAcceleration * deltaTime);
            }
            else
            {
                // Decelerate when no input - return to center smoothly
                currentSteerVelocity = Mathf.MoveTowards(currentSteerVelocity, 0f, 
                    steerDeceleration * deltaTime);
                targetSteerOffset = 0f; // Reset target when no input
            }
            
            // Update steer offset with smooth velocity-based movement
            steerOffset += currentSteerVelocity * deltaTime;
            steerOffset = Mathf.Clamp(steerOffset, -steerMaxPositionOffset, steerMaxPositionOffset);
            
            // Smooth position interpolation
            Vector3 finalPosition = targetPosition + right * steerOffset;
            transform.position = Vector3.Lerp(transform.position, finalPosition, 
                positionSmoothing * deltaTime);
            
            // Smooth rotation interpolation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                rotationSmoothing * deltaTime);
            
            // Smooth roll angle interpolation for visual feedback
            currentRollAngle = Mathf.Lerp(currentRollAngle, targetRollAngle, 
                rollSmoothing * deltaTime);
            mesh.transform.localRotation = Quaternion.Euler(0f, 0f, -currentRollAngle);
        }
        
        if (queuedSoundway != null && swapTween == null)
        {
            queuedSoundway.GetSpline(0).Evaluate(0.0f, out var position, out var tangent, out var up);
            swapTween = transform.DORotate(Quaternion.LookRotation(tangent, up).eulerAngles,
                    AudioManager.Instance.TimePerBeat*2)
                .SetEase(Ease.InOutSine);
        }
    }

    public void OnDestroy()
    {
        if (swapTween != null)
        {
            swapTween.Kill();
        }
    }
}
    