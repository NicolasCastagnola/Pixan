using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonfireSelectable : MonoBehaviour
{
    private Bonfire _bonfire;
    private BonfireMenu _bonfireMenu;
    public int ID { get; private set; }

    [SerializeField] private TMP_Text bonfireAreaName;
    [SerializeField] private Button _button;

    public void SetActiveBonfire() => _bonfireMenu.SetCurrentSelected(_bonfire);

    public void Initialize(int id, bool bonfireState, BonfireMenu bonfireMenu, Bonfire bonfire)
    {
        ID = id;

        bonfireAreaName.text = bonfireState ? bonfire.BonfireName : "******************";
        _button.interactable = bonfireState;

        _bonfire = bonfire;
        _bonfireMenu = bonfireMenu;
    }
}
