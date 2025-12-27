using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PlayerInventorySO))]
public class PlayerInventoryEditor : Editor
{
    private bool _showCurrencies = true;
    private Dictionary<ItemType, bool> _tabFoldouts = new Dictionary<ItemType, bool>();

    public override void OnInspectorGUI()
    {
        PlayerInventorySO inventory = (PlayerInventorySO)target;

        // --- PART 1: CURRENCY ---
        EditorGUILayout.Space();
        _showCurrencies = EditorGUILayout.BeginFoldoutHeaderGroup(_showCurrencies, "Currencies");
        if (_showCurrencies)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Current Gems:", inventory.currentGems.ToString(), EditorStyles.boldLabel);
            // You can add Gold or other resources here
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // --- PART 2: TABS DATA ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inventory Tabs", EditorStyles.boldLabel);

        if (inventory.InventoryTabs == null)
        {
            EditorGUILayout.HelpBox("Inventory is not initialized. Enter Play Mode or check Initialize().", MessageType.Info);
            return;
        }

        foreach (var tab in inventory.InventoryTabs)
        {
            ItemType type = tab.Key;
            List<InventorySlot> slots = tab.Value;
            int limit = inventory.TabLimits.ContainsKey(type) ? inventory.TabLimits[type] : 0;

            // Manage foldout state of each Tab
            if (!_tabFoldouts.ContainsKey(type)) _tabFoldouts[type] = false;

            string header = $"{type} ({slots.Count}/{limit})";
            _tabFoldouts[type] = EditorGUILayout.BeginFoldoutHeaderGroup(_tabFoldouts[type], header);

            if (_tabFoldouts[type])
            {
                EditorGUI.indentLevel++;
                if (slots.Count == 0)
                {
                    EditorGUILayout.LabelField("Empty", EditorStyles.miniLabel);
                }
                else
                {
                    for (int i = 0; i < slots.Count; i++)
                    {
                        DrawSlot(slots[i]);
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        // Button to Manual Refresh if needed
        if (GUILayout.Button("Manual Refresh UI"))
        {
            Repaint();
        }
    }

    private void DrawSlot(InventorySlot slot)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        // Display Icon if available
        if (slot.itemDefinition.icon != null)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(slot.itemDefinition.icon);
            GUILayout.Label(texture, GUILayout.Width(30), GUILayout.Height(30));
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(slot.itemDefinition.itemName, EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"ID: {slot.itemDefinition.id} | Amount: {slot.quantity}", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
}