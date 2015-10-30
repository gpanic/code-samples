using System;
using System.Collections;
using System.Xml.Serialization;

[Serializable]
public class FrameRateData
{
    [XmlAttribute]
    public float Average { get; set; }
    [XmlAttribute]
    public float Min { get; set; }
    [XmlAttribute]
    public float Max { get; set; }
    [XmlAttribute]
    public int Updates { get; set; } // number of frames processed

    public FrameRateData()
    {
        Min = 1000;
    }
}
