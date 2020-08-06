using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleLights : MonoBehaviour
{
    [System.Serializable]
    public class VehicleLight
    {
        public Light Light = null;
        public Material ActiveMaterial = null;
        public Material InactiveMaterial = null;
        public MeshRenderer MeshRenderer = null;

        private bool _active = false;
        private bool _destroyed = false;

        public void HandleActive(bool active)
        {
            if (_active == active || _destroyed) return;

            _active = active;
            MeshRenderer.material = active ? ActiveMaterial : InactiveMaterial;
            Light.gameObject.SetActive(active);
        }

        public void Destroy()
        {
            _destroyed = true;
            MeshRenderer.gameObject.SetActive(false);
        }
    }

    [SerializeField] private List<VehicleLight> _backLights = new List<VehicleLight>();
    [SerializeField] private List<VehicleLight> headLights = new List<VehicleLight>();

    private Vehicle _vehicle = null;

    private void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        foreach (VehicleLight vehicleLight in _backLights)
        {
            vehicleLight.HandleActive(_vehicle.IsBraking || _vehicle.ReverseGear);
        }

        foreach(VehicleLight vehicleLight in headLights)
        {
            vehicleLight.HandleActive(_vehicle.IsBeingDriven && GameManager.Instance.IsNight);
        }
    }
}
