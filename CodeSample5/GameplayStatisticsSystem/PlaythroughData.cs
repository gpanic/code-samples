using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
[XmlType(TypeName = "Playthrough")]
public class PlaythroughData
{
    [XmlAttribute]
    public int Id { get; set; }
    [XmlAttribute]
    public DateTime Date { get; set; }
    [XmlAttribute]
    public bool Finished { get; set; }
    [XmlAttribute]
    public int Respawns { get; set; }
    public PlayTimeData PlayTime { get; set; }
    public FrameRateData FrameRate { get; set; }
    public List<LevelData> LevelData { get; set; }

    public PlaythroughData()
    {
        PlayTime = new PlayTimeData();
        LevelData = new List<LevelData>();
        FrameRate = new FrameRateData();
    }
}
