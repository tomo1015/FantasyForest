using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// MonoBehaviour拡張クラス
/// </summary>
public abstract class MonoBehaviourEX : MonoBehaviour
{
    protected delegate void FuncUpdateStep();   // 戻り値：falseで再更新（関数変更時に即更新したい時に使用。）
    protected FuncUpdateStep m_updateStep;      // 更新時に呼び出される関数（更新させたい関数処理を代入する。）

    /// <summary>
    /// 更新内容があるか
    /// </summary>
    public bool IsUpdateStep
    {
        get
        {
            return m_updateStep != null;
        }
    }

    protected virtual void Update()
    {
        m_updateStep?.Invoke();
    }
}