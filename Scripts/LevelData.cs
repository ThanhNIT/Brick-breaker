using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "BrickBreaker/LevelData")]
public class LevelData : ScriptableObject
{
    public int columns = 7;
    public int rows = 4;
    public int[] hpPerRow;
    public int ballCount = 1; // số bóng trong level này
    public float brickWidth = 1.15f;
    public float brickHeight = 0.5f;
}