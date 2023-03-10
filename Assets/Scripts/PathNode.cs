using UnityEngine;

public class PathNode
{
    public Vector3 worldPos;
    public bool walkable;
    public int gridX, gridY;

    public int gCost; //distance from start node
    public int hCost; //distance from end node (heuristic)
    public int fCost => gCost + hCost; //sum of gCost and fCost

    public PathNode parent;

    public PathNode(Vector3 worldPos, bool walkable, int gridX, int gridY)
    {
        this.worldPos = worldPos;
        this.walkable = walkable;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}
