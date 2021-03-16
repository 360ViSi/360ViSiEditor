using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerMenuManager : MonoBehaviour
{
    [SerializeField] SO_String playerProjectFilePath;
    public void OpenFile()
    {
        var path = StandaloneFileBrowser.OpenFilePanel("Load a project file", "", "json", false)[0];
        playerProjectFilePath.data = path;

        SceneManager.LoadScene("PlayerScene");
    }

    public void OpenEditor()
    {
        SceneManager.LoadScene("EditorScene");
    }
}
