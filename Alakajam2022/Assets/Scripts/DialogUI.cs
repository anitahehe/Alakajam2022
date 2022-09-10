using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class DialogUI : MonoBehaviour
{
    public GameObject UI;
    public TMP_Text lineText;
    public TMP_Text nameText;
    public GameObject continueIndicator;
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

    bool textButtonPressed;

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
        RunPassage(currentStory.GetStartingPassage());
    }

    public void Deactivate()
    {
        OnDialogComplete.Invoke();
        UI.SetActive(false);
    }

    public void RunPassage(Passage passage)
    {
        RunLine(passage.text, 0);

    }

    public void RunLine(string line, int character)
    {
        if (character != currentCharacter)
            ChangeCharacter(character);

        StartCoroutine(DoRunLine(line));
    }

    IEnumerator DoRunLine(string line)
    {
        lineText.text = line;

        // animate 
        int counter = 0;
        if (textSpeed > 0.0f)
        {
            foreach (char c in lineText.text)
            {
                // make characters visible slowly
                counter++;
                lineText.maxVisibleCharacters = counter;

                if (Input.GetKeyDown(skipKey))
                {
                    // We've requested a skip of the entire line.
                    // Display all of the text immediately.
                    lineText.maxVisibleCharacters = lineText.text.Length;
                    break;
                }

                yield return new WaitForSeconds(textSpeed);
            }
        }

        continueIndicator.SetActive(true);
        OnLineComplete.Invoke();

        textButtonPressed = false;

        // wait for a skip key to move to next line
        while (textButtonPressed == false)
        {
            textButtonPressed = Input.GetKeyDown(skipKey);
            yield return null;
        }

        Deactivate();
    }

    public void RunOptions(string options)
    {

    }


    public void ChangeCharacter(int character)
    {
        currentCharacter = character;
    }
}
