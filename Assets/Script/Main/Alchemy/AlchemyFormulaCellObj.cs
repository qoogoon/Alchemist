using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlchemyFormulaCellObj : MonoBehaviour {
    public bool m_bActive;
    public bool m_bAccord;

    private GameObject m_OverlapObj;
    public bool m_Overlap;
    public bool m_stickObj;
    private AlchemyCtrl m_aCtrl;
    // Use this for initialization
    void Start () {
        m_Overlap = false;
        m_bActive = false;
        m_bAccord = false;
        m_stickObj = false;

        m_aCtrl = GameObject.Find("AlchemyCtrl").GetComponent<AlchemyCtrl>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!m_stickObj  && m_Overlap && !m_aCtrl.isMouseDrag())
        {
            //Debug.Log("dd");
           // m_stickObj = true;
        }else
        {
            //m_stickObj = false;
        }
    }
    public void clear()
    {
        m_Overlap = false;
        m_bActive = false;
        m_bAccord = false;
        m_stickObj = false;
        m_OverlapObj = null;
    }

    public GameObject getOverlapObj()
    {
        return m_OverlapObj;
    }

    public bool isStickObj()
    {
        return m_stickObj;
    }

    public void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.tag.Equals("alchemyShapeCell") && !m_OverlapObj)    //같은 모양의 도형이 겹쳐 지면
        {
            AlchemyShapeCellObj cell = coll.gameObject.GetComponent<AlchemyShapeCellObj>();
            m_OverlapObj = coll.gameObject;
            m_Overlap = true;       //겹쳐짐.
            cell.setAccord(true);   //겹쳐진 오브젝트에도 일치 했음을 보냄
            if (cell.CenterCell)
            {
                cell.setDropPosition(transform.position);   //벡터를 맞추기 위해 정가운데 위치한 모양일 경우 저장
            }
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag.Equals("alchemyShapeCell") && coll.gameObject == m_OverlapObj)    //겹쳐져 있던 모양이 Exit되면
        {
            AlchemyShapeCellObj cell = coll.gameObject.GetComponent<AlchemyShapeCellObj>();        //해당 모양의 오브젝트
            m_OverlapObj = null;
            m_Overlap = false;  //겹쳐지지 않음
            cell.setAccord(false);
        }
    }
    
}
