using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterShakeAbility))]
public class Character : MonoBehaviour, IPunObservable, IDamaged, IState
{
    public PhotonView PhotonView { get; private set; }
    public Stat Stat;
    public State State { get; private set; } = State.Live;

    public State GetState()
    {
        return State;
    }


    private List<CharacterAbility> _abilities;


    private float DistroyTime = 2f;

    private CinemachineImpulseSource impulseSource;

    public Image HitEffectImageUI;
    public float HitEffectDelay = 0.2f;

    public Animator _animator;

    public T GetAbility<T>() where T : CharacterAbility
    {
        foreach (CharacterAbility ability in _abilities)
        {
            if (ability is T)
            {
                return ability as T;
            }
        }

        return null;
    }

    private void Awake()
    {
        _abilities = GetComponentsInChildren<CharacterAbility>().ToList<CharacterAbility>();

        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
        _animator = GetComponent<Animator>();
    }

    private Vector3 _receivedPosition;
    private Quaternion _receivedRotation;
    private void Update()
    {
        float damping = 20f;

        if (!PhotonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * damping);
            transform.rotation = Quaternion.Slerp(transform.rotation, _receivedRotation, Time.deltaTime * damping);
        }
    }
    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream(통로)은 서버에서 주고받을 데이터가 담겨있는 변수
        if (stream.IsWriting)        // 데이터를 전송하는 상황
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Stamina);
        }
        else if (stream.IsReading)   // 데이터를 수신하는 상황
        {
            // 데이터를 전송한 순서와 똑같이 받은 데이터를 캐스팅 해야된다.
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receivedRotation = (Quaternion)stream.ReceiveNext();
            Stat.Health = (int)stream.ReceiveNext();
            Stat.Stamina = (float)stream.ReceiveNext();
        }
        // info는 송수신 성공/실패 여부에 대한 메세지 담겨있다.
    }

    [PunRPC]
    public void Damaged(int damage)
    {
        if (State == State.Death)
        {
            return;
        }

        Stat.Health -= damage;

        if (PhotonView.IsMine)
        {
            CinemachineImpulseSource impulseSource = null;
            if (TryGetComponent<CinemachineImpulseSource>(out impulseSource))
            {
                float strength = 0.4f;
                impulseSource.GenerateImpulseWithVelocity(Random.insideUnitSphere.normalized * strength);
            }
            
            UI_DamageEffect.Instance.Show(0.5f);
        }

        GetAbility<CharacterShakeAbility>().Shake();

        if (Stat.Health <= 0)
        {
            State = State.Death;

            _animator.SetTrigger("Die");
            StartCoroutine(CharacterDestroy(DistroyTime));
        }


    }
    public IEnumerator CharacterDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);
    }
}


