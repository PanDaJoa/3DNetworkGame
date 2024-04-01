using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    public TMP_InputField inputField;

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
}
