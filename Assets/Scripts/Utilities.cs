using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

///<summary>
/// Set of static utility functions used in multiple places in the project
///</summary>
public static class Utilities
{
    public static string FloatToTime(float timePercentage, float maxTime, bool returnLength = false)
    {
        var seconds = maxTime * timePercentage;
        var minutes = (int)(seconds / 60);
        seconds %= 60; //error?

        string result = "";

        if (seconds < 10)
            result += $"{minutes}:0{seconds:0.0}";
        else
            result += $"{minutes}:{seconds:0.0}";

        if (returnLength)
            result += $" / {FloatToTime(1, maxTime)}";

        return result;
    }

    ///<summary>
    /// Returns all enums of specified type
    ///</summary>
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    ///<summary>
    /// Splits a filepath with the correct directoryseparator (that works)
    ///</summary>
    public static string[] FilePathToStringArray(string path)
    {
        var pathArr = path.Split(Path.DirectorySeparatorChar);
        if (pathArr.Length == 1)
            pathArr = path.Split(Path.AltDirectorySeparatorChar);

        if (pathArr.Length < 2)
            Debug.LogError("Splitting didn't work properly, check parameter");

        return pathArr;
    }

    public static string FolderPathFromFilePath(string path)
    {
        var pathArr = FilePathToStringArray(path);
        var folderPath = "";
        for (int i = 0; i < pathArr.Length - 1; i++)
        {
            folderPath += pathArr[i];
            folderPath += Path.DirectorySeparatorChar;
        }
        return folderPath;
    }

    public static string FileNameFromFilePath(string path, bool includeExtension)
    {
        var pathArr = FilePathToStringArray(path);

        if (includeExtension)
            return pathArr[pathArr.Length - 1];

        return pathArr[pathArr.Length - 1].Split('.')[0];
    }
}

