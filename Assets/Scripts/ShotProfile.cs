using UnityEngine;

public enum ShotTechnique
{
    Ground,
    Lob,
    Curve,
    Volley,
    Header,
    FirstTouch,
    Place,
    Power,
    Deflected
}

[System.Serializable]
public struct ShotProfile
{
    public float power;
    public float accuracy;
    public float angle;
    public float pressure;
    public ShotTechnique technique;
    public bool firstTouch;

    public static ShotProfile Default => new ShotProfile
    {
        power = 1f,
        accuracy = 1f,
        angle = 0f,
        pressure = 0f,
        technique = ShotTechnique.Ground,
        firstTouch = false
    };
}
