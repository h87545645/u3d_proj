using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    public float updateInterval = .5f;
    private float accum = 0;
    private int frames = 0;
    private float timeleft;
    private string stirngFps;
    // Start is called before the first frame update
    void Start()
    {
        timeleft = updateInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        if(timeleft <= .0)
        {
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            stirngFps = format;
            timeleft = updateInterval;
            accum = .0f;
            frames = 0;
        }
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = GUIStyle.none;
        guiStyle.fontSize = 30;
        guiStyle.normal.textColor = Color.red;
        guiStyle.alignment = TextAnchor.UpperLeft;
        Rect rt = new Rect(40, 0, 100, 100);
        GUI.Label(rt, stirngFps, guiStyle);
    }
}
