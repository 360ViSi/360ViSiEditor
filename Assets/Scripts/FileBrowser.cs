using System.Collections;
using System.Collections.Generic;
using SFB;
using UnityEngine;

public class FileBrowser : MonoBehaviour
{
    public void OpenFileBrowser(){
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", @"C:\Unity\", "", false);
        var pathArr = paths[0].Split('\\');
        var filename = pathArr[pathArr.Length - 1];
        GetComponentInParent<NodeInspectorTextElement>().UpdateValueFromFileBrowser(filename);
    }
}
