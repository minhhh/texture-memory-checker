using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class FPSCounter : MonoBehaviour
{
    private LimitedSizeStack<float> frameTimes;
    private const int frameCount = 100;
    public Text txt;
    StringBuilder sb;
    int fps;

    // Use this for initialization
    void Start ()
    {
        txt = GetComponentInChildren<Text> ();

        frameTimes = new LimitedSizeStack<float> (frameCount);
        for (int i = 0; i < frameCount; i++) {
            frameTimes.Push (1);
        }
        sb = new StringBuilder (100);
    }

    // Update is called once per frame
    void Update ()
    {
        frameTimes.Push (Time.deltaTime);
        float sum = 0;
        foreach (float t in frameTimes) {
            sum += t;
        }

        fps = (int)(frameCount / sum);
        sb.Length = 0;
        sb.Append ("FPS - ");
        sb.Append (fps);

        txt.text = sb.ToString ();
    }
}
