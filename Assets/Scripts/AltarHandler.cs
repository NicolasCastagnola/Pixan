using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AltarHandler : MonoBehaviour
{
    bool madnessBool, highnessBool, sacrificeBool;
    [SerializeField] DoorBehaviour doorBehaviour;
    [SerializeField] GameObject SoulMadness, SoulHighness, SoulSacrifice;

    //called by locoHandler

    private void Start()
    {
        if(PlayerPrefs.GetInt("Souls/madness", 0) == 1)
        {
            madnessBool = true;
            Destroy(SoulMadness);
        }
        if (PlayerPrefs.GetInt("Souls/highness", 0) == 1)
        {
            highnessBool = true;
            Destroy(SoulHighness);

        }
        if (PlayerPrefs.GetInt("Souls/sacrifice", 0) == 1)
        {
            sacrificeBool = true;
            Destroy(SoulSacrifice);

        }

        if (madnessBool && highnessBool && sacrificeBool)
        {
            UnlockDoor();
        }
    }

    public void altarDrop(string key)
    {
        if(key == "madness" && madnessBool == false)
        {
            madnessBool = true;
            PlayerPrefs.SetInt($"Souls/{key}", 1);

            if(Canvas_Inventory.Instance!=null)
                Canvas_Inventory.Instance.HideSlotItem(3);
        }
        if(key == "highness" && highnessBool == false)
        {
            highnessBool = true;
            PlayerPrefs.SetInt($"Souls/{key}", 1);

            if (Canvas_Inventory.Instance != null)
                Canvas_Inventory.Instance.HideSlotItem(4);

        }
        if (key == "sacrifice" && sacrificeBool == false)
        {
            sacrificeBool = true;
            PlayerPrefs.SetInt($"Souls/{key}", 1);

            if (Canvas_Inventory.Instance != null)
                Canvas_Inventory.Instance.HideSlotItem(5);
        }

        if (madnessBool && highnessBool && sacrificeBool)
        {
            UnlockDoor();
        }
    }

    public void UnlockDoor()
    {
        doorBehaviour.OpenDoor();
    }
}
