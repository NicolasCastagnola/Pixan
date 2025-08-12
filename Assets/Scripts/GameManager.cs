using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static bool disableLogs = true;
    public static GameManager instance {
        get {
            if (_instance == null){
                _instance = FindObjectOfType<GameManager>();
                //DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    #region Awake
    void Awake(){
        if (_instance == null){
            _instance = this;
            //DontDestroyOnLoad(this);
        }
        else {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }
    #endregion

    private void Start()
    {
        LockCursor(true);

        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;
    }


    IEnumerator _WaitForXSeconds(float time, Action function, bool waitBefore = true){
        if(waitBefore) yield return new WaitForSeconds(time);
        function?.Invoke();
        if(!waitBefore) yield return new WaitForSeconds(time);
    }

    public void LockCursor(bool value)=>Cursor.lockState = value?CursorLockMode.Locked: CursorLockMode.Confined;

    public void WaitForXSeconds(float time, Action function, bool waitBefore = true)
    =>StartCoroutine(_WaitForXSeconds(time, function, waitBefore));

    public static void ShowLog(string message)
    {
        if (disableLogs) return;
        Debug.Log(message);
    }
}
