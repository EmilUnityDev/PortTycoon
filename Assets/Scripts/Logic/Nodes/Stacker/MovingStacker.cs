using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingStacker : MonoBehaviour
{
    [SerializeField] private Transform _chopper;

    private Vector3 _startEulerRotation;

    private void Start()
    {
        _startEulerRotation = _chopper.transform.eulerAngles;
    }

    private void Update()
    {
        _chopper.transform.rotation = Quaternion.Euler(0f, _chopper.transform.rotation.eulerAngles.y, _startEulerRotation.z);
        _chopper.transform.localRotation = Quaternion.Euler(_chopper.transform.localRotation.eulerAngles.x, 0f, _chopper.transform.localRotation.eulerAngles.z);
    }
}