using Unity.Netcode;
using UnityEngine;

public class CarControllerOffline : MonoBehaviour
{
    public WheelCollider[] wheelColliders;
    public Transform[] wheelTransforms;
    public float maxSteerAngle = 30f;
    public float maxMotorTorque = 1500f;

    private float currentSteerAngle;
    private float currentMotorTorque;

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        HandleSteering(horizontalInput);
        Accelerate(verticalInput);
        UpdateWheelPoses();
    }

    void HandleSteering(float steering)
    {
        currentSteerAngle = maxSteerAngle * steering;
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.steerAngle = currentSteerAngle;
        }
    }

    void Accelerate(float acceleration)
    {
        currentMotorTorque = maxMotorTorque * acceleration;
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.motorTorque = currentMotorTorque;
            wheelCollider.brakeTorque = 0f; // Sadece gaz verildiğinde freni serbest bırak
        }
    }

    void UpdateWheelPoses()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            Quaternion wheelRotation;
            Vector3 wheelPosition;
            wheelColliders[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelTransforms[i].position = wheelPosition;
            wheelTransforms[i].rotation = wheelRotation;
        }
    }
}