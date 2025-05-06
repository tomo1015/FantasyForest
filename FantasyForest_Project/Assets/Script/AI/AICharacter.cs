using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;
using System;
using Unity.VisualScripting;

public class AICharacter : BaseCharacter
{
    /// <summary>
    /// タワー占拠の判定距離
    /// </summary>
    private const float TOWER_CAPTURE_RANGE = 30f;

    /// <summary>
    /// 攻撃可能な距離
    /// </summary>
    private const float ATTACK_RANGE = 10f;

    /// <summary>
    /// 防御時の移動速度倍率
    /// </summary>
    private const float DEFENSE_SPEED_MULTIPLIER = 0.5f;

    /// <summary>
    /// 防御時の加速度
    /// </summary>
    private const float DEFENSE_ACCELERATION = 50f;

    // NavMeshAgent
    private NavMeshAgent agent;
    public NavMeshAgent NavMeshAgent => agent;

    // AIの状態管理
    [SerializeField]
    private AI_STATUS ai_status;
    public AI_STATUS AiStatus
    {
        get => ai_status;
        set => ai_status = value;
    }

    // タワー管理クラス
    public TowerManager towerManager;

    // 占拠目標のタワーオブジェクト
    [SerializeField]
    private GameObject CaptureTowerObject = null;
    public GameObject CaptureTower
    {
        get => CaptureTowerObject;
        set => CaptureTowerObject = value;
    }

    // 防御対象のタワーオブジェクト
    [SerializeField]
    private GameObject DefenseTowerObject = null;
    public GameObject DefenseTower
    {
        get => DefenseTowerObject;
        set => DefenseTowerObject = value;
    }

    // 攻撃対象のキャラクターオブジェクト
    private GameObject AttackObject = null;
    public GameObject AttackTarget
    {
        get => AttackObject;
        set => AttackObject = value;
    }

    // 攻撃モードかどうか
    private bool isAttackMode = false;
    public bool IsAttackMode
    {
        get => isAttackMode;
        set => isAttackMode = value;
    }

    // パトロールカウンター
    private int PatrolCount = 0;

    protected override void Start()
    {
        // 基底クラスのStartメソッドを呼び出す
        base.Start();

        // エージェントの取得
        agent = GetComponent<NavMeshAgent>();
        ai_status = AI_STATUS.NONE; // AIの初期状態を設定
    }

    protected override void Update()
    {
        // HPの更新
        base.Update();

        // AIのアクティブ状態の確認
        bool is_active = getActive();

        // アクティブでない場合は処理を終了
        if (!is_active) { return; }

        switch (ai_status)
        {
            case AI_STATUS.NONE:
                // 検索状態に移行
                ai_status = AI_STATUS.SEARCH;
                break;
            case AI_STATUS.SEARCH:
                // タワーの探索
                TowerSearch();
                break;
            case AI_STATUS.CAPTURE:
                // 占拠の実行
                Capture();
                break;
            case AI_STATUS.MOVE:
                // 移動の実行
                Move();
                break;
            case AI_STATUS.ATTACK:
                // 攻撃の実行
                Attack();
                break;
            case AI_STATUS.DEFENSE:
                // 防御の実行
                Defense();
                break;
            default: 
                break; 
        }
    }

    /// <summary>
    /// 指定チームのタワーを探索し、最適なCaptureTowerObjectを設定する共通メソッド
    /// </summary>
    /// <param name="searchColor">探索対象のチームカラー</param>
    private void SearchTower(TEAM_COLOR searchColor)
    {
        int blueTowerCount = towerManager.getBlueTowerCount();
        int redTowerCount = towerManager.getRedTowerCount();
        int natureTowerCount = towerManager.getNatureTowerCount();

        List<GameObject> blueTowerList = towerManager.getBlueTowerList();
        List<GameObject> redTowerList = towerManager.getRedTowerList();
        List<GameObject> natureTowerList = towerManager.getNatureTowerList();

        // 中立タワーがあれば最も近いものを優先
        if (natureTowerCount > 0)
        {
            float nearDis = float.MaxValue;
            foreach (GameObject natureTower in natureTowerList)
            {
                float distance = Vector3.Distance(natureTower.transform.position, agent.transform.position);
                if (distance < nearDis)
                {
                    nearDis = distance;
                    CaptureTowerObject = natureTower;
                }
            }
            return;
        }

        // 敵タワーが多い場合は敵タワーを優先
        if ((searchColor == TEAM_COLOR.BLUE && blueTowerCount < redTowerCount) ||
            (searchColor == TEAM_COLOR.RED && redTowerCount < blueTowerCount))
        {
            var targetList = (searchColor == TEAM_COLOR.BLUE) ? redTowerList : blueTowerList;
            float nearDis = float.MaxValue;
            foreach (GameObject targetTower in targetList)
            {
                float distance = Vector3.Distance(targetTower.transform.position, agent.transform.position);
                if (distance < nearDis)
                {
                    nearDis = distance;
                    CaptureTowerObject = targetTower;
                }
            }
            return;
        }

        // それ以外は自チームのタワーで最も近いもの
        var selfList = (searchColor == TEAM_COLOR.BLUE) ? blueTowerList : redTowerList;
        float selfNearDis = float.MaxValue;
        foreach (GameObject selfTower in selfList)
        {
            float distance = Vector3.Distance(selfTower.transform.position, agent.transform.position);
            if (distance < selfNearDis)
            {
                selfNearDis = distance;
                CaptureTowerObject = selfTower;
            }
        }
    }

