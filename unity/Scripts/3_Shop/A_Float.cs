using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Float : MonoBehaviour
{
    public float amplitude;  // 위아래로 움직이는 범위
    public float frequency;    // 움직이는 주기

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0, y, 0);
    }
}
