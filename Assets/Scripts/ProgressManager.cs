﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissionStatusCode
{
    Started,
    Completed,
    Closed
}

public class ProgressManager: Singleton<ProgressManager>
{
    // Should communicate with Dialogue System on what dialogue should show when interacting with same interactable
    // Needed to communicate with the dialogue system
    private Dictionary<AMission, MissionStatusCode> missionsStatus;
     
    [HideInInspector]
    public bool allStampsUnlocked = false;

    public List<StampCollectible> stampCollectibles;                        // List of collectibles needed to be able to add stamps in the inspector
    public Dictionary<StampCollectible, bool> stampsUnlockStatus;           // Use a dictinoary to keep track of what stamps are unlocked

    public List<BingoBallCollectible> bingoBallCollectibles;                    // List of collectibles needed to be able to add bingo balls in the inspector
    public Dictionary<BingoBallCollectible, bool> bingoBallsUnlockStatus;        // Keeps track of unlock status for bingo balls


    void Start()
    {
        stampsUnlockStatus = new Dictionary<StampCollectible, bool>();
        bingoBallsUnlockStatus = new Dictionary<BingoBallCollectible, bool>();


        // All stamp collectibles are set to locked (false) at beginning of game
        foreach(StampCollectible stamp in stampCollectibles)
        {
            stampsUnlockStatus.Add(stamp, false);
        }

        missionsStatus = new Dictionary<AMission, MissionStatusCode>();
    }

  
    public void UnlockStampCollectible(StampCollectible stamp)
    {
        if(!stampsUnlockStatus.ContainsKey(stamp))
        {
            Debug.LogError("Cannot find stamp collectible from all unlockable stamps");
            return; 
        }
        else
        {
            stampsUnlockStatus[stamp] = true;
            Debug.Log("Unlocked " + stamp.collectibleName);

            if(HasUnlockedAllStamps())
            {
                allStampsUnlocked = true;
            }
        }
    }


    public bool HasUnlockedAllStamps()
    {
        return !stampsUnlockStatus.ContainsValue(false);
    }


    // Returns a list of all currently unlocked stamps
    public List<StampCollectible> GetUnlockedStamps()
    {
        List<StampCollectible> availableStamps = new List<StampCollectible>();

        foreach(KeyValuePair<StampCollectible, bool> stamp in stampsUnlockStatus)
        {
            if(stamp.Value)
            {
                availableStamps.Add(stamp.Key);
            }
        }

        return availableStamps;
    }


    public void UnlockBingoBallCollectible(BingoBallCollectible bingoBall)
    {
        if (!bingoBallsUnlockStatus.ContainsKey(bingoBall))
        {
            Debug.LogError("Cannot find stamp collectible from all unlockable bingo balls");
            return;
        }
        else
        {
            bingoBallsUnlockStatus[bingoBall] = true;
            Debug.Log("Unlocked " + bingoBall.collectibleName);
        }
    }


    public bool HasUnlockedBingoBall(BingoBallCollectible bingoBall)
    {
        if(!bingoBallsUnlockStatus.ContainsKey(bingoBall))
        {
            Debug.LogError("Bingo ball does not exist");
            return false;
        }
        else
        {
            return bingoBallsUnlockStatus[bingoBall];
        }
    }


    // To be called in Mission Manager when mission is completed
    public void AddMission(AMission mission)
    {
        missionsStatus.Add(mission, MissionStatusCode.Started);
    }


    // Updates the status of the mission 
    public void UpdateMissionStatus(AMission mission, MissionStatusCode status)
    {
        if (!missionsStatus.ContainsKey(mission))
        {
            Debug.LogError("Trying to update a nonexistant mission");
            return;
        }

        missionsStatus[mission] = status;
    }

    public MissionStatusCode GetMissionStatus(AMission mission)
    {
        return missionsStatus[mission];
    }

    // TODO: Save/Load Progress (Look into Scriptable Objects)
}