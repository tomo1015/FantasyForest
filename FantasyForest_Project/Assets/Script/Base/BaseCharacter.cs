using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    public enum TEAM_COLOR{
        NONE,
        TEAM_BLUE,
        TEAM_RED,
    }

    public TEAM_COLOR team_color;//チームカラー
    private int current_hp;//現在HP
    private int max_hp;//最大HP

    private bool isActive;//生存状態かどうか

    public bool getActive()
    {
        return isActive; 
    }


    protected virtual void Start()
    {
        Debug.Log("基本クラスのStart");
        //HPの設定
        max_hp = 100;
        current_hp = max_hp;
        isActive = true;
    }

    protected virtual void Update()
    {
        Debug.Log("基本クラスのUpdate");
        //HPの処理
        if(current_hp <= 0)
        {
            isActive = false; 
            //TODO：リスポーン管理クラスへ登録する
        }
    }
}
