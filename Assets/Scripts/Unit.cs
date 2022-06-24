using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Vector3 _targetPosition;
    float stoppingDistance = .1f;
    [SerializeField] private Animator _unitAnimator;

    private void Awake()
    {
        _targetPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(_targetPosition, transform.position) > stoppingDistance)
        {
            Vector3 moveDirection = (_targetPosition - transform.position).normalized;
            float moveSpeed = 5f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            //change direction over time instead of instant
            float rotateSpeed = 5f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            _unitAnimator.SetBool("isRunning", true);
        }
        else
        {
            _unitAnimator.SetBool("isRunning", false);
        }
    }

    public void Move(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }
}