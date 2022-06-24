using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld _instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        _instance = this;
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
    
}