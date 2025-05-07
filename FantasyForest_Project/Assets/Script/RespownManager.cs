using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Constants;
using UnityEngine.UIElements.Experimental;
using System;

/// <summary>
/// 繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺ｮ繝ｪ繧ｹ繝昴�繝ｳ繧堤ｮ｡逅�☆繧九け繝ｩ繧ｹ
/// </summary>
public class RespownManager : SingletonMonoBehaviour<RespownManager>
{
    /// <summary>
    /// 繝ｪ繧ｹ繝昴�繝ｳ蠕�ｩ滉ｸｭ縺ｮ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ繝ｪ繧ｹ繝
    /// </summary>
    public List<GameObject> standRespownList = new List<GameObject>();

    /// <summary>
    /// 繝ｪ繧ｹ繝昴�繝ｳ縺ｾ縺ｧ縺ｮ蠕�ｩ滓凾髢
    /// </summary>
    private const int RESPAWN_LIMIT_TIME = 100;

    /// <summary>
    /// 繧ｿ繝ｯ繝ｼ邂｡逅�け繝ｩ繧ｹ
    /// </summary>
    public TowerManager towerManager;

    [SerializeField]
    private List<GameObject> respownPosition;

    /// <summary>
    /// 繝ｪ繧ｹ繝昴�繝ｳ蜃ｦ逅�ｒ螳溯｡後☆繧
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
    /// 繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ繧偵Μ繧ｹ繝昴�繝ｳ縺輔○繧
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
    /// 繝√�繝縺ｫ蠢懊§縺溘Μ繧ｹ繝昴�繝ｳ菴咲ｽｮ繧貞叙蠕励☆繧
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
    /// 繝√�繝縺ｮ繧ｿ繝ｯ繝ｼ縺ｫ蝓ｺ縺･縺�※繝ｪ繧ｹ繝昴�繝ｳ菴咲ｽｮ繧呈ｱｺ螳壹☆繧
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
    /// 繝�ヵ繧ｩ繝ｫ繝医�繝ｪ繧ｹ繝昴�繝ｳ菴咲ｽｮ繧貞叙蠕励☆繧
    /// </summary>
    private Vector3 GetDefaultRespawnPosition()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        int randomIndex = UnityEngine.Random.Range(1, 4);
        return respownPosition[randomIndex].transform.position;
    }

    /// <summary>
    /// 譛繧りｿ代＞繧ｿ繝ｯ繝ｼ縺ｮ繝ｪ繧ｹ繝昴�繝ｳ菴咲ｽｮ繧呈爾縺
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
