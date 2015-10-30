using System.Collections;
using System.Xml.Serialization;

[System.Serializable]
[XmlType(TypeName = "Checkpoint")]
public class CheckpointData
{
    [XmlAttribute]
    public int Id { get; set; }
    [XmlAttribute]
    public bool Reached { get; set; }
    [XmlAttribute]
    public bool Skipped { get; set; }
    [XmlAttribute]
    public int Respawns { get; set; }
    [XmlAttribute]
    public string Time { get; set; }
    [XmlAttribute]
    public float AverageFps { get; set; }

    public static CheckpointData[] ArrayOf(int size)
    {
        CheckpointData[] c = new CheckpointData[size];
        for (int i = 0; i < size; ++i)
        {
            c[i] = new CheckpointData() { Id = i + 1, Respawns = 0, Time = TimeUtil.FormatTime(0), AverageFps = 0 };
        }
        return c;
    }
}
