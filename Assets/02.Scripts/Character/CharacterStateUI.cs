using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStateUI : MonoBehaviour
{
    public static CharacterStateUI Instance { get; private set; }

    public Character _character;
    public Slider HealthSliderUI;
    public Slider StaminaSliderUI;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        HealthSliderUI.value = _character.Stat.Health/_character.Stat.MaxHealth;
        StaminaSliderUI.value = _character.Stat.Stamina/_character.Stat.MaxStamina;
    }
}
