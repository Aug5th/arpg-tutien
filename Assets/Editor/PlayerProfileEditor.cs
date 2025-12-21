using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerProfileSO))]
public class PlayerProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerProfileSO profile = (PlayerProfileSO)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("--- RUNTIME DEBUG INFO ---", EditorStyles.boldLabel);

        float progress = (profile.maxXP > 0) ? (float)profile.currentXP / profile.maxXP : 0;
        EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), progress, $"XP: {profile.currentXP}/{profile.maxXP}");

        EditorGUILayout.LabelField("Current Level:", profile.currentLevel.ToString(), EditorStyles.boldLabel);

        // Cheat Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add 50 XP"))
        {
            profile.AddExperience(50);
        }
        if (GUILayout.Button("Add 500 XP"))
        {
            profile.AddExperience(500);
        }
        EditorGUILayout.EndHorizontal();

        // 5. Quan trọng: Yêu cầu Unity vẽ lại liên tục khi đang Play
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}