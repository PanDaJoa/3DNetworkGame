using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform Target;
    public float YDistance = 10f;
    private Vector3 _initialEulerAngles;

    //private float _mx = 0;
    //private float _my = 0;

    public float RotationSpeed = 500;

    public static MinimapCamera Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        
        _initialEulerAngles = transform.eulerAngles;
    }
    private void LateUpdate()
    {
        
        Vector3 targetPosition = Target.position;
        targetPosition.y = YDistance;


        transform.position = targetPosition;
        Vector3 targetEulerAngles = Target.eulerAngles;
        targetEulerAngles.x = _initialEulerAngles.x;
        targetEulerAngles.z = _initialEulerAngles.z;
        transform.eulerAngles = targetEulerAngles;
    }
}
