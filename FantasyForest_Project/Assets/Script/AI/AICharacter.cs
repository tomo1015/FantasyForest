using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;

public class AICharacter : BaseCharacter
{
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //AIの状態管理
    [SerializeField]
    private AI_STATUS ai_status;
    public AI_STATUS getAiStatus() { return ai_status; }
    public void setAiStatus(AI_STATUS setStatus) { ai_status = setStatus; }

    //塔の管理クラス
    public TowerManager towerManager;

    //抽選で確定させた占領する塔のオブジェクト
    [SerializeField]
    private GameObject CaptureObject = null;
    public GameObject getCaptureObject() { return CaptureObject; }
    public void setCaptureObject(GameObject value) { CaptureObject = value; }

    //攻撃範囲内に入ってきたキャラクターオブジェクト
    [SerializeField]
    private GameObject AttackObject = null;
    public GameObject getAttackObject() { return AttackObject; }
    public void setAttackObject(GameObject value) { AttackObject = value; }

    //攻撃状態かどうか
    private bool isAttackMode = false;
    public bool getIsAttackMode() { return isAttackMode; }
    public void setIsAttackMode(bool value) { isAttackMode = value; }


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
                TowerSearch();
                //探索処理実行
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
        if (team_color == TEAM_COLOR.BLUE)
        {
            if(natureTowerCount > 0)
            {
                //中立タワーリストの中から距離が一番近いものを目指す
                float nearDis = 0.0f;
                foreach (GameObject natureTower in natureTowerList)
                {
                    float distance = Vector3.Distance(natureTower.transform.position, agent.transform.position);
                    if (nearDis == 0 || nearDis > distance)
                    {
                        nearDis = distance;
                        CaptureObject = natureTower;
                    }
                }
            }
            else if(blueTowerCount < redTowerCount)
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
                        CaptureObject = redTower;
                    }
                }
            }
            else
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
                        CaptureObject = blueTower;
                    }
                }
            }
        }

        //目指すタワーの位置を入力
        agent.SetDestination(CaptureObject.transform.position);

        //移動速度設定
        agent.speed = 50;
        agent.acceleration = 50;
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
        }
        else
        {
            //占領範囲内ではないが目的地に近づいた場合は
            //走るアニメーションから歩行アニメーションへ変更
            Vector3 TowerDiffPosition = CaptureObject.transform.position - agent.transform.position;
            if (Vector3.Magnitude(TowerDiffPosition) < 50)
            {
                base.StopAnimation(ANIMATION_STATE.RUN);
            }
            //タワーに近づいた場合（占領範囲内）は
            //移動を停止し、占領ステートへ変更

            if (Vector3.Magnitude(TowerDiffPosition) < 30)
            {
                //タワー占領範囲内
                agent.speed = 0;
                agent.acceleration = 0;
                agent.velocity = Vector3.zero;
                agent.isStopped = true;

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
        if(CaptureObject.GetComponent<Tower>().tower_color == team_color)
        {
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
}
