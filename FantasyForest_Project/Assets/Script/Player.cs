using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーキャラクターの制御を行うクラス
/// </summary>
public class Player : BaseCharacter
{
    /// <summary>
    /// 回転の補間係数
    /// </summary>
    private const float ROTATION_SMOOTHING = 0.2f;

    /// <summary>
    /// 最小移動判定距離
    /// </summary>
    private const float MIN_MOVE_DISTANCE = 0.001f;

    /// <summary>
    /// 移動速度の係数
    /// </summary>
    private const float MOVEMENT_SPEED_FACTOR = 0.5f;

    /// <summary>
    /// 移動方向
    /// </summary>
    private Vector3 moveDirection;

    /// <summary>
    /// 移動量
    /// </summary>
    private Vector3 moveVelocity;

    /// <summary>
    /// 直前のフレームにおける位置
    /// </summary>
    private Vector3 latestPosition;

    // Start is called before the first frame update
    protected override void Start()
    {
        //ベースクラス処理
        base.Start();
        latestPosition = transform.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        HandleMovement();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        UpdateRotation();
    }

    /// <summary>
    /// 移動入力の処理と移動量の計算を行う
    /// </summary>
    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Vertical");
        float moveZ = Input.GetAxisRaw("Horizontal");
        moveDirection = new Vector3(moveX, 0, moveZ);
        moveDirection.Normalize();//正規化（斜めの距離が長くなるのを防ぐ）

        moveVelocity = moveDirection * GetCharacterSpeed() * MOVEMENT_SPEED_FACTOR;
    }

    /// <summary>
    /// 計算された移動量を物理演算に適用する
    /// </summary>
    private void ApplyMovement()
    {
        Rigidbody.linearVelocity = new Vector3(moveVelocity.x, Rigidbody.linearVelocity.y, moveVelocity.z);
    }

    /// <summary>
    /// 移動方向に応じてキャラクターの回転を更新する
    /// </summary>
    private void UpdateRotation()
    {
        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 previousPosition = new Vector3(latestPosition.x, 0, latestPosition.z);
        Vector3 diffDistance = currentPosition - previousPosition;
        latestPosition = transform.position;

        if (ShouldUpdateRotation(diffDistance))
        {
            Quaternion targetRotation = Quaternion.LookRotation(diffDistance);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATION_SMOOTHING);
        }
    }

    /// <summary>
    /// 回転を更新すべきかどうかを判定する
    /// </summary>
    private bool ShouldUpdateRotation(Vector3 diffDistance)
    {
        return (Mathf.Abs(diffDistance.x) > MIN_MOVE_DISTANCE || 
                Mathf.Abs(diffDistance.z) > MIN_MOVE_DISTANCE) && 
               moveDirection != Vector3.zero;
    }
}

