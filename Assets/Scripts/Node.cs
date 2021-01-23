using UnityEngine;

public class Node
{
    public Node SW, SE, NW, NE;
    public RectInt area;
    public NodeType type;
    public int depth;
    public bool expanded;

    public Node[] Expand()
    {
        if(this.depth > 6)
            return null;

        var width = area.width / 2;
        var minx = area.xMin;
        var miny = area.yMin;
        var depth = this.depth + 1;

        expanded = true;
        
        SW = new Node();
        SW.area = new RectInt(minx, miny, width, width);
        SW.depth = depth;
        SW.type = NodeType.SW;
        
        SE = new Node();
        SE.area = new RectInt(minx + width, miny, width, width);
        SE.depth = depth;
        SE.type = NodeType.SE;

        NW = new Node();
        NW.area = new RectInt(minx, miny + width, width, width);
        NW.depth = depth;
        NW.type = NodeType.NW;

        NE = new Node();
        NE.area = new RectInt(minx + width, miny + width, width, width);
        NE.depth = depth;
        NE.type = NodeType.NE;

        return new Node[]{SW, SE, NW, NE};
    }
}