    /// <summary>
    /// 青チーム用のタワー探索処理
    /// </summary>
    private void BlueTowerSearch()
    {
        SearchTower(TEAM_COLOR.BLUE);
    }

    /// <summary>
    /// 赤チーム用のタワー探索処理
    /// </summary>
    private void RedTowerSearch()
    {
        SearchTower(TEAM_COLOR.RED);
    }

    /// <summary>
    /// タワーの探索を実行し、最適な目標を設定する
    /// </summary>
    private void TowerSearch()
    {
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

        agent.SetDestination(CaptureTower.transform.position);
        agent.speed = getCharacterSpeed();
        agent.acceleration = getCharacterSpeed();
        agent.velocity = new Vector3(50,0,50);
        agent.isStopped = false;

        base.PlayAnimation(ANIMATION_STATE.RUN);
        AiStatus = AI_STATUS.MOVE;
    }

    /// <summary>
    /// キャラクターの移動を制御する
    /// 攻撃モード時は攻撃対象へ、通常時は占拠目標のタワーへ移動
    /// </summary>
    private void Move()
    {
        if (CaptureTower == null)
        {
            AiStatus = AI_STATUS.SEARCH;
            return;
        }

        if (IsAttackMode && AttackTarget != null)
        {
            agent.SetDestination(AttackTarget.transform.position);
            AiStatus = AI_STATUS.ATTACK;
            return;
        }

        Vector3 towerDiffPosition = CaptureTower.transform.position - agent.transform.position;
        if (Vector3.Magnitude(towerDiffPosition) < TOWER_CAPTURE_RANGE)
        {
            StopMovement();
            base.StopAnimation(ANIMATION_STATE.RUN);

            if (CaptureTower.GetComponent<Tower>()?.tower_color == team_color)
            {
                SwitchToDefenseMode();
            }
            else
            {
                AiStatus = AI_STATUS.CAPTURE;
            }
        }
    }

    /// <summary>
    /// キャラクターの移動を停止させる
    /// </summary>
    private void StopMovement()
    {
        agent.speed = 0;
        agent.acceleration = 0;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }

    /// <summary>
    /// 防御モードに切り替える
    /// タワーの防御リストに追加し、パトロール位置を設定する
    /// </summary>
    private void SwitchToDefenseMode()
    {
        AiStatus = AI_STATUS.DEFENSE;
        DefenseTower = CaptureTower;
        CaptureTower = null;

        var defenseTower = DefenseTower.GetComponent<Tower>();
        if (defenseTower != null)
        {
            defenseTower.defenseCharacterList.Add(gameObject);
            agent.destination = defenseTower.defensePatrolPosition[PatrolCount].position;

            agent.speed = getCharacterSpeed() * DEFENSE_SPEED_MULTIPLIER;
            agent.acceleration = DEFENSE_ACCELERATION;
            agent.isStopped = false;

            base.PlayAnimation(ANIMATION_STATE.RUN);
        }
    }

    /// <summary>
    /// タワーの占拠を実行する
    /// タワーが自チームのものになった場合は探索状態に戻る
    /// </summary>
    private void Capture()
    {
        if(CaptureTower.GetComponent<Tower>()?.tower_color == team_color)
        {
            StopMovement();
            AiStatus = AI_STATUS.SEARCH;
        }
    }

    /// <summary>
    /// 攻撃対象への攻撃を実行する
    /// 攻撃範囲内に入った場合のみ攻撃を実行
    /// </summary>
    private void Attack()
    {
        if (AttackTarget == null)
        {
            StopAttackMode();
            return;
        }

        var targetCharacter = AttackTarget.GetComponent<BaseCharacter>();
        if (targetCharacter == null || !targetCharacter.getActive())
        {
            StopAttackMode();
            return;
        }

        Vector3 attackDiffPosition = AttackTarget.transform.position - agent.transform.position;
        if (Vector3.Magnitude(attackDiffPosition) < ATTACK_RANGE)
        {
            StopMovement();
            base.StopAnimation(ANIMATION_STATE.RUN);
            base.PlayAnimation(ANIMATION_STATE.ATTACK);
            targetCharacter.WeponTakeDamege(WEPON.Sword);
        }
    }

    /// <summary>
    /// 攻撃モードを終了し、探索状態に戻る
    /// </summary>
    private void StopAttackMode()
    {
        base.StopAnimation(ANIMATION_STATE.ATTACK);
        IsAttackMode = false;
        AttackTarget = null;
        AiStatus = AI_STATUS.SEARCH;
    }

    /// <summary>
    /// タワーの防御を実行する
    /// パトロール位置を巡回しながら防御を行う
    /// </summary>
    private void Defense()
    {
        if (DefenseTower == null)
        {
            AiStatus = AI_STATUS.SEARCH;
            return;
        }

        var defenseTower = DefenseTower.GetComponent<Tower>();
        if (defenseTower == null || defenseTower.defensePatrolPosition.Length == 0)
        {
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            agent.destination = defenseTower.defensePatrolPosition[PatrolCount].position;
            PatrolCount = (PatrolCount + 1) % defenseTower.defensePatrolPosition.Length;
        }
    }
}
