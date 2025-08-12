using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicIngame : MonoBehaviour
{
    [SerializeField] AudioClip battle;
    bool cantuse;

    public void OnGetPosition()
    {
        if (cantuse) return;

        GetComponent<AudioSource>().DOFade(0, 0.5f).OnComplete(() =>
        {
            cantuse = true;
            GetComponent<AudioSource>().clip = battle;
            GetComponent<AudioSource>().DOFade(0.005f, 0.5f);
            GetComponent<AudioSource>().Play();
        });
    }
}
