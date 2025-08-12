using UnityEngine;
using Audio;

public class MiguelRockStunTrigger : MonoBehaviour
{
    [SerializeField] AudioConfigurationData hitSound;
    [SerializeField] float initialVelocity;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (Canvas_Pause.IsPlaying)
        {
            rb.velocity = new Vector3(0, -initialVelocity, 0);
            rb.useGravity = true;
            rb.isKinematic = false;
        }
        else
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MigueEnragedDamageSystem>())
            return;

        if(other.gameObject.layer==7/*Ground*/|| other.gameObject.layer == 10/*Enemy*/)
        {
            hitSound.Play2D();
            Destroy(gameObject);
        }
    }
}
