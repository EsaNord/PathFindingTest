using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGridManager : MonoBehaviour
{
    public LayerMask m_ObstacleMask;
    public Vector2 m_vGridSize;
    public float m_fPathNodeWidth;
    public GameObject m_goCapsule;

    Node[,] m_aGrid;

    private Vector3 m_vNodePos;
    private Vector3 m_vStartPos;
    private int m_iNodeAmountX, m_iNodeAmountY;
    private bool m_bBlocked;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_vGridSize.x, 1, m_vGridSize.y));        
        if (m_aGrid != null)
        {
            foreach (Node node in m_aGrid)
            {
                if (node.m_bIsBlocked)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawWireCube(node.m_vPosition, new Vector3(m_fPathNodeWidth * 2, 1, m_fPathNodeWidth * 2));
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(NodeFromWorldPosition(m_goCapsule.transform.position).m_vPosition, new Vector3(m_fPathNodeWidth * 2, 1, m_fPathNodeWidth * 2));
        }
    }

    private void Awake()
    {        
        m_vStartPos = new Vector3(transform.position.x - ((m_vGridSize.x / 2) - m_fPathNodeWidth),
            transform.position.y + 0, transform.position.z - ((m_vGridSize.y / 2) - m_fPathNodeWidth));        

        m_iNodeAmountX = (int)(m_vGridSize.x / (m_fPathNodeWidth * 2));
        m_iNodeAmountY = (int)(m_vGridSize.y / (m_fPathNodeWidth * 2));
        m_aGrid = new Node[m_iNodeAmountX, m_iNodeAmountY];

        //Debug.Log("X: " + m_iNodeAmountX);
        //Debug.Log("Y: " + m_iNodeAmountY);        

        for (int x = 0; x < m_iNodeAmountX; x++)
        {
            for (int y = 0; y < m_iNodeAmountY; y++)
            {
                m_vNodePos = new Vector3(m_vStartPos.x + (x * m_fPathNodeWidth * 2), 0,
                    m_vStartPos.z + (y * m_fPathNodeWidth * 2));

                m_bBlocked = Physics.CheckSphere(m_vNodePos, m_fPathNodeWidth, m_ObstacleMask);
                m_aGrid[x, y] =new Node(m_bBlocked, m_vNodePos);
                //Debug.Log("NodePos: " + m_vNodePos);
            }
        }
    }

    private Node NodeFromWorldPosition(Vector3 position)
    {
        float xf = (-transform.position.x + position.x + (m_vGridSize.x / 2)) / m_vGridSize.x;
        float yf = (-transform.position.z + position.z + (m_vGridSize.y / 2)) / m_vGridSize.y;

        xf = Mathf.Clamp01(xf);
        yf = Mathf.Clamp01(yf);

        //Debug.Log("xf: " + xf);

        int x = (int)Mathf.Clamp(m_iNodeAmountX * xf, 0, m_iNodeAmountX);
        int y = (int)Mathf.Clamp(m_iNodeAmountY * yf, 0, m_iNodeAmountY);

        //Debug.Log("CapsPos: " + position);
        //Debug.Log("X: " + x + "/Y: " + y);

        return m_aGrid[x, y];
    }
}