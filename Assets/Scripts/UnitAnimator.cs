using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.onStartMoving += MoveAction_OnStartMoving;
            moveAction.onStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent(out ShootAction shootAction))
        {
            shootAction.onShoot += ShootAction_OnShoot;
        }
    }

    public void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        _animator.SetBool("isRunning", true);
    }

    public void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        _animator.SetBool("isRunning", false);
    }

    public void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        _animator.SetTrigger("shoot");

        Transform bulletProjectileTransform =
            Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();


        var targetPosition = e.targetUnit.GetWorldPosition();

        targetPosition.y = shootPointTransform.position.y;
        
        bulletProjectile.Setup(targetPosition);
    }
}