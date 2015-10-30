using System.Collections;
using System.Xml.Serialization;

[System.Serializable]
public class PlayTimeData
{
    [XmlAttribute]
    public float Time { get; set; }
    [XmlAttribute]
    public string TimeFormatted { get { return TimeUtil.FormatTime(Time); } set { } }
}
