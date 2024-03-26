using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : MonoBehaviour
{
    public static UI_CharacterStat Instance { get; private set; }

    public Character MyCharacter;
    public Slider HealthSliderUI;
    public Slider StaminaSliderUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (MyCharacter == null)
        {
            Debug.Log("캐릭터없음");   
            return;
        }

        HealthSliderUI.value = (float)MyCharacter.Stat.Health / MyCharacter.Stat.MaxHealth;
        StaminaSliderUI.value = MyCharacter.Stat.Stamina / MyCharacter.Stat.MaxStamina;

    }
}
