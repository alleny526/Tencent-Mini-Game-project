using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleGameManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNewGameButton()
    {
        
    }

    public void StartNewGameYes()
    {
        PlayerPrefs.DeleteKey("Process");
        PlayerPrefs.SetInt("Process", 1);
        EnterGame();

    }

    public void StartNewGameNo()
    {

    }

    public void EnterGame()
    {
        if(!PlayerPrefs.HasKey("Process"))
        {
            PlayerPrefs.SetInt("Process", 1);
        }

        int GameLevel = PlayerPrefs.GetInt("Process");
        string GameLevelString = "Scenes/" + "CG_" + GameLevel.ToString();
        SceneManager.LoadSceneAsync(GameLevelString);
    }

}
