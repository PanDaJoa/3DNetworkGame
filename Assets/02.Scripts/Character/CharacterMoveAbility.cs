using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterMoveAbility : CharacterAbility
{
    // 목표: [W],[A],[S],[D] 및 방향키를 누르면 캐릭터를 그 뱡향으로 이동시키고 싶다.
    private CharacterController _characterController;
    public Animator _animator;

    public float _yVelocity;
    private float _gravity = -7;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private PhotonView _photonView;
    private void Update()
    {
        if (!_owner.PhotonView.IsMine)
        {
            return;
        }
        // 순서
        // 1. 사용자의 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. '캐릭터가 바라보는 방향'을 기준으로 방향을 설정한다.
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);

        _animator.SetFloat("Move", dir.magnitude);

        // 4. 중력 적용하세요.
        dir.y = _yVelocity;
        _yVelocity += _gravity * Time.deltaTime;


        // 3. 이동속도에 따라 그 방향으로 이동한다.
        float moveSpeed = _owner.Stat.MoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && _owner.Stat.Stamina > 0)
        {
            moveSpeed = _owner.Stat.RunSpeed;
            _owner.Stat.Stamina -= Time.deltaTime * _owner.Stat.RunConsumeStamina;
        }
        else
        {
            _owner.Stat.Stamina += Time.deltaTime * _owner.Stat.RecoveryStamina;
            if (_owner.Stat.Stamina >= _owner.Stat.MaxStamina)
            {
                _owner.Stat.Stamina = _owner.Stat.MaxStamina;
            }
        }

        // 4. 이동속도에 따라 그 방향으로 이동한다.
        _characterController.Move(dir * (moveSpeed * Time.deltaTime));

        if (_characterController.isGrounded && Input.GetKey(KeyCode.Space))
        {
            _yVelocity = _owner.Stat.JumpPower;
        }
    }

}

