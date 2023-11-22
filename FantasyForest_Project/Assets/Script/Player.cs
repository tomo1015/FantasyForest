using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseCharacter
{
    // Start is called before the first frame update
    protected override void Start()
    {
        //ベースクラス処理
        base.Start();
        Debug.Log("派生クラスのStart");

        //プレイヤー初期設定 
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();   
        Debug.Log("派生クラスのUpdate");
    }
}
