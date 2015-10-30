using System.Collections;
using System.Xml.Serialization;

[System.Serializable]
[XmlType(TypeName = "Level")]
public class LevelData
{
    [XmlAttribute]
    public int Id { get; set; }
    public CheckpointData[] CheckpointData { get; set; }
}
