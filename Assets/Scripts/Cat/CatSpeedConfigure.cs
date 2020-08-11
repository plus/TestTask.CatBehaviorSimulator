using UnityEngine;

[CreateAssetMenu(fileName = "CatSpeedConfigure", menuName = "CatSpeedConfigure")]
public class CatSpeedConfigure : ScriptableObject
{
    public float DefaultAcceleration = 8f;
    public float DefaultSpeed = 3.5f;
    public float DefaultAngularSpeed = 300f;

    public float FastAcceleration = 50f;
    public float FastSpeed = 25f;
    public float FastAngularSpeed = 600f;

    public float SuperFastAcceleration = 200f;
    public float SuperFastSpeed = 50f;
    public float SuperFastAngularSpeed = 1000f;
}
