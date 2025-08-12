// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.Events;
// using System;
// using TMPro;
//
// public class InventoryStatsMenu : MonoBehaviour
// {
//     public static InventoryStatsMenu Instance { get; private set; }
//
//     GameObject canvas;
//
//     public static bool isOpen;
//
//     [SerializeField] UnityEvent onOpen;
//     [SerializeField] UnityEvent onClose;
//
//     private Color originalColor;
//
//     public int PoinsStats => pointsStats;
//     private int pointsStats = 5;
//     private int pointsUsed;
//     public TextMeshProUGUI pointsText;
//
//     public Action<StatType> onUpgradeStat;
//
//     public Action onRefreshPoints;
//
//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else
//         {
//             GameManager.ShowLog("Hay mas de un InventoryScript");
//             Destroy(this);
//         }
//     }
//
//     void Start()
//     {
//         canvas = transform.GetChild(0).gameObject;
//         originalColor = pointsText.color;
//
//         UpdateText();
//     }
//
//     void UpdateText()
//     {
//         pointsText.text = pointsStats.ToString();
//     }
//
//     public void AddUsedPoint()
//     {
//         pointsUsed += 1;
//     }
//
//     public void AddPoints()
//     {
//         pointsStats++;
//         UpdateText();
//     }
//
//     public void RemovePoints()
//     {
//         pointsStats--;
//
//         if (pointsStats <= 0) pointsStats = 0;
//
//         UpdateText();
//     }
//
//     public void OpenCloseInventary(InputAction.CallbackContext ctx)
//     {
//         if (ctx.started)
//         {
//             Open();
//         }
//     }
//
//     public void Open()
//     {
//         if (isOpen)
//         {
//             onClose?.Invoke();
//             canvas.SetActive(false);
//             Time.timeScale = 1;
//             GameManager.instance.LockCursor(true);
//         }
//         else
//         {
//             onOpen?.Invoke();
//             canvas.SetActive(true);
//             Time.timeScale = 0;
//             GameManager.instance.LockCursor(false);
//         }
//
//         isOpen = !isOpen;
//     }
//
//     public void RefreshPoints()
//     {
//         foreach (var item in GetComponentsInChildren<StatSlot>())
//         {
//             item.ClearSlots();
//         }
//
//         onRefreshPoints?.Invoke();
//         pointsStats += pointsUsed;
//         pointsUsed = 0;
//         UpdateText();
//     }
//
//     public void CantUpgradeStat()
//     {
//         StopAllCoroutines();
//         StartCoroutine(CantUpgradeCO());
//     }
//
//     private IEnumerator CantUpgradeCO()
//     {
//         int counter = 5;
//         while (counter > 0)
//         {
//             pointsText.color = Color.red;
//             yield return null;
//             pointsText.color = originalColor;
//             counter--;
//             yield return null;
//         }
//     }
// }
