using System;
using System.Linq;
using System.ComponentModel;
using System.IO;
using SFB;
using UnityEngine;
using UnityEngine.SceneManagement;

///<summary>
/// Manager for project level top left buttons of the Editor (Open, Save, Clear)
///</summary>
public class ProjectManager : MonoBehaviour
{
    public static ProjectManager instance;
    [SerializeField] StructureManager structureManager;
    [SerializeField] YesNoDialog yesNoDialog;
    [SerializeField] SO_String playerProjectPath;
    string fileName = "new project";
    string folderPath = @"C:\Unity\"; //S NOTE change this to reflect my documents?
    string json = ".json";
    bool wasSaved = false;

    public string FileName { get => fileName; }
    public string FolderPath { get => folderPath; }

    public string FullPath { get => folderPath + fileName + json; }
    public StructureManager StructureManager { get => structureManager; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (this != instance) Destroy(gameObject);
    }

    private void Start()
    {
        OpenProject(@"C:\Unity\solo.json"); //S TODO (save most recent to playerprefs and load on start)
    }

    public void NewProject()
    {
        yesNoDialog.Initialize(ClearStructure, "Are you sure you want to delete all current nodes from the workspace? \n All unsaved changes will be lost.");
    }

    ///<summary>
    /// Supplied to the yesNoDialog as parameter to give a warning befor clearing all the nodes
    ///</summary>
    void ClearStructure(bool value)
    {
        if (value == false) return;

        wasSaved = false;
        StructureManager.ClearStructure();
    }

    public void OpenProject(string path)
    {
        if (path == "")
            path = StandaloneFileBrowser.OpenFilePanel("Load a new project file", FolderPath, "json", false)[0];

        folderPath = Utilities.FolderPathFromFilePath(path);
        fileName = Utilities.FileNameFromFilePath(path, false);

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

        folderPath = Utilities.FolderPathFromFilePath(path);
        fileName = Utilities.FileNameFromFilePath(path, false);

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

    public void TestProject()
    {
        SaveProject();
        playerProjectPath.data = FullPath;
        SceneManager.LoadScene("PlayerScene");
    }
}