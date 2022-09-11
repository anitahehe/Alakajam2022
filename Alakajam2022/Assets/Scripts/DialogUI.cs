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
    [TabGroup("References")] public CanvasGroup UIGroup;
    [TabGroup("References")] public TMP_Text lineText;
    [TabGroup("References")] public TMP_Text nameText;
    [TabGroup("References")] public Animator continueIndicator;
    [TabGroup("References")] public Transform optionsParent;
    [TabGroup("References")] public GameObject optionButtonPrefab;
    [TabGroup("References")] public Image portrait;
    [TabGroup("References")] public Image textbox;

    [TabGroup("UX")] public float textSpeed;
    [TabGroup("UX")] public KeyCode skipKey;
    [TabGroup("UX")] public KeyCode turboKey;
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
    IEnumerator VolumeFadeRoutine;
    bool dialogActive;

    AudioSource audio;
    TwineParser parser;
    public void Start()
    {
        parser = GetComponent<TwineParser>();
        audio = GetComponent<AudioSource>();
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
        StartCoroutine(FadeUI(1, true));
        currentPassageID = 0;
        RunPassage(currentStory.GetStartingPassage());
        Time.timeScale = 0;
    }

    IEnumerator FadeUI(float alpha, bool off)
    {
        float timer = 0;
        float startAlpha = UIGroup.alpha;
        while (timer < fadeDuration)
        {
            UIGroup.alpha = Mathf.Lerp(startAlpha, alpha, timer / fadeDuration);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        UIGroup.alpha = alpha;
        UI.SetActive(off);
    }

    public void Deactivate()
    {
        dialogActive = false;
        OnDialogComplete.Invoke();
        StartCoroutine(FadeUI(0, false));
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

        if (Input.GetKey(turboKey))
        {
            textButtonPressed = true;
        }
    }

    public IEnumerator FadeOut(AudioSource audioObject, float FadeTime)
    {
        float startVolume = audioObject.volume;

        while (audioObject.volume > 0)
        {
            audioObject.volume -= startVolume * Time.unscaledDeltaTime / FadeTime;
            yield return null;
        }

        audioObject.Stop();
        audioObject.volume = startVolume;
        VolumeFadeRoutine = null;
    }

    IEnumerator DoRunPassage(Passage passage)
    {
        textButtonPressed = true;

        while (passage.HasNext())
        {
            continueIndicator.SetInteger("state", 0);

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
            continueIndicator.SetInteger("state", 1);
            foreach (Transform child in optionsParent)
            {
                Destroy(child.gameObject);
            }

            if (VolumeFadeRoutine != null)
                StopCoroutine(VolumeFadeRoutine);

            VolumeFadeRoutine = null;
            audio.volume = 1;

            if (currentCharacter.dialogSounds.Length > 0)
            {
                audio.clip = currentCharacter.dialogSounds[Random.Range(0, currentCharacter.dialogSounds.Length)];
                audio.Play();
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

            VolumeFadeRoutine = FadeOut(audio, 0.5f);
            
            StartCoroutine(VolumeFadeRoutine);
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
        currentCharacter = character;
        if (portrait.sprite != null && character.portrait == null)
        {
            if (ColorRoutine != null)
            {
                StopCoroutine(ColorRoutine);
                ColorRoutine = null;
            }
            ColorRoutine = PortraitColorChange(fadedColor);
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
            ColorRoutine = PortraitColorChange(Color.white);
            StartCoroutine(ColorRoutine);
        }

        if (character.portrait != null && character.portrait != portrait.sprite)
            StartCoroutine(PortraitChange(character.portrait));

        if (character.font != null)
        {
            lineText.font = character.font;
        }
        lineText.fontStyle = character.fontStyle;
        nameText.text = character.name;
        lineText.color = character.setColor;
        textbox.color = character.setColor;
        continueIndicator.GetComponent<Image>().color = character.setColor;

    }

    IEnumerator PortraitChange(Sprite newPortrait)
    {
        float timer = 0;
        Color startColor = portrait.color;
        while (timer < fadeDuration / 2)
        {
            portrait.color = Color.Lerp(startColor, Color.clear, timer / (fadeDuration / 2));
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        portrait.sprite = newPortrait;
        portrait.color = Color.clear;

        timer = 0f;
        while (timer < fadeDuration / 2)
        {
            portrait.color = Color.Lerp(Color.clear, Color.white, timer / (fadeDuration / 2));
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        portrait.color = Color.white;
    }

    IEnumerator PortraitColorChange(Color endColor)
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
    public FontStyles fontStyle;
    public AudioClip[] dialogSounds;
}
