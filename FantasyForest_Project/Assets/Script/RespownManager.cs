using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Constants;
using UnityEngine.UIElements.Experimental;
using System;

/// <summary>
/// キャラクターのリスポーンを管理するクラス
/// </summary>
public class RespownManager : SingletonMonoBehaviour<RespownManager>
{
    /// <summary>
    /// リスポーン待機中のキャラクターリスト
    /// </summary>
    public List<GameObject> standRespownList = new List<GameObject>();

    /// <summary>
    /// リスポーンまでの待機時間
    /// </summary>
    private const int RESPAWN_LIMIT_TIME = 100;

    /// <summary>
    /// タワー管理クラス
    /// </summary>
    public TowerManager towerManager;

    [SerializeField]
    private List<GameObject> respownPosition;

    /// <summary>
    /// リスポーン処理を実行する
    /// </summary>
    private void exeRespown()
    {
        // リスポーン待機リストが空の場合は処理を終了
        if (standRespownList.Count <= 0) { return; }

        for (int i = 0; i < standRespownList.Count; i++)
        {
            var targetCharacter = standRespownList[i].GetComponent<BaseCharacter>();
            // リスポーン時間が制限時間に達した場合
            if (targetCharacter.RespownTime >= RESPAWN_LIMIT_TIME)
            {
                // キャラクターをリスポーンさせる
                RespawnCharacter(targetCharacter);
                standRespownList.RemoveAt(i);
            }
            else
            {
                // リスポーン待機時間をカウントアップ
                targetCharacter.RespownTime++;
            }
        }
    }

    /// <summary>
    /// キャラクターをリスポーンさせる
    /// </summary>
    private void RespawnCharacter(BaseCharacter targetCharacter)
    {
        // リスポーン位置を取得
        Vector3 respawnPos = GetRespawnPosition(targetCharacter);
        targetCharacter.transform.position = respawnPos;

        // キャラクターを有効化し、ステータスを初期化
        targetCharacter.gameObject.SetActive(true);
        targetCharacter.CharacterStatus();
        targetCharacter.RespownTime = 0;
    }

    /// <summary>
    /// チームに応じたリスポーン位置を取得する
    /// </summary>
    private Vector3 GetRespawnPosition(BaseCharacter targetCharacter)
    {
        switch (targetCharacter.team_color)
        {
            // case TEAM_COLOR.RED:
            //     return GetTeamRespawnPosition(towerManager.getRedTowerCount(), 
            //                                towerManager.getRedTowerList(), 
            //                                targetCharacter);
            // case TEAM_COLOR.BLUE:
            //     return GetTeamRespawnPosition(towerManager.getBlueTowerCount(), 
            //                                towerManager.getBlueTowerList(), 
            //                                targetCharacter);
            default:
                return GetDefaultRespawnPosition();
        }
    }

    /// <summary>
    /// チームのタワーに基づいてリスポーン位置を決定する
    /// </summary>
    private Vector3 GetTeamRespawnPosition(int towerCount, List<GameObject> towerList, BaseCharacter targetCharacter)
    {
        // タワーが存在しない場合はデフォルトのリスポーン位置を返す
        if (towerCount <= 0)
        {
            return GetDefaultRespawnPosition();
        }

        return FindNearestTowerRespawnPosition(towerList, targetCharacter);
    }

    /// <summary>
    /// デフォルトのリスポーン位置を取得する
    /// </summary>
    private Vector3 GetDefaultRespawnPosition()
    {
        // ランダムシードを現在時刻で初期化
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        // ランダムなリスポーン位置を選択
        int randomIndex = UnityEngine.Random.Range(1, 4);
        return respownPosition[randomIndex].transform.position;
    }

    /// <summary>
    /// 最も近いタワーのリスポーン位置を探す
    /// </summary>
    private Vector3 FindNearestTowerRespawnPosition(List<GameObject> towerList, BaseCharacter targetCharacter)
    {
        float nearestDistance = float.MaxValue;
        Vector3 respawnPosition = Vector3.zero;

        foreach (GameObject tower in towerList)
        {
            var towerComponent = tower.GetComponent<Tower>();
            // リスポーン不可能なタワーはスキップ
            if (!towerComponent.IsTargetTowerRespown) { continue; }

            // タワーとキャラクター間の距離を計算
            float distance = Vector3.Distance(tower.transform.position, targetCharacter.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                respawnPosition = towerComponent.TowerRespownLocation.transform.position;
            }
        }

        return respawnPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        exeRespown();
    }
}
