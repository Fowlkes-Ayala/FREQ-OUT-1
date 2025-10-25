using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoverboardController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float strafeSpeed = 3f;
    [SerializeField] private float maxRollAngle = 60f; // Maximum roll angle in degrees
    [SerializeField] private GameObject mesh;

    private Vector2 steerInput = Vector2.zero;
    public void OnSteer(InputValue value)
    {
        steerInput = value.Get<Vector2>();
        float horizontalInput = steerInput.x;
        float angle = maxRollAngle * horizontalInput;
        mesh.transform.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    public void Update()
    {
        Vector3 forwardMovement = transform.forward * (forwardSpeed * Time.deltaTime);
        Vector3 strafeMovement = transform.right * (strafeSpeed * steerInput.x * Time.deltaTime);
        transform.position += forwardMovement + strafeMovement;
    }
}
    