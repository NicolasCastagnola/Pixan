using System;
using UnityEngine;
public class DeathBarrier : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        print(other.gameObject.TryGetComponent<HealthComponent>(out var mplayer));

        if (other.gameObject.TryGetComponent<HealthComponent>(out var component))
        {
            component.InstantKill();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out var player))
        {
            player.SetFallDeathCamera(new GameObject("EmptyObject").AddComponent<TransformSetter>().SetPosition(other.transform).transform);
        }
    }
}

public class TransformSetter : MonoBehaviour
{
    public TransformSetter SetPosition(Transform otherTransform)
    {
        transform.position = otherTransform.position;
        
        return this;
    }
    public TransformSetter SetRotation(Vector3 eulerAngles)
    {
        transform.eulerAngles = eulerAngles;
        
        return this;
    }

}