using UnityEngine;
using UnityEngine.AI;
using Constants;
using System.Collections.Generic;

public class AICharacter : BaseCharacter
{
    /// <summary>
    /// タワー占領の判定範囲
    /// </summary>
    private const float TOWER_CAPTURE_RANGE = 30f;

    /// <summary>
    /// 攻撃可能な範囲
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
    private TowerManager towerManager;

    // 占領目標のタワーオブジェクト
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
    [SerializeField]
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

        isAttackMode = false;//攻撃状態の初期化

        // エージェントの取得
        agent = GetComponent<NavMeshAgent>();
        // TowerManagerの取得
        towerManager = TowerManager.Instance;
        ai_status = AI_STATUS.NONE; // AIの初期状態を設定
    }

    protected override void Update()
    {
        // HPの更新
        base.Update();
        //更新した結果いなくなった場合はAIのステータスを初期化する
        if(base.getActive() == false){
            ai_status = AI_STATUS.NONE;
        }

        // AIステータス管理
        switch (ai_status)
        {
            case AI_STATUS.NONE:
                // 探索状態に移行
                ai_status = AI_STATUS.SEARCH;
                break;
            case AI_STATUS.SEARCH:
                // タワーの探索
                TowerSearch();
                break;
            case AI_STATUS.CAPTURE:
                // 占領の実行
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
        if (towerManager == null)
        {
            Debug.LogWarning("TowerManager is not initialized");
            return;
        }

        int blueTowerCount = towerManager.getBlueTowerCount();
        int redTowerCount = towerManager.getRedTowerCount();
        int natureTowerCount = towerManager.getNatureTowerCount();

        List<GameObject> blueTowerList = towerManager.getBlueTowerList();
        List<GameObject> redTowerList = towerManager.getRedTowerList();
        List<GameObject> natureTowerList = towerManager.getNatureTowerList();

        GameObject FindNearestTower(List<GameObject> towerList)
        {
            if (towerList == null || towerList.Count == 0) return null;
            
            float nearDis = float.MaxValue;
            GameObject nearestTower = null;
            
            foreach (GameObject tower in towerList)
            {
                if (tower == null) continue;
                
                float distance = Vector3.Distance(tower.transform.position, agent.transform.position);
                if (distance < nearDis)
                {
                    nearDis = distance;
                    nearestTower = tower;
                }
            }
            return nearestTower;
        }

        // 中立タワーがあれば最も近いものを優先
        if (natureTowerCount > 0)
        {
            CaptureTowerObject = FindNearestTower(natureTowerList);
            return;
        }

        // 敵タワーが多い場合は敵タワーを優先
        if ((searchColor == TEAM_COLOR.BLUE && blueTowerCount < redTowerCount) ||
            (searchColor == TEAM_COLOR.RED && redTowerCount < blueTowerCount))
        {
            var targetList = (searchColor == TEAM_COLOR.BLUE) ? redTowerList : blueTowerList;
            CaptureTowerObject = FindNearestTower(targetList);
            return;
        }

        // それ以外は自チームのタワーで最も近いもの
        var selfList = (searchColor == TEAM_COLOR.BLUE) ? blueTowerList : redTowerList;
        CaptureTowerObject = FindNearestTower(selfList);
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
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning($"{gameObject.name}: NavMeshAgent is not properly initialized");
            return;
        }

        switch (team_color)
        {
            case TEAM_COLOR.BLUE:
                BlueTowerSearch();
                break;
            case TEAM_COLOR.RED:
                RedTowerSearch();
                break;
            default:
                return;
        }

        if (CaptureTower == null)
        {
            Debug.LogWarning("No suitable tower found for capture");
            return;
        }

        agent.SetDestination(CaptureTower.transform.position);
        agent.speed = GetCharacterSpeed();
        agent.acceleration = GetCharacterSpeed();
        agent.velocity = new Vector3(50, 0, 50);
        agent.isStopped = false;
        base.PlayAnimation(ANIMATION_STATE.RUN);
        AiStatus = AI_STATUS.MOVE;
    }

    /// <summary>
    /// キャラクターの移動を制御する
    /// 攻撃モード時は攻撃対象へ、通常時は占領目標のタワーへ移動
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
            if (agent != null && agent.isActiveAndEnabled && agent.gameObject.activeInHierarchy && agent.isOnNavMesh)
            {
                agent.SetDestination(AttackTarget.transform.position);
            }
            else
            {
                // 必要に応じてデバッグログ
                Debug.LogWarning($"{gameObject.name}: NavMeshAgentが無効、非アクティブ、またはNavMesh上にいません");
            }
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
    /// キャラクターの移動を停止する
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

            agent.speed = GetCharacterSpeed() * DEFENSE_SPEED_MULTIPLIER;
            agent.acceleration = DEFENSE_ACCELERATION;
            agent.isStopped = false;

            base.PlayAnimation(ANIMATION_STATE.RUN);
        }
    }

    /// <summary>
    /// タワーの占領を実行する
    /// タワーが自チームのものになった場合は探索状態に戻る
    /// </summary>
    private void Capture()
    {
        if (CaptureTower == null)
        {
            AiStatus = AI_STATUS.SEARCH;
            return;
        }

        var tower = CaptureTower.GetComponent<Tower>();
        if (tower == null)
        {
            AiStatus = AI_STATUS.SEARCH;
            return;
        }

        //占領中、攻撃エリアの範囲を変更する
        ChangeChildBoxColliderSize("AttackSearchArea",new Vector3(10f,0.5f,10f));

        if (tower.tower_color == team_color)
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
        float distance = Vector3.Magnitude(attackDiffPosition);
        
        // 攻撃対象の方向を向く
        Vector3 targetDirection = attackDiffPosition.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        if (distance < ATTACK_RANGE)
        {
            StopMovement();
            base.StopAnimation(ANIMATION_STATE.RUN);
            base.PlayAnimation(ANIMATION_STATE.ATTACK);
            targetCharacter.WeaponTakeDamage(WEAPON.Sword);
            //相手のHPがゼロになったら、攻撃ターゲットを初期化する。
            //AIステータスは初期化する
            if(!targetCharacter.getActive()){
                AttackTarget = null;
                AiStatus = AI_STATUS.NONE;
            }
        }
        else
        {
            // 攻撃範囲外なら対象に近づく
            agent.isStopped = false;
            Vector3 directionToTarget = (AttackTarget.transform.position - transform.position).normalized;
            agent.destination = AttackTarget.transform.position - (directionToTarget * (ATTACK_RANGE / 2));
            //base.PlayAnimation(ANIMATION_STATE.RUN);
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

    /// <summary>
    /// 子オブジェクトのBoxColliderのサイズを変更する
    /// </summary>
    /// <param name="childName">子オブジェクトの名前</param>
    /// <param name="newSize">新しいサイズ</param>
    private void ChangeChildBoxColliderSize(string childName, Vector3 newSize)
    {
        Transform childTransform = transform.Find(childName);
        if (childTransform != null)
        {
            BoxCollider boxCollider = childTransform.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.size = newSize;
            }
            else
            {
                Debug.LogWarning($"BoxCollider not found on child object: {childName}");
            }
        }
        else
        {
            Debug.LogWarning($"Child object not found: {childName}");
        }
    }
}
