using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public GameObject UI;
    public TMP_Text lineText;
    public TMP_Text nameText;
    public GameObject continueIndicator;
    public Transform optionsParent;
    public GameObject optionButtonPrefab;
    public float textSpeed;
    public KeyCode skipKey;

    public UnityEvent onDialogStart;
    public UnityEvent onLineStart;
    public UnityEvent OnLineComplete;
    public UnityEvent OnDialogComplete;

    int currentCharacter;

    [MultiLineProperty(10)]
    public string testDialog;

    StoryInfo currentStory;
    int currentPassageID;

    bool textButtonPressed;
    bool optionSelected;

    TwineParser parser;
    public void Start()
    {
        parser = GetComponent<TwineParser>();
        // DEBUGGING, REMOVE LATER
        Initialize(0);
        
        // get an event hook to initialize this ui when requested
    }

    public void Initialize(int storyIndex)
    {
        currentStory = parser.GetStoryByIndex(0);
        UI.SetActive(true);
        currentPassageID = 0;
        RunPassage(currentStory.GetStartingPassage());
    }

    public void Deactivate()
    {
        OnDialogComplete.Invoke();
        UI.SetActive(false);
    }

    public void RunPassage(Passage passage)
    {
        StartCoroutine(DoRunPassage(passage));
    }

    private void Update()
    {
        if (Input.GetKeyDown(skipKey) || Input.GetMouseButtonDown(0))
        {
            textButtonPressed = true;
        }
    }

    IEnumerator DoRunPassage(Passage passage)
    {
        lineText.text = passage.text;
        textButtonPressed = false;
        continueIndicator.SetActive(false);
        foreach (Transform child in optionsParent)
        {
            Destroy(child.gameObject);
        }

        // animate 
        int counter = 0;
        if (textSpeed > 0.0f)
        {
            foreach (char c in lineText.text)
            {
                // make characters visible slowly
                counter++;
                lineText.maxVisibleCharacters = counter;

                if (textButtonPressed)
                {
                    // We've requested a skip of the entire line.
                    // Display all of the text immediately.
                    lineText.maxVisibleCharacters = lineText.text.Length;
                    break;
                }

                yield return new WaitForSeconds(textSpeed);
            }
        }

        OnLineComplete.Invoke();

        if (passage.HasMultipleBranches())
        {
            optionSelected = false;
            // display all options
            foreach(Link link in passage.links)
            {
                Button button = Instantiate(optionButtonPrefab, optionsParent).GetComponent<Button>();
                button.GetComponentInChildren<TMP_Text>().text = link.name;
                button.onClick.AddListener(() => RunOption(link.pid));
            }
            while (optionSelected == false)
                yield return null;
        }
        else
        {
            textButtonPressed = false;

            continueIndicator.SetActive(true);
            // wait for a skip key to move to next line
            while (textButtonPressed == false)
                yield return null;

            if (passage.NumOfBranches() == 0)
                Deactivate();
            else
                RunPassage(currentStory.GetPassageByID(++currentPassageID));
        }
    }

    public void RunOption(int pid)
    {
        optionSelected = true;
        currentPassageID = pid;
        RunPassage(currentStory.GetPassageByID(pid));
    }


    public void ChangeCharacter(int character)
    {
        currentCharacter = character;
    }
}
