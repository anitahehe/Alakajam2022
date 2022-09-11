using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    [TabGroup("References")] public GameObject UI;
    [TabGroup("References")] public TMP_Text lineText;
    [TabGroup("References")] public TMP_Text nameText;
    [TabGroup("References")] public Image continueIndicator;
    [TabGroup("References")] public Transform optionsParent;
    [TabGroup("References")] public GameObject optionButtonPrefab;
    [TabGroup("References")] public Image portrait;
    [TabGroup("References")] public Image textbox;

    [TabGroup("UX")] public float textSpeed;
    [TabGroup("UX")] public KeyCode skipKey;
    [TabGroup("UX")] public Color fadedColor;
    [TabGroup("UX")] public float fadeDuration;

    [TabGroup("CharData")] public Character[] characters;
    Character currentCharacter;

    public UnityEvent onDialogStart;
    public UnityEvent onLineStart;
    public UnityEvent OnLineComplete;
    public UnityEvent OnDialogComplete;

    StoryInfo currentStory;
    int currentPassageID;
    bool textButtonPressed;
    bool optionSelected;

    IEnumerator ColorRoutine;
    bool dialogActive;

    TwineParser parser;
    public void Start()
    {
        parser = GetComponent<TwineParser>();
        // DEBUGGING, REMOVE LATER
        //InitializeStory(0);
        
        // get an event hook to initialize this ui when requested
    }

    public void InitializeStory(int storyIndex)
    {
        if (dialogActive)
        {
            Debug.LogError("dialog is already active");
            return;
        }
        if (storyIndex >= parser.storyInfo.Count)
        {
            Debug.LogError("no story at index " + storyIndex + " exists");
            return;
        }

        dialogActive = true;
        currentStory = parser.GetStoryByIndex(storyIndex);
        currentCharacter = null;
        UI.SetActive(true);
        currentPassageID = 0;
        RunPassage(currentStory.GetStartingPassage());
        Time.timeScale = 0;
    }

    public void Deactivate()
    {
        dialogActive = false;
        OnDialogComplete.Invoke();
        UI.SetActive(false);
        Time.timeScale = 1;
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
        textButtonPressed = true;

        while (passage.HasNext())
        {
            continueIndicator.gameObject.SetActive(true);

            // wait for a skip key to move to next line
            while (textButtonPressed == false)
                yield return null;

            string[] text = passage.GetNext().Split("\n", 2);

            // test if we need a new character to be displayed
            bool newChar = currentCharacter == null;
            if (!newChar)
                newChar = currentCharacter.nameID != text[0];

            if (newChar)
            {
                foreach (Character character in characters)
                {
                    if (text[0] == character.nameID)
                    {
                        ChangeCharacter(character);
                        continue;
                    }
                }
            }

            lineText.text = text[text.Length - 1];
            textButtonPressed = false;
            continueIndicator.gameObject.SetActive(false);
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

                    float timer = 0;
                    while (timer < textSpeed)
                    {
                        timer += Time.unscaledDeltaTime;
                        yield return null;
                    }
                }
            }

            OnLineComplete.Invoke();
            textButtonPressed = false;
        }

        if (passage.links == null)
        {
            OnDialogComplete.Invoke();
            Deactivate();
        }
        else
        {
            optionSelected = false;
            foreach (Link link in passage.links)
            {
                Button button = Instantiate(optionButtonPrefab, optionsParent).GetComponent<Button>();
                button.GetComponentInChildren<TMP_Text>().text = link.name;
                button.GetComponent<Image>().color = currentCharacter.setColor;
                button.onClick.AddListener(() => RunOption(link.pid));
            }

            while (optionSelected == false)
                yield return null;

            yield break;
        }
    }

    public void RunOption(int pid)
    {
        optionSelected = true;
        currentPassageID = pid;
        RunPassage(currentStory.GetPassageByID(pid));
    }

    public void ChangeCharacter(Character character)
    {
        if (portrait.sprite != null && character.portrait == null)
        {
            if (ColorRoutine != null)
            {
                StopCoroutine(ColorRoutine);
                ColorRoutine = null;
            }
            ColorRoutine = PortraitColorChange(fadedColor, fadeDuration);
            StartCoroutine(ColorRoutine);
        }
        else if (portrait.sprite == null && character.portrait == null)
        {
            portrait.color = Color.clear;
        }
        else if (portrait.color != Color.white && character.portrait != null)
        {
            if (ColorRoutine != null)
            {
                StopCoroutine(ColorRoutine);
                ColorRoutine = null;
            }
            ColorRoutine = PortraitColorChange(Color.white, fadeDuration);
            StartCoroutine(ColorRoutine);
        }

        if (character.portrait != null)
            portrait.sprite = character.portrait;

        if (character.font != null)
        {
            lineText.font = character.font;
        }

        nameText.text = character.name;
        lineText.color = character.setColor;
        textbox.color = character.setColor;
        continueIndicator.color = character.setColor;
        currentCharacter = character;
    }

    IEnumerator PortraitColorChange(Color endColor, float speed)
    {
        float timer = 0;
        Color startColor = portrait.color;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            portrait.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            yield return null;
        }
        portrait.color = endColor;

        ColorRoutine = null;
    }
}

[System.Serializable]
public class Character
{
    public Sprite portrait;
    public string name;
    public string nameID;
    public Color setColor;
    public TMP_FontAsset font;
}
