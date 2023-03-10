using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelGenerator))]
public class PathGenerator : MonoBehaviour
{
    public List<GameObject> pathObjs;

    [Header("Path Settings")]
    [SerializeField] private float minTerrainHeight = 0.4f;
    [SerializeField] private float maxTerrainHeight = 0.6f;
    [SerializeField] private GameObject pathObj;

    //[Header("Giant Plane Stuff")] //for testing
    //[SerializeField] private Transform giantNoisePlane;

    private float[,] noiseMap;
    private int noiseDepth, noiseWidth;

    private PathNode[,] grid;
    private List<PathNode> openNodes;
    private HashSet<PathNode> closedNodes;
    private List<PathNode> pathNodes;

    public void GeneratePath(int levelW, int levelD, int tileW, int tileD)
    {
        //get dimensions of entire level
        int levelWidth = levelW * tileW;
        int levelDepth = levelD * tileD;
        int halfTileW = tileW / 2;
        int halfTileD = tileD / 2;

        //create noise map of entire level
        int offsetX = (levelWidth / 2) - halfTileW;
        int offsetZ = (levelDepth / 2) - halfTileD;
        noiseMap = NoiseGenerator.GenerateNoiseMap(levelWidth, levelDepth, new Vector2(-offsetX * 2, -offsetZ * 2));
        noiseDepth = noiseMap.GetLength(0);
        noiseWidth = noiseMap.GetLength(1);

        //set position, scale, and texture of giant plane (just for demonstration)
        //giantNoisePlane.position = new Vector3(offsetX, 25, offsetZ);
        //giantNoisePlane.localScale = new Vector3(levelW, 1, levelD);
        //Texture2D texture = TextureGenerator.TextureFromHeightMap(noiseMap);
        //giantNoisePlane.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;

        //create grid of pathfinding nodes
        grid = new PathNode[noiseWidth, noiseDepth];
        for(int y = 0; y < noiseDepth; y++)
        {
            for(int x = 0; x < noiseWidth; x++)
            {
                float height = noiseMap[x, y];
                bool walkable = height > minTerrainHeight && height < maxTerrainHeight;
                float worldHeight = LevelGenerator.Instance.heightCurve.Evaluate(height);
                worldHeight *= LevelGenerator.Instance.heightScale;
                Vector3 pos = new Vector3(noiseDepth - y - halfTileD, worldHeight, noiseWidth - x - halfTileW);
                grid[x, y] = new PathNode(pos, walkable, x, y);
            }
        }

        //update location of goal cube to end of path
        Vector3 goalPos = GetEndNode().worldPos + Vector3.up * 5;
        LevelGenerator.Instance.SpawnGoal(goalPos);

        //start pathfinding
        StartCoroutine(Pathfind());
    }
    
    //a* pathfinding algorithm
    private IEnumerator Pathfind()
    {
        //get start and end nodes of path
        PathNode startNode = GetStartNode();
        PathNode endNode = GetEndNode();

        //create sets of open and closed nodes
        openNodes = new List<PathNode>();
        closedNodes = new HashSet<PathNode>();

        //add start node to set of open nodes
        openNodes.Add(startNode);

        //find the path from the start node to the closed node
        while(openNodes.Count > 0)
        {
            PathNode currentNode = openNodes[0];
            for (int i = 1; i < openNodes.Count; i++)
            {
                //find open node with lowest fCost
                if(openNodes[i].fCost > currentNode.fCost) continue;

                //take node with lower hCost if fCost are equal
                if(openNodes[i].fCost == currentNode.fCost &&
                openNodes[i].hCost >= currentNode.hCost) continue;

                currentNode = openNodes[i];
            }

            //make current node closed instead of open
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            //if current node is end node, pathfinding is finished
            if(currentNode == endNode)
            {
                yield return RetracePath(startNode, endNode);
                StopCoroutine(Pathfind());
            }

            //loop through current nodes' neighbors
            foreach(PathNode neighbor in GetNeighbors(currentNode))
            {
                //skip node if not walkable or already closed
                if(!neighbor.walkable || closedNodes.Contains(neighbor)) continue;

                //calculate new move cost to this neighbor
                int newMoveCost = currentNode.gCost + GetNodeDistance(currentNode, neighbor);

                //check to see if a shorter path was found or if neighbor wasn't open
                if(newMoveCost < neighbor.gCost || !openNodes.Contains(neighbor))
                {
                    //update fCost of neighbor by recalculating gCost and hCost
                    neighbor.gCost = newMoveCost;
                    neighbor.hCost = GetNodeDistance(neighbor, endNode);
                    neighbor.parent = currentNode; //parent node within path

                    //make node open if it wasn't before
                    if(!openNodes.Contains(neighbor)) openNodes.Add(neighbor);
                }
            }
            yield return new WaitForSeconds(0.005f);
        }
    }

