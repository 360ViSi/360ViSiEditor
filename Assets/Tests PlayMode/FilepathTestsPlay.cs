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
            Assert.AreEqual(filename, "simu.json");
        }

        [UnityTest]
        public IEnumerator NewProject()
        {
            SceneManager.LoadScene("EditorScene", LoadSceneMode.Single);
            yield return null;
            var path = @"C:\Unity\PlayTest.json";
            ProjectManager.instance.NewProject(path);

            Debug.Log(ProjectManager.instance.FileName);
            Debug.Log(ProjectManager.instance.FolderPath);


            Assert.AreEqual(ProjectManager.instance.FileName, "PlayTest.json");
            Assert.AreEqual(ProjectManager.instance.FolderPath, @"C:\Unity\");
            yield return null;

            ProjectManager.instance.StructureManager.JsonToSimulation();
            Debug.Break();
            yield return null;
        }
    }
}
