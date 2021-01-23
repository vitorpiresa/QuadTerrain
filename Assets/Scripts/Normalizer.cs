using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normalizer
{
    Chunk m_Chunk;

    private List<Node> GetLeafs()
    {
        Queue<Node> nodes = new Queue<Node>();
        List<Node> leafs = new List<Node>();

        nodes.Enqueue(m_Chunk.node);
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

    public static void Normalize(Chunk chunk)
    {
        Queue<Node> nodes = new Queue<Node>();
        List<Node> leafs = new List<Node>();

        nodes.Enqueue(chunk.node);
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

        foreach (var leaf in leafs)
            for (int x = leaf.area.xMin; x < leaf.area.xMax; x++)
                for (int y = leaf.area.yMin; y < leaf.area.yMax; y++)
                {
                    ExpandToLevel(chunk, x - 1, y, leaf.depth - 1);
                    ExpandToLevel(chunk, x, y - 1, leaf.depth - 1);
                    ExpandToLevel(chunk, x + 1, y, leaf.depth - 1);
                    ExpandToLevel(chunk, x, y + 1, leaf.depth - 1);
                }
    }

    static void ExpandToLevel(Chunk chunk, int x, int y, int level)
    {
        if (x < 0 || y < 0 || x >= chunk.area.width || y >= chunk.area.height)
            return;

        var queue = new Queue<Node>(new Node[] { chunk.node });

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node.depth >= level)
                return;

            if (!node.expanded)
                node.Expand();

            if (node.NE.area.Contains(new Vector2Int(x, y)))
                queue.Enqueue(node.NE);
            if (node.NW.area.Contains(new Vector2Int(x, y)))
                queue.Enqueue(node.NW);
            if (node.SE.area.Contains(new Vector2Int(x, y)))
                queue.Enqueue(node.SE);
            if (node.SW.area.Contains(new Vector2Int(x, y)))
                queue.Enqueue(node.SW);
        }
    }
}
