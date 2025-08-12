using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAllColliders : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider is BoxCollider)
            {
                BoxCollider boxCollider = (BoxCollider)collider;

                Matrix4x4 cubeTransform = Matrix4x4.TRS(
                    boxCollider.transform.position,
                    boxCollider.transform.rotation,
                    boxCollider.transform.lossyScale
                );

                Gizmos.matrix = cubeTransform;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
            /*else if (collider is SphereCollider)
            {
                SphereCollider sphereCollider = (SphereCollider)collider;
                Gizmos.DrawWireSphere(sphereCollider.bounds.center, sphereCollider.radius);
            }
            else if (collider is CapsuleCollider)
            {
                CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
                Vector3 point1 = capsuleCollider.transform.position + capsuleCollider.center + Vector3.up * capsuleCollider.height * 0.5f;
                Vector3 point2 = capsuleCollider.transform.position + capsuleCollider.center - Vector3.up * capsuleCollider.height * 0.5f;
                float radius = capsuleCollider.radius;
                Gizmos.DrawWireSphere(point1, radius);
                Gizmos.DrawWireSphere(point2, radius);
                Gizmos.DrawWireCube(capsuleCollider.bounds.center, new Vector3(radius * 2, capsuleCollider.height, radius * 2));
            }*/
        }
    }
}