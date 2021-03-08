using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_StringList", menuName = "ScriptableObjects/SO_StringList", order = 1)]
public class SO_StringList : ScriptableObject
{
  public List<string> data = new List<string>();
}
