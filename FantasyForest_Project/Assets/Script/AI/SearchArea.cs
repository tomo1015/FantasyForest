using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class SearchArea : MonoBehaviour
{
    private GameObject parentObject;
    // Start is called before the first frame update
    void Start()
    {
        //親オブジェクトを取得
        parentObject = transform.parent.gameObject;
    }

    /// <summary>
    /// コライダー判定処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Character")
        {
            return;
        }


        //コライダーに入ってきたのが敵チームなら
        if (other.gameObject.GetComponent<BaseCharacter>().team_color != parentObject.GetComponent<BaseCharacter>().team_color)
        {
            //攻撃状態にセット
            parentObject.GetComponent<AICharacter>().setIsAttackMode(true);
            //占領に向かっていたタワーの情報を削除
            parentObject.GetComponent<AICharacter>().setCaptureObject(null);
            //攻撃対象のゲームオブジェクト設定
            parentObject.GetComponent<AICharacter>().setAttackObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Character")
        {
            return;
        }

        //コライダーから出て行ったのが青チームなら
        if (other.gameObject.GetComponent<BaseCharacter>().team_color != parentObject.GetComponent<BaseCharacter>().team_color)
        {
            //攻撃状態を解除
            parentObject.GetComponent<AICharacter>().setIsAttackMode(false);
            //攻撃対象のゲームオブジェクト初期化
            parentObject.GetComponent<AICharacter>().setAttackObject(null);
            //再度タワー探索処理へ
            parentObject.GetComponent<AICharacter>().setAiStatus(AI_STATUS.SEARCH);
        }
    }
}
