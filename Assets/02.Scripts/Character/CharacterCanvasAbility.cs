using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasAbility : CharacterAbility
{
    public Canvas MyCanvas;
    public Text NicknameTextUI;
    public Slider HpSliderUI, StaminaSliderUI;

    private void Start()
    {
        NicknameTextUI.text = _owner.PhotonView.Controller.NickName;
    }

    private void Update()
    {
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }

        HpSliderUI.value = (float)_owner.Stat.Health / _owner.Stat.MaxHealth;
        StaminaSliderUI.value = _owner.Stat.Stamina / _owner.Stat.MaxStamina;
    }
}
