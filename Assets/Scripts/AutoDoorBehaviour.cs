using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AutoDoorBehaviour : MonoBehaviour
{
    [SerializeField] Animation anim;

    public AudioSource audioS;
    public AudioClip[] audioC;

    public void OpenDoor()
    {
        audioS.clip = audioC[0];
        audioS.Play();
        anim.Play();
    }
}
