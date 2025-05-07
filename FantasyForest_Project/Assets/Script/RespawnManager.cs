using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Constants;
using UnityEngine.UIElements.Experimental;
using System;

/// <summary>
/// キャラクターのリスポーンを管理するクラス
/// </summary>
public class RespawnManager : SingletonMonoBehaviour<RespawnManager>
{
    /// <summary>
    /// リスポーン待機中のキャラクターリスト
    /// </summary>
    public List<GameObject> standRespawnList = new List<GameObject>();

    /// <summary>
    /// リスポーンまでの待機時間（フレーム数）
    /// </summary>
    private const int RESPAWN_LIMIT_FRAMES = 100;

    /// <summary>
    /// タワー管理クラス
    /// </summary>
    public TowerManager towerManager;

    [SerializeField]
    private List<GameObject> respawnPosition;

    /// <summary>
    /// リスポーン処理を実行する
    /// </summary>
    private void ExecuteRespawn()
    {
        // リスポーン待機リストが空の場合は処理を終了
        if (standRespawnList.Count <= 0) { return; }

        for (int i = 0; i < standRespawnList.Count; i++)
        {
            var targetCharacter = standRespawnList[i].GetComponent<BaseCharacter>();
            // リスポーン時間が制限時間に達した場合
            if (targetCharacter.RespownTime >= RESPAWN_LIMIT_FRAMES)
            {
                // キャラクターをリスポーンさせる
                RespawnCharacter(targetCharacter);
                standRespawnList.RemoveAt(i);
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
        return GetDefaultRespawnPosition();
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
        return respawnPosition[randomIndex].transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteRespawn();
    }
}
