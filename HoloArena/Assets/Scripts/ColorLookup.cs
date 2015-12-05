using UnityEngine;
using System.Collections;

public class ColorLookup : MonoBehaviour
{
    public PlayerColorData[] PlayerColors;

    public PlayerColorData GetColorData(string name)
    {
        for (int i = 0; i < PlayerColors.Length; ++i)
        {
            if (PlayerColors[i].Name == name)
            {
                return PlayerColors[i];
            }
        }
        return null;
    }
}

[System.Serializable]
public class PlayerColorData
{
    public string Name;
    public Color MyColor;
    public Texture ColorTexture;
}
