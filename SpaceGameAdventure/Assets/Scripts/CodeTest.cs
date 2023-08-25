using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(ScriptParser.ReadBetween("hello world", "e", "w"));
    }
}
