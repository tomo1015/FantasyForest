using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public abstract class BaseCharacter : MonoBehaviour
{
    //チーム状態
    public TEAM_COLOR team_color;//チームカラー

    //キャラクター基本部分
    [SerializeField]
    protected Rigidbody Rigidbody = null;

    //アニメーション
    private Animator Animator;
    //アニメーションで設定したフラグの名前
    private const string key_isRun = "isRun";
    private const string key_isAttack = "isAttack";
    private const string key_isDown = "isDown";

    //HP状態
    private int current_hp;//現在HP
    private int max_hp;//最大HP

    private bool isActive;//生存状態かどうか

    public bool getActive()
    {
        return isActive; 
    }


    protected virtual void Start()
    {
        //コンポーネント取得
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();

        //HPの設定
        max_hp = 100;
        current_hp = max_hp;
        isActive = true;
    }

    protected virtual void Update()
    {
        //HPの処理
        if(current_hp <= 0)
        {
            isActive = false; 
            //TODO：リスポーン管理クラスへ登録する
        }
    }

    /// <summary>
    /// キャラクターアニメーション再生
    /// </summary>
    protected virtual void PlayAnimation(ANIMATION_STATE playState)
    {
        switch (playState)
        {
            case ANIMATION_STATE.IDLE:
                //待機状態
                break;
            case ANIMATION_STATE.RUN:
                //移動状態
                Animator.SetBool(key_isRun, true);
                break;
            case ANIMATION_STATE.ATTACK:
                //攻撃状態
                //Animator.SetBool(key_isAttack, true);
                break;
            case ANIMATION_STATE.DOWN:
                //ダウン状態
                //Animator.SetBool(key_isDown, true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// キャラクターアニメーション停止
    /// </summary>
    protected virtual void StopAnimation(ANIMATION_STATE stopState)
    {
        switch (stopState)
        {
            case ANIMATION_STATE.IDLE:
                //待機状態
                break;
            case ANIMATION_STATE.RUN:
                //移動状態を停止
                Animator.SetBool(key_isRun, false);
                break;
            case ANIMATION_STATE.ATTACK:
                //攻撃状態を停止
                //Animator.SetBool(key_isAttack, false);
                break;
            case ANIMATION_STATE.DOWN:
                //ダウン状態を停止
                //Animator.SetBool(key_isDown, false);
                break;
            default:
                break;
        }
    }
}
