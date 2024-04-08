using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Collider))]
public class ItemObject : MonoBehaviourPun
{
    [Header("아이템 타입")]
    public ItemType ItemType;
    public float Value;

    private void Start()
    {
        if (photonView.IsMine)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector.y = 1f;
            randomVector.Normalize();
            randomVector *= UnityEngine.Random.Range(3, 5f);
            rigidbody.AddForce(randomVector, ForceMode.Impulse);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();

            if (!character.PhotonView.IsMine || character.State == State.Death)
            {
                return;
            }
            //int a = (int)ItemType;
            //if (a >= 2)
            //    a = 2;
            //character.GetComponent<CharacterEffectAbility>().RequestPlay(a);

            switch (ItemType)
            {
                case ItemType.HealthPotion: 
                {
            character.GetComponent<CharacterEffectAbility>().RequestPlay(0);
                    character.Stat.Health += (int)Value;
                    if (character.Stat.Health >= character.Stat.MaxHealth)
                    {
                        character.Stat.Health = character.Stat.MaxHealth;
                    }
                    break;
                }
                case ItemType.StaminaPotion:
                {
            character.GetComponent<CharacterEffectAbility>().RequestPlay(1);
                    character.Stat.Stamina += Value;
                    if (character.Stat.Stamina >= character.Stat.MaxStamina)
                    {
                        character.Stat.Stamina = character.Stat.MaxStamina;
                    }                    
                    break;
                }
                case ItemType.ScoreItem100:
                case ItemType.ScoreItem50:
                case ItemType.ScoreItem20:
                {
            character.GetComponent<CharacterEffectAbility>().RequestPlay(2);
                    character.AddPropertyIntValue("Score", (int)Value);
                    break;
                }



            }

            gameObject.SetActive(false);
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
}
