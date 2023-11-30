using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;

public class AICharacter : BaseCharacter
{
    //�i�r���b�V���G�[�W�F���g
    private NavMeshAgent agent;
    //AI�̏�ԊǗ�
    [SerializeField]
    private AI_STATUS ai_status;

    //���̊Ǘ��N���X
    public TowerManager towerManager;

    //���I�Ŋm�肳������̂��铃�̃I�u�W�F�N�g
    private GameObject CaptureObject;

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
                        CaptureObject = natureTower;
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
                        CaptureObject = redTower;
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
                        CaptureObject = blueTower;
                    }
                }
            }
        }

        //�ڎw���^���[�̈ʒu�����
        agent.SetDestination(CaptureObject.transform.position);

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

        //��͈͓̔��ł͂Ȃ����ړI�n�ɋ߂Â����ꍇ��
        //����A�j���[�V����������s�A�j���[�V�����֕ύX
        Vector3 TowerDiffPosition = CaptureObject.transform.position - agent.transform.position;
        if (Vector3.Magnitude(TowerDiffPosition) < 50)
        {
            base.StopAnimation(ANIMATION_STATE.RUN);
        }
        //�^���[�ɋ߂Â����ꍇ�i��͈͓̔��j��
        //�ړ����~���A��̃X�e�[�g�֕ύX

        if (Vector3.Magnitude(TowerDiffPosition) < 30)
        {
            //�^���[��͈͓̔�
            agent.speed = 0;
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;

            ai_status = AI_STATUS.CAPTURE;
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
        if(CaptureObject.GetComponent<Tower>().tower_color == team_color)
        {
            ai_status = AI_STATUS.SEARCH;
        }
    }
}
