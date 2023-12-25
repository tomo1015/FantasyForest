using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Respown();
    }

    private void Respown()
    {
        //���X�|�[���ΏۃN���X�ɓo�^���ꂽ�L�����N�^�[���̎擾
        //�Q�[���V�[�����ɂ���^���[�I�u�W�F�N�g����
        //������Ԃ̃`�[���J���[�ɂ���ď�Ԃ�U�蕪����
        for (int i = 0; i < standRespownList.Count; i++)
        {
            var targetCharacter = standRespownList[i].GetComponent<BaseCharacter>();
            if (targetCharacter.RespownTime >= RespownLimitTime)
            {
                //�Ώۂ̃L�����N�^�[�����X�|�[�����Ԃ𖞂�������
                if(targetCharacter.team_color == Constants.TEAM_COLOR.RED)
                {
                    //���̃L�����N�^�[���ԐF�̃`�[���̏ꍇ
                    //�|�ꂽ�ʒu�����ԉ����Ƃ���̃^���[���烊�X�|�[������悤�ɂ���
                    //���������S�̃I�u�W�F�N�g�̃^���[�͑ΏۊO�ɂ���
                    int redTowerCount = towerManager.getRedTowerCount();
                    if (redTowerCount > 0)
                    {
                        //�ԐF�̃^���[���X�g
                        List<GameObject> redTowerList = towerManager.getRedTowerList();
                        float nearDis = 0.0f;
                        foreach (GameObject redTower in redTowerList)
                        {
                            if (!redTower.GetComponent<Tower>().IsTargetTowerRespown)
                            {
                                continue;
                            }

                            //��ԉ������̂�������
                            float distance = Vector3.Distance(redTower.transform.position, targetCharacter.transform.position);
                            if (nearDis == 0 || nearDis > distance)
                            {
                                nearDis = distance;
                                respown_position = redTower.GetComponent<Tower>().TowerRespownLocation.transform.position;
                            }
                        }
                    }
                }

                //�A�N�e�B�u��Ԃɂ���
                targetCharacter.start_position = respown_position;//���X�|�[���ʒu�̐ݒ�
                targetCharacter.setActive(true);
                targetCharacter.RespownTime = 0;//�Ǘ����Ԃ�0�Ƀ��Z�b�g

                //���X�|�[���Ώۃ��X�g����폜
                standRespownList.Remove(standRespownList[i]);
            } else
            {
                //���X�|�[���ł��Ȃ��ꍇ�̓J�E���^�[�𑝂₷
                targetCharacter.RespownTime++;
            }
        }
    }
}
