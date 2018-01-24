using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

    public Transform m_tSeeker, m_tTarget;
    PathGridManager m_Grid;

    private void Awake()
    {
        m_Grid = GetComponent<PathGridManager>();
    }

    private void Update()
    {
        FindPath(m_tSeeker.position, m_tTarget.position);
    }

    private List<Node> FindPath(Vector3 start, Vector3 end)
    {
        Node startNode = m_Grid.NodeFromWorldPosition(start);
        Node endNode = m_Grid.NodeFromWorldPosition(end);

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].m_iFCost < currentNode.m_iFCost || openSet[i].m_iFCost == currentNode.m_iFCost &&
                    openSet[i].m_iHCost < currentNode.m_iHCost)
                    currentNode = openSet[i];

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == endNode)
                    return RetracePath(startNode, endNode);

                foreach (Node neighbour in m_Grid.GetNeighbours(currentNode))
                {
                    if (neighbour.m_bIsBlocked || closedSet.Contains(neighbour))
                        continue;

                    int NewMovementCost = currentNode.m_iGCost + GetDistance(currentNode, neighbour);

                    if (NewMovementCost < neighbour.m_iGCost || !openSet.Contains(neighbour))
                    {
                        neighbour.m_iGCost = NewMovementCost;
                        neighbour.m_iHCost = GetDistance(neighbour, endNode);
                        neighbour.m_nParent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);                        
                    }
                }
            }
        }
        return null;
    }

    private List<Node> RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.m_nParent;
        }
        path.Reverse();
        m_Grid.path = path;

        return path;
    }

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
