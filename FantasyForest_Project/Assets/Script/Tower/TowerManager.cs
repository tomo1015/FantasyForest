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

    public int getBlueTowerCount() { return blueTowerList.Count; }
    public int getRedTowerCount() { return redTowerList.Count; }
    public int getNatureTowerCount() { return natureTowerList.Count; }

    private void Start()
    {
        //Q[V[ɂ^[IuWFNg
        //Ԃ̃`[J[ɂďԂU蕪
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
        //^[̐̏ԂXV
    }
}
