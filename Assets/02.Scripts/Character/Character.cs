using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
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


    private float DestroyTime = 2f;

    private CinemachineImpulseSource impulseSource;

    public Image HitEffectImageUI;
    public float HitEffectDelay = 0.2f;

    public Animator _animator;

    public Transform[] spawnPoints;

    public ItemObject itemFactory;

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

    private void Start()
    {
        SetRandomPositionAndRotation();
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

        if (transform.position.y <= -20f)
        {
            Death();
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
    public void AddLog(string logmessage)
    {
        UI_RoomInfo.Instance.AddLog(logmessage);
    }

    [PunRPC]
    public void Damaged(int damage, int actorNumber)
    {
        if (State == State.Death)
        {
            return;
        }

        Stat.Health -= damage;
        if (Stat.Health <= 0)
        {
            if (PhotonView.IsMine)
            {
                OnDeath(actorNumber);
            }

            PhotonView.RPC(nameof(Death), RpcTarget.All);
        }

        GetComponent<CharacterShakeAbility>().Shake();

        if (PhotonView.IsMine)
        {
            OnDamagedMine();
        }
    }
    private void OnDeath(int actorNumber)
    {
        if (actorNumber >= 0)
        {
            string nickname = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).NickName;
            string logMessage = $"\n{nickname}님이 {PhotonView.Owner.NickName}을 처치하였습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
        }
        else
        {
            string logMessage = $"\n{PhotonView.Owner.NickName}이 운명을 다했습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
        }
    }

    private void OnDamagedMine()
    {
        // 카메라 흔들기 위해 Impulse를 발생시킨다.
        CinemachineImpulseSource impulseSource;
        if (TryGetComponent<CinemachineImpulseSource>(out impulseSource))
        {
            float strength = 0.4f;
            impulseSource.GenerateImpulseWithVelocity(UnityEngine.Random.insideUnitSphere.normalized * strength);
        }

        UI_DamageEffect.Instance.Show(0.5f);
    }

    [PunRPC]
    private void Death()
    {
        if (State == State.Death)
        {
            return;
        }
        State = State.Death;

        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<CharacterAttackAbility>().InActiveCollider();


        // 죽고나서 5초후 리스폰
        if (PhotonView.IsMine)
        {
            // 팩토리패턴: 
            ItemObjectFactory.Instance.RequestCreate(ItemType.HealthPotion, transform.position);
            ItemObjectFactory.Instance.RequestCreate(ItemType.StaminaPotion, transform.position);

            StartCoroutine(Death_Coroutine());
        }
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        SetRandomPositionAndRotation();

        PhotonView.RPC(nameof(Live), RpcTarget.All);
    }

    private void SetRandomPositionAndRotation()
    {
        Vector3 spawnPoint = BattleScene.Instance.GetRandomSpawnPoint();
        GetComponent<CharacterMoveAbility>().Teleport(spawnPoint);
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

    [PunRPC]
    private void Live()
    {
        State = State.Live;

        Stat.Init();

        GetComponent<Animator>().SetTrigger("Live");

    }

}


