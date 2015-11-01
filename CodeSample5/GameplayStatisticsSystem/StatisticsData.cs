using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StatisticsData
{
    public List<PlaythroughData> PlaythroughData { get; set; }

    public StatisticsData()
    {
        PlaythroughData = new List<PlaythroughData>();
    }

    public void UpdateRespawn(int checkpoint)
    {
        GetCurrentPlaythroughData().Respawns++;
        GetCheckpointData(checkpoint).Respawns++;
    }

    public void UpdateCheckpoint(int checkpoint, float averageFps, int newUpdatesNum, float maxFps, float minFps, float time)
    {
        PlaythroughData currentPlaythrough = GetCurrentPlaythroughData();
        currentPlaythrough.PlayTime.Time += time;

        CheckpointData currentCheckpoint = GetCheckpointData(checkpoint);
        currentCheckpoint.Time = TimeUtil.FormatTime(time);
        currentCheckpoint.AverageFps = averageFps;
        currentCheckpoint.Reached = true;

        // calculate moving average of fps
        FrameRateData frameRate = currentPlaythrough.FrameRate;
        int sumUpdates = frameRate.Updates + newUpdatesNum;
        float previousFps = frameRate.Average * frameRate.Updates / sumUpdates;
        float currentFps = averageFps * newUpdatesNum / sumUpdates;
        frameRate.Average = (float)Math.Round(previousFps + currentFps, 1);
        frameRate.Updates = sumUpdates;

        frameRate.Max = (float)Math.Round(Math.Max(frameRate.Max, maxFps), 0);
        frameRate.Min = (float)Math.Round(Math.Min(frameRate.Min, minFps), 0);
    }

    public void FinishPlaythrough()
    {
        GetCurrentPlaythroughData().Finished = true;
    }

    // start new playthrough
    public void Init(int level, int checkpointNum)
    {
        PlaythroughData newPlaythrough = new PlaythroughData() { Id = PlaythroughData.Count, Date = DateTime.Now };

        // populate checkpoint data with starting values 
        CheckpointData[] cd = CheckpointData.ArrayOf(checkpointNum);
        cd[0].Reached = true;
        newPlaythrough.LevelData.Add(new LevelData() { Id = level, CheckpointData = cd });

        PlaythroughData.Add(newPlaythrough);
    }

    public void SkipCheckpoint(int checkpoint)
    {
        GetCheckpointData(checkpoint).Skipped = true;
    }

    private PlaythroughData GetCurrentPlaythroughData()
    {
        if (PlaythroughData.Count <= 0)
        {
            throw new System.IndexOutOfRangeException("No playthrough data exists");
        }
        return PlaythroughData[PlaythroughData.Count - 1];
    }

    private LevelData GetCurrentLevelData()
    {
        PlaythroughData currentPlaythrough = GetCurrentPlaythroughData();
        if (currentPlaythrough.LevelData.Count <= 0)
        {
            throw new System.IndexOutOfRangeException("No level data exists");
        }
        return currentPlaythrough.LevelData[currentPlaythrough.LevelData.Count - 1];
    }

    private CheckpointData GetCheckpointData(int checkpoint)
    {
        LevelData currentLevel = GetCurrentLevelData();
        if (currentLevel.CheckpointData.Length <= 0)
        {
            throw new System.IndexOutOfRangeException("No checkpoint data exists");
        }
        if (currentLevel.CheckpointData.Length < checkpoint)
        {
            throw new System.ArgumentException("Checkpoint does not exist");
        }
        return currentLevel.CheckpointData[checkpoint - 1];
    }
}
