using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CampFire : MonoBehaviour
{
    public int Damage = 20;

    public float CoolTime = 1f; 
    private float _timer = 0f;

    private IDamaged _target = null;

    private void OnTriggerEnter(Collider other)
    {
        IDamaged damagedObject = other.GetComponent<IDamaged>();
        if (damagedObject == null)
        {
            return;
        }

        PhotonView photonView = other.GetComponent<PhotonView>();
        if (photonView == null || !photonView.IsMine)
        {
            return;
        }

        _target = damagedObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if(_target == null)
        {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= CoolTime)
        {
            _timer = 0f;
            _target.Damaged(Damage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamaged damagedObject = other.GetComponent<IDamaged>();
        if (damagedObject == null)
        {
            return;
        }

        if(damagedObject == _target)
        {
            _target = null;
            _timer = 0f;
        }
    }
}
