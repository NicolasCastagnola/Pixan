using UnityEngine;

public class MiguelStunTrigger : MonoBehaviour
{
    public BoxCollider coll;

    [SerializeField] MiguelFinalBoss migue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MiguelRockStunTrigger>()!=null)
            migue.StunBoss();
    }
}
