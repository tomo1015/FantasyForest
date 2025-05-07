using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

/// <summary>
/// キャラクターの索敵範囲を管理するクラス
/// </summary>
public class SearchArea : MonoBehaviour
{
    /// <summary>
    /// 親オブジェクト（AIキャラクター）
    /// </summary>
    private GameObject parentObject;

    /// <summary>
    /// AIキャラクターコンポーネントのキャッシュ
    /// </summary>
    private AICharacter aiCharacter;

    /// <summary>
    /// 親キャラクターのベースコンポーネントのキャッシュ
    /// </summary>
    private BaseCharacter parentCharacter;

    /// <summary>
    /// キャラクターのタグ名
    /// </summary>
    private const string CHARACTER_TAG = "Character";

    void Start()
    {
        parentObject = transform.parent.gameObject;
        aiCharacter = parentObject.GetComponent<AICharacter>();
        parentCharacter = parentObject.GetComponent<BaseCharacter>();

        if (aiCharacter == null || parentCharacter == null)
        {
            Debug.LogError("必要なコンポーネントが見つかりません: " + gameObject.name);
        }
    }

    /// <summary>
    /// 索敵範囲内に敵キャラクターが入った時の処理
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
    /// 索敵範囲から敵キャラクターが出た時の処理
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
    /// 対象が有効なキャラクターかどうかを判定
    /// </summary>
    private bool IsValidCharacter(Collider other)
    {
        return other != null && other.CompareTag(CHARACTER_TAG);
    }

    /// <summary>
    /// 対象が敵キャラクターかどうかを判定
    /// </summary>
    private bool IsEnemyCharacter(BaseCharacter otherCharacter)
    {
        return otherCharacter != null && 
               otherCharacter.team_color != parentCharacter.team_color;
    }

    /// <summary>
    /// 敵キャラクターが索敵範囲に入った時の処理
    /// </summary>
    private void HandleEnemyEnter(GameObject enemyObject)
    {
        if (aiCharacter == null) return;

        aiCharacter.IsAttackMode = true;
        aiCharacter.CaptureTower = null;
        aiCharacter.AttackTarget = enemyObject;
    }

    /// <summary>
    /// 敵キャラクターが索敵範囲から出た時の処理
    /// </summary>
    private void HandleEnemyExit()
    {
        if (aiCharacter == null) return;

        aiCharacter.IsAttackMode = false;
        aiCharacter.CaptureTower = null;
        aiCharacter.AiStatus = AI_STATUS.SEARCH;
    }
}
