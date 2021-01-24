using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teste : Quadtree
{
    ushort[,] m_Heighmap;

    // Start is called before the first frame update
    public override void Start()
    {
        var file = File.OpenRead("Assets/Heightmap.hex");
        var br = new BinaryReader(file);
        int w = br.ReadInt32();
        int h = br.ReadInt32();
    
        m_Heighmap = new ushort[w, h];

        for(int x = 0; x < w; x++)
            for(int y = 0; y < h; y++)
                m_Heighmap[x, y] = br.ReadUInt16();
        
        base.Start();
    }

    public override float GetHeight(int x, int y)
    {
        return m_Heighmap[x, y] * 1024 / ushort.MaxValue;
    }
}
