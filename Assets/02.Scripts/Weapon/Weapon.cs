using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterAttackAbility MycharacterAttackAbility;
    private void OnTriggerEnter(Collider other)
    {
        MycharacterAttackAbility.OnTriggerEnter(other);
    }
}
