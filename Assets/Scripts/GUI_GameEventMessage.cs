using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUI_GameEventMessage : MonoBehaviour
{
    [SerializeField] private AnimatedContainer animatedContainer;
    [SerializeField] private TMP_Text _messageDisplay;

    public void ShowMessage(string message, Color color, float duration = 0f) => StartCoroutine(WaitForShowMessage(message, color, duration));
    private IEnumerator WaitForShowMessage(string message, Color color, float duration = 0f)
    {
        _messageDisplay.text = message;
        _messageDisplay.color = color;
        
        animatedContainer.Open();
        
        yield return new WaitForSeconds(duration);
        
        animatedContainer.Close();
    }

}
