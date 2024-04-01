using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum SecneNames
{
    Lobby,  // 0
    Main,   // 1
}
public class LobbyScene : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button Button;

    // 클릭 이벤트 핸들러
    public void OnPointerClick( )
    {
        // 마우스 클릭 시 플레이스홀더 텍스트를 지웁니다.
        if (inputField != null && inputField.placeholder != null)
        {
            TextMeshProUGUI a = inputField.placeholder.GetComponent<TextMeshProUGUI>();
            a.text = "";
        }
    }
    public void OnJoinButton()
    {
        SceneManager.LoadScene(1);
    }
}
