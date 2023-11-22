using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacter : BaseCharacter
{
    public enum AI_STATUS
    {
        NONE,//何もしていない
        SEARCH,//探索
        CAPTURE,//占領
        MOVE,//移動
        ATTACK,//攻撃
    }

    //ナビメッシュエージェント
    private NavMeshAgent agent;


    //AIの状態管理
    private AI_STATUS ai_status;



    protected override void Start()
    {
        //基本クラスの処理
        base.Start();


        //このクラスだけの処理
        agent = GetComponent<NavMeshAgent>();//ナビメッシュエージェント取得
        ai_status = AI_STATUS.NONE;//AIの状態を一旦初期状態へ変更
    }

    protected override void Update()
    {
        //HP管理処理
        base.Update();

        //AIステータス管理
        bool is_active = base.getActive();

        //生存していないなら処理しない
        if (!is_active) { return; }

        switch (ai_status)
        {
            case AI_STATUS.SEARCH:
                //探索処理実行
                break;
            case AI_STATUS.CAPTURE:
                //占領処理実行
                break;
            case AI_STATUS.MOVE:
                //移動処理実行
                break;
            case AI_STATUS.ATTACK:
                //攻撃処理実行
                break;
            default: 
                break; 
        }
    }
}
