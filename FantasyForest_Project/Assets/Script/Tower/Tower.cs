using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private float captureGauge = 0;//占領状態を表すゲージ

    [SerializeField]
    List<GameObject> blueCharaList;//青軍のキャラリスト
    [SerializeField]
    List<GameObject> redCharaList;//赤軍のキャラリスト

    private bool is_capture_blue = false;//青チームが占領しているフラグ
    private bool is_capture_red = false;//赤チームが占領しているフラグ

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
        //タワー占領処理
        Capture();
    }

    /// <summary>
    /// タワー占領処理
    /// </summary>
    private void Capture()
    {
        //リストにいなければ占領中ではないと判断する
        if(blueCharaList.Count <= 0 && redCharaList.Count <= 0) { return; }

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
}
