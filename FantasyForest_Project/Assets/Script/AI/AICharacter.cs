using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacter : BaseCharacter
{
    public enum AI_STATUS
    {
        NONE,//�������Ă��Ȃ�
        SEARCH,//�T��
        CAPTURE,//���
        MOVE,//�ړ�
        ATTACK,//�U��
    }

    //�i�r���b�V���G�[�W�F���g
    private NavMeshAgent agent;


    //AI�̏�ԊǗ�
    private AI_STATUS ai_status;



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
        bool is_active = base.getActive();

        //�������Ă��Ȃ��Ȃ珈�����Ȃ�
        if (!is_active) { return; }

        switch (ai_status)
        {
            case AI_STATUS.SEARCH:
                //�T���������s
                break;
            case AI_STATUS.CAPTURE:
                //��̏������s
                break;
            case AI_STATUS.MOVE:
                //�ړ��������s
                break;
            case AI_STATUS.ATTACK:
                //�U���������s
                break;
            default: 
                break; 
        }
    }
}
