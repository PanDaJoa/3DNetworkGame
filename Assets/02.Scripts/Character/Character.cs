using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public PhotonView PhotonView { get; private set; }
    public Stat Stat;

    private bool isDead = false;
    private void Awake()
    {
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();

        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
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
        Stat.Health -= damage;

        if (Stat.Health <= 0 )
        {
            isDead = true;
            HandleDeath();
        }


    }
    private void HandleDeath()
    {
        // 체력이 0 이하일 때만 캐릭터를 비활성화하고 게임에서 나가는 처리를 수행
        if (Stat.Health <= 0)
        {
            // 캐릭터를 비활성화하거나 다른 처리를 수행할 수 있습니다.
            gameObject.SetActive(false);

            // 여기서는 간단히 로그를 출력합니다.
            Debug.Log("플레이어가 사망했습니다.");

            // Photon 네트워크를 통해 플레이어를 나가게 처리
            LeaveGame();
        }
    }
    private void LeaveGame()
    {
        // Photon 네트워크를 통해 플레이어를 나가게 처리
        PhotonNetwork.LeaveRoom();
    }
}


