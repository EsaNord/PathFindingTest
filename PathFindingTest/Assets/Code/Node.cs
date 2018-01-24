using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public bool m_bIsBlocked;
    public Vector3 m_vPosition;

    public int m_iGridX;
    public int m_iGridY;

    public int m_iGCost;
    public int m_iHCost;
    public int m_iFCost
    {
        get { return m_iGCost + m_iHCost; }
    }

    public Node m_nParent; 

	public Node (bool bIsBlocked, Vector3 vPosition, int gridX, int gridY)
    {
        m_bIsBlocked = bIsBlocked;
        m_vPosition = vPosition;
        m_iGridX = gridX;
        m_iGridY = gridY;
    }    
}