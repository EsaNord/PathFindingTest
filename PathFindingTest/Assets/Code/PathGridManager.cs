using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGridManager : MonoBehaviour
{
    public LayerMask m_ObstacleMask;
    public Vector2 m_vGridSize;
    public float m_fPathNodeWidth;
    public GameObject m_goCapsule;
    public List<Node> path;

    Node[,] m_aGrid;

    private Vector3 m_vNodePos;
    private Vector3 m_vStartPos;
    private int m_iNodeAmountX, m_iNodeAmountY;
    private bool m_bBlocked;        

    // Debugging
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_vGridSize.x, 1, m_vGridSize.y));        
        if (m_aGrid != null)
        {
            foreach (Node node in m_aGrid)
            {
                Gizmos.color = (node.m_bIsBlocked) ? Color.red : Color.white;               

                if (path != null)
                    if (path.Contains(node))
                        Gizmos.color = Color.black;
                
                Gizmos.DrawWireCube(node.m_vPosition, new Vector3(m_fPathNodeWidth * 2, 1, m_fPathNodeWidth * 2));
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(NodeFromWorldPosition(m_goCapsule.transform.position).m_vPosition, new Vector3(m_fPathNodeWidth * 2, 1, m_fPathNodeWidth * 2));
        }
    }

    private void Awake()
    {
        CreateGrid();
    }

    /// <summary>
    /// Creates node grid for pathfinding and also checks if there are obstacles in nodes.
    /// </summary>
    private void CreateGrid()
    {
        m_vStartPos = new Vector3(transform.position.x - ((m_vGridSize.x / 2) - m_fPathNodeWidth),
            transform.position.y + 0, transform.position.z - ((m_vGridSize.y / 2) - m_fPathNodeWidth));

        // Calculates amount of nodes in grid. 
        m_iNodeAmountX = (int)(m_vGridSize.x / (m_fPathNodeWidth * 2));
        m_iNodeAmountY = (int)(m_vGridSize.y / (m_fPathNodeWidth * 2));
        m_aGrid = new Node[m_iNodeAmountX, m_iNodeAmountY];

        //Debug.Log("X: " + m_iNodeAmountX);
        //Debug.Log("Y: " + m_iNodeAmountY);        

        // Creates 2D node grid.
        for (int x = 0; x < m_iNodeAmountX; x++)
        {
            for (int y = 0; y < m_iNodeAmountY; y++)
            {
                m_vNodePos = new Vector3(m_vStartPos.x + (x * m_fPathNodeWidth * 2), 0,
                    m_vStartPos.z + (y * m_fPathNodeWidth * 2));

                // Checks if node is blocked by obstacle collider.
                m_bBlocked = Physics.CheckSphere(m_vNodePos, m_fPathNodeWidth, m_ObstacleMask);
                m_aGrid[x, y] = new Node(m_bBlocked, m_vNodePos, x, y);                
            }
        }
    }

    /// <summary>
    /// Calculates corresponding node in the grid from target objects position.
    /// </summary>
    /// <param name="position">Vector3 position of the object</param>
    /// <returns>Corresponding node</returns>
    public Node NodeFromWorldPosition(Vector3 position)
    {
        float xf = (-transform.position.x + position.x + (m_vGridSize.x / 2)) / m_vGridSize.x;
        float yf = (-transform.position.z + position.z + (m_vGridSize.y / 2)) / m_vGridSize.y;

        xf = Mathf.Clamp01(xf);
        yf = Mathf.Clamp01(yf);

        //Debug.Log("xf: " + xf);

        // Clamps node selection to prevent out of bounds error from list.
        int x = (int)Mathf.Clamp(m_iNodeAmountX * xf, 0, m_iNodeAmountX - 1);
        int y = (int)Mathf.Clamp(m_iNodeAmountY * yf, 0, m_iNodeAmountY - 1);

        //Debug.Log("CapsPos: " + position);
        //Debug.Log("X: " + x + "/Y: " + y); 
        
        // Gets nodes neighbours.
        GetNeighbours(m_aGrid[x, y]);

        return m_aGrid[x, y];
    }

    /// <summary>
    /// Searches nodes neighbouring nodes.
    /// </summary>
    /// <param name="node">Target node</param>
    /// <returns>List of target nodes neighbours</returns>
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();        
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.m_iGridX + x;
                int checkY = node.m_iGridY + y;
                if (checkX >= 0 && checkX < m_iNodeAmountX
                    && checkY >= 0 && checkY < m_iNodeAmountY)
                {
                    neighbours.Add(m_aGrid[checkX, checkY]);
                }
            }
        }

        // Do not use this if AStar script is in use.
        //foreach (Node n in neighbours)
        //{
        //    Debug.Log("Node posX: " + n.m_iGridX + " Node posY: " + n.m_iGridY);
        //}        
        
        return neighbours;
    }
}