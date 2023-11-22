using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private float captureGauge = 0;//��̏�Ԃ�\���Q�[�W

    [SerializeField]
    List<GameObject> blueCharaList;//�R�̃L�������X�g
    [SerializeField]
    List<GameObject> redCharaList;//�ԌR�̃L�������X�g

    private bool is_capture_blue = false;//�`�[������̂��Ă���t���O
    private bool is_capture_red = false;//�ԃ`�[������̂��Ă���t���O

    private float blueCaptureLimit = 500;
    private float redCaptureLimit = -500;

    private float captureGaugeValue = 0.1f;


    public float GetGaptureGauge()
    {
        return captureGauge; 
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�^���[��̏���
        Capture();
    }

    /// <summary>
    /// �^���[��̏���
    /// </summary>
    private void Capture()
    {
        //���X�g�ɂ��Ȃ���ΐ�̒��ł͂Ȃ��Ɣ��f����
        if(blueCharaList.Count <= 0 && redCharaList.Count <= 0) { return; }

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
}
