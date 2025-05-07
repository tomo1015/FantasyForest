using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェードオブジェクトの基底クラス
/// フェードのバリエーションを増やす場合はこのクラスを継承
/// </summary>
public abstract class Fade : MonoBehaviourEX
{ 

    public readonly struct SortingLayerName
    {
        public const string Back = "Back";
        public const string Default = "Default";
        public const string Flont = "Flont";
        public const string Fade = "Fade";
    }

    protected Canvas canvas = null;

    /// <summary>
    /// フェードアウトかどうか
    /// </summary>
    public virtual bool isFadeOut
    {
        get
        {
            return m_updateStep == FadeOutStep;
        }
    }


    /// <summary>
    /// フェードインかどうか
    /// </summary>
    public virtual bool isDFadeIn
    {
        get
        {
            return m_updateStep == FadeInStep;
        }
    }

    /// <summary>
    /// フェードイン開始
    /// </summary>
    public virtual void StartFadeIn()
    {
        m_updateStep = FadeInStep;
    }


    protected virtual void Awake()
    {
        //シーンを超えるので破棄されないように設定
        DontDestroyOnLoad(gameObject);

        //フェードアウト開始
        m_updateStep = FadeOutStep;

        //キャンバス設定
        canvas = GetComponent<Canvas>(); 
        canvas.enabled = true;
        canvas.sortingLayerName = SortingLayerName.Fade;
    }

    /// <summary>
    /// フェードアウト時の更新
    /// </summary>
    protected abstract void FadeOutStep();

    /// <summary>
    /// フェードイン時の更新
    /// </summary>
    protected abstract void FadeInStep();

    /// <summary>
    /// 終了時の更新
    /// </summary>
    protected virtual void EndStep()
    {
        Destroy(gameObject);
    }
}