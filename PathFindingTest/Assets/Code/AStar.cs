using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A* pathfinding.
/// </summary>
public class AStar : MonoBehaviour {

    public Transform m_tSeeker, m_tTarget;
    PathGridManager m_Grid;

    /// <summary>
    /// Searhing PathGridManager in Awake method.
    /// </summary>
    private void Awake()
    {
        m_Grid = GetComponent<PathGridManager>();
    }
      
    private void Update()
    {
        FindPath(m_tSeeker.position, m_tTarget.position);
    }

    /// <summary>
    /// Searches path between start point and end point.
    /// </summary>
    /// <param name="start">Vector3 position of start point</param>
    /// <param name="end">Vector3 position of end point</param>
    /// <returns>Returns either null or retraced path between start and end points if path is found</returns>
    private List<Node> FindPath(Vector3 start, Vector3 end)
    {
        Node startNode = m_Grid.NodeFromWorldPosition(start);
        Node endNode = m_Grid.NodeFromWorldPosition(end);

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

        // Add start node directly to openSet.
        openSet.Add(startNode);
                
        // 
        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                // Checks if current nodes f-cost is lower or same and h-cost is lower than node's that is already contained in openSet list
                if (openSet[i].m_iFCost < currentNode.m_iFCost || (openSet[i].m_iFCost == currentNode.m_iFCost &&
                    openSet[i].m_iHCost < currentNode.m_iHCost))
                    currentNode = openSet[i];

                // If true, node is removed from openSet and added to closedSet.
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // If current node is same as end node then path is retraced.
                if (currentNode == endNode)
                    return RetracePath(startNode, endNode);

                // Checks nodes neighbouring nodes.
                foreach (Node neighbour in m_Grid.GetNeighbours(currentNode))
                {
                    // If node is blocked or is already containded in openSet.
                    if (neighbour.m_bIsBlocked || closedSet.Contains(neighbour))
                        continue;

                    // Calculates movement cost.
                    int NewMovementCost = currentNode.m_iGCost + GetDistance(currentNode, neighbour);

                    // Checks if current nodes movement cost is cheaper than next nodes or it isn't already contained in openSet. 
                    if (NewMovementCost < neighbour.m_iGCost || !openSet.Contains(neighbour))
                    {
                        // If true, current node is set as a parent node, and G- & H-cost is calculated.
                        neighbour.m_iGCost = NewMovementCost;
                        neighbour.m_iHCost = GetDistance(neighbour, endNode);
                        neighbour.m_nParent = currentNode;

                        // And it is added to openSet if not already in it.
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);                        
                    }
                }
            }
        }
        // if no path is found then return null.
        return null;
    }

    /// <summary>
    /// Retraces path from start node to end node.
    /// </summary>
    /// <param name="start">Start node</param>
    /// <param name="end">End node</param>
    /// <returns>Retraced path between start and end nodes</returns>
    private List<Node> RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.m_nParent;
        }

        // List's order is reversed and it is send to PathGridManger.
        path.Reverse();
        m_Grid.path = path;

        return path;
    }

    /// <summary>
    /// Calculates and scores distance between two nodes.
    /// Moving diagonally costs more than moving horizontally or vertically.
    /// </summary>
    /// <param name="nodeA">First node</param>
    /// <param name="nodeB">Second node</param>
    /// <returns>Calculated score between nodes</returns>
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int iDistX = Mathf.Abs(nodeA.m_iGridX - nodeB.m_iGridX);
        int iDistY = Mathf.Abs(nodeA.m_iGridY - nodeB.m_iGridY);

        if (iDistX > iDistY)        
            return 14 * iDistY + 10 * (iDistX - iDistY);        
        else        
            return 14 * iDistX + 10 * (iDistY - iDistX);        
    }
}
