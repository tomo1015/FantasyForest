using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class SearchArea : MonoBehaviour
{
    private GameObject parentObject;
    // Start is called before the first frame update
    void Start()
    {
        //�e�I�u�W�F�N�g���擾
        parentObject = transform.parent.gameObject;
    }

    /// <summary>
    /// �R���C�_�[���菈��
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Character")
        {
            return;
        }


        //�R���C�_�[�ɓ����Ă����̂��G�`�[���Ȃ�
        if (other.gameObject.GetComponent<BaseCharacter>().team_color != parentObject.GetComponent<BaseCharacter>().team_color)
        {
            //�U����ԂɃZ�b�g
            parentObject.GetComponent<AICharacter>().setIsAttackMode(true);
            //��̂Ɍ������Ă����^���[�̏����폜
            parentObject.GetComponent<AICharacter>().setCaptureObject(null);
            //�U���Ώۂ̃Q�[���I�u�W�F�N�g�ݒ�
            parentObject.GetComponent<AICharacter>().setAttackObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Character")
        {
            return;
        }

        //�R���C�_�[����o�čs�����̂��`�[���Ȃ�
        if (other.gameObject.GetComponent<BaseCharacter>().team_color != parentObject.GetComponent<BaseCharacter>().team_color)
        {
            //�U����Ԃ�����
            parentObject.GetComponent<AICharacter>().setIsAttackMode(false);
            //�U���Ώۂ̃Q�[���I�u�W�F�N�g������
            parentObject.GetComponent<AICharacter>().setAttackObject(null);
            //�ēx�^���[�T��������
            parentObject.GetComponent<AICharacter>().setAiStatus(AI_STATUS.SEARCH);
        }
    }
}
