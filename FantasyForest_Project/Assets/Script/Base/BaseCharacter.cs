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
        Rigidbody = GetComponent<Rigidbody>();

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
}
