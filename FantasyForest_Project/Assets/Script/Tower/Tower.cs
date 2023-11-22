using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private TEAM_COLOR tower_color = TEAM_COLOR.NATURAL;

    private float captureGauge = 0;//��̏�Ԃ�\���Q�[�W
    List<GameObject> blueCharaList = new List<GameObject>();//�R�̃L�������X�g
    List<GameObject> redCharaList = new List<GameObject>();//�ԌR�̃L�������X�g
    private bool is_capture_blue = false;//�`�[������̂��Ă���t���O
    private bool is_capture_red = false;//�ԃ`�[������̂��Ă���t���O
    private float blueCaptureLimit = 500;//�`�[���̂��̂Ƃ��Đ�̂ł���Q�[�W���
    private float redCaptureLimit = -500;//�ԃ`�[���̂��̂Ƃ��Đ�̂ł���Q�[�W���
    private float naturalCaptureLimit = 0;//�����̂��̂Ƃ��Ĉ����Q�[�W���
    private float captureGaugeValue = 0.1f;//��̒��̊�{�Q�[�W���x

    [SerializeField]
    private Material[] towerMaterials = new Material[3];

    //�^���[�̃Q�[�W��Ԏ擾
    public float GetGaptureGauge()
    {
        return captureGauge; 
    }


    // Start is called before the first frame update
    void Start()
    {
        SetUpTower();
    }

    // Update is called once per frame
    void Update()
    {
        //�^���[��̏���
        Capture();

        //�^���[�̏�Ԃ�ς���
        CangeTower();
    }

    private void SetUpTower()
    {
        switch (tower_color)
        {
            //TODO�F�������C��t����
            case TEAM_COLOR.NATURAL:
                //�����ݒ肪������Ԃ̃^���[�Ȃ�
                //gameObject.GetComponent<Renderer>().material.color = towerMaterials[1].color;
                break;
            case TEAM_COLOR.RED:
                //�����ݒ肪�ԏ�Ԃ̃^���[�Ȃ�
                //gameObject.GetComponent<Renderer>().material.color = towerMaterials[0].color;
                break;
            case TEAM_COLOR.BLUE:
                //�����ݒ肪��Ԃ̃^���[�Ȃ�
                //gameObject.GetComponent<Renderer>().material.color = towerMaterials[2].color;
                break;
        }
    }

    /// <summary>
    /// �^���[��̏���
    /// </summary>
    private void Capture()
    {
        //���X�g�ɂ��Ȃ���ΐ�̒��ł͂Ȃ��Ɣ��f����
        //if(blueCharaList.Count <= 0 && redCharaList.Count <= 0) { return; }

        if (blueCharaList.Count > redCharaList.Count)
        {
            //�`�[���̐l�����ԃ`�[����葽����ΐ`�[������̂��Ă���Ɣ��f
            is_capture_blue = true;
            is_capture_red = false;
        } 
        else if(blueCharaList.Count < redCharaList.Count) {
            //�`�[���̐l�����ԃ`�[����菭�Ȃ���ΐԃ`�[������̂��Ă���Ɣ��f
            is_capture_blue = false;
            is_capture_red = true;
        }
        else
        {
            //�l���������̏ꍇ�͐�̂ł��Ȃ��Ƃ���
            is_capture_blue = false;
            is_capture_red = false;
        }

        //��̒��̃`�[����������ɑΉ����ăQ�[�W�𑝌�������
        if (is_capture_blue) { 
            //����ɒB���Ă���ꍇ�͂���ȏ�J�E���g�͑����Ȃ�
            if(captureGauge > blueCaptureLimit) { return; }

            //��̒��̐l���������قǁA�Q�[�W�̑����ʂ͑���
            captureGauge += (captureGaugeValue * blueCharaList.Count); 
        }
        if(is_capture_red) {
            //����ɒB���Ă���ꍇ�͂���ȏ�J�E���g�͑����Ȃ�
            if (captureGauge > redCaptureLimit) { return; }

            captureGauge -= (captureGaugeValue * redCharaList.Count); 
        }
    }

    /// <summary>
    /// �R���C�_�[���菈��
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //�R���C�_�[�ɓ����Ă����̂��`�[���Ȃ�
        if (other.gameObject.tag == "TEAM_BLUE")
        {
            //�`�[����̃��X�g�ɒǉ�
            blueCharaList.Add(other.gameObject);
        }

        //�R���C�_�[�ɓ����Ă����̂��ԃ`�[���Ȃ�
        if (other.gameObject.tag == "TEAM_RED")
        {
            //�ԃ`�[����̃��X�g�ɒǉ�
            redCharaList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //�R���C�_�[����o�čs�����̂��`�[���Ȃ�
        if (other.gameObject.tag == "TEAM_BLUE")
        {
            //��̃��X�g����폜
            blueCharaList.Remove(other.gameObject);
        }

        //�R���C�_�[����o�čs�����̂��ԃ`�[���Ȃ�
        if (other.gameObject.tag == "TEAM_RED")
        {
            //��̃��X�g����폜
            redCharaList.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// �^���[�̏�Ԃ�ύX
    /// </summary>
    private void CangeTower()
    {
        if(captureGauge >= blueCaptureLimit)
        {
            tower_color = TEAM_COLOR.BLUE;
        }
        else if(captureGauge <= redCaptureLimit)
        {
            tower_color = TEAM_COLOR.RED;
        } 
        else if(captureGauge == naturalCaptureLimit)
        {
            tower_color = TEAM_COLOR.NATURAL;
        }
    }
}
