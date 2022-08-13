using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PersistentPathFinder : MonoBehaviour
{
    [MenuItem("Assets/Open PersistentDataPath")]
    static void Open()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
