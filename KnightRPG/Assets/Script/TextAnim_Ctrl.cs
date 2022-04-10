using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnim_Ctrl : MonoBehaviour
{
    public float get_time = 0.5f;
    float time = 0.0f;

    void OnEnable()
    {
        time = get_time;
    }

    void FixedUpdate()
    {
        if (time > 0.0f)
            time -= 0.01f;

        if (time <= 0.0f)
            this.gameObject.SetActive(false);
    }
}
