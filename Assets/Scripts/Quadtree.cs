using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree : MonoBehaviour
{
    [SerializeField] Texture2D m_Heightmap;

    [SerializeField] List<(Texture2D, int, int)> m_Tiles;

    [SerializeField] Chunk m_Chunk;

    [SerializeField] MeshFilter m_Filter;

    public virtual void Start()
    {
        m_Chunk = new Chunk();
        m_Chunk.area = new RectInt(0, 0, 2048, 2048);
        m_Chunk.node = new Node();
        m_Chunk.node.area = m_Chunk.area;
        m_Chunk.node.depth = 0;

        ExpandChunk();
        Normalizer.Normalize(m_Chunk);

        var mb = new MeshBuilder();
        var mesh = mb.BakeMesh(m_Chunk, GetHeight);

        m_Filter.sharedMesh = mesh;
    }

    void ExpandChunk()
    {
        var nodes = new Queue<Node>();
        nodes.Enqueue(m_Chunk.node);
        while(nodes.Count > 0)
        {
            var node = nodes.Dequeue();
            if(!Normalized(node))
            {
                var childs = node.Expand();
                if(childs != null)
                    foreach(var childNode in childs)
                        nodes.Enqueue(childNode);
            }
        }
    }

    public virtual float GetHeight(int x, int y)
    {
        y += 256;
        return m_Heightmap.GetPixelBilinear(x / (float)m_Heightmap.width, y / (float)m_Heightmap.height).r * 1024;
    }

    private bool Normalized() => false;

    private bool Normalized(Node node)
    {
        for(int x = node.area.xMin; x < node.area.xMax; x++)
            for(int y = node.area.yMin; y < node.area.yMax; y++)
            {
                var p1 = GetHeight(node.area.xMin, node.area.yMin);
                var p2 = GetHeight(node.area.xMin, node.area.yMax);
                var p3 = GetHeight(node.area.xMax, node.area.yMax);
                var p4 = GetHeight(node.area.xMax, node.area.yMin);
                
                var c1 = Mathf.Clamp(x / (float)node.area.width, p1, p4);
                var c2 = Mathf.Clamp(x / (float)node.area.width, p2, p3);
                var c3 = Mathf.Clamp(y / (float)node.area.width, c1, c2);
                
                var diff = c3 - GetHeight(x, y);
                if(Mathf.Abs(diff) >= 12)
                    return false;
            }
        return true;
    }
}

public class Chunk
{
    public Node node;
    public RectInt area;

    public List<Node> GetLeafs()
    {
        Queue<Node> nodes = new Queue<Node>();
        List<Node> leafs = new List<Node>();

        nodes.Enqueue(node);
        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();
            if (node.expanded)
            {
                nodes.Enqueue(node.NE);
                nodes.Enqueue(node.NW);
                nodes.Enqueue(node.SE);
                nodes.Enqueue(node.SW);
            }
            else
                leafs.Add(node);
        }
        return leafs;
    }
}