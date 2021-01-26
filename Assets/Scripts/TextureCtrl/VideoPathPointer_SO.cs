using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "this_is_a_video_folder", menuName = "ScriptableObjects/VideoPathPointer_SO", order = 1)]
public class VideoPathPointer_SO : ScriptableObject
{
  public string[] fileExtensions;
}
