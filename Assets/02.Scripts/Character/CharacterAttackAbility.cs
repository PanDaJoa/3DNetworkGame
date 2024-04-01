using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class CharacterAttackAbility : CharacterAbility
{
    // SOLID 법칙: 객체지향 5가지 법칙
    // 1. 단일 책임 원칙 (가장 단순하지만 꼭 지켜야 하는 원칙)
    // - 클래스는 단 한개의 책임을 가져야 한다.
    // - 클래스를 변경하는 이유는 단 하나여야 한다.
    // - 이를 지키지 않으면 한 책임 변경에 의해 다른 책임과 관련된 코드도 영향을 미칠 수 있어서
    //    -> 유지보수가 매우 어렵다.
    // 준수 전략
    // - 기존의 클래스로 해결할 수 없다면 새로운 클래스를 구현

    private Animator _animator;
    private float _attackTimer = 0;

    public Collider WeaponCollider;

    public CharacterController _characterController;

    private List<IDamaged> _damagedList = new List<IDamaged>();

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        if (!_owner.PhotonView.IsMine || _owner.State == State.Death)
        {
            return;
        }
        _attackTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _attackTimer >= _owner.Stat.AttackCoolTime && _owner.Stat.Stamina > _owner.Stat.AttackStamina)
        {
            if (!_characterController.isGrounded)
            {

                _owner.PhotonView.RPC(nameof(Attack4), RpcTarget.All);

            }
            else
            {
                _owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, Random.Range(1, 4));
            }
                _owner.Stat.Stamina -= _owner.Stat.RunConsumeStamina * _owner.Stat.AttackConsumeSpeed;

            _attackTimer = 0f;

            // RpcTarget.All      : 모두에게
            // RpcTarget.Others   : 나 자신을 제외하고 모두에게
            // RpcTarget.Master   : 방장에게만
        }
    }

    [PunRPC]
    public void PlayAttackAnimation(int index)
    {
        _animator.SetTrigger($"Attack{index}");
    }
    [PunRPC]
    public void Attack4()
    {
        _animator.SetTrigger("Attack4");
    }

    public void OnTriggerEnter(Collider other)
    {
        if(_owner.State == State.Death)
        {
            return;
        }

        if (_owner.PhotonView.IsMine == false || other.transform == transform)
        {
            return;
        }

        

        // O: 개방 폐쇄 원칙 + 인터페이스 
        // 수정에는 닫혀있고, 확장에는 열려있다.
        IDamaged damagedAbleObject = other.GetComponent<IDamaged>();
        
        if (damagedAbleObject != null)
        {
            IState stateObject = other.GetComponent<IState>();
            if (stateObject != null && stateObject.GetState() == State.Death)
            {
                return;
            }

            if (_damagedList.Contains(damagedAbleObject))
            {
                return;
            }

            _damagedList.Add(damagedAbleObject);

            //생성
            GameObject hiteffect = PhotonNetwork.Instantiate("HitEffect", Vector3.zero, Quaternion.identity);
            //위치
            hiteffect.transform.position = ((other.transform.position + transform.position) / 2f) + (Vector3.up * 0.7f);
            //파티클 재생
            hiteffect.GetComponent<ParticleSystem>().Play();



            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                
                photonView.RPC("Damaged", RpcTarget.All, _owner.Stat.Damage, _owner.PhotonView.OwnerActorNr);
            }
            //damagedAbleObject.Damaged(_owner.Stat.Damage);
        }
    }
    public void ActiveCollider()
    {
        WeaponCollider.enabled = true;
    }
    public void InActiveCollider()
    {
        WeaponCollider.enabled = false;

        _damagedList.Clear();
    }


}
