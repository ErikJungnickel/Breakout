using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class LevelEditor : EditorWindow
{
    public LevelsSO levels;

    private Level selectedLevel;

    [MenuItem("Custom/Level Editor")]
    public static void Init()
    {
        LevelEditor editor = EditorWindow.GetWindow<LevelEditor>("Level Editor");
        editor.minSize = new Vector2(800, 600);
        
    }

    void OnGUI()
    {
        if (levels == null)
            return;
        GUILayout.BeginHorizontal("editor");

        GUILayout.BeginVertical("controls");
        GUILayout.Label("Num Levels: " + levels.levels.Count);
        if (GUILayout.Button("Add"))
        {
            Level newLevel = new Level(18, 10);
            newLevel.name = "Level " + (levels.levels.Count + 1);
            newLevel.FillRandom();
            newLevel.levelIndex = levels.levels.Count;

            levels.levels.Add(newLevel);
            EditorUtility.SetDirty(levels);
        }

        if (GUILayout.Button("Delete all"))
        {
            levels.levels.Clear();
            EditorUtility.SetDirty(levels);
        }

        foreach (Level level in levels.levels)
        {
            if (GUILayout.Button(level.name, "Label"))
                selectedLevel = level;
        }
        GUILayout.EndVertical();

        ShowLevelEditor();
        
        GUILayout.EndHorizontal();       
    }

    void ShowLevelEditor()
    {
        if (selectedLevel == null || selectedLevel.levelStructure == null)
            return;

        GUILayout.BeginVertical();
        for (int y = 0; y < selectedLevel.height; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < selectedLevel.width; x++)
            {               
                int index = y * selectedLevel.width + x;

                if (selectedLevel.levelStructure[index] == 0){
                    GUI.color = Color.white;
                    if (GUILayout.Button(""))
                    {
                        selectedLevel.levelStructure[index] = 1;
                        EditorUtility.SetDirty(levels);
                    }
                }
                if (selectedLevel.levelStructure[index] == 1)
                {
                    GUI.color = Color.gray;
                    if (GUILayout.Button(""))
                    {
                        selectedLevel.levelStructure[index] = 2;
                        EditorUtility.SetDirty(levels);
                    }
                }

                if (selectedLevel.levelStructure[index] == 2)
                {
                    GUI.color = Color.blue;
                    if (GUILayout.Button(""))
                    {
                        selectedLevel.levelStructure[index] = 3;
                        EditorUtility.SetDirty(levels);
                    }
                }
                if (selectedLevel.levelStructure[index] == 3)
                {
                    GUI.color = Color.red;
                    if (GUILayout.Button(""))
                    {
                        selectedLevel.levelStructure[index] = 0;
                        EditorUtility.SetDirty(levels);
                    }
                }
            }
            GUILayout.EndHorizontal();    
        }

        GUILayout.Space(20);

        GUI.color = Color.white;

        if (GUILayout.Button("All empty"))
        {
            for (int y = 0; y < selectedLevel.height; y++)
            {
                for (int x = 0; x < selectedLevel.width; x++)
                {
                    int index = y * selectedLevel.width + x;
                    selectedLevel.levelStructure[index] = 0;
                }
            }
            EditorUtility.SetDirty(levels);
        }

        if (GUILayout.Button("Fill random"))
        {
            selectedLevel.FillRandom();
            EditorUtility.SetDirty(levels);
        }

        GUILayout.EndVertical();
    }    
}
