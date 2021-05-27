using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
  int posRes = 0; //Current index in the resolutions array

  Resolution[] resolutions; //List of Unity-capabale resolutions

  Text currentRes;

  Button newGame;
  Button loadGame;
  Button lowerRes;
  Button higherRes;
  Button apply;

  Slider volumeSlider;

  GameObject canvas;
  GameObject optionsMenu;

  void Start()
  {
      canvas = GameObject.Find("Canvas");

      //Options Stuff
      optionsMenu = canvas.transform.Find("OptionsMenu").gameObject;
      volumeSlider = optionsMenu.transform.Find("VolumeSlider").gameObject.GetComponent<Slider>();
      currentRes = optionsMenu.transform.Find("CurrentRes").gameObject.GetComponent<Text>();
      resolutions = Screen.resolutions;
      currentRes.text = ResToString(Screen.currentResolution);

      lowerRes = optionsMenu.transform.Find("LowerRes").gameObject.GetComponent<Button>();
      lowerRes.onClick.AddListener(LowerResolution);

      higherRes = optionsMenu.transform.Find("HigherRes").gameObject.GetComponent<Button>();
      higherRes.onClick.AddListener(UpResolution);

      apply = optionsMenu.transform.Find("Apply").gameObject.GetComponent<Button>();
      apply.onClick.AddListener(Apply);

      //Continue Stuff


      newGame = GameObject.Find("New Game").GetComponent<Button>();
      newGame.onClick.AddListener(StartGame);

      loadGame = GameObject.Find("Load Game").GetComponent<Button>();
      newGame.onClick.AddListener(ContinueGame);

      optionsMenu.SetActive(false);

      if(SaveLoad.savedGames.Count == 0)
      {
          //loadGame.SetActive(false);
      }
  }

  public void StartGame()
  {
      LevelManager.current = new LevelManager(); //Create new Level Manager when starting new game
			SaveLoad.Save(0); //Save the current game as a new saved game
			SceneManager.LoadScene("Main"); //Load first scene
  }

  public void ContinueGame()
  {
    if(SaveLoad.savedGames.Count > 0)
    {
      SaveLoad.Load(0);
      SceneManager.LoadScene(LevelManager.current.playerData.sceneID); //Load last scene
    }
  }

  /*void CreateButton(Transform parent, Vector3 position, Vector2 size,
  UnityEngine.Events.UnityAction method, string butText)
  {
    GameObject button = new GameObject();
    button.transform.parent = parent;
    button.AddComponent<RectTransform>();
    button.AddComponent<Button>();
    //button.GetComponent<Button>().text = "Level: " + butText;
    button.transform.position = position;
    //button.GetComponent<RectTransform>().SetSize(size);
    button.GetComponent<Button>().onClick.AddListener(method);
  }*/

  public void ChangeVolume()
  {
      AudioListener.volume = volumeSlider.value;
  }

  string ResToString(Resolution res)
  {
      return res.width + " x " + res.height;
  }

  public void UpResolution()
  {
      if(posRes < resolutions.Length-1)
      {
        posRes++;
      }
      else
      {
          posRes = 0;
      }

      currentRes.text = ResToString(resolutions[posRes]);
  }

  public void LowerResolution()
  {
      if(posRes > 0)
      {
        posRes--;
      }
      else
      {
          posRes = resolutions.Length-1;
      }

      currentRes.text = ResToString(resolutions[posRes]);
  }

  public void Apply()
  {
      Screen.SetResolution(resolutions[posRes].width, resolutions[posRes].height, true);
  }

  public void Quit()
  {
      Application.Quit();
  }

}
