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
    [SerializeField] YesNoDialog yesNoDialog;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (this != instance) Destroy(gameObject);
    }
    string fileName = "new project";
    string folderPath = @"C:\Unity\"; //S NOTE change this
    string json = ".json";
    bool wasSaved = false;

    public string FileName { get => fileName; }
    public string FolderPath { get => folderPath; }

    public string FullPath { get => folderPath + fileName + json; }
    public StructureManager StructureManager { get => structureManager; }

    public void NewProject()
    {
        yesNoDialog.Initialize(ClearStructure, "Are you sure you want to delete all current nodes from the workspace? \n All unsaved changes will be lost.");
    }

    ///<summary>
    /// Supplied to the yesNoDialog as parameter to give a warning befor clearing all the nodes
    ///</summary>
    void ClearStructure(bool value)
    {
        if(value == false) return;

        wasSaved = true;
        StructureManager.ClearStructure();
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
        var fileNameSplit = pathArr[pathArr.Length - 1].Split('.');
        fileName = fileNameSplit[0];

        structureManager.JsonToSimulation();
        wasSaved = true;
    }

    ///<summary>
    /// Save As - asks for location and filename with windows dialog
    ///</summary>
    public void SaveProjectAs(string path)
    {
        if (path == "")
            path = StandaloneFileBrowser.SaveFilePanel("Save the project file", FolderPath, fileName, "json");

        var pathArr = path.Split(Path.DirectorySeparatorChar);
        if (pathArr.Length == 1)
            pathArr = path.Split(Path.AltDirectorySeparatorChar);

        folderPath = "";
        for (int i = 0; i < pathArr.Length - 1; i++)
        {
            folderPath += pathArr[i];
            folderPath += Path.DirectorySeparatorChar;
        }
        var fileNameSplit = pathArr[pathArr.Length - 1].Split('.');
        fileName = fileNameSplit[0];

        structureManager.SimulationToJson(path);
        wasSaved = true;
    }

    ///<summary>
    /// Save without any dialogs, overwrites the last saved or opened project
    ///</summary>
    public void SaveProject()
    {
        if (wasSaved == false)
        {
            SaveProjectAs("");
            return;
        }

        structureManager.SimulationToJson(FullPath);
    }
}