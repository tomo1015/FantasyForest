using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class Tower : MonoBehaviour
{
    public TEAM_COLOR tower_color;  // タワーの所属チーム

    // タワーの状態
    private float captureGauge = 500;  // 占領状態を表すゲージ

    [SerializeField]
    List<GameObject> blueCharaList = new List<GameObject>();  // 青チームのキャラリスト
    public List<GameObject> getBlueCharaList() { return blueCharaList; }

    [SerializeField]
    List<GameObject> redCharaList = new List<GameObject>();  // 赤チームのキャラリスト
    public List<GameObject> getRedCharaList() { return redCharaList; }

    private bool is_capture_blue = false;  // 青チームが占領中フラグ
    private bool is_capture_red = false;   // 赤チーム占領中フラグ
    private float blueCaptureLimit = 1000; // 青チームの場合の占領可能ゲージ上限 TODO：テスト
    private float redCaptureLimit = 0;     // 赤チームの場合の占領可能ゲージ上限 TODO：テスト
    private float naturalCaptureLimit = 500;// 中立の場合の基準ゲージ値
    private float captureGaugeValue = 0.1f;// 占領時の基本ゲージ速度
    
    private int captureRange = 25;  // AIが占領を試みるタワーの有効範囲
    public int getCaptureRange() { return captureRange; }

    // 本拠地かどうか
    [SerializeField]
    private bool isMainTower = false;
    public bool getIsMainTower() {  return isMainTower; }

    // 防衛中のキャラクター数
    public List<GameObject> defenseCharacterList = new List<GameObject>();

    [SerializeField]
    private Material[] towerMaterials = new Material[3];

    public Transform[] defensePatrolPosition;  // 巡回ポイント

    // タワー管理クラス
    [SerializeField]
    private TowerManager towerManager;
    public GameObject TowerRespownLocation;
    public bool IsTargetTowerRespown;  // タワーからリスポーン可能かどうか

    // タワーのゲージ状態取得
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
        // タワー占領処理
        Capture();

        // タワーの状態を変更
        CangeTower();
    }

    private void SetUpTower()
    {
        // 初期設定のタワーに応じてタワーの色を変更
        switch (tower_color)
        {
            // TODO：マテリアル適用
            case TEAM_COLOR.NATURAL:
                tower_color = TEAM_COLOR.NATURAL;
                captureGauge = naturalCaptureLimit;
                // 初期設定が中立状態のタワーなら
                // gameObject.GetComponent<Renderer>().material.color = towerMaterials[1].color;
                break;
            case TEAM_COLOR.RED:
                tower_color = TEAM_COLOR.RED;
                captureGauge = redCaptureLimit;
                // 初期設定が赤状態のタワーなら
                // gameObject.GetComponent<Renderer>().material.color = towerMaterials[0].color;
                break;
            case TEAM_COLOR.BLUE:
                tower_color = TEAM_COLOR.BLUE;
                // 占領ゲージを変更
                captureGauge = blueCaptureLimit;
                // 初期設定が青状態のタワーなら
                // gameObject.GetComponent<Renderer>().material.color = towerMaterials[2].color;
                break;
        }
    }

    /// <summary>
    /// タワー占領処理
    /// </summary>
    private void Capture()
    {
        if (blueCharaList.Count > redCharaList.Count)
        {
            // 青チームの人数が赤チームより多ければ青チームが占領中と判定
            is_capture_blue = true;
            is_capture_red = false;
        } 
        else if(blueCharaList.Count < redCharaList.Count) {
            // 青チームの人数が赤チームより少なければ赤チームが占領中と判定
            is_capture_blue = false;
            is_capture_red = true;
        }
        else
        {
            // 人数が同じ場合は占領できないとする
            is_capture_blue = false;
            is_capture_red = false;
        }

        // 占領中のチームの状況に応じてゲージを増減させる
        if (is_capture_blue) { 
            // 上限に達している場合はこれ以上カウントは行わない
            if(captureGauge > blueCaptureLimit) { return; }

            // 占領中の人数が多いほど、ゲージの増加量は多い
            captureGauge += (captureGaugeValue * blueCharaList.Count); 
        }

        if(is_capture_red) {
            // 上限に達している場合はこれ以上カウントは行わない
            if (captureGauge < redCaptureLimit) { return; }

            captureGauge -= (captureGaugeValue * redCharaList.Count); 
        }
    }

    /// <summary>
    /// コライダー侵入処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // コライダーに入ってきたのが青チームなら
        if (other.gameObject.GetComponent<BaseCharacter>().team_color == TEAM_COLOR.BLUE)
        {
            // 青チーム側のリストに追加
            blueCharaList.Add(other.gameObject);
        }

        // コライダーに入ってきたのが赤チームなら
        if (other.gameObject.GetComponent<BaseCharacter>().team_color == TEAM_COLOR.RED)
        {
            // 赤チーム側のリストに追加
            redCharaList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // コライダーから出て行ったのが青チームなら
        if (other.gameObject.GetComponent<BaseCharacter>().team_color == TEAM_COLOR.BLUE)
        {
            // 青のリストから削除
            blueCharaList.Remove(other.gameObject);
        }

        // コライダーから出て行ったのが赤チームなら
        if (other.gameObject.GetComponent<BaseCharacter>().team_color == TEAM_COLOR.RED)
        {
            // 赤のリストから削除
            redCharaList.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// タワーの状態を変更
    /// </summary>
    private void CangeTower()
    {
        List<GameObject> blueTowerList = towerManager.getBlueTowerList();
        List<GameObject> redTowerList = towerManager.getRedTowerList();
        List<GameObject> natureTowerList = towerManager.getNatureTowerList();
        if (captureGauge >= blueCaptureLimit)
        {
            // 青チームの所有に変更
            tower_color = TEAM_COLOR.BLUE;
            // 中立状態のタワーリストから該当するものを削除し
            // 青のタワーリストに追加する
            if (natureTowerList.Contains(gameObject) == true)
            {
                natureTowerList.Remove(gameObject);
                blueTowerList.Add(gameObject);
            }
        }
        else if(captureGauge <= redCaptureLimit)
        {
            // 赤チームの所有に変更
            tower_color = TEAM_COLOR.RED;
            // 中立状態のタワーリストから該当するものを削除し
            // 赤チームのタワーリストに追加する
            if (natureTowerList.Contains(gameObject) == true)
            {
                natureTowerList.Remove(gameObject);
                redTowerList.Add(gameObject);
            }
        } 
        else if(captureGauge == naturalCaptureLimit)
        {
            // 中立状態に変更
            tower_color = TEAM_COLOR.NATURAL;

            if (blueTowerList.Contains(gameObject) == true)
            {
                // 青のタワーリストにある場合はリストから除外
                // 中立状態のタワーリストへ追加
                blueTowerList.Remove(gameObject);
                natureTowerList.Add(gameObject);

            } else if(redTowerList.Contains(gameObject) == true){
                // 赤チームのタワーリストにある場合はリストから除外
                // 中立状態のタワーリストへ追加
                redTowerList.Remove(gameObject);
                natureTowerList.Add(gameObject);
            }
        }
    }
}
