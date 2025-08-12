using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChargeBulletAttack : MonoBehaviour
{
    public float speed = 25f;

    public int lifeTime = 60;
    int currentLifeTime;

    public Transform spriteTransform;
    public BoxCollider[] colls;

    private void Start()
    {
        StartCoroutine(update());
        spriteTransform.DOScale(0.5f, 0.1f)
            .OnComplete(()=> {
                foreach (var coll in colls)
                    coll.enabled = true;
                
                spriteTransform.DOScale(1f, 0.3f);
        });
    }

    public IEnumerator update()
    {

        while (true)
        {
            transform.position+=transform.forward * (speed * Time.deltaTime);

            currentLifeTime++;
            if (currentLifeTime > lifeTime)
                break;

            yield return new WaitForEndOfFrame();
        }
        transform.DOScale(0, 0.2f)
            .OnUpdate(() => transform.position += transform.forward * (speed * Time.deltaTime))
            .OnComplete(()=> Destroy(gameObject));
    }
}
