using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneColorChanger : MonoBehaviour
{
    [SerializeField] ChangeSceneColor inside, outside;

    [SerializeField] Transform pivotSeparator;

    private void Awake()
    {
        StartCoroutine(WaitForChangeColor());
    }
    IEnumerator WaitForChangeColor()
    {
        yield return new WaitUntil(() => Player.Instance!=null);
        yield return new WaitUntil(() => Player.Instance.transform.position != Vector3.zero);

        ChangeColorBasedOnPos(Player.Instance.transform.position);

        yield return new WaitUntil(()=>Canvas_Playing.Instance != null);
        Canvas_Playing.Instance.BonfireMenu.OnWarp+= x => ChangeColorBasedOnPos(x.transform.position);//color based in next bonfire
    }
    public void ChangeColorBasedOnPos(Vector3 pos)
    {
        if (pos.x >= pivotSeparator.position.x && pos.y >= pivotSeparator.position.y)
            outside.InstantChangeColor();
        else
            inside.InstantChangeColor();
    }
}
