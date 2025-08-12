using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryLevelBar : MonoBehaviour
{
    private static Image levelBar;

    private void Awake()
    {
        levelBar = GetComponent<Image>();
    }

    public static void UpdateExperienceBar(float ammount)
    {
        levelBar.fillAmount = ammount;
    }
}