    private IEnumerator RetracePath(PathNode startNode, PathNode endNode)
    {
        pathNodes = new List<PathNode>();
        PathNode currentNode = endNode;

        //retrace the path from end node to start node   
        while(currentNode != startNode)
        {
            pathNodes.Add(currentNode);
            currentNode = currentNode.parent;
            yield return new WaitForSeconds(0.005f);
        }

        //reverse path to read from start to end
        pathNodes.Reverse();

        //spawn path objects in order from start to end
        pathObjs = new List<GameObject>();
        int i = 0;
        while(i < pathNodes.Count)
        {
            Vector3 pos = pathNodes[i].worldPos;
            GameObject obj = Instantiate(pathObj, pos, Quaternion.identity);
            pathObjs.Add(obj);
            yield return new WaitForSeconds(0.25f);
            i++;
        }
    }

    //finds first walkable node near player's start tile
    private PathNode GetStartNode()
    {
        for(int y = grid.GetLength(1) - 1; y >= 0; y--)
        {
            for(int x = grid.GetLength(0) - 1; x >= grid.GetLength(0) * (2 / 3); x--)
            {
                if(!grid[x, y].walkable) continue;
                return grid[x, y];
            }
        }
        return null;
    }

    //finds first walkable node near level's end tile
    private PathNode GetEndNode()
    {
        for(int y = 0; y < grid.GetLength(1); y++)
        {
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                if(!grid[x, y].walkable) continue;
                return grid[x, y];
            }
        }
        return null;
    }

    private List<PathNode> GetNeighbors(PathNode node)
    {
        //find all neighbors of given node in grid
        List<PathNode> neighbors = new List<PathNode>();
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                //skip center node (given node)
                if(x == 0 && y == 0) continue;
                
                //make sure neighbor node exists within grid bounds
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if(checkX < 0 || checkX >= grid.GetLength(0)
                || checkY < 0 || checkY >= grid.GetLength(1)) continue;

                neighbors.Add(grid[checkX, checkY]); //add neighbor node to list
            }
        }
        return neighbors;
    }

    private int GetNodeDistance(PathNode nodeA, PathNode nodeB)
    {
        //get vertical and horizontal distances between nodes
        int xDist = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int yDist = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        //each diagnal move is 14, while each horizontal and vertical move is 10
        if(xDist > yDist) return (14 * yDist) + 10 * (xDist - yDist); //if horizontal distance is greater, equation is 14y + 10(x-y)
        else return (14 * xDist) + 10 * (yDist - xDist); //otherwise vertical distance is greater and equation is 14x + 10(y-x)
    }

    private void OnDrawGizmos()
    {
        if(grid == null) return;
        
        PathNode start = GetStartNode();
        PathNode end = GetEndNode();

        foreach(PathNode pn in grid)
        {
            if(!pn.walkable) continue;

            Gizmos.color = Color.white;
            if(openNodes.Contains(pn)) Gizmos.color = Color.green;
            else if(closedNodes.Contains(pn)) Gizmos.color = Color.red;

            if(pn == start || pn == end) Gizmos.color = Color.blue;
            if(pathNodes != null && pathNodes.Contains(pn)) Gizmos.color = Color.yellow;
            
            Gizmos.DrawSphere(pn.worldPos, 0.25f);
        }
    }
}
