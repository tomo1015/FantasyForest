using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;
using System;
using Unity.VisualScripting;

public class AICharacter : BaseCharacter
{
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    public NavMeshAgent getNavmeshAgent() {  return agent; }

    //AIの状態管理
    [SerializeField]
    private AI_STATUS ai_status;
    public AI_STATUS getAiStatus() { return ai_status; }
    public void setAiStatus(AI_STATUS setStatus) { ai_status = setStatus; }

    //塔の管理クラス
    public TowerManager towerManager;

    //抽選で確定させた占領する塔のオブジェクト
    [SerializeField]
    private GameObject CaptureTowerObject = null;
    public GameObject getCaptureObject() { return CaptureTowerObject; }
    public void setCaptureObject(GameObject value) { CaptureTowerObject = value; }

    //キャラクターが防衛するタワーのオブジェクト
    [SerializeField]
    private GameObject DefenseTowerObject = null;
    public GameObject getDefenseTowerObject() {  return DefenseTowerObject; }
    public void setDefenseTowerObject(GameObject value) { DefenseTowerObject = value;}

    //攻撃範囲内に入ってきたキャラクターオブジェクト
    private GameObject AttackObject = null;
    public GameObject getAttackObject() { return AttackObject; }
    public void setAttackObject(GameObject value) { AttackObject = value; }

    //攻撃状態かどうか
    private bool isAttackMode = false;
    public bool getIsAttackMode() { return isAttackMode; }
    public void setIsAttackMode(bool value) { isAttackMode = value; }


    //タワーの周りを回る用
    private int PatrolCount = 0;

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
        bool is_active = getActive();

        //生存していないなら処理しない
        if (!is_active) { return; }

        switch (ai_status)
        {
            case AI_STATUS.NONE:
                //初期状態の時は一旦抽選処理にする
                ai_status = AI_STATUS.SEARCH;
                break;
            case AI_STATUS.SEARCH:
                //探索処理実行
                TowerSearch();
                break;
            case AI_STATUS.CAPTURE:
                //占領処理実行
                Capture();
                break;
            case AI_STATUS.MOVE:
                //移動処理実行
                Move();
                break;
            case AI_STATUS.ATTACK:
                //攻撃処理実行
                Attack();
                break;
            case AI_STATUS.DEFENSE:
                //防衛処理
                Defense();
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
        //キャラクターの所属軍によって処理が変わる
        switch (team_color)
        {
            case TEAM_COLOR.BLUE:
                BlueTowerSearch();
                break;
            case TEAM_COLOR.RED:
                RedTowerSearch();
                break;
            default:
                break;
        }

        //目指すタワーの位置を入力
        agent.SetDestination(CaptureTowerObject.transform.position);

        //移動速度設定
        agent.speed = getCharacterSpeed();
        agent.acceleration = getCharacterSpeed();
        agent.velocity = new Vector3(50,0,50);
        agent.isStopped = false;

        //移動アニメーション再生開始
        base.PlayAnimation(ANIMATION_STATE.RUN);

        //移動するべきタワーが見つかったので、AIのステートを移動に変更
        ai_status = AI_STATUS.MOVE;
    }


    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        //基本的には探索時に見つけたタワーへ向かう
        //途中で敵と遭遇した場合（コライダーに入った場合）はターゲットを変更する。
        //ターゲットに近づいた場合のみ攻撃ステートへ変更
        if (isAttackMode)
        {
            //攻撃位置の方向へ設定
            agent.SetDestination(AttackObject.transform.position);

            //ステートを攻撃へ変更
            ai_status = AI_STATUS.ATTACK;

            //ステートを変更したのでこれ以降の処理はなし
            return;
        }


