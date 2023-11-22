using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseCharacter
{
    [SerializeField] private float speed = 0;//移動速度

    private Vector3 moveDirection;//移動方向
    private Vector3 moveVelocity;//移動量
    private Vector3 latestPosition;//直前のフレームにおける位置

    // Start is called before the first frame update
    protected override void Start()
    {
        //ベースクラス処理
        base.Start();

        //プレイヤー初期設定
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //移動処理
        Move();
    }

    private void FixedUpdate()
    {
        //移動処理で求めた結果をもとにキャラクターを移動させる
        Rigidbody.velocity = new Vector3(moveVelocity.x, Rigidbody.velocity.y, moveVelocity.z);

        //移動する方向に対してキャラクターを回転させる
        MoveRotation();
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        //キー入力
        float moveX = Input.GetAxisRaw("Vertical"); 
        float moveZ = Input.GetAxisRaw("Horizontal");

        moveDirection = new Vector3(moveX, 0, moveZ);
        moveDirection.Normalize();//正規化（斜めの距離が長くなるのを防ぐ）

        moveVelocity = moveDirection * speed;
    }

    /// <summary>
    /// 移動する方向にキャラクターを回転させる
    /// </summary>
    private void MoveRotation()
    {
        Vector3 diffDistance = new Vector3(transform.position.x,0,transform.position.z) - new Vector3(latestPosition.x,0, latestPosition.z);
        latestPosition = transform.position;

        if (Mathf.Abs(diffDistance.x) > 0.001f || Mathf.Abs(diffDistance.z) > 0.001f)
        {
            if(moveDirection == Vector3.zero) { return; }

            Quaternion rotation = Quaternion.LookRotation(diffDistance);
            rotation = Quaternion.Slerp(Rigidbody.transform.rotation, rotation, 0.2f);
            
            transform.rotation = rotation;
        }
    }
}
