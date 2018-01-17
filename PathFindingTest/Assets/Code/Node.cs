using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public bool m_bIsBlocked;
    public Vector3 m_vPosition;
    

	public Node (bool bIsBlocked, Vector3 vPosition)
    {
        m_bIsBlocked = bIsBlocked;
        m_vPosition = vPosition;
    }    
}