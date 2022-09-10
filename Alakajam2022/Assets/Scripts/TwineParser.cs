using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/*
 * To Use:
 * 1. Get Story using GetStoryByIndex(int index) 
 * 2. Get First Passage using StoryInfo.GetStartingPassage()
 * 
 * 3. Check for Dialogie branches using Passage.HasMultipleBranches(), Passage.NumOfBranches() and Passage.IsTerminalNode()
 * 4. Output dialogue using Passage.text, Passage.name
 * 5. Output branches using Link.name
 * 6. Get Next Dialogue Id using Passage.GetNextPassageId(int branchIndex = 0)
 * 7. Repeat step 3
 */



public class TwineParser : MonoBehaviour
{
    [Header("###########Add in Twine stories here:###########")]
    public TextAsset[] stories;

    public List<StoryInfo> storyInfo;


    [Button("Refresh Story Info")]
    // Start is called before the first frame update
    void Start()
    {
        storyInfo = new List<StoryInfo>();
        foreach(TextAsset story in stories)
        {
            StoryInfo info = StoryInfo.CreateFromJSON(story.ToString());
            info.ScrubForButtonPrompts();
            storyInfo.Add(info);
        }
    }

    public StoryInfo GetStoryByIndex(int index)
    {
        StoryInfo returnStory = null;
        if (index < storyInfo.Count)
        {
            returnStory = storyInfo[index];
        }
        else
        {
            Debug.LogError("Story index out of scope");
        }
        return returnStory;
    }
}
