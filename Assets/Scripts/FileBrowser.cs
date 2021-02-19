using System.IO;
using System.Collections;
using System.Collections.Generic;
using SFB;
using UnityEngine;

public class FileBrowser : MonoBehaviour
{
    //If there is more use for this functionality, make more generic method
    public void OpenFileBrowser(){
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", @"C:\Unity\", "", false);
        
        var pathArr = paths[0].Split(Path.DirectorySeparatorChar);
        if(pathArr.Length == 1)
            pathArr = paths[0].Split(Path.AltDirectorySeparatorChar);

        var filename = pathArr[pathArr.Length - 1];
        Debug.Log(Path.AltDirectorySeparatorChar + " - " + filename);
        GetComponentInParent<NodeInspectorTextElement>().UpdateValueFromFileBrowser(filename);
    }
}
