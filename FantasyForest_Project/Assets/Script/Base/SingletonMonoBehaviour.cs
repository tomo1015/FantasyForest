using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// �V���O���g���p���N���X
/// </summary>
/// <typeparam name="T">�h���N���X��</typeparam>
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
        // ���̃Q�[���I�u�W�F�N�g�ɃA�^�b�`����Ă��邩���ׂ�
        // �A�^�b�`����Ă���ꍇ�͔j������B
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
        Destroy(gameObject);    // �Q�[���I�u�W�F�N�g���Ɣj��(�d�l�ɂ���Ă͕ύX)
        return false;
    }
}
