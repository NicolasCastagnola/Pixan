using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BonfireMenu : MonoBehaviour
{
    public Action<Bonfire> OnWarp;
    
    private readonly List<BonfireSelectable> _bonfireSelectables = new List<BonfireSelectable>();
    
    private BonfireSelectable currentSelected;

    private Bonfire bonfireToTravelTo;
    
    [SerializeField] private AnimatedContainer mainAnimatedContainer;
    [SerializeField] private BonfireSelectable bonfireSelectablePrefab;
    [SerializeField] private Transform selectableParent;
    [SerializeField] private RawImage fogImage;

    private void Awake()
    => mainAnimatedContainer.OnOpeningStart += OpeningStart;
    private void Start()
    => OnWarp += 
        x => PlayerPrefs.SetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".LastRestedBonfire"
            , Mathf.Max(BonfireManager.Instance.LevelBonfires.IndexOf(x), 0));

    private void OnDestroy() => mainAnimatedContainer.OnOpeningStart -= OpeningStart;
    private void OpeningStart()
    {
        Color startColor = fogImage.color;
        startColor.a = 0.8f;
        Color newColor = fogImage.color;
        newColor.a = 0f;

        fogImage.color = startColor;

        DOTween.To(
            () => startColor,
            x => fogImage.color = x,
            newColor,
            1.7f
        ).SetEase(Ease.InOutSine);

        //PopulateAvailableBonfireList
        if (_bonfireSelectables.Count > 0) 
        {
            foreach (var selectable in _bonfireSelectables)
            {
                Destroy(selectable.gameObject);
            }
            
            _bonfireSelectables.Clear();
        }
        
        for (var i = 0; i < BonfireManager.Instance.LevelBonfires.Count; i++)
        {
            bool isUnlocked = BonfireManager.Instance.LevelBonfires[i].BonfireUnlocked;
            
            var obj = Instantiate(bonfireSelectablePrefab, selectableParent);
               
            obj.Initialize(i, isUnlocked,this, BonfireManager.Instance.LevelBonfires[i]);
            
            _bonfireSelectables.Add(obj);
        }
    }
    public void Warp() => OnWarp?.Invoke(bonfireToTravelTo);
    public void SetCurrentSelected(Bonfire bonfire) => bonfireToTravelTo = bonfire;
    public void Show()
    {
        if (bonfireToTravelTo == null)
        {
            bonfireToTravelTo = BonfireManager.Instance.LevelBonfires[0];
        }
        
        mainAnimatedContainer.Open();
    }
    public void Hide() => mainAnimatedContainer.Close();
}