        //占領範囲内ではないが目的地に近づいた場合は
        //走るアニメーションから歩行アニメーションへ変更
        Vector3 TowerDiffPosition = CaptureTowerObject.transform.position - agent.transform.position;
        //タワーに近づいた場合（占領範囲内）は
        //移動を停止し、占領ステートへ変更
        if (Vector3.Magnitude(TowerDiffPosition) < 30)
        {
            //タワー占領範囲内
            agent.speed = 0;
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;

            //移動アニメーションを停止
            base.StopAnimation(ANIMATION_STATE.RUN);

            if (CaptureTowerObject.GetComponent<Tower>().tower_color == team_color)
            {
                //目的とするタワーが自軍のものならステータスを防衛状態へ変更
                ai_status = AI_STATUS.DEFENSE;

                DefenseTowerObject = CaptureTowerObject;//防衛用のタワーオブジェクトへ移動
                CaptureTowerObject = null;//占領の目標のタワーオブジェクトを破棄

                var defenseTower = DefenseTowerObject.GetComponent<Tower>();
                
                //タワーの防衛を行っているキャラクターリストへ入れる
                defenseTower.defenseCharacterList.Add(gameObject);
                //防衛状態に移動する際に移動位置を決める
                agent.destination = defenseTower.defensePatrolPosition[PatrolCount].position;

                //防衛時の移動状態設定
                agent.speed = getCharacterSpeed() / 2;//移動時のスピードの半分
                agent.acceleration = 50;
                agent.isStopped = false;

                //移動アニメーション
                base.PlayAnimation(ANIMATION_STATE.RUN);
            }
            else
            {
                //タワーが自軍のものでないならステータスを占領状態へ変更
                ai_status = AI_STATUS.CAPTURE;
            }
        }
    }

    /// <summary>
    /// 占領処理
    /// </summary>
    private void Capture()
    {
        //占領状態
        //ターゲットとした塔の占領が自軍のものになったら
        //AIのステートをタワー探索へ変更
        if(CaptureTowerObject.GetComponent<Tower>().tower_color == team_color)
        {
            agent.speed = 0;
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;

            ai_status = AI_STATUS.SEARCH;
        }
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private void Attack()
    {
        //攻撃対象の敵が倒れた場合は攻撃アニメーションを停止してタワー探索処理へ
        if (AttackObject.GetComponent<BaseCharacter>().getActive() == false)
        {
            //攻撃アニメーション停止
            base.StopAnimation(ANIMATION_STATE.ATTACK);
            //攻撃対象リセット
            isAttackMode = false;
            AttackObject = null;

            //タワー探索へステート変更
            ai_status = AI_STATUS.SEARCH;
        }
        else
        {
            //対象のキャラクターに近づいたら、攻撃アニメーションを再生させる
            Vector3 AttackDiffPosition = AttackObject.transform.position - agent.transform.position;
            //TODO：攻撃範囲は装備している武器によって変わるものとする
            if (Vector3.Magnitude(AttackDiffPosition) < 10)
            {
                //移動を停止
                agent.speed = 0;
                agent.acceleration = 0;
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                base.StopAnimation(ANIMATION_STATE.RUN);

                //攻撃アニメーション再生
                base.PlayAnimation(ANIMATION_STATE.ATTACK);

                //TODO：仮で攻撃
                //相手側のHPを減らす
                AttackObject.GetComponent<BaseCharacter>().WeponTakeDamege(WEPON.Sword);
            }
        }
    }

    /// <summary>
    /// 防衛処理
    /// </summary>
    private void Defense()
    {
        var defenseTower = DefenseTowerObject.GetComponent<Tower>();

        //防衛中の場合に自軍の防衛タワー数が一定数以下になったら、
        //タワー探索処理へステートを変更


        //タワーの周りを巡回するように防衛する
        //巡回途中に敵キャラクターが攻撃範囲に入ったら、
        //攻撃ステートへ変更する

        if (defenseTower.defensePatrolPosition.Length == 0)
        {
            //処理しない
            return;
        }

        //待機ポイントに近づいたら
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            //次の移動先を決めて移動させる
            agent.destination = defenseTower.defensePatrolPosition[PatrolCount].position;
            PatrolCount = (PatrolCount + 1) % defenseTower.defensePatrolPosition.Length;
        }
    }

    /// <summary>
    /// 青軍のタワー探索処理
    /// </summary>
    private void BlueTowerSearch()
    {
        //タワーのカウント数
        int blueTowerCount = towerManager.getBlueTowerCount();
        int redTowerCount = towerManager.getRedTowerCount();
        int natureTowerCount = towerManager.getNatureTowerCount();

        //タワーオブジェクトそのもののデータ
        List<GameObject> blueTowerList = towerManager.getBlueTowerList();
        List<GameObject> redTowerList = towerManager.getRedTowerList();
        List<GameObject> natureTowerList = towerManager.getNatureTowerList();

        if (natureTowerCount > 0)
        {
            //中立タワーリストの中から距離が一番近いものを目指す
            float nearDis = 0.0f;
            foreach (GameObject natureTower in natureTowerList)
            {
                float distance = Vector3.Distance(natureTower.transform.position, agent.transform.position);
                if (nearDis == 0 || nearDis > distance)
                {
                    nearDis = distance;
                    CaptureTowerObject = natureTower;
                    break;
                }
            }
            return;
        }


        if (blueTowerCount < redTowerCount)
        {
            //敵軍のタワーが自軍タワーより多いのであれば
            //敵軍として占領されているタワーのうちから自分との距離が一番遠いものを目指す（タワーへの攻撃占領）
            float nearDis = 0.0f;
            foreach (GameObject redTower in redTowerList)
            {
                float distance = Vector3.Distance(redTower.transform.position, agent.transform.position);
                if (nearDis == 0 || nearDis < distance)
                {
                    nearDis = distance;
                    CaptureTowerObject = redTower;
                    break;
                }
            }
            return;
        }

        if (blueTowerCount >= redTowerCount)
        {
            //自軍タワーが多い場合は自軍占領タワーリストから
            //現在の位置に一番近いものをターゲットとする（タワーの防衛）
            //中立タワーリストの中から距離が一番近いものを目指す
            float nearDis = 0.0f;
            foreach (GameObject blueTower in blueTowerList)
            {
                float distance = Vector3.Distance(blueTower.transform.position, agent.transform.position);
                if (nearDis == 0 || nearDis > distance)
                {
                    nearDis = distance;
                    CaptureTowerObject = blueTower;
                    break;
                }
            }
            return;
        }
    }


    /// <summary>
    /// 赤軍のタワー探索処理
    /// </summary>
    private void RedTowerSearch()
    {
        //タワーのカウント数
        int blueTowerCount = towerManager.getBlueTowerCount();
        int redTowerCount = towerManager.getRedTowerCount();
        int natureTowerCount = towerManager.getNatureTowerCount();

        //タワーオブジェクトそのもののデータ
        List<GameObject> blueTowerList = towerManager.getBlueTowerList();
        List<GameObject> redTowerList = towerManager.getRedTowerList();
        List<GameObject> natureTowerList = towerManager.getNatureTowerList();

        if (natureTowerCount > 0)
        {
            //中立タワーリストの中から距離が一番近いものを目指す
            float nearDis = 0.0f;
            foreach (GameObject natureTower in natureTowerList)
            {
                float distance = Vector3.Distance(natureTower.transform.position, agent.transform.position);
                if (nearDis == 0 || nearDis > distance)
                {
                    nearDis = distance;
                    CaptureTowerObject = natureTower;
                }
            }
        }


        if(redTowerCount < blueTowerCount)
        {
            //敵軍のタワーが自軍タワーより多いのであれば
            //敵軍として占領されているタワーのうちから自分との距離が一番遠いものを目指す（タワーへの攻撃占領）
            float nearDis = 0.0f;
            foreach (GameObject redTower in redTowerList)
            {
                float distance = Vector3.Distance(redTower.transform.position, agent.transform.position);
                if (nearDis == 0 || nearDis < distance)
                {
                    nearDis = distance;
                    CaptureTowerObject = redTower;
                }
            }
        }

        if(redTowerCount >= blueTowerCount)
        {
            //自軍タワーが多い場合は自軍占領タワーリストから
            //現在の位置に一番近いものをターゲットとする（タワーの防衛）
            //中立タワーリストの中から距離が一番近いものを目指す
            float nearDis = 0.0f;
            foreach (GameObject blueTower in blueTowerList)
            {
                float distance = Vector3.Distance(blueTower.transform.position, agent.transform.position);
                if (nearDis == 0 || nearDis > distance)
                {
                    nearDis = distance;
                    CaptureTowerObject = blueTower;
                }
            }
        }
    }
}
