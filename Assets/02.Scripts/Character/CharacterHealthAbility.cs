using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthAbility : CharacterAbility
{
    void Start()
    {
        Owner.Stat.Health = Owner.Stat.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
