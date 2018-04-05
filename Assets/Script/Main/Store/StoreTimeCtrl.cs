using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StoreTimeCtrl : MonoBehaviour {
    //public value
    public bool bDutorialSelling = false;
    public bool Pause = false;

    //priavet
    private float sec;
    private List<GameObject> m_adventurerList;
    private int m_nAdventurerNumber;
    private StoreCtrl m_storeCtrl;
    
    // Use this for initialization
    void Start () {
        m_storeCtrl = GameObject.Find("StoreCtrl").GetComponent<StoreCtrl>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void startTimeLine(List<GameObject> adventurerList)
    {
        m_adventurerList = adventurerList;
        m_nAdventurerNumber = m_adventurerList.Count;
        StartCoroutine(timeLineRoutine());
    }

    IEnumerator timeLineRoutine()
    {
        sec = GameObject.Find("AdventurerCtrl").GetComponent<AdventurerCtrl>().nAdventurerVisitNumber * 2;
        
        Scrollbar timeBar = gameObject.GetComponent<Scrollbar>();
        timeBar.value = 0;
        List<int> appearTimeList = new List<int>();
        for(int i = m_nAdventurerNumber -1; i >= 0; i--)
        {
            appearTimeList.Add((int)((float)(sec / m_nAdventurerNumber)) * i);
        }

        int nCount = 0;
        int nListCount = m_adventurerList.Count - 1;    //모험가가 포개 질때 가장 먼저온 놈이 위에 오게 하기위함
        float fValue = 1f / (sec+5);

        bDutorialSelling = false;
        while (1f > timeBar.value)
        {
            yield return new WaitForSeconds(1f);
            if (!Pause)
            {
                if (nListCount >= 0 && nCount >= appearTimeList[nListCount])
                {
                    m_adventurerList[nListCount].GetComponent<AdventurerObj>().startAction();
                    nListCount--;
                }
                timeBar.value += fValue;
                nCount++;
            }
            
        }
        bDutorialSelling = true;
        m_storeCtrl.ClosingStore();
        gameObject.SetActive(false);
        Debug.Log("end");
    }

}
