using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPathPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
