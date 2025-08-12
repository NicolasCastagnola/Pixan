
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CureComponent : MonoBehaviour
{
    public KeyCode cureKey = KeyCode.E;
    public int bottles = 3;

    private bool isInRange = false;
    bool used = false;

    [SerializeField] private UnityEvent onTakeCure;

    [SerializeField] HealthHeadBehaviour headBehaviour;

    private void Start()
    {
        //used != 0, unique ID by add position(x+y+z)
        used = PlayerPrefs.GetInt("CureComponent" + transform.position.x + transform.position.y + transform.position.z, 0) != 0;
        if(used==true)
            onTakeCure?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!used && other.CompareTag("Player"))
        {
            isInRange = true;
            Canvas_Playing.Instance.UIFader.ShowText("Press " + cureKey.ToString() + " to grab Estus of Tepeu", 0.5f, 3f, 0.5f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isInRange && other.CompareTag("Player") && Input.GetKeyDown(cureKey)&&!used)
        {
            used = true;
            onTakeCure?.Invoke();
            Player.Instance.UpdateHealItem(bottles,true);
            PlayerPrefs.SetInt("CureComponent" + transform.position.x + transform.position.y + transform.position.z, 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!used && other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
    public void FinishAnim()
    {
        headBehaviour.TurnEyesOff();
        Destroy(gameObject);
    }

    //private void OnGUI()
    //{
    //    if (isInRange)
    //    {
    //        //GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 50, 100, 30), "Presiona 'E' para curarte");
            
    //    }
    //}
}
