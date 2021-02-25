using System;
using System.Linq;
using System.ComponentModel;
using System.IO;
using SFB;
using UnityEngine;

public class ProjectManager : MonoBehaviour
{
    public static ProjectManager instance;
    [SerializeField] StructureManager structureManager;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (this != instance) Destroy(gameObject);
    }
    string fileName = "simu.json";
    string folderPath = @"C:\Unity\"; //S NOTE change this

    public string FileName { get => fileName; }
    public string FolderPath { get => folderPath; }

    public string FullPath { get => folderPath + fileName; }
    public StructureManager StructureManager { get => structureManager; }

    public void NewProject(string path = "")
    {
        if (path == "")
            path = StandaloneFileBrowser.SaveFilePanel("Create a new project file", FolderPath, fileName, "json");

        var pathArr = path.Split(Path.DirectorySeparatorChar);
        if (pathArr.Length == 1)
            pathArr = path.Split(Path.AltDirectorySeparatorChar);

        folderPath = "";
        for (int i = 0; i < pathArr.Length - 1; i++)
        {
            folderPath += pathArr[i];
            folderPath += Path.DirectorySeparatorChar;
        }

        fileName = pathArr[pathArr.Length - 1];
    }

    public void OpenProject(string path)
    {
        if (path == "")
            path = StandaloneFileBrowser.OpenFilePanel("Load a new project file", FolderPath, "json", false)[0];

        var pathArr = path.Split(Path.DirectorySeparatorChar);
        if (pathArr.Length == 1)
            pathArr = path.Split(Path.AltDirectorySeparatorChar);

        folderPath = "";
        for (int i = 0; i < pathArr.Length - 1; i++)
        {
            folderPath += pathArr[i];
            folderPath += Path.DirectorySeparatorChar;
        }

        fileName = pathArr[pathArr.Length - 1];

        structureManager.JsonToSimulation();
    }
}