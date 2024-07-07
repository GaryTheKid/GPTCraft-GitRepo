using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GameObject testPathCube;
    public GameObject testWalkableCube;

    private readonly int[,] dirs = new int[,]
    {
        {-1, -1, -1}, {-1, 0, -1}, {-1, 1, -1},
        { 0, -1, -1}, { 0, 0, -1}, { 0, 1, -1},
        { 1, -1, -1}, { 1, 0, -1}, { 1, 1, -1},

        {-1, -1, 0}, {-1, 0, 0}, {-1, 1, 0},
        { 0, -1, 0},             { 0, 1, 0},
        { 1, -1, 0}, { 1, 0, 0}, { 1, 1, 0},

        {-1, -1, 1}, {-1, 0, 1}, {-1, 1, 1},
        { 0, -1, 1}, { 0, 0, 1}, { 0, 1, 1},
        { 1, -1, 1}, { 1, 0, 1}, { 1, 1, 1},
    };

    public class Node
    {
        public Vector3Int position;
        public bool walkable;
        public float gCost;
        public float hCost;
        public Node parent;

        public float FCost => gCost + hCost;

        public Node(Vector3Int pos, bool isWalkable)
        {
            position = pos;
            walkable = isWalkable;
            gCost = Mathf.Infinity;
            hCost = 0;
            parent = null;
        }
    }

    public Node[,,] FindWalkNodes(Vector3Int startPos, int exploreRange, int AIHeight)
    {
        Node[,,] nodes = new Node[exploreRange * 2 + 1, exploreRange * 2 + 1, exploreRange * 2 + 1];

        for (int x = -exploreRange; x <= exploreRange; x++)
        {
            for (int y = -exploreRange; y <= exploreRange; y++)
            {
                for (int z = -exploreRange; z <= exploreRange; z++)
                {
                    Vector3Int pos = new Vector3Int(startPos.x + x, startPos.y + y, startPos.z + z);
                    bool isWalkable = IsWalkable(pos, AIHeight); // 实现这个函数来判断给定位置是否可通过
                    nodes[x + exploreRange, y + exploreRange, z + exploreRange] = new Node(pos, isWalkable);

                    /*if (isWalkable)
                    {
                        Instantiate(testWalkableCube, pos + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity); // 在路径上生成路径的cube
                    }*/
                }
            }
        }

        return nodes;
    }

    public List<Vector3Int> FindPath(Node[,,] nodes, Vector3Int startPos, Vector3Int targetPos, int exploreRange)
    {
        List<Vector3Int> path = new List<Vector3Int>();

        int xGap = targetPos.x - startPos.x;
        int yGap = targetPos.y - startPos.y;
        int zGap = targetPos.z - startPos.z;

        if (xGap > exploreRange || 
            xGap < -exploreRange ||
            yGap > exploreRange ||
            yGap < -exploreRange ||
            zGap > exploreRange ||
            zGap < -exploreRange)
        {
            Debug.Log("Target out of reach.");
            return path;
        }

        Node startNode = nodes[exploreRange, exploreRange, exploreRange];
        Node targetNode = nodes[xGap + exploreRange, yGap + exploreRange, zGap + exploreRange];

        if (!startNode.walkable || !targetNode.walkable)
        {
            Debug.Log("Start or target node is not walkable.");
            return path;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                path = RetracePath(startNode, targetNode);
                break;
            }

            for (int i = 0; i < dirs.GetLength(0); i++)
            {
                int newX = currentNode.position.x + dirs[i, 0];
                int newY = currentNode.position.y + dirs[i, 1];
                int newZ = currentNode.position.z + dirs[i, 2];                

                if (newX >= startPos.x - exploreRange && newX <= startPos.x + exploreRange && 
                    newY >= startPos.y - exploreRange && newY <= startPos.y + exploreRange && 
                    newZ >= startPos.z - exploreRange && newZ <= startPos.z + exploreRange)
                {
                    Node neighbor = nodes[newX - startPos.x + exploreRange, newY - startPos.y + exploreRange, newZ - startPos.z + exploreRange];

                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbor = currentNode.gCost + Vector3Int.Distance(currentNode.position, neighbor.position);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = Vector3Int.Distance(neighbor.position, targetNode.position);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

        }

        /*// test
        foreach (var node in path)
        {
            Instantiate(testPathCube, node + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity); // 在路径上生成路径的cube
            print(node);
        }*/

        return path;
    }

    private List<Vector3Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    private bool IsWalkable(Vector3Int position, int height)
    {
        // 这里实现判断给定位置是否可通过的逻辑，根据你的需求来修改
        // 这里简单地假设所有的点都是可通过的，你需要根据实际情况来修改这个函数

        bool isWalkable = !TerrainManager.singleton.IsBlockWalkable(position + new Vector3Int(0, -1, 0));

        for (int i = 0; i < height; i++)
        {
            Vector3Int tileAbove = position + new Vector3Int(0, i, 0);
            isWalkable &= TerrainManager.singleton.IsBlockWalkable(tileAbove);
        }

        return isWalkable;
    }
}