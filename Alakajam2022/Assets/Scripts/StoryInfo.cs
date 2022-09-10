using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Link
{
    public string name;
    public string link;
    public int pid;
}

[System.Serializable]
public class Passage
{
    public string text;
    public Link[] links;
    public string name;
    public int pid;

    public int NumOfBranches()
    {
        return links.Length;
    }

    public bool HasMultipleBranches()
    {
        return links.Length > 1;
    }
    public bool IsTerminalNode()
    {
        return links.Length == 0;
    }

    public int GetNextPassageId(int branchIndex = 0)
    {
        int returnID = -1;

        if (links.Length > 0)
        {
            if (links.Length >= branchIndex)
            {
                Debug.LogWarning(String.Format("Passage of index {0} is does not have {1} branches, returning first branch", pid, branchIndex + 1));
                branchIndex = 0;
            }
            returnID = links[branchIndex].pid;
        }
        else 
        {
            Debug.LogError(String.Format("Passage of index {0} is Terminal Node", pid));
        }
        if (links.Length > 1)
        {
            Debug.LogWarning(String.Format("Passage of index {0} has multiple branches, return first one", pid));
        }

        return returnID;
    }
}

[System.Serializable]
public class StoryInfo
{
    public string name;
    public string creator;
    public Passage[] passages;
    public int startnode;

    public static StoryInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<StoryInfo>(jsonString);
    }

    public Passage GetStartingPassage()
    {
        return GetPassageByID(startnode);
    }

    public Passage GetPassageByID(int id)
    {
        Passage returnPassage = null;

        foreach (Passage passage in passages)
        {
            if (passage.pid == id)
            {
                returnPassage = passage;
                break;
            }
        }

        if (returnPassage == null)
        {
            Debug.LogError(String.Format("Passage of index {0} does not exist", id));
        }

        return returnPassage;
    }
}
