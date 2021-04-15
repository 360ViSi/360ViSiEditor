using System.IO;
using System;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

public class ZipHandler : MonoBehaviour
{
    public string zipPath = @"C:\Unity\export\testpack.zip";

    [ContextMenu("SaveZip")]
    void SaveZip()
    {
        // if (Directory.Exists(@"C:\Unity\export") == false)
        //     Directory.CreateDirectory(@"C:\Unity\export");

        ZipFile.CreateFromDirectory(@"C:\Unity\", zipPath, CompressionLevel.Optimal, false);
    }

    [ContextMenu("LoadZip")]
    void LoadZip()
    {

    }
}
