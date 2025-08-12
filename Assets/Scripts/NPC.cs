using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class NPC : Interactable
{
    [ShowInInspector, ReadOnly] private bool _alreadyInteracted;
    [ShowInInspector, ReadOnly] private string SaveRootKey => $"NPC/{Name}";
    [ShowInInspector, ReadOnly] public string Name => gameObject.name;

    [SerializeField] protected Dialogue Dialogue;

    public void Initialize()
    {
        GameManager.ShowLog(SaveRootKey+PlayerPrefs.GetInt(SaveRootKey));
        _alreadyInteracted = PlayerPrefs.GetInt(SaveRootKey) != 0;
        if (_alreadyInteracted)
            Dialogue.OnCompleteDialogue?.Invoke();
    }
    public void SetAlreadyInteracted(bool value)
    {
        PlayerPrefs.SetInt(SaveRootKey, value ? 1 : 0);
        
        _alreadyInteracted = value;
    }

    public override void OnInteract()
    {
        if (Canvas_Playing.Instance.DialogueController.ActiveDialogue) return;

        Canvas_Playing.Instance.HideInteractDisplay();
        Canvas_Playing.Instance.DialogueController.StartDialogue(Dialogue, _alreadyInteracted);
    }
    public override void OnExitOnReachableRadius()
    {
        Canvas_Playing.Instance.DialogueController.CloseDialogue();
    }
}
