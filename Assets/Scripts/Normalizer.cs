using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normalizer
{
    public static void Normalize(Texture2D texture)
    {
        for(int x = 0; x < texture.width; x++)
            for(int y = 0; y < texture.height; y++)
            {
                var level = PixelToLevel(texture.GetPixel(x, y));
            }
    }

    private static int PixelToLevel(Color32 color)
    {
        Color[] levels = new Color[]
        {
            new Color(1, 1, 1, 1),
            new Color(1, 0, 0, 1),
            new Color(0, 1, 0, 1),
            new Color(0, 0, 1, 1),
            new Color(1, 1, 0, 1),
            new Color(1, 0, 1, 1),
            new Color(0, 1, 1, 1),
            new Color(0, 0, 0, 1)
        };
        return System.Array.IndexOf(levels, color);
    }

    private static void NormalizePixelToLevel(int x, int y)
    {
        
    }
}
