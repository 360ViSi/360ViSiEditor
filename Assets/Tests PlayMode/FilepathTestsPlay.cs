using System;
using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class FilepathTestsPlay
    {
        ///<summary>
        /// Tests if the default full path returns simu.json from default folder.
        ///</summary>
        [UnityTest]
        public IEnumerator FilepathTestsPlayMode()
        {
            SceneManager.LoadScene("EditorScene", LoadSceneMode.Single);
            yield return null;
            var path = ProjectManager.instance.FullPath;

            var pathArr = path.Split(Path.DirectorySeparatorChar);
            if (pathArr.Length == 1)
                pathArr = path.Split(Path.AltDirectorySeparatorChar);
            var filename = pathArr[pathArr.Length - 1];

            Debug.Log(filename);
            Assert.AreEqual(filename, "new project.json");
        }

        ///<summary>
        /// Tests creating a new project in editor and loading in player will PAUSE the tests 
        ///</summary>
        [UnityTest]
        public IEnumerator NewProject()
        {
            SceneManager.LoadScene("EditorScene", LoadSceneMode.Single);
            yield return null;
            var path = @"C:\Unity\PlayTest.json";
            ProjectManager.instance.NewProject();
            ProjectManager.instance.SaveProject(path);

            Debug.Log(ProjectManager.instance.FileName);
            Debug.Log(ProjectManager.instance.FolderPath);

            Assert.AreEqual(ProjectManager.instance.FileName, "PlayTest");
            Assert.AreEqual(ProjectManager.instance.FolderPath, @"C:\Unity\");

            //LTest that the new project can also be loaded
            ProjectManager.instance.StructureManager.JsonToSimulation();
            yield return null;
        }
    }
}
