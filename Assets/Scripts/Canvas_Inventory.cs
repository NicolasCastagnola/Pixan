using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class Canvas_Inventory : BaseMonoSingleton<Canvas_Inventory>
{
    private Color originalColor;
    private int pointsUsed;

    public static bool isOpen;

    [field:SerializeField] public InventorySystem Inventory { get; private set; }

    [SerializeField] private AnimatedContainer mainContainer;
    [SerializeField] private UnityEvent onOpen;
    [SerializeField] private UnityEvent onClose;

    [SerializeField] Button iceButton;
    [SerializeField] Button fireButton;

    [SerializeField] TextMeshProUGUI iceText;
    [SerializeField] TextMeshProUGUI fireText;

    [SerializeField] Image levelBar;

    public ParticleSystem iceParticle  => Player.Instance.view.iceWeaponParticle;
    public ParticleSystem fireParticle => Player.Instance.view.fireWeaponParticle;

    [SerializeField] Button[] statsButton;

    public int PointsStats { get; private set; } = 0;

    public TextMeshProUGUI pointsText;
    public Action<StatType> onUpgradeStat;
    public Action onRefreshPoints;

    [SerializeField] GameObject levelUpSelection;

    [SerializeField] private List<GameObject> inventoryItemSlot;

    public void ShowSlotItem (int index) => inventoryItemSlot[index].SetActive(true);
    public void HideSlotItem(int index) => inventoryItemSlot[index].SetActive(false);

    protected override void Start()
    {
        originalColor = pointsText.color;
        UpdateText();

        base.Start();
        LoadPoints();
    }

    private void UpdateText() => pointsText.text = PointsStats.ToString();
    public void AddUsedPoint() => pointsUsed += 1;
    public void AddPoints()
    {
        PointsStats++;
        UpdateText();
        SavePoints();//save
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Open();
        }
    }
    public void RemovePoints()
    {
        PointsStats--;

        if (PointsStats <= 0) PointsStats = 0;

        UpdateText();
        SavePoints();//save
        //if (levelUpAmmount == 5) {
        //    OpenRewardSelection();
        //};
    }

    public void OpenRewardSelection()
    {
        levelUpSelection.SetActive(true);
    }

    public void CloseRewardSelection(int skill)
    {
        if(SaveCheckManager.Instance) SaveCheckManager.Instance.AddSkill((PlayerSkills)skill);
        levelUpSelection.SetActive(false);

        switch ((PlayerSkills)skill)
        {
            case PlayerSkills.FireSkill:
                fireParticle.Play();
                fireText.text = "Allows you to shoot down obstacles that require the fire skill to break. (Got it)";
                fireButton.interactable = false;
                break;
            case PlayerSkills.IceSkill:
                iceParticle.Play();
                iceText.text = "Allows you to shoot down obstacles that require the ice skill to break. (Got it)";
                iceButton.interactable = false;
                break;
            default:
                break;
        }
    }

    public void OpenCloseInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Open();
        }
    }

    public void Open()
    {
        if (isOpen)
        {
            onClose?.Invoke();
            mainContainer.Hide();
            //Time.timeScale = 1;
            GameManager.instance.LockCursor(true);
        }
        else
        {
            onOpen?.Invoke();
            mainContainer.Show();


            bool isPlayerSittingOnBonfire = Player.Instance.StateMachine.GetCurrentState() == States.PlayerInBonfire;
            foreach (var item in statsButton)
            {//if isnt in bonfire, cant interact with stats
                item.gameObject.SetActive(isPlayerSittingOnBonfire);
            }

            //Time.timeScale = 0;
            GameManager.instance.LockCursor(false);
        }

        isOpen = !isOpen;
    }

    public void RefreshPoints()
    {
        foreach (var item in GetComponentsInChildren<StatSlot>())
        {
            item.ClearSlots();
        }

        onRefreshPoints?.Invoke();
        PointsStats += pointsUsed;
        pointsUsed = 0;
        UpdateText();

        if (SaveCheckManager.Instance) SaveCheckManager.Instance.CleanSkills();

        iceButton.interactable=true;
        fireButton.interactable = true;

        iceText.text = "Allows you to shoot down obstacles that require the ice skill to break.";
        fireText.text = "Allows you to shoot down obstacles that require the fire skill to break.";

        iceParticle.Stop();
        fireParticle.Stop();

        SavePoints();//save
    }
    public void SavePoints()
    {
        foreach (var item in GetComponentsInChildren<StatSlot>())
        {
            PlayerPrefs.SetInt(item.StatLevel.Item1, item.StatLevel.Item2);
        }
        if (SaveCheckManager.Instance) SaveCheckManager.Instance.SaveSkills();

        PlayerPrefs.SetInt("PointsUsed", pointsUsed);
        PlayerPrefs.SetInt("PointsStats", PointsStats);
        PlayerPrefs.SetFloat("LevelBar", levelBar.fillAmount);

    }
    public void LoadPoints()
    {
        foreach (var item in GetComponentsInChildren<StatSlot>())
        {
            item.StatLevel=Tuple.Create(item.StatLevel.Item1,PlayerPrefs.GetInt(item.StatLevel.Item1));
            item.UpdateUI();
        }

        //search if has hability, add more if needed
        if (PlayerPrefs.GetInt(PlayerSkills.FireSkill.ToString()) == 1)
            CloseRewardSelection((int)PlayerSkills.FireSkill);
        if (PlayerPrefs.GetInt(PlayerSkills.IceSkill.ToString()) == 1)
            CloseRewardSelection((int)PlayerSkills.IceSkill);

        pointsUsed=PlayerPrefs.GetInt("PointsUsed");
        PointsStats=PlayerPrefs.GetInt("PointsStats");
        UpdateText();

        var _levelBar= PlayerPrefs.GetFloat("LevelBar");

        levelBar.fillAmount = _levelBar;
        Canvas_Playing.Instance.playerRewardBar.fillAmount = _levelBar;
    }

    public void CantUpgradeStat()
    {
        StopAllCoroutines();
        StartCoroutine(CantUpgradeCO());
    }

    private IEnumerator CantUpgradeCO()
    {
        int counter = 5;
        while (counter > 0)
        {
            pointsText.color = Color.red;
            yield return null;
            pointsText.color = originalColor;
            counter--;
            yield return null;
        }
    }

    public int GetSavedDamagePoints()
    {
        return PlayerPrefs.GetInt("Damage", 0);
    }
    public int GetSavedSpeedPoints()
    {
        return PlayerPrefs.GetInt("Speed", 0);
    }
    public int GetSavedHealthPoints()
    {
        return PlayerPrefs.GetInt("Health", 0);
    }
}
