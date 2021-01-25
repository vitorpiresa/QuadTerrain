using UnityEngine;

public class Node
{
    public Node SW, SE, NW, NE;
    public RectInt area;
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
        
        SE = new Node();
        SE.area = new RectInt(minx + width, miny, width, width);
        SE.depth = depth;

        NW = new Node();
        NW.area = new RectInt(minx, miny + width, width, width);
        NW.depth = depth;

        NE = new Node();
        NE.area = new RectInt(minx + width, miny + width, width, width);
        NE.depth = depth;

        return new Node[]{SW, SE, NW, NE};
    }
}