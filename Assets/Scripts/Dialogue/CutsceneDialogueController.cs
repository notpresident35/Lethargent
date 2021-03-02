using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class CutsceneDialogueController : MonoBehaviour {

	public static event Action<Story> OnCreateStory;

	public List<TextAsset> inkJSONAssets;


	[SerializeField]
	private Transform dialogueRoot = null;
	[SerializeField]
	private GameObject background;

	// UI Prefabs
	[SerializeField]
	private Text textPrefab = null;

	private Story story;

	void Awake () {
		CutsceneManager.CutsceneStart += StartStory;
		CutsceneManager.CutsceneContinue += Continue;
		CutsceneManager.CutsceneStop += RemoveChildren;
	}

	// Creates a new Story object with the compiled story, then calls OnCreateStory using it and displays the first options
	void StartStory () {
		story = new Story (inkJSONAssets[CutsceneManager.CutsceneID].text);
		if (OnCreateStory != null) OnCreateStory (story);
		RefreshView ();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView () {
		// Continue gets the next line of the story
		string text = story.Continue ();
		// This removes any white space from the text.
		text = text.Trim ();
		// Lines saying REPEAT will be skipped, continuing to display the previous line's text without re-animating it.
		if (text == "REPEAT") { return; } 

		// Remove all the UI on screen
		RemoveChildren ();

		// Lines saying SKIP will be skipped, clearing the previous line to leave no dialogue displayed
		if (text == "SKIP") { return; } 
		// Display the text on screen!
		CreateContentView (text);
		background.SetActive (true);
	}


	void Continue () {
		if (story.canContinue) {
			RefreshView ();
		}
    }

	// Creates a textbox showing the the line of text
	void CreateContentView (string text) {
		Text storyText = Instantiate (textPrefab) as Text;
		storyText.text = text;
		storyText.transform.SetParent (dialogueRoot, false);
	}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren () {
		int childCount = dialogueRoot.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			Destroy (dialogueRoot.GetChild (i).gameObject);
		}
		background.SetActive (false);
	}

	public object GetVariable (string name) {
		return story.variablesState [name];
	}

	public void SetVariable (string name, object value) {
		story.variablesState [name] = value;
	}
}
