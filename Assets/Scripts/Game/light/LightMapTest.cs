using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMapTest : MonoBehaviour
{
    public Texture2D lightmap1;

    public Texture2D lightmap2;

    public Texture2D greenDir1;

    public Texture2D greenDir2;

    private void OnGUI()
    {
        if (GUILayout.Button("<size=50>lightmap1</size>"))
        {
            LightmapData data = new LightmapData();
            data.lightmapColor = lightmap1;
            data.lightmapDir = greenDir1;
            LightmapSettings.lightmaps = new LightmapData[1] { data };
        }
        if (GUILayout.Button("<size=50>lightmap2</size>"))
        {
            LightmapData data = new LightmapData();
            data.lightmapColor = lightmap2;
            data.lightmapDir = greenDir2;
            LightmapSettings.lightmaps = new LightmapData[1] { data };
        }
    }
}
