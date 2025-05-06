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
        if (standRespownList.Count <= 0) { return; }

        for (int i = 0; i < standRespownList.Count; i++)
        {
            var targetCharacter = standRespownList[i].GetComponent<BaseCharacter>();
            if (targetCharacter.RespownTime >= RESPAWN_LIMIT_TIME)
            {
                RespawnCharacter(targetCharacter);
                standRespownList.RemoveAt(i);
            }
            else
            {
                targetCharacter.RespownTime++;
            }
        }
    }

    /// <summary>
    /// キャラクターをリスポーンさせる
    /// </summary>
    private void RespawnCharacter(BaseCharacter targetCharacter)
    {
        Vector3 respawnPos = GetRespawnPosition(targetCharacter);
        targetCharacter.transform.position = respawnPos;

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
            case TEAM_COLOR.RED:
                return GetTeamRespawnPosition(towerManager.getRedTowerCount(), 
                                           towerManager.getRedTowerList(), 
                                           targetCharacter);
            case TEAM_COLOR.BLUE:
                return GetTeamRespawnPosition(towerManager.getBlueTowerCount(), 
                                           towerManager.getBlueTowerList(), 
                                           targetCharacter);
            default:
                return GetDefaultRespawnPosition();
        }
    }

    /// <summary>
    /// チームのタワーに基づいてリスポーン位置を決定する
    /// </summary>
    private Vector3 GetTeamRespawnPosition(int towerCount, List<GameObject> towerList, BaseCharacter targetCharacter)
    {
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
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
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
            if (!towerComponent.IsTargetTowerRespown) { continue; }

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
