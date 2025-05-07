using UnityEngine;
using UnityEngine.AI;
using Constants;

public class AICharacter : BaseCharacter
{
    /// <summary>
    /// 繧ｿ繝ｯ繝ｼ蜊諡縺ｮ蛻､螳夊ｷ晞屬
    /// </summary>
    private const float TOWER_CAPTURE_RANGE = 30f;

    /// <summary>
    /// 謾ｻ謦�庄閭ｽ縺ｪ霍晞屬
    /// </summary>
    private const float ATTACK_RANGE = 10f;

    /// <summary>
    /// 髦ｲ蠕｡譎ゅ�遘ｻ蜍暮溷ｺｦ蛟咲紫
    /// </summary>
    private const float DEFENSE_SPEED_MULTIPLIER = 0.5f;

    /// <summary>
    /// 髦ｲ蠕｡譎ゅ�蜉騾溷ｺｦ
    /// </summary>
    private const float DEFENSE_ACCELERATION = 50f;

    // NavMeshAgent
    private NavMeshAgent agent;
    public NavMeshAgent NavMeshAgent => agent;

    // AI縺ｮ迥ｶ諷狗ｮ｡逅
    [SerializeField]
    private AI_STATUS ai_status;
    public AI_STATUS AiStatus
    {
        get => ai_status;
        set => ai_status = value;
    }

    // 繧ｿ繝ｯ繝ｼ邂｡逅�け繝ｩ繧ｹ
    public TowerManager towerManager;

    // 蜊諡逶ｮ讓吶�繧ｿ繝ｯ繝ｼ繧ｪ繝悶ず繧ｧ繧ｯ繝
    [SerializeField]
    private GameObject CaptureTowerObject = null;
    public GameObject CaptureTower
    {
        get => CaptureTowerObject;
        set => CaptureTowerObject = value;
    }

    // 髦ｲ蠕｡蟇ｾ雎｡縺ｮ繧ｿ繝ｯ繝ｼ繧ｪ繝悶ず繧ｧ繧ｯ繝
    [SerializeField]
    private GameObject DefenseTowerObject = null;
    public GameObject DefenseTower
    {
        get => DefenseTowerObject;
        set => DefenseTowerObject = value;
    }
    private GameObject AttackObject = null;
    public GameObject AttackTarget
    {
        get => AttackObject;
        set => AttackObject = value;
    }

    // 謾ｻ謦�Δ繝ｼ繝峨°縺ｩ縺�°
    private bool isAttackMode = false;
    public bool IsAttackMode
    {
        get => isAttackMode;
        set => isAttackMode = value;
    }

    // 繝代ヨ繝ｭ繝ｼ繝ｫ繧ｫ繧ｦ繝ｳ繧ｿ繝ｼ
    private int PatrolCount = 0;

