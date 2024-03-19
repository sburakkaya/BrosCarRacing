using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WheelData
{
    public WheelCollider wheelCollider;
    public Transform visualWheel;
    [Range(-1f, 1.0f)]
    public float compression = 0.2f; // Sıkışma sınırı
    [Range(0.1f, 1.0f)]
    public float extension = 0.2f; // Genişleme sınırı
    [Range(0f, 100000f)]
    public float spring = 30000f; // Süspansiyon sertliği
    [Range(0f, 5000f)]
    public float damper = 1500f; // Amortisör sertliği
    [Range(-1.0f, 1.0f)]
    public float targetPosition = 0.5f; // Hedef konum
}

public class CustomSuspension : MonoBehaviour
{
    public List<WheelData> wheels;
    [Header("Global Wheel Settings")]
    [Range(-1f, 1.0f)]
    public float globalCompression = 0.2f;
    [Range(0.1f, 1.0f)]
    public float globalExtension = 0.2f;
    [Range(0f, 100000f)]
    public float globalSpring = 30000f;
    [Range(0f, 5000f)]
    public float globalDamper = 1500f;
    [Range(-1.0f, 1.0f)]
    public float globalTargetPosition = 0.5f;
    void Start()
    {
        foreach (WheelData wheelData in wheels)
        {
            WheelCollider wheel = wheelData.wheelCollider;
            JointSpring suspensionSpring = wheel.suspensionSpring;
            suspensionSpring.spring = wheelData.spring;
            suspensionSpring.damper = wheelData.damper;
            wheel.suspensionSpring = suspensionSpring;
            wheel.forceAppPointDistance = wheelData.compression;
            wheel.center = new Vector3(wheel.center.x, wheel.center.y - wheelData.compression, wheel.center.z);
        }
    }
    
    void OnValidate()
    {
        foreach (WheelData wheelData in wheels)
        {
            SetGlobalParameters(wheelData);
        }
    }

    void SetGlobalParameters(WheelData wheelData)
    {
        wheelData.compression = globalCompression;
        wheelData.extension = globalExtension;
        wheelData.spring = globalSpring;
        wheelData.damper = globalDamper;
        wheelData.targetPosition = globalTargetPosition;
    }

    void Update()
    {
        foreach (WheelData wheelData in wheels)
        {
            WheelCollider wheel = wheelData.wheelCollider;
            wheel.forceAppPointDistance = wheelData.extension + wheelData.targetPosition;

            Vector3 wheelPosition;
            Quaternion wheelRotation;
            wheel.GetWorldPose(out wheelPosition, out wheelRotation);

            Transform visualWheel = wheelData.visualWheel;
            visualWheel.position = wheelPosition;
            visualWheel.rotation = wheelRotation;
        }
    }
}