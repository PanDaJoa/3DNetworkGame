using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerRankingSlot : MonoBehaviour
{
    public TextMeshProUGUI RankingTextUI;
    public TextMeshProUGUI NicknameTextUI;
    public TextMeshProUGUI KillCountTextUI;
    public TextMeshProUGUI ScoreTextUI;

    public void Set(Player player)
    {
        RankingTextUI.text = "1";
        NicknameTextUI.text = player.NickName;
        KillCountTextUI.text = "10";
        ScoreTextUI.text = "10000";
    }
}
