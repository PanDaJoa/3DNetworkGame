using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
public class Character : MonoBehaviour
{
    public PhotonView PhotonView { get; private set; }
    public Stat Stat;

    private void Start()
    {
        Stat.Init();
    }
    
}
