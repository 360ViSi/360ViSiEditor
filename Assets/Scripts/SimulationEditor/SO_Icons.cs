using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "SO_Icons", menuName = "360Platform/SO_Icons")]
public class SO_Icons : ScriptableObject
{
    public Icon[] icons = new Icon[0];

    public Sprite GetIconSprite(string name)
    {
        var suitableIcons = icons.Where(e => e.iconName == name);
        if (suitableIcons.Count() == 1)
            return suitableIcons.FirstOrDefault().sprite;

        //ERRORS
        if (suitableIcons.Count() == 0)
            Debug.LogError($"Icon with name {name} not found in {this.name}");
        if (suitableIcons.Count() == 0)
            Debug.LogError($"Icon with name {name} not found in {this.name}");

        return null;
    }

    private void OnValidate()
    {
        foreach (var item in icons)
        {
            if (item.iconName == string.Empty) continue;

            if (icons.Where(e => e.iconName == item.iconName).Count() > 1)
                Debug.LogError($"Multiple icons with the same iconName \"{item.iconName}\" in {this.name}");
        }
    }
}

[Serializable]
public class Icon
{
    public string iconName;
    public Sprite sprite;
}