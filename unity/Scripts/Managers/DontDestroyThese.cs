using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyThese : MonoBehaviour
{
    void Awake()
    {
        if (GameObject.FindObjectsOfType<DontDestroyThese>().Length > 1) Destroy(this.gameObject);
        else DontDestroyOnLoad(this.gameObject);
    }
}
