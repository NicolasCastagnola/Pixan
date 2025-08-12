using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenOrb : MonoBehaviour
{
    string Name { get => gameObject.name + (transform.position.x + transform.position.y + transform.position.z); }
    [SerializeField] GameObject orb;

    private void Start()
    {
        if (PlayerPrefs.GetInt(Name, 0) == 1)
            Finish();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("Player")) Interact();
    }
    void Interact()
    {
        Canvas_Playing.Instance.OnGetGreenOrb("+1 \n Orb Point");
        PlayerPrefs.SetInt(Name, 1);

        Finish();
    }
    void Finish()
    {
        orb.SetActive(false);

        Destroy(this);
    }
}
