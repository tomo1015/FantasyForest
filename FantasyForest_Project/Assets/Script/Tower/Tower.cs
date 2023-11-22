using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private TEAM_COLOR tower_color = TEAM_COLOR.NATURAL;

    private float captureGauge = 0;//占領状態を表すゲージ
    List<GameObject> blueCharaList = new List<GameObject>();//青軍のキャラリスト
    List<GameObject> redCharaList = new List<GameObject>();//赤軍のキャラリスト
    private bool is_capture_blue = false;//青チームが占領しているフラグ
    private bool is_capture_red = false;//赤チームが占領しているフラグ
    private float blueCaptureLimit = 500;//青チームのものとして占領できるゲージ上限
    private float redCaptureLimit = -500;//赤チームのものとして占領できるゲージ上限
    private float naturalCaptureLimit = 0;//中立のものとして扱うゲージ上限
    private float captureGaugeValue = 0.1f;//占領中の基本ゲージ速度

    [SerializeField]
    private Material[] towerMaterials = new Material[3];

    //タワーのゲージ状態取得
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
        //タワー占領処理
        Capture();

        //タワーの状態を変える
        CangeTower();
    }

    private void SetUpTower()
    {
        switch (tower_color)
        {
            //TODO：メモリ気を付ける
            case TEAM_COLOR.NATURAL:
                //初期設定が中立状態のタワーなら
                //gameObject.GetComponent<Renderer>().material.color = towerMaterials[1].color;
                break;
            case TEAM_COLOR.RED:
                //初期設定が赤状態のタワーなら
                //gameObject.GetComponent<Renderer>().material.color = towerMaterials[0].color;
                break;
            case TEAM_COLOR.BLUE:
                //初期設定が青状態のタワーなら
                //gameObject.GetComponent<Renderer>().material.color = towerMaterials[2].color;
                break;
        }
    }

    /// <summary>
    /// タワー占領処理
    /// </summary>
    private void Capture()
    {
        //リストにいなければ占領中ではないと判断する
        //if(blueCharaList.Count <= 0 && redCharaList.Count <= 0) { return; }

        if (blueCharaList.Count > redCharaList.Count)
        {
            //青チームの人数が赤チームより多ければ青チームが占領していると判断
            is_capture_blue = true;
            is_capture_red = false;
        } 
        else if(blueCharaList.Count < redCharaList.Count) {
            //青チームの人数が赤チームより少なければ赤チームが占領していると判断
            is_capture_blue = false;
            is_capture_red = true;
        }
        else
        {
            //人数が同じの場合は占領できないとする
            is_capture_blue = false;
            is_capture_red = false;
        }

        //占領中のチームがある方に対応してゲージを増減させる
        if (is_capture_blue) { 
            //上限に達している場合はそれ以上カウントは増えない
            if(captureGauge > blueCaptureLimit) { return; }

            //占領中の人数が多いほど、ゲージの増加量は多い
            captureGauge += (captureGaugeValue * blueCharaList.Count); 
        }
        if(is_capture_red) {
            //上限に達している場合はそれ以上カウントは増えない
            if (captureGauge > redCaptureLimit) { return; }

            captureGauge -= (captureGaugeValue * redCharaList.Count); 
        }
    }

    /// <summary>
    /// コライダー判定処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //コライダーに入ってきたのが青チームなら
        if (other.gameObject.tag == "TEAM_BLUE")
        {
            //青チーム占領リストに追加
            blueCharaList.Add(other.gameObject);
        }

        //コライダーに入ってきたのが赤チームなら
        if (other.gameObject.tag == "TEAM_RED")
        {
            //赤チーム占領リストに追加
            redCharaList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //コライダーから出て行ったのが青チームなら
        if (other.gameObject.tag == "TEAM_BLUE")
        {
            //占領リストから削除
            blueCharaList.Remove(other.gameObject);
        }

        //コライダーから出て行ったのが赤チームなら
        if (other.gameObject.tag == "TEAM_RED")
        {
            //占領リストから削除
            redCharaList.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// タワーの状態を変更
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
