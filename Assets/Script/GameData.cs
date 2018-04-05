using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class GameData : MonoBehaviour
{
 
    public Text goldText;
    

    public GameData()
    {
        

    }
    void Awake()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void init()
    {
        
        goldText.text = getPlayerGold() + "G";
    }
    public bool isFirstGame()   //첫 게임인지?
    {
        if (PlayerPrefs.GetInt("FirstGame", 1) == 0)
        {
            return false;
        }else{
            return true;
        }
    }

    public void setFirstGame(bool bFrist)
    {
        Debug.Log("test:" + bFrist);
        if (bFrist)
        {
            PlayerPrefs.SetInt("FirstGame", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FirstGame", 0);
        }
    }

    public bool isDutorial()
    {
        if (PlayerPrefs.GetInt("Dutorial", 1) == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void setDutorial(bool bDutorial)
    {
        if (bDutorial)
        {
            PlayerPrefs.SetInt("Dutorial", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Dutorial", 0);
        }
    }

    public int getActNum()  //1막, 2막~~
    {
        return PlayerPrefs.GetInt("ActNum", 0);
    }

    public void setActNum(int nActNum)
    {
        PlayerPrefs.SetInt("ActNum", nActNum);
    }

    //날짜
    public int getDay()
    {
        return PlayerPrefs.GetInt("Day", 0);
    }

    public void setDay(int nDay)
    {
        PlayerPrefs.SetInt("Day", nDay);
    }

    

    //재료 수집 레벨
    public void setPlayerLv(int nLv)
    {
        PlayerPrefs.SetInt("PlayerLv", nLv);
        
    }

    public int getPlayerLv()
    {
        return PlayerPrefs.GetInt("PlayerLv", 1);  //Lv1부터 시작
    }

    //돈
    public void setPlayerGold(int nGold)
    {
        //Debug.Log(nGold);
        PlayerPrefs.SetInt("PlayerGold", nGold);
        goldText.text = nGold + "G";
    }

    public int getPlayerGold()
    {
        return PlayerPrefs.GetInt("PlayerGold");
    }

    public void addPlayerGold(int nGold)
    {
        setPlayerGold(getPlayerGold() + nGold);
    }
}
