using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Respown();
    }

    private void Respown()
    {
        //リスポーン対象クラスに登録されたキャラクター情報の取得
        //ゲームシーン内にあるタワーオブジェクトから
        //初期状態のチームカラーによって状態を振り分ける
        for (int i = 0; i < standRespownList.Count; i++)
        {
            var targetCharacter = standRespownList[i].GetComponent<BaseCharacter>();
            if (targetCharacter.RespownTime >= RespownLimitTime)
            {
                //対象のキャラクターがリスポーン時間を満たしたら
                if(targetCharacter.team_color == Constants.TEAM_COLOR.RED)
                {
                    //そのキャラクターが赤色のチームの場合
                    //倒れた位置から一番遠いところのタワーからリスポーンするようにする
                    //ただし中心のオブジェクトのタワーは対象外にする
                    int redTowerCount = towerManager.getRedTowerCount();
                    if (redTowerCount > 0)
                    {
                        //赤色のタワーリスト
                        List<GameObject> redTowerList = towerManager.getRedTowerList();
                        float nearDis = 0.0f;
                        foreach (GameObject redTower in redTowerList)
                        {
                            if (!redTower.GetComponent<Tower>().IsTargetTowerRespown)
                            {
                                continue;
                            }

                            //一番遠いものを見つける
                            float distance = Vector3.Distance(redTower.transform.position, targetCharacter.transform.position);
                            if (nearDis == 0 || nearDis > distance)
                            {
                                nearDis = distance;
                                respown_position = redTower.GetComponent<Tower>().TowerRespownLocation.transform.position;
                            }
                        }
                    }
                }

                //アクティブ状態にする
                targetCharacter.start_position = respown_position;//リスポーン位置の設定
                targetCharacter.setActive(true);
                targetCharacter.RespownTime = 0;//管理時間を0にリセット

                //リスポーン対象リストから削除
                standRespownList.Remove(standRespownList[i]);
            } else
            {
                //リスポーンできない場合はカウンターを増やす
                targetCharacter.RespownTime++;
            }
        }
    }
}
