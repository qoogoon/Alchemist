using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneCtrl : MonoBehaviour {
    public Sprite[] Act1Scenes;
    public Sprite[] Act2Scenes;
    public Sprite[] Act3Scenes;
    public Sprite[] Act4Scenes;

    private GameData m_gameData;
    private Sprite[] m_scenes;
    private GameObject[] m_SceneObjs;
    private int m_nSceneCount;
    private bool m_SceneMoving = false;  
	// Use this for initialization
	void Start () {
        m_gameData = new GameData();
        m_gameData.setFirstGame(false);

        switch (m_gameData.getActNum())
        {
            case 0:
                m_scenes = Act1Scenes;
                break;
            case 1:
                m_scenes = Act2Scenes;
                break;
            case 2:
                m_scenes = Act3Scenes;
                break;
            case 3:
                m_scenes = Act4Scenes;
                break;
        }

        m_nSceneCount = 0;
        GameObject parentObj = GameObject.Find("Scenes");    //복제할 위치
        GameObject SceneBakerObj = GameObject.Find("SceneBaker");   //복제 대상

        m_SceneObjs = new GameObject[m_scenes.Length];
        int nObjCount = 0;
        for (int i = m_scenes.Length - 1; i > -1; i--)
        {
            m_SceneObjs[i] = (GameObject)Instantiate(SceneBakerObj, parentObj.transform);
            m_SceneObjs[i].transform.localPosition = parentObj.transform.localPosition;
            m_SceneObjs[i].AddComponent<Image>().sprite = m_scenes[i];
            nObjCount++;
        }
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NextScene()
    {
        StartCoroutine(MoveScene());
    }

    IEnumerator MoveScene()
    {
        if(m_nSceneCount < m_SceneObjs.Length-1 && !m_SceneMoving)
        {
            m_SceneMoving = true;
            float fWidth = m_SceneObjs[m_nSceneCount].GetComponent<RectTransform>().rect.width;

            while (m_SceneObjs[m_nSceneCount].transform.localPosition.x < fWidth)
            {
                m_SceneObjs[m_nSceneCount].transform.Translate(Vector3.right * 1f);
                yield return new WaitForSeconds(0.001f);
            }
            m_nSceneCount++;
            m_SceneMoving = false;
        }else if(m_nSceneCount >= m_SceneObjs.Length-1)   //씬 초과. 메인으로넘어가기
        {
            
            if(m_gameData.getActNum() < 3)
            {
                m_gameData.setActNum(m_gameData.getActNum() + 1);   //act 번호 증가
            }
            else  //마지막 act
            {
                m_gameData.setActNum(0);    //초기화
            }
            SceneManager.LoadScene("main");
        }
    }
}
