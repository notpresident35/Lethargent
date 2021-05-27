using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausingScript : MonoBehaviour
{
    private GameObject pauseMenu;
    private GameObject player;
    private InputManager control;

    private bool isCurrentlyPaused = false;

    void Awake()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        pauseMenu.SetActive(false); // now that we found the pausemenu, make it inactive.
    }

    void Start()
    {
        Time.timeScale = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        control = GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>();
    }

    void Update()
    {
        //Toggle pause menu when pressing Escape
        if(control.pause)
            PauseControl();
    }

    public void PauseGame()
    {
        isCurrentlyPaused = true;
        Time.timeScale = 0; //Stop time
        pauseMenu.SetActive(true); //Show pause menu
    }

    public void ResumeGame()
     {
        isCurrentlyPaused = false;
        Time.timeScale = 1; //Resume time
        pauseMenu.SetActive(false); //Hide pause menu
    }

    //Allows pausing game using external scripts
    public void PauseControl()
    {
        if (isCurrentlyPaused) //If pause menu is on
            ResumeGame();
        else //If pause menu is off
            PauseGame();
    }

    public void SaveGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        LevelManager.current.playerData.sceneID = scene.buildIndex;
        LevelManager.current.playerData.playerPosX = player.transform.position.x;
        LevelManager.current.playerData.playerPosY = player.transform.position.y;
        LevelManager.current.playerData.playerPosZ = player.transform.position.z;
        SaveLoad.WriteToDisk();
    }

    public void LoadGame()
    {
        SaveLoad.LoadFromDisk();
        if (LevelManager.current.playerData.finishedGame)
        {
            //restartText.gameObject.SetActive(true);
            if (control.load)
            {
                goto loading;
            }
            else
            {
                return;
            }
        }
        loading:
            LevelManager.current.isSceneBeingLoaded = true;
            int whichScene = LevelManager.current.playerData.sceneID;
            SceneManager.LoadScene(whichScene);

            float t_x = LevelManager.current.playerData.playerPosX;
            float t_y = LevelManager.current.playerData.playerPosY;
            float t_z = LevelManager.current.playerData.playerPosZ;
            player.transform.position = new Vector3(t_x, t_y, t_z);
    }

    public void OpenOptions()
    {

    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
