using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, 0.5f);
    }
}
