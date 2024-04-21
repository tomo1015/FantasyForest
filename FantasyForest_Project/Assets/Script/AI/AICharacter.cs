using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;
using System;

public class AICharacter : BaseCharacter
{
    //�i�r���b�V���G�[�W�F���g
    private NavMeshAgent agent;
    //AI�̏�ԊǗ�
    [SerializeField]
    private AI_STATUS ai_status;
    public AI_STATUS getAiStatus() { return ai_status; }
    public void setAiStatus(AI_STATUS setStatus) { ai_status = setStatus; }

    //���̊Ǘ��N���X
    public TowerManager towerManager;

    //���I�Ŋm�肳������̂��铃�̃I�u�W�F�N�g
    [SerializeField]
    private GameObject CaptureTowerObject = null;
    [SerializeField]
    private GameObject DefenseTowerObject = null;
    public GameObject getCaptureObject() { return CaptureTowerObject; }
    public void setCaptureObject(GameObject value) { CaptureTowerObject = value; }

    //�U���͈͓��ɓ����Ă����L�����N�^�[�I�u�W�F�N�g
    [SerializeField]
    private GameObject AttackObject = null;
    public GameObject getAttackObject() { return AttackObject; }
    public void setAttackObject(GameObject value) { AttackObject = value; }

    //�U����Ԃ��ǂ���
    [SerializeField]
    private bool isAttackMode = false;
    public bool getIsAttackMode() { return isAttackMode; }
    public void setIsAttackMode(bool value) { isAttackMode = value; }


    //�^���[�̎�������p
    private int PatrolCount = 0;

    protected override void Start()
    {
        //��{�N���X�̏���
        base.Start();

        //���̃N���X�����̏���
        agent = GetComponent<NavMeshAgent>();//�i�r���b�V���G�[�W�F���g�擾
        ai_status = AI_STATUS.NONE;//AI�̏�Ԃ���U������Ԃ֕ύX
    }

    protected override void Update()
    {
        //HP�Ǘ�����
        base.Update();

        //AI�X�e�[�^�X�Ǘ�
        bool is_active = getActive();

        //�������Ă��Ȃ��Ȃ珈�����Ȃ�
        if (!is_active) { return; }

        switch (ai_status)
        {
            case AI_STATUS.NONE:
                //������Ԃ̎��͈�U���I�����ɂ���
                ai_status = AI_STATUS.SEARCH;
                break;
            case AI_STATUS.SEARCH:
                TowerSearch();
                //�T���������s
                break;
            case AI_STATUS.CAPTURE:
                //��̏������s
                Capture();
                break;
            case AI_STATUS.MOVE:
                //�ړ��������s
                Move();
                break;
            case AI_STATUS.ATTACK:
                //�U���������s
                Attack();
                break;
            case AI_STATUS.DEFENSE:
                //�h�q����
                defense();
                break;
            default: 
                break; 
        }
    }

