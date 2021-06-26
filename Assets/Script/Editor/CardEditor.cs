using UnityEditor;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Card card = (Card)target;

        base.OnInspectorGUI();

        if (card.type.ToString().Equals("Action"))
        {
            card.powerCost = EditorGUILayout.IntField("PowerCost", card.powerCost);
            card.timeCost = EditorGUILayout.IntField("TimeCost", card.timeCost);
        }

        if (card.type.ToString().Equals("Operation"))
        {
            card.manaCost = EditorGUILayout.IntField("ManaCost", card.manaCost);
            card.timeCost = EditorGUILayout.IntField("TimeCost", card.timeCost);
        }

        if (card.type.ToString().Equals("Effect"))
        {
            card.manaCost = EditorGUILayout.IntField("ManaCost", card.manaCost);
            card.timeCost = EditorGUILayout.IntField("TimeCost", card.timeCost);
        }

        if (card.type.ToString().Equals("Equipment"))
        {
            card.manaCost = EditorGUILayout.IntField("ManaCost", card.manaCost);
        }
    }

}
