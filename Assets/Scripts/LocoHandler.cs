using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocoHandler : MonoBehaviour
{
    [SerializeField] AltarHandler altarHandler;
    [SerializeField] List<LocoPivot> locoPivots = new List<LocoPivot>();
    [SerializeField] KeyBehaviour soulOfMadness;
    [SerializeField] int interactedAmount;

    private void Awake()
    {
        StartLocos();
    }

    public void CancelLoco(LocoPivot loco)
    {
        locoPivots.Remove(loco);
        PlayerPrefs.SetInt("Interactedamount", interactedAmount);

        if (interactedAmount >= 5)
        {
            GameManager.ShowLog("Alma de los locos");
            locoPivots.Clear();
            if (PlayerPrefs.GetInt("Soulofmadness",0) == 0)
            {
                soulOfMadness.gameObject.SetActive(true);
                Canvas_Playing.Instance.DoInventoryMessage("Soul of madness added to the altar");
            }

        }
    }

    public void StartLocos()
    {
        if(interactedAmount >= 5)
        {
            locoPivots.ForEach(loco => { loco.Interacted(); });
            locoPivots.Clear();
            Destroy(soulOfMadness.gameObject);
        }
    }
    
    public void addCounter()
    {
        interactedAmount++;
    }
}
