using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using System.Linq;

public class UI_PlayerRanking : MonoBehaviourPunCallbacks
{
    public List<UI_PlayerRankingSlot> Slots;
    public UI_PlayerRankingSlot MySlot;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Refresh();
    }
    // 플레이어가 룸에서 퇴장했을 때 호출되는 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Refresh();
    }
    public override void OnJoinedRoom()
    {
        Refresh();
    }
    private void Refresh()
    {
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
        List<Player> playerList = players.Values.ToList();

        int playerCount = Math.Min(playerList.Count, 5);
        foreach (UI_PlayerRankingSlot slot in Slots)
        {
            slot.gameObject.SetActive(false);
        }
        for (int i = 0; i < playerCount; i++)
        {
            Slots[i].gameObject.SetActive(true);
            Slots[i].Set(playerList[i]);
        }
        MySlot.Set(PhotonNetwork.LocalPlayer);
    }
}
