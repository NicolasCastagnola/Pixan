using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [ShowInInspector, ReadOnly] private Queue<DialogueEventHandler> sentences = new Queue<DialogueEventHandler>();
    [ShowInInspector, ReadOnly] private Dialogue CurrentDialogue;

    private bool _shouldUseTypedSequence;
    public bool ActiveDialogue { get; private set; }

    private AudioConfigurationData currentDialogueClip;
    
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;

    public void StartDialogue(Dialogue dialogue, bool alreadyInteracted = false, bool shouldUseTypedSequence = false)
    {
        if (ActiveDialogue) return;
        
        dialogueText.text = string.Empty;

        _shouldUseTypedSequence = shouldUseTypedSequence;

        CurrentDialogue = dialogue;
        
        Canvas_Playing.Instance.DialogueAnimatedContainer.Open();

        ActiveDialogue = true;
        
        sentences.Clear();

        if (alreadyInteracted&& dialogue.AlreadyInteractedSentences.Length > 0)
        {
            foreach (var sentence in dialogue.AlreadyInteractedSentences)
            {
                sentences.Enqueue(sentence);
            }
        }
        else
        {
            foreach (var sentence in dialogue.Sentences)
            {
                sentences.Enqueue(sentence);
            }
        }

        if (_shouldUseTypedSequence)
        {
            StartCoroutine(TypeSentence());
        }
        else
        {
            DisplaySentence();
        }
    }

    private void DisplaySentence()
    {
        if (sentences.Count == 0)
        {
            CurrentDialogue.NPC.SetAlreadyInteracted(true);
            CurrentDialogue.OnCompleteDialogue?.Invoke();//On Complete Lecture Execute Action
            CloseDialogue();
            return;
        }
        var current = sentences.Peek();

        // if (current.shouldOpenAQuestionWindow)
        // {
        //     Canvas_Playing.Instance.ShowQuestionWindow(current.Question, current.AnswerOne, current.AnswerTwo);
        //     return;
        // }

        if (currentDialogueClip != null)
        {
            currentDialogueClip.Stop(CurrentDialogue.NPC.name);
            currentDialogueClip = null;
        }

        if (current.TextAudioPair.Voice != null)
        {
            currentDialogueClip = current.TextAudioPair.Voice;
            currentDialogueClip.Play2D(CurrentDialogue.NPC.name);    
        }
        
        
        dialogueText.text = current.TextAudioPair.Text;
        
        if (current.value != null)
        {
            InventorySystem.Instance.Add(current.value);
        }
    }

    private IEnumerator TypeSentence()
    {
        var current = sentences.Peek();
        
        foreach (char letter in current.TextAudioPair.Text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void Update()
    {

        if (CurrentDialogue == null || sentences.Count == 0 || currentDialogueClip == null) return;
  
        if (Input.GetKeyDown(KeyCode.E))
        {
            DisplayNextSentence();
        }
    }

    private void EndDialogue()
    {
        CloseDialogue();
        sentences.Clear();
        CurrentDialogue.NPC.SetAlreadyInteracted(true);
    }

    private void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        } 
        
        sentences.Dequeue();

        dialogueText.text = "";
        
        if (_shouldUseTypedSequence)
        {
            StartCoroutine(TypeSentence());
        }
        else
        {
            DisplaySentence();
        }
    }
    public void CloseDialogue()
    {
        Canvas_Playing.Instance.DialogueAnimatedContainer.Close();

        StartCoroutine(WaitForInteractAgain());

        if (currentDialogueClip != null)
        {
            currentDialogueClip.Stop(CurrentDialogue.NPC.name);    
        }

        CurrentDialogue = null;
        currentDialogueClip = null;
    }

    private IEnumerator WaitForInteractAgain()
    {
        yield return new WaitForSeconds(1f);
        
        ActiveDialogue = false;
    }
}

[Serializable]
public class Dialogue
{
    public NPC NPC;

    public DialogueEventHandler[] Sentences;
    
    public DialogueEventHandler[] AlreadyInteractedSentences;

    public UnityEngine.Events.UnityEvent OnCompleteDialogue;
}

[Serializable]
public class DialogueEventHandler
{
    public InventoryItemData value;
    public TextAudioPair TextAudioPair;

    public bool shouldOpenAQuestionWindow;
    [ShowIf(nameof(shouldOpenAQuestionWindow))]public string Question;
    [ShowIf(nameof(shouldOpenAQuestionWindow))]public string AnswerOne;
    [ShowIf(nameof(shouldOpenAQuestionWindow))]public string AnswerTwo;
}

[Serializable]
public class TextAudioPair
{
    [TextArea(5, 10)] public string Text;
    public AudioConfigurationData Voice;
}

