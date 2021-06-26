using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]

public class Card : ScriptableObject
{
    public int number;

    [Space]
    [Space]
    public Sprite artwork;
    public new string name;

    [Multiline(5)]
    public string description;

    
    public enum Type { Action, Operation, Effect, Equipment, Godgiven };
    [Space]
    [Space]
    public Type type;

    [HideInInspector]
    public int powerCost;

    [HideInInspector]
    public int manaCost;

    [HideInInspector]
    public int timeCost;
}
