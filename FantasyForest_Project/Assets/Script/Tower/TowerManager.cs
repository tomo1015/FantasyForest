using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : SingletonMonoBehaviour<TowerManager>
{
    [SerializeField]
    List<GameObject> towerList = new List<GameObject>();

    [SerializeField]
    List<GameObject> blueTowerList = new List<GameObject>();
    [SerializeField]
    List<GameObject> redTowerList = new List<GameObject>();
    [SerializeField]
    List<GameObject> natureTowerList = new List<GameObject>();


    public List<GameObject> getBlueTowerList() { return blueTowerList; }
    public List<GameObject> getRedTowerList() { return redTowerList; }
    public List<GameObject> getNatureTowerList() { return natureTowerList; }

    private void Start()
    {
        //ゲームシーン内にあるタワーオブジェクトから
        //初期状態のチームカラーによって状態を振り分ける
        for(int i = 0; i < towerList.Count; i++){
            switch (towerList[i].GetComponent<Tower>().tower_color)
            {
                case Constants.TEAM_COLOR.RED:
                    redTowerList.Add(towerList[i]);
                    break;

                case Constants.TEAM_COLOR.BLUE:
                    blueTowerList.Add(towerList[i]);
                    break;

                case Constants.TEAM_COLOR.NATURAL:
                    natureTowerList.Add(towerList[i]);
                    break;
            }
        }
    }

    private void Update()
    {
        //タワーの占領状態を更新
    }
}
