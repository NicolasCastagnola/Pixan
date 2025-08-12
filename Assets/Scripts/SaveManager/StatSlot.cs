using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
//using System;

public class StatSlot : MonoBehaviour
{
    public Tuple<string, int> StatLevel { 
        get => Tuple.Create(statType.ToString(), stat); 
        set => stat = value.Item2; 
    }
    int stat;
    [SerializeField] int maxStat=8;

    [SerializeField] private Image levelSlot;
    [SerializeField] StatType statType;
    private Button button;

    private void Start()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(UpgradeStat);
    }
    int HaveSkills()
    {
        int _value = 0;
        if (SaveCheckManager.Instance.CheckIfHaveSkills(0))
            _value++;
        if (SaveCheckManager.Instance.CheckIfHaveSkills((PlayerSkills)1))
            _value++;

        return _value;
    }
    void UpgradeStat()
    {
        if (Canvas_Inventory.Instance.PointsStats <= 0)
        {
            Canvas_Inventory.Instance.CantUpgradeStat();
            return;
        }
        stat++;
        if (statType == StatType.Damage)//si entra en nivel uno a aumentar nivel significa que toca mejora, lo mismo para el nivel 4
            if (((float)stat / maxStat >= 0.25f && HaveSkills() == 0) || (float)stat / maxStat >= 1 && HaveSkills() == 1)
                Canvas_Inventory.Instance.OpenRewardSelection();
        UpdateUI();


        Canvas_Inventory.Instance.onUpgradeStat?.Invoke(statType);
        Canvas_Inventory.Instance.RemovePoints();

        if (Canvas_Inventory.Instance != null)
            Canvas_Inventory.Instance.SavePoints();//save
    }
    public void UpdateUI()
    {
        if (stat == maxStat) button.interactable = false;
        levelSlot.DOFillAmount((float)stat / maxStat, 0.2f).SetUpdate(true);
    }

    public void ClearSlots()
    {
        stat = 0;
        levelSlot.DOFillAmount(0, 0.2f).SetUpdate(true).OnComplete(() => button.interactable = true);
    }
}

public enum StatType { Health, Damage, Speed }