    /// <summary>
    /// �^���[�̒T������
    /// </summary>
    private void TowerSearch()
    {
        //�^���[�̃J�E���g��
        int blueTowerCount = towerManager.getBlueTowerCount();
        int redTowerCount = towerManager.getRedTowerCount();
        int natureTowerCount = towerManager.getNatureTowerCount();

        //�^���[�I�u�W�F�N�g���̂��̂̃f�[�^
        List<GameObject> blueTowerList = towerManager.getBlueTowerList();
        List<GameObject> redTowerList = towerManager.getRedTowerList();
        List<GameObject> natureTowerList = towerManager.getNatureTowerList();
        //�L�����N�^�[���̂��̌��݈ʒu
        Vector3 nowPosition = agent.transform.position;

        //�^���[�S�̂̊Ǘ��N���X�̏��
        //�L�����N�^�[�̏����R���R�������ꍇ
        if (team_color == TEAM_COLOR.BLUE)
        {
            if(natureTowerCount > 0)
            {
                //�����^���[���X�g�̒����狗������ԋ߂����̂�ڎw��
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
            else if(blueTowerCount < redTowerCount)
            {
                //�G�R�̃^���[�����R�^���[��葽���̂ł����
                //�G�R�Ƃ��Đ�̂���Ă���^���[�̂������玩���Ƃ̋�������ԉ������̂�ڎw���i�^���[�ւ̍U����́j
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
            else
            {
                //���R�^���[�������ꍇ�͎��R��̃^���[���X�g����
                //���݂̈ʒu�Ɉ�ԋ߂����̂��^�[�Q�b�g�Ƃ���i�^���[�̖h�q�j
                //�����^���[���X�g�̒����狗������ԋ߂����̂�ڎw��
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

        //�ڎw���^���[�̈ʒu�����
        agent.SetDestination(CaptureTowerObject.transform.position);

        //�ړ����x�ݒ�
        agent.speed = 50;
        agent.acceleration = 50;
        agent.velocity = new Vector3(50,0,50);
        agent.isStopped = false;

        //�ړ��A�j���[�V�����Đ��J�n
        base.PlayAnimation(ANIMATION_STATE.RUN);

        //�ړ�����ׂ��^���[�����������̂ŁAAI�̃X�e�[�g���ړ��ɕύX
        ai_status = AI_STATUS.MOVE;
    }


    /// <summary>
    /// �ړ�����
    /// </summary>
    private void Move()
    {
        //��{�I�ɂ͒T�����Ɍ������^���[�֌�����
        //�r���œG�Ƒ��������ꍇ�i�R���C�_�[�ɓ������ꍇ�j�̓^�[�Q�b�g��ύX����B
        //�^�[�Q�b�g�ɋ߂Â����ꍇ�̂ݍU���X�e�[�g�֕ύX
        if (isAttackMode)
        {
            //�U���ʒu�̕����֐ݒ�
            agent.SetDestination(AttackObject.transform.position);

            //�X�e�[�g���U���֕ύX
            ai_status = AI_STATUS.ATTACK;

            //�X�e�[�g��ύX�����̂ł���ȍ~�̏����͂Ȃ�
            return;
        }

        //��͈͓̔��ł͂Ȃ����ړI�n�ɋ߂Â����ꍇ��
        //����A�j���[�V����������s�A�j���[�V�����֕ύX
        Vector3 TowerDiffPosition = CaptureTowerObject.transform.position - agent.transform.position;
        //�^���[�ɋ߂Â����ꍇ�i��͈͓̔��j��
        //�ړ����~���A��̃X�e�[�g�֕ύX
        if (Vector3.Magnitude(TowerDiffPosition) < 30)
        {
            //�^���[��͈͓̔�
            agent.speed = 0;
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;

            //�ړ��A�j���[�V�������~
            base.StopAnimation(ANIMATION_STATE.RUN);

            if (CaptureTowerObject.GetComponent<Tower>().tower_color == team_color)
            {
                //�ړI�Ƃ���^���[�����R�̂��̂Ȃ�X�e�[�^�X��h�q��Ԃ֕ύX
                ai_status = AI_STATUS.DEFENSE;
                DefenseTowerObject = CaptureTowerObject;//�h�q�p�̃^���[�I�u�W�F�N�g�ֈړ�
                CaptureTowerObject = null;//��̖̂ڕW�̃^���[�I�u�W�F�N�g��j��

                var defenseTower = DefenseTowerObject.GetComponent<Tower>();
                //�h�q��ԂɈړ�����ۂɈړ��ʒu�����߂�
                agent.destination = defenseTower.defensePatrolPosition[PatrolCount].position;

                //�h�q���̈ړ���Ԑݒ�
                agent.speed = 25;
                agent.acceleration = 50;
                agent.isStopped = false;

                //�ړ��A�j���[�V����
                base.PlayAnimation(ANIMATION_STATE.RUN);
            }
            else
            {
                //�^���[�����R�̂��̂łȂ��Ȃ�X�e�[�^�X���̏�Ԃ֕ύX
                ai_status = AI_STATUS.CAPTURE;
            }
        }
    }

    /// <summary>
    /// ��̏���
    /// </summary>
    private void Capture()
    {
        //��̏��
        //�^�[�Q�b�g�Ƃ������̐�̂����R�̂��̂ɂȂ�����
        //AI�̃X�e�[�g���^���[�T���֕ύX
        if(CaptureTowerObject.GetComponent<Tower>().tower_color == team_color)
        {
            ai_status = AI_STATUS.SEARCH;
        }
    }

    /// <summary>
    /// �U������
    /// </summary>
    private void Attack()
    {
        //�U���Ώۂ̓G���|�ꂽ�ꍇ�͍U���A�j���[�V�������~���ă^���[�T��������
        if (AttackObject.GetComponent<BaseCharacter>().getActive() == false)
        {
            //�U���A�j���[�V������~
            base.StopAnimation(ANIMATION_STATE.ATTACK);
            //�U���Ώۃ��Z�b�g
            isAttackMode = false;
            AttackObject = null;

            //�^���[�T���փX�e�[�g�ύX
            ai_status = AI_STATUS.SEARCH;
        }
        else
        {
            //�Ώۂ̃L�����N�^�[�ɋ߂Â�����A�U���A�j���[�V�������Đ�������
            Vector3 AttackDiffPosition = AttackObject.transform.position - agent.transform.position;
            //TODO�F�U���͈͂͑������Ă��镐��ɂ���ĕς����̂Ƃ���
            if (Vector3.Magnitude(AttackDiffPosition) < 10)
            {
                //�ړ����~
                agent.speed = 0;
                agent.acceleration = 0;
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                base.StopAnimation(ANIMATION_STATE.RUN);

                //�U���A�j���[�V�����Đ�
                base.PlayAnimation(ANIMATION_STATE.ATTACK);

                //TODO�F���ōU��
                //���葤��HP�����炷
                AttackObject.GetComponent<BaseCharacter>().WeponTakeDamege(WEPON.Sword);
            }
        }
    }

    /// <summary>
    /// �h�q����
    /// </summary>
    private void defense()
    {
        //�h�q���̏ꍇ�Ɏ��R�̖h�q�^���[������萔�ȉ��ɂȂ�����A
        //�^���[�T�������փX�e�[�g��ύX


        //�^���[�̎�������񂷂�悤�ɖh�q����
        //����r���ɓG�L�����N�^�[���U���͈͂ɓ�������A
        //�U���X�e�[�g�֕ύX����
        var defenseTower = DefenseTowerObject.GetComponent<Tower>();
        if (defenseTower.defensePatrolPosition.Length == 0)
        {
            //�������Ȃ�
            return;
        }

        //�ҋ@�|�C���g�ɋ߂Â�����
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            //���̈ړ�������߂Ĉړ�������
            agent.destination = defenseTower.defensePatrolPosition[PatrolCount].position;
            PatrolCount = (PatrolCount + 1) % defenseTower.defensePatrolPosition.Length;
        }
    }
}
