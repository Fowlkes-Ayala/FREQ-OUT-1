using System;
using DefaultNamespace;
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
    [SerializeField, Range(0f, 10f)] private float splineNormalizedSpeed = 0.2f;
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

    
    private Vector2 steerInput = Vector2.zero;
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
        bool hasSwapped = SoundwayManager.Instance.SwapSoundways(swapRight, out var newSoundway);
        if (hasSwapped)
        {
            currentSoundway = newSoundway;
        }
    }
    
    public void Start()
    {
        currentSoundway = SoundwayManager.Instance.GetCurrentSoundway();
    }


    public void Update()
    {
        if (currentSoundway != null)
        {
            var spline = currentSoundway.Spline;
            // Advance normalized parameter
            splineT += splineNormalizedSpeed * Time.deltaTime;
            if (splineT > 1f)
            {
                splineT = loopSpline ? splineT - 1f : 1f;
            }

            // Sample spline (position + rotation)
            var sample = spline.Evaluate(splineT, out var position,  out var tangent, out var up);
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(tangent, up);
            
            // Optionally apply lateral offset along the spline's right vector (strafe)
            if (Mathf.Abs(steerInput.x) > Mathf.Epsilon)
            {
                Vector3 right = transform.rotation * Vector3.right;
                transform.position += right * (steerSpeed * steerInput.x * Time.deltaTime);
            }
        }
    }
}
    