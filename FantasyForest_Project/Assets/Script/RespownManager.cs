using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Constants;
using UnityEngine.UIElements.Experimental;

public class RespownManager : SingletonMonoBehaviour<RespownManager>
{

    //���X�|�[���ΏۃL�����N�^�[
    public List<GameObject> standRespownList = new List<GameObject>();

    //���X�|�[���Ɋ|���鎞��
    private int RespownLimitTime = 100;

    //���̊Ǘ��N���X
    public TowerManager towerManager;

    private Vector3 respown_position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        exeRespown();
    }

    /// <summary>
    /// ���X�|�[�����{����
    /// </summary>
    private void exeRespown()
    {
        //���X�|�[���Ώۂ����Ȃ��Ȃ珈�����s��Ȃ��悤�ɂ���
        if (standRespownList.Count <= 0) { return; }

        //���X�|�[���ΏۃN���X�ɓo�^���ꂽ�L�����N�^�[���̎擾
        //�Q�[���V�[�����ɂ���^���[�I�u�W�F�N�g����
        //������Ԃ̃`�[���J���[�ɂ���ď�Ԃ�U�蕪����
        for (int i = 0; i < standRespownList.Count; i++)
        {
            var targetCharacter = standRespownList[i].GetComponent<BaseCharacter>();
            if (targetCharacter.RespownTime >= RespownLimitTime)
            {//�Ώۂ̃L�����N�^�[�����X�|�[�����Ԃ𖞂�������

                switch (targetCharacter.team_color)
                {
                    case TEAM_COLOR.RED:
                        //�^���[�̏����擾
                        int redTowerCount = towerManager.getRedTowerCount();
                        List<GameObject> redTowerList = towerManager.getRedTowerList();
                        //�ԃ`�[���p�̃��X�|�[���ʒu�m��
                        respown_position = RespownTowerPosition(redTowerCount, redTowerList, targetCharacter);
                        break;
                    case TEAM_COLOR.BLUE:
                        //�`�[���p�̃��X�|�[���ʒu�m��
                        int blueTowerCount = towerManager.getRedTowerCount();
                        List<GameObject> blueTowerList = towerManager.getRedTowerList();
                        respown_position = RespownTowerPosition(blueTowerCount, blueTowerList, targetCharacter);
                        break;
                    default:
                        break;
                }

                //�A�N�e�B�u��Ԃɂ���
                targetCharacter.transform.position = respown_position;//���X�|�[���ʒu�̐ݒ�
                targetCharacter.gameObject.SetActive(true);//�I�u�W�F�N�g�̍ĕ\��

                //�L�����N�^�[�X�e�[�^�X�̐ݒ�
                targetCharacter.CharacterStatus();

                targetCharacter.RespownTime = 0;//�Ǘ����Ԃ�0�Ƀ��Z�b�g

                //���X�|�[���Ώۃ��X�g����폜
                standRespownList.Remove(standRespownList[i]);
            } 
            else
            {
                //���X�|�[���ł��Ȃ��ꍇ�̓J�E���^�[�𑝂₷
                targetCharacter.RespownTime++;
            }
        }
    }

    /// <summary>
    /// �^���[�ł̃��X�|�[���ʒu�����肷��
    /// </summary>
    /// <param name="towerCount"></param>
    /// <param name="towerList"></param>
    /// <param name="targetCharacter"></param>
    /// <returns></returns>
    private Vector3 RespownTowerPosition(int towerCount = 0, List<GameObject> towerList = null, BaseCharacter targetCharacter = null)
    {
        Vector3 respown_position = new Vector3(0, 0, 0);
        if (towerCount <= 0)
        {
            //��̂��Ă���^���[��0�Ȃ�^���[�ł̃��X�|�[���͂ł��Ȃ��Ɣ��f
            //TODO�F���̒i�K�ł̃��X�|�[���ʒu�͖���
            return respown_position;
        }

        //�|�ꂽ�ʒu�����ԉ����Ƃ���̃^���[���烊�X�|�[������悤�ɂ���
        //���������S�̃I�u�W�F�N�g�̃^���[�͑ΏۊO�ɂ���
        float nearDis = 0.0f;
        foreach (GameObject tower in towerList)
        {
            if (!tower.GetComponent<Tower>().IsTargetTowerRespown) { continue; }

            //��ԉ������̂�������
            float distance = Vector3.Distance(tower.transform.position, targetCharacter.transform.position);
            if (nearDis == 0 || nearDis > distance)
            {
                nearDis = distance;
                respown_position = tower.GetComponent<Tower>().TowerRespownLocation.transform.position;
            }
        }

        return respown_position;
    }
}
