using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartCtrl : MonoBehaviour {
    public GameObject SignObj;

    private GameData gData;
    // Use this for initialization
    void Start () {
        gData = new GameData();

        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GameStart()
    {
        Animator ani = SignObj.GetComponent<Animator>();
        ani.SetBool("open", true);
        GameObject.Find("Button").GetComponent<Animator>().SetBool("start",true);

        StartCoroutine(loadNextScene());
    }

    IEnumerator loadNextScene()
    {
        yield return new WaitForSeconds(1.3f);
        if (gData.isFirstGame())
        {
            gData.setFirstGame(false);
            SceneManager.LoadScene("scenario");
        }
        else
        {
            SceneManager.LoadScene("main");
        }
        
    }

    public void setInit()
    {
        Debug.Log("초기화");
        gData.setFirstGame(true);
        gData.setActNum(0);
        gData.setDay(0);    //날짜

        
    }
}
