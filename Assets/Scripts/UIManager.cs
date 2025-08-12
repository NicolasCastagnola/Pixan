using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

public class UIManager : MonoBehaviour
{
    // public static UIManager Instance { get; private set; }
    //
    // [SerializeField] Image playerLife;
    // [SerializeField] Image playerPosture;
    // [SerializeField] Image playerRewardBar, playerRewardBackground;
    //
    // [SerializeField] GameObject lvlMaxText;
    //
    // [SerializeField] GameObject rewardPanel;
    // [SerializeField] RectTransform rewardLife;
    //
    // Vector3 initialPos;
    // public Canvas mainCanvas;
    //
    // float wasHitTime;
    //
    // public AudioSource audioS;
    //
    // private void Awake()
    // {
    //     if (Instance == null) Instance = this;
    //     else Destroy(this);
    //     mainCanvas = GetComponent<Canvas>();
    // }
    //
    // public void UpdateLife(float ammount)
    // {
    //     playerLife.DOFillAmount((playerLife.fillAmount - (ammount / 100)), 0.1f);
    // }
    //
    // private void Update()
    // {
    //     if (playerPosture.fillAmount == 1) return;
    //
    //     wasHitTime += Time.deltaTime;
    //
    //     if (wasHitTime > 5) UpdatePostureReset();
    // }
    //
    // public void UpdatePostureReset()
    // {
    //     playerPosture.DOFillAmount(1, 0.1f);
    //     wasHitTime = 0;
    // }
    //
    // public void UpdatePosture(float ammount)
    // {
    //     playerPosture.DOFillAmount((playerPosture.fillAmount - ((ammount * 1.5f) / 100)), 0.1f).OnComplete(() => {
    //         if (playerPosture.fillAmount <= 0f)
    //         {
    //             playerPosture.DOFillAmount(1, 0.5f);
    //             Player.instance.FSM.ChangeState(States.PlayerStun);
    //         }
    //
    //         wasHitTime = 0;
    //     });
    //
    //     UpdateLife(ammount / 4);
    // }
    //
    // public void OnUpdateExp(int value, int ammountToLevelUp, UnityEvent uv)
    // {
    //     StopAllCoroutines();
    //
    //     var color = playerRewardBar.color;
    //     color.a = 1;
    //     playerRewardBar.color = color;
    //     playerRewardBackground.color = Color.white;
    //
    //
    //     playerRewardBar.DOFillAmount(((float)value / ammountToLevelUp), 1).OnComplete(() => {
    //         if (value == ammountToLevelUp)
    //         {
    //             //playerRewardBar.transform.parent.parent.DOMove(playerRewardBar.transform.parent.parent.position + new Vector3(180, 0, 0), 1);
    //             OnMaxLvl();
    //
    //             uv?.Invoke();
    //         }
    //         StartCoroutine(RewardAnim(0));
    //     });
    //
    //     audioS.Play();
    // }
    // public void OnMaxLvl()
    // =>
    //     lvlMaxText.transform.DOScale(1, 0.5f)
    //         .OnStart(()=> lvlMaxText.SetActive(true))
    //         .OnComplete(() => lvlMaxText.transform.DOScale(0, 0.5f).SetDelay(2.5f)
    //         .OnComplete(() => lvlMaxText.SetActive(false)));
    //
    // IEnumerator RewardAnim(float alpha)
    // {
    //     yield return new WaitForSecondsRealtime(3);
    //
    //     var color = playerRewardBar.color;
    //     var color2 = playerRewardBackground.color;
    //     float clampedAlpha = Mathf.Clamp(alpha, 0.2f, 0.8f);
    //
    //     while (clampedAlpha > color.a ? color.a <= clampedAlpha : color.a >= clampedAlpha)
    //     {
    //         color.a = Mathf.Lerp(color.a, alpha, 0.2f);
    //         color2.a = Mathf.Lerp(color2.a, alpha, 0.2f);
    //
    //         playerRewardBar.color = color;
    //         playerRewardBackground.color = color2;
    //
    //
    //         yield return new WaitForEndOfFrame();
    //     }
    //     color.a = alpha;
    //     playerRewardBar.color = color;
    //
    //
    //     color2.a = alpha;
    //     playerRewardBackground.color = color2;
    //     GameManager.ShowLog("hola");
    //     yield break;
    // }
    //
    // public void OpenCloseReward()
    // {
    //     lvlMaxText.SetActive(true);
    // }
    //
    // public void OnSelectReward(bool value)
    // {
    //     if (value) Player.instance.IncreaseDamage();
    //     else
    //     {
    //         rewardLife.DOSizeDelta(new Vector2(2000, rewardLife.sizeDelta.y), 1f);
    //         var child = rewardLife.transform.GetChild(0).GetComponent<RectTransform>();
    //         child.DOSizeDelta(new Vector2(1930, child.sizeDelta.y), 1f);
    //     }
    // }
}
