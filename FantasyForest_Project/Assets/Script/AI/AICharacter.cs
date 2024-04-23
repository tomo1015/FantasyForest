using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;
using System;
using Unity.VisualScripting;

public class AICharacter : BaseCharacter
{
    //�i�r���b�V���G�[�W�F���g
    private NavMeshAgent agent;
    public NavMeshAgent getNavmeshAgent() { return agent; }

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
    public GameObject getCaptureObject() { return CaptureTowerObject; }
    public void setCaptureObject(GameObject value) { CaptureTowerObject = value; }

    //�L�����N�^�[���h�q����^���[�̃I�u�W�F�N�g
    [SerializeField]
    private GameObject DefenseTowerObject = null;
    public GameObject getDefenseTowerObject() { return DefenseTowerObject; }
    public void setDefenseTowerObject(GameObject value) { DefenseTowerObject = value; }

    //�U���͈͓��ɓ����Ă����L�����N�^�[�I�u�W�F�N�g
    [SerializeField]
    private GameObject AttackObject = null;
    public GameObject getAttackObject() { return AttackObject; }
    public void setAttackObject(GameObject value) { AttackObject = value; }

    //�U����Ԃ��ǂ���
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
                //�T���������s
                TowerSearch();
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
                Defense();
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
        //�L�����N�^�[�̏����R�ɂ���ď������ς��
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

        //�ړ��A�j���[�V�����Đ��J�n
        base.PlayAnimation(ANIMATION_STATE.RUN);

        //�ڎw���^���[�̈ʒu�����
        agent.SetDestination(CaptureTowerObject.transform.position);

        //�ړ����x�ݒ�
        agent.speed = getCharacterSpeed();
        agent.acceleration = getCharacterSpeed();
        agent.velocity = new Vector3(50, 0, 50);
        agent.isStopped = false;

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

        //�^���[�Ƃ̋��������ȉ��Ȃ�
        if (Vector3.Magnitude(TowerDiffPosition) < CaptureTowerObject.GetComponent<Tower>().getCaptureRange())
        {
            if (CaptureTowerObject.GetComponent<Tower>().tower_color == team_color)
            {
                //�ړ��A�j���[�V����
                base.PlayAnimation(ANIMATION_STATE.RUN);

                //�ړI�Ƃ���^���[�����R�̂��̂Ȃ�X�e�[�^�X��h�q��Ԃ֕ύX
                ai_status = AI_STATUS.DEFENSE;

                DefenseTowerObject = CaptureTowerObject;//�h�q�p�̃^���[�I�u�W�F�N�g�ֈړ�
                CaptureTowerObject = null;//��̖̂ڕW�̃^���[�I�u�W�F�N�g��j��

                var defenseTower = DefenseTowerObject.GetComponent<Tower>();

                //�^���[�̖h�q���s���Ă���L�����N�^�[���X�g�֓����
                defenseTower.defenseCharacterList.Add(gameObject);
                //�h�q��ԂɈړ�����ۂɈړ��ʒu�����߂�
                agent.destination = defenseTower.defensePatrolPosition[PatrolCount].position;

                //�h�q���̈ړ���Ԑݒ�
                agent.speed = getCharacterSpeed() / 2;//�ړ����̃X�s�[�h�̔���
                agent.acceleration = 50;
                agent.isStopped = false;
            }
            else
            {
                //�ړ��A�j���[�V�������~
                base.StopAnimation(ANIMATION_STATE.RUN);

                //�^���[��͈͓̔�
                agent.speed = 0;
                agent.acceleration = 0;
                agent.velocity = Vector3.zero;
                agent.isStopped = true;

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
        //�^���[���̒��ɁA�^���[�̐�͈͓̔��ɓG�������Ă�����
        //���̓G���^�[�Q�b�g�Ƃ���

        //��̏��
        //�^�[�Q�b�g�Ƃ������̐�̂����R�̂��̂ɂȂ�����
        //AI�̃X�e�[�g���^���[�T���֕ύX
        if (CaptureTowerObject.GetComponent<Tower>().tower_color == team_color)
        {
            agent.speed = 0;
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;

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
    private void Defense()
    {
        var defenseTower = DefenseTowerObject.GetComponent<Tower>();

        //�h�q���̏ꍇ�Ɏ��R�̖h�q�^���[������萔�ȉ��ɂȂ�����A
        //�^���[�T�������փX�e�[�g��ύX


        //�^���[�̎�������񂷂�悤�ɖh�q����
        //����r���ɓG�L�����N�^�[���^���[�̐�͈͓̔��ɂ���Ȃ��
        //��̒��̓G�Ɍ������A�U���X�e�[�g�֕ύX����
        switch (team_color)
        {
            case TEAM_COLOR.BLUE:
                //�R�̏ꍇ�͐ԌR�̏��
                if (defenseTower.getRedCharaList().Count > 0 && AttackObject == null)
                {
                    float nearDis = 0.0f;
                    foreach (GameObject redCharacterAgent in defenseTower.getRedCharaList())
                    {
                        float distance = Vector3.Distance(redCharacterAgent.transform.position, agent.transform.position);
                        if (nearDis == 0 || nearDis > distance)
                        {
                            nearDis = distance;
                            AttackObject = redCharacterAgent;
                            isAttackMode = true;//�U����Ԃ�
                        }
                    }
                }
                break;
            case TEAM_COLOR.RED:
                break;
            default:
                break;
        }


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

    /// <summary>
    /// �R�̃^���[�T������
    /// </summary>
    private void BlueTowerSearch()
    {
        if (towerManager.getNatureTowerList().Count > 0)
        {
            //�����^���[���X�g�̒����狗������ԋ߂����̂�ڎw��
            float nearDis = 0.0f;
            foreach (GameObject natureTower in towerManager.getNatureTowerList())
            {
                float distance = Vector3.Distance(natureTower.transform.position, agent.transform.position);
                if (nearDis == 0 || nearDis > distance)
                {
                    nearDis = distance;
                    CaptureTowerObject = natureTower;
                }
            }
        }
        else if (towerManager.getBlueTowerList().Count < towerManager.getRedTowerList().Count)
        {
            float nearDis = 0.0f;
            foreach (GameObject redTower in towerManager.getRedTowerList())
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
            foreach (GameObject blueTower in towerManager.getBlueTowerList())
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


    /// <summary>
    /// �ԌR�̃^���[�T������
    /// </summary>
    private void RedTowerSearch()
    {
        if (towerManager.getNatureTowerList().Count > 0)
        {
            //�����^���[���X�g�̒����狗������ԋ߂����̂�ڎw��
            float nearDis = 0.0f;
            foreach (GameObject natureTower in towerManager.getNatureTowerList())
            {
                float distance = Vector3.Distance(natureTower.transform.position, agent.transform.position);
                if (nearDis == 0 || nearDis > distance)
                {
                    nearDis = distance;
                    CaptureTowerObject = natureTower;
                }
            }
        }
        else if (towerManager.getRedTowerList().Count < towerManager.getBlueTowerList().Count)
        {
            //�G�R�̃^���[�����R�^���[��葽���̂ł����
            //�G�R�Ƃ��Đ�̂���Ă���^���[�̂������玩���Ƃ̋�������ԉ������̂�ڎw���i�^���[�ւ̍U����́j
            float nearDis = 0.0f;
            foreach (GameObject redTower in towerManager.getRedTowerList())
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
            foreach (GameObject blueTower in towerManager.getBlueTowerList())
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
