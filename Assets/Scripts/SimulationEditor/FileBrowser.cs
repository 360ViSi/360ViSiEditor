using System.IO;
using System.Collections;
using System.Collections.Generic;
using SFB;
using UnityEngine;

public class FileBrowser : MonoBehaviour
{
    public void GetVideoFilepath()
    {
        var path = StandaloneFileBrowser.OpenFilePanel("Open File", ProjectManager.instance.FolderPath, "", false)[0];

        var pathArr = path.Split(Path.DirectorySeparatorChar);
        if (pathArr.Length == 1)
            pathArr = path.Split(Path.AltDirectorySeparatorChar);

        var filename = pathArr[pathArr.Length - 1];
        GetComponentInParent<NodeInspectorTextElement>().UpdateValueFromFileBrowser(filename);
    }
}