using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

/// <summary>
/// 繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺ｮ邏｢謨ｵ遽�峇繧堤ｮ｡逅�☆繧九け繝ｩ繧ｹ
/// </summary>
public class SearchArea : MonoBehaviour
{
    /// <summary>
    /// 隕ｪ繧ｪ繝悶ず繧ｧ繧ｯ繝茨ｼ�I繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ�
    /// </summary>
    private GameObject parentObject;

    /// <summary>
    /// AI繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ繧ｳ繝ｳ繝昴�繝阪Φ繝医�繧ｭ繝｣繝�す繝･
    /// </summary>
    private AICharacter aiCharacter;

    /// <summary>
    /// 隕ｪ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺ｮ繝吶�繧ｹ繧ｳ繝ｳ繝昴�繝阪Φ繝医�繧ｭ繝｣繝�す繝･
    /// </summary>
    private BaseCharacter parentCharacter;

    /// <summary>
    /// 繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺ｮ繧ｿ繧ｰ蜷
    /// </summary>
    private const string CHARACTER_TAG = "Character";

    void Start()
    {
        parentObject = transform.parent.gameObject;
        aiCharacter = parentObject.GetComponent<AICharacter>();
        parentCharacter = parentObject.GetComponent<BaseCharacter>();

        if (aiCharacter == null || parentCharacter == null)
        {
            Debug.LogError("蠢�ｦ√↑繧ｳ繝ｳ繝昴�繝阪Φ繝医′隕九▽縺九ｊ縺ｾ縺帙ｓ: " + gameObject.name);
        }
    }

    /// <summary>
    /// 邏｢謨ｵ遽�峇蜀�↓謨ｵ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺悟�縺｣縺滓凾縺ｮ蜃ｦ逅
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidCharacter(other)) return;

        var otherCharacter = other.gameObject.GetComponent<BaseCharacter>();
        if (IsEnemyCharacter(otherCharacter))
        {
            HandleEnemyEnter(other.gameObject);
        }
    }

    /// <summary>
    /// 邏｢謨ｵ遽�峇縺九ｉ謨ｵ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺悟�縺滓凾縺ｮ蜃ｦ逅
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (!IsValidCharacter(other)) return;

        var otherCharacter = other.gameObject.GetComponent<BaseCharacter>();
        if (IsEnemyCharacter(otherCharacter))
        {
            HandleEnemyExit();
        }
    }

    /// <summary>
    /// 蟇ｾ雎｡縺梧怏蜉ｹ縺ｪ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺九←縺�°繧貞愛螳
    /// </summary>
    private bool IsValidCharacter(Collider other)
    {
        return other != null && other.CompareTag(CHARACTER_TAG);
    }

    /// <summary>
    /// 蟇ｾ雎｡縺梧雰繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺九←縺�°繧貞愛螳
    /// </summary>
    private bool IsEnemyCharacter(BaseCharacter otherCharacter)
    {
        return otherCharacter != null && 
               otherCharacter.team_color != parentCharacter.team_color;
    }

    /// <summary>
    /// 謨ｵ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺檎ｴ｢謨ｵ遽�峇縺ｫ蜈･縺｣縺滓凾縺ｮ蜃ｦ逅
    /// </summary>
    private void HandleEnemyEnter(GameObject enemyObject)
    {
        if (aiCharacter == null) return;

        aiCharacter.IsAttackMode = true;
        aiCharacter.CaptureTower = null;
        aiCharacter.AttackTarget = enemyObject;
    }

    /// <summary>
    /// 謨ｵ繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ縺檎ｴ｢謨ｵ遽�峇縺九ｉ蜃ｺ縺滓凾縺ｮ蜃ｦ逅
    /// </summary>
    private void HandleEnemyExit()
    {
        if (aiCharacter == null) return;

        aiCharacter.IsAttackMode = false;
        aiCharacter.CaptureTower = null;
        aiCharacter.AiStatus = AI_STATUS.SEARCH;
    }
}
