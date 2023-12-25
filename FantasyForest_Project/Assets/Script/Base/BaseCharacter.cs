using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public abstract class BaseCharacter : MonoBehaviour
{
    //チーム状態
    public TEAM_COLOR team_color;//チームカラー

    //リスポーン管理クラス
    [SerializeField]
    private RespownManager respownManager;
    //リスポーン内部時間
    public int RespownTime = 0;

    //キャラクター基本部分
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

    //見えない武器のゲームオブジェクト
    private GameObject EquipWeponGameObject;

    //装備する武器
    public WEPON weponName;

    [SerializeField]
    private bool isActive;//生存状態かどうか

    public bool getActive()
    {
        return isActive; 
    }

    public void setActive(bool value)
    {
        isActive = value;
    }


    protected virtual void Start()
    {
        //コンポーネント取得
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();

        //指定の武器を装備させる
        //装備させるためのオブジェクトを探索
        //TODO：指定したキャラクターに対応した武器を装備させる

        //HPの設定
        max_hp = 100;
        current_hp = max_hp;
        isActive = true;
    }

    protected virtual void Update()
    {
        //キャラクターのHP情報をチェック
        if(current_hp <= 0)
        {
            //HPが0になったら各種判定に使っているアクティブ情報をFalse
            isActive = false;
            //リスポーン管理クラスへ登録
            respownManager.standRespownList.Add(gameObject);
            //対象となったキャラオブジェクトを非アクティブ化
            gameObject.SetActive(false);
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

    /// <summary>
    /// どの武器によってダメージを受けたか
    /// </summary>
    /// <param name="takeWepon"></param>
    public void WeponTakeDamege(WEPON takeWepon)
    {
        //受けた武器によってダメージを変化させる
        switch (takeWepon)
        {
            case WEPON.NONE:
                break;
            case WEPON.Sword:
                current_hp -= 2;
                break;
            case WEPON.Bow:
                current_hp -= 1;
                break;
            case WEPON.Arrow:
                current_hp -= 5;
                break;
        }
    }
}
