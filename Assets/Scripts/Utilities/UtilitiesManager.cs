using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class UtilitiesManager : MonoBehaviour
{
    public static UtilitiesManager Instance { get; private set; }

    [SerializeField] private UnityEvent onStart;
    [SerializeField] private UnityEvent onStartEnd;
    [SerializeField] private UnityEvent onEnd;

    private List<Vase> vases = new List<Vase>();
    public GameObject key;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        onStart?.Invoke();

        /*foreach (var item in FindObjectsOfType<Vase>().ToList())
        {
            vases.Add(item);
        }*/

        //vases.Skip(Random.Range(0, vases.Count - 1)).First().HasKey = true;
    }

    public void EndGame()
    {
        onStartEnd?.Invoke();
    }

    public void EndGameReset()
    {
        onEnd?.Invoke();
    }
}
