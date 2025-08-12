using UnityEngine;
using System.Collections;

public class NPC_Spirit : NPC
{
    [SerializeField] Renderer[] meshes;
    [SerializeField] ParticleSystem[] particles;

    [SerializeField] bool active;

    public override void OnInteract()
    {
        if(!active)
            base.OnInteract();
    }
    public override void OnEnterReachableRadius(InteractComponent component)
    {
        if (!active)
            base.OnEnterReachableRadius(component);
    }
    public override void OnExitOnReachableRadius()
    {
        if(!active)
            base.OnExitOnReachableRadius();
    }

    public void FadeOut() { if (!active && this.isActiveAndEnabled) StartCoroutine(_FadeOut()); }
    public IEnumerator _FadeOut()
    {
        active = true;
        Canvas_Playing.Instance.DialogueController.CloseDialogue();

        GetComponent<Collider>().enabled = false;

        var colorA = 0.8f;
        while (colorA > 0)
        {
            colorA -= 0.03f;

            foreach (var mesh in meshes)
                mesh.material.SetFloat("_Alpha", colorA);//fade alpha

            yield return new WaitForSeconds(0.1f);
        }

        foreach (var particle in particles)//stop particles
            particle.Stop(true);

        yield return new WaitForSeconds(2f);

        transform.root.gameObject.SetActive(false);
    }
}
