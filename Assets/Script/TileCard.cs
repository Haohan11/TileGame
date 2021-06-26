using UnityEngine;

[CreateAssetMenu(fileName = "New TileCard", menuName = "TileCard")]
public class TileCard : ScriptableObject
{
    public int number;

    public enum Type { Vegetation, MetalRock, Water };
    [Space]
    [Space]
    public Sprite artwork;
    public new string name;

    [Multiline(5)]
    public string description;

    [Space]
    [Space]
    public Type type;
}
