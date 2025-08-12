using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Canvas_Playing : BaseMonoSingleton<Canvas_Playing>
{
    private static readonly int Transition = Animator.StringToHash("Transition");
    private event Action OnPause, OnBonfireMenuClosed, OnWarpFeedbackCompleted;

    public static bool InBonfireMenu;
    public static bool InDialogue => Instance.DialogueController.ActiveDialogue;

    private Vector3 initialPos;
    private float wasHitTime;

    public Image playerRewardBar;

    public TMP_Text healPotions;

    [SerializeField] private AnimatedContainer _container;
    [SerializeField] private Image playerLife;
    [SerializeField] private Image playerStamina;
    [SerializeField] private Image playerRewardBackground;
    [SerializeField] private GameObject lvlMaxText;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private RectTransform rewardLife;
    [field: SerializeField] public UIFader UIFader { get; private set; }
    [field: SerializeField] public AnimatedContainer DialogueAnimatedContainer { get; private set; }
    [field: SerializeField] public DialogueController DialogueController { get; private set; }
    [field: SerializeField] public BossHealthBar BossHealthBar { get; private set; }
    [field: SerializeField] public GUI_GameEventMessage GameEventMessage { get; private set; }
    [field: SerializeField] public BonfireMenu BonfireMenu { get; private set; }

    [SerializeField] private AnimatedContainer gameAreaContainer;
    [SerializeField] private float gameAreaContainerDuration;
    [SerializeField] private TMP_Text gameAreaContainerDisplay;
    
    [SerializeField] private float warpFeedbackDuration = 4f;
    [SerializeField] private Animator warpFeedbackAnimator;
    
    [SerializeField] private GameObject interactDisplayContainer;
    [SerializeField] private TMP_Text interactDisplay;
    [SerializeField] private CanvasGroup orbTextUIfader;

    public void Initialize(Action onPause)
    {
        OnPause = onPause;

        BonfireMenu.OnWarp += WarpFeedback;
    }
    private void WarpFeedback(Bonfire bonfire) => StartCoroutine(WaitForWarpFeedback(bonfire));
    private IEnumerator WaitForWarpFeedback(Bonfire bonfire)
    {
        warpFeedbackAnimator.SetBool(Transition, true);
        
        yield return new WaitForSeconds(warpFeedbackDuration);
            
        warpFeedbackAnimator.SetBool(Transition, false);

        Player.Instance.Motor.SetPosition(bonfire.BonfireTravelPoint.transform.position);

        CloseBonfireMenu();
        
        OnWarpFeedbackCompleted?.Invoke();
    }

    public void Terminate()
    {
        BonfireMenu.OnWarp -= WarpFeedback;
        
        // OnPause = null;
    }
    public void ShowAreaTitle(string areaName, Action finishedShowing) => StartCoroutine(WaitShowAreaTitle(areaName, finishedShowing));
    private IEnumerator WaitShowAreaTitle(string areaName, Action finishedShowing)
    {
        gameAreaContainerDisplay.text = areaName;
        
        gameAreaContainer.Open();
        
        yield return new WaitForSeconds(gameAreaContainerDuration);
        
        gameAreaContainer.Close();
        
        finishedShowing?.Invoke();
    }
    public void UpdateLife(float percentage) => playerLife.DOFillAmount(percentage, 0.1f);
    public void UpdateStamina(float staminaStaminaPercentage) => playerStamina.DOFillAmount(staminaStaminaPercentage, 0.1f);
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Canvas_Inventory.isOpen || !Canvas_Pause.IsPlaying || InBonfireMenu) return;

            if (Player.Instance != null && Player.Instance.Health.Current <= 0) return;

            Canvas_Pause.IsPlaying = false;
            OnPause?.Invoke();
        }
    }
    public void UpdatePostureReset()
    {
        playerStamina.DOFillAmount(1, 0.1f);
        wasHitTime = 0;
    }

    // public void UpdatePosture(float ammount)
    // {
    //     playerPosture.DOFillAmount((playerPosture.fillAmount - ((ammount * 1.5f) / 100)), 0.1f).OnComplete(() =>
    //     {
    //         if (playerPosture.fillAmount <= 0f)
    //         {
    //             playerPosture.DOFillAmount(1, 0.5f);
    //             Player.Instance.Controller.InterruptAction(CharacterState.PlayerStun);
    //         }
    //
    //         wasHitTime = 0;
    //     });
    //
    //     UpdateLife(ammount / 4);
    // }

  	public void OnGetGreenOrb(string text)
    {
        DoInventoryMessage(text);
        Canvas_Inventory.Instance.AddPoints();
    }
    public void DoInventoryMessage(string text)
    {
        orbTextUIfader.DOFade(1, 0.5f).OnComplete(() => orbTextUIfader.DOFade(0, 0.5f).SetDelay(5f));
        orbTextUIfader.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
    // public void UpdateStamina(float amount) => playerPosture.DOFillAmount((playerPosture.fillAmount - (amount / 100)), 0.1f);
    //
    // public float GetStamina() => playerPosture.fillAmount * 100f;
    //
    // public float GetPosture() => playerPosture.fillAmount;
    //
    // public void UpdateOnlyPosture(float ammount)
    // {
    //     playerPosture.DOFillAmount((playerPosture.fillAmount - ((ammount * 1.5f) / 100)), 0.1f).OnComplete(() =>
    //     {
    //         wasHitTime = 0;
    //     });
    // }

    public void OnUpdateExp(int value, int ammountToLevelUp, UnityEvent uv)
    {
        StopAllCoroutines();

        var color = playerRewardBar.color;
        color.a = 1;
        playerRewardBar.color = color;
        playerRewardBackground.color = Color.white;


        playerRewardBar.DOFillAmount(((float)value / ammountToLevelUp), 1).OnUpdate(() => SecondaryLevelBar.UpdateExperienceBar((float)value / ammountToLevelUp)).OnComplete(() =>
        {
            if (value == ammountToLevelUp)
            {
                //playerRewardBar.transform.parent.parent.DOMove(playerRewardBar.transform.parent.parent.position + new Vector3(180, 0, 0), 1);
                OnMaxLvl();
                Canvas_Inventory.Instance.AddPoints();
                uv?.Invoke();
            }

            StartCoroutine(RewardAnim(0));
        });

    }
    public void OnMaxLvl()
    =>
        lvlMaxText.transform.DOScale(1, 0.5f)
            .OnStart(() => lvlMaxText.SetActive(true))
            .OnComplete(() => lvlMaxText.transform.DOScale(0, 0.5f).SetDelay(2.5f)
            .OnComplete(() =>
            {
                lvlMaxText.SetActive(false);
                playerRewardBar.fillAmount = 0;
                SecondaryLevelBar.UpdateExperienceBar(0);
            }));

    IEnumerator RewardAnim(float alpha)
    {
        yield return new WaitForSecondsRealtime(3);

        var color = playerRewardBar.color;
        var color2 = playerRewardBackground.color;
        float clampedAlpha = Mathf.Clamp(alpha, 0.2f, 0.8f);

        while (clampedAlpha > color.a ? color.a <= clampedAlpha : color.a >= clampedAlpha)
        {
            color.a = Mathf.Lerp(color.a, alpha, 0.2f);
            color2.a = Mathf.Lerp(color2.a, alpha, 0.2f);

            playerRewardBar.color = color;
            playerRewardBackground.color = color2;


            yield return new WaitForEndOfFrame();
        }
        color.a = alpha;
        playerRewardBar.color = color;


        color2.a = alpha;
        playerRewardBackground.color = color2;
        yield break;
    }

    public void OpenCloseReward()
    {
        lvlMaxText.SetActive(true);
    }
    public void HideInteractDisplay()
    {
        interactDisplayContainer.SetActive(false);
    }
    public void ShowInteractDisplay(string onHoverMessage)
    {
        interactDisplayContainer.SetActive(true);
        interactDisplay.SetText(onHoverMessage);
    }
    public void ShowBonfireMenu(Action bonfireMenuClosed, Action onWarp)
    {
        HideInteractDisplay();

        OnWarpFeedbackCompleted = onWarp;
        OnBonfireMenuClosed = bonfireMenuClosed;

        Cursor.lockState = CursorLockMode.None;

        InBonfireMenu = true;

        BonfireMenu.Show();
    }
    public void CloseBonfireMenu()
    {
        BonfireMenu.Hide();

        //if(Canvas_Inventory.Instance!=null)
        //    Canvas_Inventory.Instance.SavePoints();//save

        InBonfireMenu = false;

        Cursor.lockState = CursorLockMode.Locked;
        
        OnBonfireMenuClosed?.Invoke();
    }
    public void ShowQuestionWindow(string currentQuestion, string currentAnswerOne, string currentAnswerTwo)
    {
        throw new NotImplementedException();
    }
}
