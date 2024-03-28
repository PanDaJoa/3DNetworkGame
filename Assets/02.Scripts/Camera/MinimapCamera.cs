using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform Target;
    public float YDistance = 10f;
    private Quaternion _initialRotation;

    public static MinimapCamera Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        // 초기 카메라의 회전 값을 가져옴
        _initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // 타겟의 위치를 가져와서 Y축 값만 조정하여 카메라 위치로 설정
        Vector3 targetPosition = Target.position;
        targetPosition.y = YDistance;
        transform.position = targetPosition;

        // 타겟의 회전 값을 가져와서 미니맵 카메라에 적용
        transform.rotation = Quaternion.Euler(90f, Target.eulerAngles.y, 0f);
    }
}