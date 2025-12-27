using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerProfileSO))]
public class PlayerProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (for the Starting Configuration)
        DrawDefaultInspector();

        PlayerProfileSO profile = (PlayerProfileSO)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("RUNTIME DATA (Active Session)", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // --- 1. INTERNAL ART STATUS ---
        if (profile.activeInternalArt != null && profile.activeInternalArt.definition != null)
        {
            InternalArtState art = profile.activeInternalArt;
            
            EditorGUILayout.LabelField($"Active Art: {art.definition.itemName}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Current Level: {art.currentLevel} / {art.definition.maxLevel}");

            // Draw XP Progress Bar
            float progress = (float)art.currentXP / art.maxXP;
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, progress, $"XP: {art.currentXP} / {art.maxXP}");

            if (GUILayout.Button("Debug: Add 50 XP"))
            {
                profile.AddExperience(50);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No Internal Art is currently active.", MessageType.Info);
        }

        // --- 2. EQUIPMENT STATUS ---
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Equipped Items:", EditorStyles.boldLabel);
        
        if (profile.currentEquipments != null && profile.currentEquipments.Count > 0)
        {
            foreach (var kvp in profile.currentEquipments)
            {
                EditorGUILayout.LabelField($"[{kvp.Key}]: {kvp.Value.itemName}");
            }
        }
        else
        {
            EditorGUILayout.LabelField("No equipment fitted.");
        }

        EditorGUILayout.EndVertical();

        // Ensure the Inspector refreshes during Play Mode
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}