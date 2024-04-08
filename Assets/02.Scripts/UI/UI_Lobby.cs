using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
public enum SecneNames
{
    Lobby,  // 0
    Main,   // 1
}
public class UI_Lobby : MonoBehaviour
{
    public TMP_InputField[] inputField;


    // 클릭 이벤트 핸들러
    public void OnPointerClick(int _num)
    {
        // 마우스 클릭 시 플레이스홀더 텍스트를 지웁니다.
        if (inputField != null && inputField[_num].placeholder != null)
        {
            TextMeshProUGUI a = inputField[_num].placeholder.GetComponent<TextMeshProUGUI>();
            a.text = "";
        }
    }
    public void OnClickMakeRoomButton()
    {
        string nickname = inputField[0].text;
        string roomID = inputField[1].text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomID))
        {
            Debug.Log("입력하세요.");
            return;
        }

        PhotonNetwork.NickName = nickname;
        PhotonNetwork.JoinOrCreateRoom(roomID, null, TypedLobby.Default);


    }
        
}
