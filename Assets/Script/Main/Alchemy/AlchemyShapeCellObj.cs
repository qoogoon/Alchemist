using UnityEngine;
using System.Collections;

public class AlchemyShapeCellObj : MonoBehaviour {
    public bool m_bActive;
    public bool m_bAccord;
    public bool CenterCell;
    private Vector3 m_vDropPosition;
	// Use this for initialization
	void Start () {
        if (transform.parent.name.Equals("shape2x2"))
        {
            if (gameObject.name.Equals("0,0"))
            {
                CenterCell = true;
            }
            else
            {
                CenterCell = false;
            }
        }
        else
        {
            if (gameObject.name.Equals("1,1"))
            {
                CenterCell = true;
            }
            else
            {
                CenterCell = false;
            }
        }
        
        m_bActive = false;
        m_bAccord = false;
    }
	
	// Update is called once per frame
	void Update () {
        m_bActive = gameObject.active;
	}

    public void setCellActive(bool bActive)
    {
        m_bActive = bActive;
    }

    public bool getCellActive()
    {
        return m_bActive;
    }

    //일치 여부 설정
    public void setAccord(bool bAccord)
    {
        m_bAccord = bAccord;
    }

    //공식셀과 일치하는지
    public bool isAccord()
    {
        return m_bAccord;
    }

    //드래그 앤 드랍되는 위치
    public void setDropPosition(Vector3 vDropPosition)
    {
        m_vDropPosition = vDropPosition;
    }

    public Vector3 getDropPosition()
    {
        return m_vDropPosition;
    }

}
