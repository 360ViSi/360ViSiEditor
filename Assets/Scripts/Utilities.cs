using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
