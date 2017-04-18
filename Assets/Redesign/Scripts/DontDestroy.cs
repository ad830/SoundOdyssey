using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour {
    public static DontDestroy i;

    void Awake()
    {
        if (!i)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
