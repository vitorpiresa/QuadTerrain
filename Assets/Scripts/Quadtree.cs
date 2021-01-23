using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree : MonoBehaviour
{
    [SerializeField] Texture2D m_Heightmap;

    [SerializeField] List<(Texture2D, int, int)> m_Tiles;

    [SerializeField] Chunk m_Chunk;

    [SerializeField] MeshFilter m_Filter;

    IEnumerator Start()
    {
        m_Chunk = new Chunk();
        m_Chunk.area = new RectInt(0, 0, 256, 256);
        m_Chunk.node = new Node();
        m_Chunk.node.area = m_Chunk.area;
        m_Chunk.node.depth = 0;
        yield return ExpandChunk();
        Normalizer.Normalize(m_Chunk);
        yield return BakeMesh();
    }

    IEnumerator ExpandChunk()
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
        yield return null;
    }

    IEnumerator BakeMesh()
    {
        var nodes = new Queue<Node>();
        nodes.Enqueue(m_Chunk.node);

        var mesh = new Mesh();
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m_Filter.mesh = mesh;

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var indices = new List<int>();

        while(nodes.Count > 0)
        {
            var node = nodes.Dequeue();
            if(node.expanded)
            {
                nodes.Enqueue(node.NE);
                nodes.Enqueue(node.NW);
                nodes.Enqueue(node.SE);
                nodes.Enqueue(node.SW);
            }
            else
            {
                var v1 = new Vector3(node.area.xMin, GetHeight(node.area.xMin, node.area.yMin), node.area.yMin);
                var v2 = new Vector3(node.area.xMin, GetHeight(node.area.xMin, node.area.yMax), node.area.yMax);
                var v3 = new Vector3(node.area.xMax, GetHeight(node.area.xMax, node.area.yMax), node.area.yMax);
                var v4 = new Vector3(node.area.xMax, GetHeight(node.area.xMax, node.area.yMin), node.area.yMin);
                
                var t = vertices.Count;
                int[] tgs = default;
                if(node.type == NodeType.SW)
                    tgs = new int[]{t, t+1, t+2, t+2, t+3, t};
                else if(node.type == NodeType.NW)
                    tgs = new int[]{t+1, t+2, t+3, t+3, t, t+1};
                else if(node.type == NodeType.NE)
                    tgs = new int[]{t+2, t+3, t, t, t+1, t+2};
                else
                    tgs = new int[]{t+3, t, t+1, t+1, t+2, t+3};
                
                var a1 = v2 - v1;
                var b1 = v3 - v1;
                Vector3 n1 = default;
                n1.x = a1.y * b1.z - a1.z * b1.y;
                n1.y = a1.z * b1.x - a1.x * b1.z;
                n1.z = a1.x * b1.y - a1.y * b1.x;
                n1.Normalize();

                vertices.AddRange(new Vector3[]{v1, v2, v3, v4});
                indices.AddRange(tgs);
                normals.AddRange(new Vector3[]{n1, n1, n1, n1});
                mesh.SetVertices(vertices);
                mesh.SetTriangles(indices, 0);
                mesh.SetNormals(normals);
                yield return null;
            }
        }
        mesh.RecalculateNormals();  
        mesh.UploadMeshData(true);
        yield return null;
    }

    private float GetHeight(int x, int y)
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
                if(Mathf.Abs(diff) >= 6)
                    return false;
            }
        return true;
    }
}

public class Chunk
{
    public Node node;
    public RectInt area;
}