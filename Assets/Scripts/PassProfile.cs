using UnityEngine;

public enum PassTechnique
{
    ShortGround,
    LongGround,
    ThroughBall,
    Lofted,
    FirstTime
}

[System.Serializable]
public struct PassProfile
{
    public float power;
    public float accuracy;
    public float pressure;
    public PassTechnique technique;
    public bool toSpace;
}
