using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;

namespace Tests
{
    public class FilepathTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void FilepathTestsSimplePasses()
        {
            var path0 = @"C:\Unity\simu.json";
            var path1 = @"C:/Unity/simu.json";

            var pathArr0 = path0.Split(Path.DirectorySeparatorChar);
            if (pathArr0.Length == 1)
                pathArr0 = path0.Split(Path.AltDirectorySeparatorChar);

            var pathArr1 = path1.Split(Path.DirectorySeparatorChar);
            if (pathArr1.Length == 1)
                pathArr1 = path1.Split(Path.AltDirectorySeparatorChar);

            Assert.AreEqual(pathArr0[pathArr0.Length - 1], "simu.json");
            Assert.AreEqual(pathArr1[pathArr1.Length - 1], "simu.json");
        }
    }
}
