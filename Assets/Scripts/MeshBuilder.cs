using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    public delegate float GetHeight(int x, int y);

    List<Node> leafs;

    List<Vector3Int> vertices = new List<Vector3Int>();

    List<int> triangles = new List<int>();

    public Mesh BakeMesh(Chunk chunk, GetHeight func)
    {
        leafs = chunk.GetLeafs();

        foreach(var leaf in leafs)
        {
            var v1 = new Vector3Int(leaf.area.xMin, 0, leaf.area.yMin);
            var v2 = new Vector3Int(leaf.area.xMin, 0, leaf.area.yMax);
            var v3 = new Vector3Int(leaf.area.xMax, 0, leaf.area.yMax);
            var v4 = new Vector3Int(leaf.area.xMax, 0, leaf.area.yMin);
            var v5 = new Vector3Int(leaf.area.xMin + leaf.area.width / 2, 0, leaf.area.yMin + leaf.area.height / 2);

            var i1 = GetVerticeOrAppend(v1);
            var i2 = GetVerticeOrAppend(v2);
            var i3 = GetVerticeOrAppend(v3);
            var i4 = GetVerticeOrAppend(v4);
            var i5 = GetVerticeOrAppend(v5);

            var steiners = NeedsSteiner(leaf);
            if(steiners[0])
            {
                var v6 = new Vector3Int(leaf.area.xMin, 0, leaf.area.yMin + leaf.area.height / 2);
                var i6 = GetVerticeOrAppend(v6);
                triangles.AddRange(new int[]{i6, i5, i1, i2, i5, i6});
            }
            else
                triangles.AddRange(new int[]{i2, i5, i1});

            if(steiners[1])
            {
                var v6 = new Vector3Int(leaf.area.xMin + leaf.area.width / 2, 0, leaf.area.yMax);
                var i6 = GetVerticeOrAppend(v6);
                triangles.AddRange(new int[]{i6, i5, i2, i3, i5, i6});
            }
            else
                triangles.AddRange(new int[]{i3, i5, i2});
            if(steiners[2])
            {
                var v6 = new Vector3Int(leaf.area.xMax, 0, leaf.area.yMin + leaf.area.height / 2);
                var i6 = GetVerticeOrAppend(v6);
                triangles.AddRange(new int[]{i6, i5, i3, i4, i5, i6});
            }
            else
                triangles.AddRange(new int[]{i4, i5, i3});
            if(steiners[3])
            {
                var v6 = new Vector3Int(leaf.area.xMin + leaf.area.width / 2, 0, leaf.area.yMin);
                var i6 = GetVerticeOrAppend(v6);
                triangles.AddRange(new int[]{i6, i5, i4, i1, i5, i6});
            }
            else
                triangles.AddRange(new int[]{i1, i5, i4});
        }

        return CreateMesh(func);
    }

    private bool[] NeedsSteiner(Node node)
    {
        var depth = node.depth + 1;
        return new bool[4]
        {
            leafs.Any(a => a.area.xMax == node.area.xMin && a.area.yMin == node.area.yMin && a.depth == depth),
            leafs.Any(a => a.area.xMin == node.area.xMin && a.area.yMin == node.area.yMax && a.depth == depth),
            leafs.Any(a => a.area.xMin == node.area.xMax && a.area.yMax == node.area.yMax && a.depth == depth),
            leafs.Any(a => a.area.xMax == node.area.xMax && a.area.yMax == node.area.yMin && a.depth == depth),
        };
    }

    private Mesh CreateMesh(GetHeight func)
    {
        var mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices.Select(a => new Vector3(a.x, func(a.x, a.z), a.z)).ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        mesh.UploadMeshData(true);
        return mesh;
    }

    private int GetVerticeOrAppend(Vector3Int vertice)
    {
        var index = vertices.IndexOf(vertice);
        if(index == -1)
        {
            index = vertices.Count;
            vertices.Add(vertice);
        }
        return index;
    }
}
