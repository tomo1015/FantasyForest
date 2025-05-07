using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// シーン管理クラス
/// </summary>
public class SceneManager : MonoBehaviour
{
    private string nextSceneName = string.Empty;//次のシーン名
    private static string currentSceneName = string.Empty;//現在のシーン名
    private static string prevSceneName = string.Empty;//前のシーン名

    public void Awake()
    {
        
    }

    /// <summary>
    /// シーン変更を行うメソッド
    /// </summary>
    public void changeScene(string sceneName, bool isSave)
    {
        nextScene(sceneName, isSave);
    }

    /// <summary>
    /// シーン再読み込み
    /// </summary>
    public void reloadScene()
    {
        nextScene(currentSceneName, false);
    }

    private void nextScene(string sceneName, bool isSave)
    {
        //次のシーン名が空でない場合は処理を行わない
        if (nextSceneName != string.Empty) return;

        //フェードオブジェクト生成

        //次に遷移するシーン名を取得
        nextSceneName = sceneName;

        //シーン遷移をコルーチンに委託

        //シーン遷移時にセーブを行う場合
        if (isSave)
        {
            //ロードUI生成

            //データのセーブを行う
        }
    }
}
