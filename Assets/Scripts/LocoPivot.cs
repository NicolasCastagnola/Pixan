using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LocoPivot : MonoBehaviour
{
    [SerializeField] GameObject npc;
    [SerializeField] ParticleSystem locoDespawn;
    [SerializeField] LocoHandler loco;

    bool hasInteracted;

    public void Interacted()
    {
        if (hasInteracted)
            return;

        hasInteracted = true;
        loco.CancelLoco(this);
        loco.addCounter();
        Destroy(GetComponent<NPC>());

        var particle = Instantiate(locoDespawn, transform.position, transform.rotation);
        particle.transform.localScale = 0.5f * Vector3.one;
        particle.transform.forward = transform.up;

        float newY = npc.transform.position.y - 1.5f;
        transform.DOScale(1, 1).OnComplete(()=>npc.transform.DOMoveY(newY, 3).OnComplete(() =>Destroy(npc)));
    }
}
