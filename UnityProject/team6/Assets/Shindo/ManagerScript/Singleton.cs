using UnityEngine;
using System;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

    private static T GetInstance_;
    
    public static T GetInstance
    {
        get
        {
            if (GetInstance_ == null)
            {
                GetInstance_ = (T)FindObjectOfType(typeof(T));

                if (GetInstance_ == null)
                {
                    Debug.LogWarning(typeof(T) + "is nothing");
                }
            }

            return GetInstance_;
        }
    }

    virtual protected void Awake()
    {
        CheckInstance();
    }

    protected bool CheckInstance()
    {
        if (GetInstance_ == null)
        {
            GetInstance_ = this as T;
            return true;
        }
        else if (GetInstance_ == this)
        {
            return true;
        }

        Destroy(this);
        return false;
    }

}
