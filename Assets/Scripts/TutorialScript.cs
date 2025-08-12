using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] UIFader uIFader;
    [SerializeField] Transform gate;

    private int level = 0;

    private bool hasStart = false;

    private bool isChanging = false;

    private void Start()
    {
        if (PlayerPrefs.HasKey("TutorialNumber"))
        {
            if (PlayerPrefs.GetInt("TutorialNumber") == 1)
            {
                gate.DOScaleX(0, 0.5f);

                hasStart = false;
                return;
            }
        }

        StartCoroutine(WaitForIntroCR());
    }

    private IEnumerator WaitForIntroCR()
    {
        yield return new WaitForSeconds(5);

        uIFader.FadeIn(0.3f);

        hasStart = true;
    }

    private void MoveToNextLevel(string textValue)
    {
        if (isChanging) return;
        StartCoroutine(MoveToNextLevelCR(textValue));
    }

    private IEnumerator MoveToNextLevelCR(string textValue)
    {
        uIFader.FadeOut(0.3f);

        yield return new WaitForSeconds(3);

        uIFader.SetText(textValue);

        level++;

        if (level == 6)
        {
            PlayerPrefs.SetInt("TutorialNumber", 1);

            gate.DOScaleX(0, 0.5f);

            hasStart = false;
            yield break;
        }

        uIFader.FadeIn(0.3f);

        isChanging = false;
    }

    private void Update()
    {
        if (!hasStart) return;

        switch (level)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    MoveToNextLevel("Use left click mouse button to attack.");
                    isChanging = true;
                }
                break;
            case 1:
                if (Input.GetMouseButtonDown(0))
                {
                    MoveToNextLevel("Hold left click mouse button to make a charged attack.");
                    isChanging = true;
                }
                break;
            case 2:
                if (Input.GetMouseButton(0))
                {
                    MoveToNextLevel("Hold right click mouse button to block.");
                    isChanging = true;
                }
                break;
            case 3:
                if (Input.GetMouseButtonDown(1))
                {
                    MoveToNextLevel("Use Space bar to roll.");
                    isChanging = true;
                }
                break;
            case 4:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    MoveToNextLevel("Use Tab to open inventory. \n You can improve your skills sitting at bonfires.");
                    isChanging = true;
                }
                break;
            case 5:
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    MoveToNextLevel("");
                    isChanging = true;
                }
                break;

            default:
                return;
        }
    }
}
