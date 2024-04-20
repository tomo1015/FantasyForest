using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Constants;
using UnityEngine.UIElements.Experimental;

public class RespownManager : SingletonMonoBehaviour<RespownManager>
{

    //リスポーン対象キャラクター
    public List<GameObject> standRespownList = new List<GameObject>();

    //リスポーンに掛かる時間
    private int RespownLimitTime = 100;

    //塔の管理クラス
    public TowerManager towerManager;

    private Vector3 respown_position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        exeRespown();
    }

    /// <summary>
    /// リスポーン実施処理
    /// </summary>
    private void exeRespown()
    {
        //リスポーン対象がいないなら処理を行わないようにする
        if (standRespownList.Count <= 0) { return; }

        //リスポーン対象クラスに登録されたキャラクター情報の取得
        //ゲームシーン内にあるタワーオブジェクトから
        //初期状態のチームカラーによって状態を振り分ける
        for (int i = 0; i < standRespownList.Count; i++)
        {
            var targetCharacter = standRespownList[i].GetComponent<BaseCharacter>();
            if (targetCharacter.RespownTime >= RespownLimitTime)
            {//対象のキャラクターがリスポーン時間を満たしたら

                switch (targetCharacter.team_color)
                {
                    case TEAM_COLOR.RED:
                        //タワーの情報を取得
                        int redTowerCount = towerManager.getRedTowerCount();
                        List<GameObject> redTowerList = towerManager.getRedTowerList();
                        //赤チーム用のリスポーン位置確定
                        respown_position = RespownTowerPosition(redTowerCount, redTowerList, targetCharacter);
                        break;
                    case TEAM_COLOR.BLUE:
                        //青チーム用のリスポーン位置確定
                        int blueTowerCount = towerManager.getRedTowerCount();
                        List<GameObject> blueTowerList = towerManager.getRedTowerList();
                        respown_position = RespownTowerPosition(blueTowerCount, blueTowerList, targetCharacter);
                        break;
                    default:
                        break;
                }

                //アクティブ状態にする
                targetCharacter.transform.position = respown_position;//リスポーン位置の設定
                targetCharacter.gameObject.SetActive(true);//オブジェクトの再表示

                //キャラクターステータスの設定
                targetCharacter.CharacterStatus();

                targetCharacter.RespownTime = 0;//管理時間を0にリセット

                //リスポーン対象リストから削除
                standRespownList.Remove(standRespownList[i]);
            } 
            else
            {
                //リスポーンできない場合はカウンターを増やす
                targetCharacter.RespownTime++;
            }
        }
    }

    /// <summary>
    /// タワーでのリスポーン位置を決定する
    /// </summary>
    /// <param name="towerCount"></param>
    /// <param name="towerList"></param>
    /// <param name="targetCharacter"></param>
    /// <returns></returns>
    private Vector3 RespownTowerPosition(int towerCount = 0, List<GameObject> towerList = null, BaseCharacter targetCharacter = null)
    {
        Vector3 respown_position = new Vector3(0, 0, 0);
        if (towerCount <= 0)
        {
            //占領しているタワーが0ならタワーでのリスポーンはできないと判断
            //TODO：この段階でのリスポーン位置は未定
            return respown_position;
        }

        //倒れた位置から一番遠いところのタワーからリスポーンするようにする
        //ただし中心のオブジェクトのタワーは対象外にする
        float nearDis = 0.0f;
        foreach (GameObject tower in towerList)
        {
            if (!tower.GetComponent<Tower>().IsTargetTowerRespown) { continue; }

            //一番遠いものを見つける
            float distance = Vector3.Distance(tower.transform.position, targetCharacter.transform.position);
            if (nearDis == 0 || nearDis > distance)
            {
                nearDis = distance;
                respown_position = tower.GetComponent<Tower>().TowerRespownLocation.transform.position;
            }
        }

        return respown_position;
    }
}
