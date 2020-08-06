using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [SerializeField] WheelCollider wheelL = null;
    [SerializeField] WheelCollider wheelR = null;
    [SerializeField] float _antiRoll = 5000.0f;

    private Rigidbody _rigidBody = null;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = wheelL.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-wheelL.transform.InverseTransformPoint(hit.point).y - wheelL.radius) / wheelL.suspensionDistance;

        bool groundedR = wheelR.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-wheelR.transform.InverseTransformPoint(hit.point).y - wheelR.radius) / wheelR.suspensionDistance;

        float antiRollForce = (travelL - travelR) * _antiRoll;

        if (groundedL)
            _rigidBody.AddForceAtPosition(wheelL.transform.up * -antiRollForce,
                   wheelL.transform.position);
        if (groundedR)
            _rigidBody.AddForceAtPosition(wheelR.transform.up * antiRollForce,
                   wheelR.transform.position);
    }
}
