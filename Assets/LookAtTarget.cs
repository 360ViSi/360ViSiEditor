using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    void LateUpdate() => transform.LookAt(target);
}
