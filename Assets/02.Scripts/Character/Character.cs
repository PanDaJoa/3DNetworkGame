using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
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

    private Vector3 _receivedPosition;
    private Quaternion _receivedRotation;

    private int _halfScore;

    public State GetState()
    {
        return State;
    }


    private List<CharacterAbility> _abilities;

    private CinemachineImpulseSource impulseSource;

    public Animator _animator;

    public int Score;
    public TextMeshProUGUI ScoreTextUI;
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

        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
        if (!PhotonView.IsMine)
        {
            return;
        }
        SetRandomPositionAndRotation();

        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("Score", 0);
        hashtable.Add("KillCount", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    [PunRPC]
    public void AddPropertyIntValue(string key, int value)
    {
        ExitGames.Client.Photon.Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashtable[key] = (int)myHashtable[key] + value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashtable);
        GetComponent<CharacterAttackAbility>().RefreshWeaponScale();
    }
    public void SetPropertyIntValue(string key, int value)
    {
        ExitGames.Client.Photon.Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashtable[key] = value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashtable);
        GetComponent<CharacterAttackAbility>().RefreshWeaponScale();

    }
    public int GetPropertyIntValue(string key)
    {
        ExitGames.Client.Photon.Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        return (int)myHashtable[key];
    }

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
        _halfScore = GetPropertyIntValue("Score") / 2;

        // 죽으면 점수를 0점으로 변경
        SetPropertyIntValue("Score", 0);

        if (actorNumber >= 0)
        {
            // 로그 메시지를 생성하여 모든 클라이언트에게 전달
            string nickname = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).NickName;
            string logMessage = $"\n<color=#FF00FF>{nickname}</color>님이 <color=#0000FF>{PhotonView.Owner.NickName}</color>을 처치하였습니다 !";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);

            Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            PhotonView.RPC(nameof(AddPropertyIntValue), targetPlayer, "Score", _halfScore);
            PhotonView.RPC(nameof(AddPropertyIntValue), targetPlayer, "KillCount", 1);
        }
        else
        {
            string logMessage = $"\n<color=#B40404>{PhotonView.Owner.NickName}이 운명을 다했습니다.</color>";
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
            DropItems();

            StartCoroutine(Death_Coroutine());
        }
    }

    private void DropItems()
    {/*- 70%: Player 스크립트에 점수가 있고 먹으면 점수가 1점씩 오른다. (3~5개 랜덤 생성)
            - (score 변수는 일단 Character에 생성)
        - 20%: 먹으면 체력이 꽉차는 아이템 1개
            - 10%: 먹으면 스태미나 꽉차는 아이템 1개*/
        // 팩토리패턴: 객체 생성과 사용 로직을 분리해서 캡슐화하는 패턴
        int randomValue = UnityEngine.Random.Range(0, 100);
        if (randomValue > 30)      // 70%
        {
            int randomCount100 = _halfScore / 100;
            int randomCount50 = _halfScore % 100 / 50;
            int randomCount20 = _halfScore % 100 % 50 / 20;
            for (int i = 0; i < randomCount100; ++i)
            {
                ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem100, transform.position);
            }
            for (int i = 0; i < randomCount50; ++i)
            {
                ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem50, transform.position);
            }
            for (int i = 0; i < randomCount20; ++i)
            {
                ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem20, transform.position);
            }
        }
        else if (randomValue > 10) // 20%
        {
            ItemObjectFactory.Instance.RequestCreate(ItemType.HealthPotion, transform.position);
        }
        else                       // 10%
        {
            ItemObjectFactory.Instance.RequestCreate(ItemType.StaminaPotion, transform.position);
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


