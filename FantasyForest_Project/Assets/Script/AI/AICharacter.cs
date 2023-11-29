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
    [SerializeField]
    private AI_STATUS ai_status;

    //塔の管理クラス

    public TowerManager towerManager;
    private List<GameObject> LotteryTowerList;

    [SerializeField]
    private Vector3 CaptureLocation;



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
            case AI_STATUS.NONE:
                //初期状態の時は一旦抽選処理にする
                ai_status = AI_STATUS.SEARCH;
                break;
            case AI_STATUS.SEARCH:
                TowerSearch();
                //探索処理実行
                break;
            case AI_STATUS.CAPTURE:
                //占領処理実行
                break;
            case AI_STATUS.MOVE:
                //移動処理実行
                Move();
                break;
            case AI_STATUS.ATTACK:
                //攻撃処理実行
                break;
            default: 
                break; 
        }
    }

    /// <summary>
    /// タワーの探索処理
    /// </summary>
    private void TowerSearch()
    {
        //タワーのカウント数
        int blueTowerCount = towerManager.getBlueTowerCount();
        int redTowerCount = towerManager.getRedTowerCount();
        int natureTowerCount = towerManager.getNatureTowerCount();

        //タワーオブジェクトそのもののデータ
        List<GameObject> blueTowerList = towerManager.getBlueTowerList();
        List<GameObject> redTowerList = towerManager.getRedTowerList();
        List<GameObject> natureTowerList = towerManager.getNatureTowerList();
        //キャラクターそのもの現在位置
        Vector3 nowPosition = agent.transform.position;

        //タワー全体の管理クラスの情報
        //キャラクターの所属軍が青軍だった場合
        if (team_color == Constants.TEAM_COLOR.BLUE)
        {
            if(natureTowerCount > 0)
            {
                //中立状態のタワーが一つでもある場合は
                //中立タワーリストの一番上にあるタワーを目指すようにする
                CaptureLocation = natureTowerList[0].gameObject.transform.position;
            }
            else if(blueTowerCount < redTowerCount)
            {
                //敵軍のタワーが自軍タワーより多いのであれば
                //敵軍として占領されているタワーのうちから自分との距離が一番遠いものを目指す
            }
            else
            {
                //自軍タワーが多い場合は自軍占領タワーリストから
                //現在の位置に一番近いものをターゲットとする（タワーの防衛）

            }
        }

        //移動するべきタワーが見つかったので、AIのステートを移動に変更
        ai_status = AI_STATUS.MOVE;
    }


    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        //基本的にはタワーへ向かう
        //タワーへ向かう前提の位置
        agent.SetDestination(CaptureLocation);
        //移動速度設定
        //agent.speed = 50;

        //途中で敵と遭遇した場合（コライダーに入った場合）はターゲットを変更する。
        //ターゲットに近づいた場合のみ攻撃ステートへ変更

        //タワーに近づいた場合（占領範囲内）は
        //移動を停止し、占領ステートへ変更
        Vector3 TowerDiffPosition = CaptureLocation - agent.transform.position;
        var vector = Vector3.Magnitude(TowerDiffPosition);
        if(Vector3.Magnitude(TowerDiffPosition) < 50)
        {
            //タワー占領範囲内
            agent.speed = 0.0f;
            agent.acceleration = 0.0f;

            ai_status = AI_STATUS.CAPTURE;
        }
    }
}
