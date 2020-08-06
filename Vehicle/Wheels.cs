using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wheels : MonoBehaviour
{
    [System.Serializable]
    public class Wheel
    {
        public WheelCollider WheelCollider = null;
        public GameObject WheelRenderer = null;
    }

    [SerializeField] private List<Wheel> _wheels = new List<Wheel>();

    private void Update()
    {
        foreach(Wheel wheel in _wheels)
        {
            Vector3 wheelPosition = Vector3.zero;
            Quaternion wheelRotation = Quaternion.identity;

            wheel.WheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
            wheel.WheelRenderer.transform.position = wheelPosition;
            wheel.WheelRenderer.transform.rotation = wheelRotation;
        }
    }

}
