using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// シングルトン用基底クラス
/// </summary>
/// <typeparam name="T">派生クラス名</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);
                instance = (T)FindObjectOfType(t);
            }
            return instance;
        }
    }

    virtual protected void Awake()
    {
        // 他のゲームオブジェクトにアタッチされているか調べる
        // アタッチされている場合は破棄する。
        CheckInstance();
    }

    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        else if (Instance == this)
        {
            return true;
        }
        Destroy(gameObject);    // ゲームオブジェクトごと破棄(仕様によっては変更)
        return false;
    }
}