    protected override void Start()
    {
        // 蝓ｺ蠎輔け繝ｩ繧ｹ縺ｮStart繝｡繧ｽ繝�ラ繧貞他縺ｳ蜃ｺ縺
        base.Start();
        
        // 繧ｨ繝ｼ繧ｸ繧ｧ繝ｳ繝医�蜿門ｾ
        agent = GetComponent<NavMeshAgent>();
        ai_status = AI_STATUS.NONE; // AI縺ｮ蛻晄悄迥ｶ諷九ｒ險ｭ螳

    protected override void Update()
    {
        // HP縺ｮ譖ｴ譁ｰ
        base.Update();

        // 繧｢繧ｯ繝�ぅ繝悶〒縺ｪ縺�ｴ蜷医�蜃ｦ逅�ｒ邨ゆｺ
        if (!is_active) { return; }

        //AIステータス管理
        switch (ai_status)
        {
            case AI_STATUS.NONE:
                // 讀懃ｴ｢迥ｶ諷九↓遘ｻ陦
                ai_status = AI_STATUS.SEARCH;
                break;
            case AI_STATUS.SEARCH:
                // 繧ｿ繝ｯ繝ｼ縺ｮ謗｢邏｢
                TowerSearch();
                break;
            case AI_STATUS.CAPTURE:
                // 蜊諡縺ｮ螳溯｡
                Capture();
                break;
            case AI_STATUS.MOVE:
                // 遘ｻ蜍輔�螳溯｡
                Move();
                break;
            case AI_STATUS.ATTACK:
                // 謾ｻ謦��螳溯｡
                Attack();
                break;
            case AI_STATUS.DEFENSE:
                // 髦ｲ蠕｡縺ｮ螳溯｡
                Defense();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 謖�ｮ壹メ繝ｼ繝縺ｮ繧ｿ繝ｯ繝ｼ繧呈爾邏｢縺励∵怙驕ｩ縺ｪCaptureTowerObject繧定ｨｭ螳壹☆繧句�騾壹Γ繧ｽ繝�ラ
    /// </summary>
    /// <param name="searchColor">謗｢邏｢蟇ｾ雎｡縺ｮ繝√�繝繧ｫ繝ｩ繝ｼ</param>
    private void SearchTower(TEAM_COLOR searchColor)
    {
        int blueTowerCount = towerManager.getBlueTowerCount();
        int redTowerCount = towerManager.getRedTowerCount();
        int natureTowerCount = towerManager.getNatureTowerCount();

        List<GameObject> blueTowerList = towerManager.getBlueTowerList();
        List<GameObject> redTowerList = towerManager.getRedTowerList();
        List<GameObject> natureTowerList = towerManager.getNatureTowerList();

        // 荳ｭ遶九ち繝ｯ繝ｼ縺後≠繧後�譛繧りｿ代＞繧ゅ�繧貞━蜈
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

        // 謨ｵ繧ｿ繝ｯ繝ｼ縺悟､壹＞蝣ｴ蜷医�謨ｵ繧ｿ繝ｯ繝ｼ繧貞━蜈
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

        // 縺昴ｌ莉･螟悶�閾ｪ繝√�繝縺ｮ繧ｿ繝ｯ繝ｼ縺ｧ譛繧りｿ代＞繧ゅ�
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
    /// 髱偵メ繝ｼ繝逕ｨ縺ｮ繧ｿ繝ｯ繝ｼ謗｢邏｢蜃ｦ逅
    /// </summary>
    private void BlueTowerSearch()
    {
        SearchTower(TEAM_COLOR.BLUE);
    }

    /// <summary>
    /// 襍､繝√�繝逕ｨ縺ｮ繧ｿ繝ｯ繝ｼ謗｢邏｢蜃ｦ逅
    /// </summary>
    private void RedTowerSearch()
    {
        SearchTower(TEAM_COLOR.RED);
    }

    /// <summary>
    /// 繧ｿ繝ｯ繝ｼ縺ｮ謗｢邏｢繧貞ｮ溯｡後＠縲∵怙驕ｩ縺ｪ逶ｮ讓吶ｒ險ｭ螳壹☆繧
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
        agent.velocity = new Vector3(50, 0, 50);
        agent.isStopped = false;
        base.PlayAnimation(ANIMATION_STATE.RUN);
        AiStatus = AI_STATUS.MOVE;
    }

    /// <summary>
    /// 繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺ｮ遘ｻ蜍輔ｒ蛻ｶ蠕｡縺吶ｋ
    /// 謾ｻ謦�Δ繝ｼ繝画凾縺ｯ謾ｻ謦�ｯｾ雎｡縺ｸ縲�壼ｸｸ譎ゅ�蜊諡逶ｮ讓吶�繧ｿ繝ｯ繝ｼ縺ｸ遘ｻ蜍
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
    /// 繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺ｮ遘ｻ蜍輔ｒ蛛懈ｭ｢縺輔○繧
    /// </summary>
    private void StopMovement()
    {
        agent.speed = 0;
        agent.acceleration = 0;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }

    /// <summary>
    /// 髦ｲ蠕｡繝｢繝ｼ繝峨↓蛻�ｊ譖ｿ縺医ｋ
    /// 繧ｿ繝ｯ繝ｼ縺ｮ髦ｲ蠕｡繝ｪ繧ｹ繝医↓霑ｽ蜉縺励√ヱ繝医Ο繝ｼ繝ｫ菴咲ｽｮ繧定ｨｭ螳壹☆繧
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
    /// 繧ｿ繝ｯ繝ｼ縺ｮ蜊諡繧貞ｮ溯｡後☆繧
    /// 繧ｿ繝ｯ繝ｼ縺瑚�繝√�繝縺ｮ繧ゅ�縺ｫ縺ｪ縺｣縺溷ｴ蜷医�謗｢邏｢迥ｶ諷九↓謌ｻ繧
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
    /// 謾ｻ謦�ｯｾ雎｡縺ｸ縺ｮ謾ｻ謦�ｒ螳溯｡後☆繧
    /// 謾ｻ謦�ｯ�峇蜀�↓蜈･縺｣縺溷ｴ蜷医�縺ｿ謾ｻ謦�ｒ螳溯｡
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
    /// 謾ｻ謦�Δ繝ｼ繝峨ｒ邨ゆｺ�＠縲∵爾邏｢迥ｶ諷九↓謌ｻ繧
    /// </summary>
    private void StopAttackMode()
    {
        base.StopAnimation(ANIMATION_STATE.ATTACK);
        IsAttackMode = false;
        AttackTarget = null;
        AiStatus = AI_STATUS.SEARCH;
    }

    /// <summary>
    /// 繧ｿ繝ｯ繝ｼ縺ｮ髦ｲ蠕｡繧貞ｮ溯｡後☆繧
    /// 繝代ヨ繝ｭ繝ｼ繝ｫ菴咲ｽｮ繧貞ｷ｡蝗槭＠縺ｪ縺後ｉ髦ｲ蠕｡繧定｡後≧
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
