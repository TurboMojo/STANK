using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable]
public class OdorImport
{
    public string Odor;
    public string Threshold;
    public string Description;
}

[System.Serializable]
public class OdorImportList
{
    public OdorImport[] Odors;
}

public class OdorImporterEditor : EditorWindow
{
    private string selectedFilePath;
    private string jsonContent;
    private OdorImportList jsonDataList = new OdorImportList();

    [MenuItem("STANK/Import Odors from JSON")]
    private static void OpenWindow()
    {
        GetWindow<OdorImporterEditor>("Odor Importer");
    }

    /*
    [MenuItem("STANK/Odor Import")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<OdorImporterEditor>();
        wnd.titleContent = new GUIContent("My Custom Editor");
    }
    */
    private void OnGUI()
    {
        GUILayout.Label("Select a JSON file to read", EditorStyles.boldLabel);

        if (GUILayout.Button("Select JSON File"))
        {
            selectedFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
            Debug.Log(selectedFilePath);
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                jsonDataList = ReadJsonFile(selectedFilePath);
            } else
            {
                Debug.Log("This");
            }
            //Debug.Log(jsonDataList.Odors[0].Odor.ToString());

            if (jsonDataList.Odors.Length > 0)
            {
                GUILayout.Label("JSON Data:", EditorStyles.boldLabel);

                foreach (var data in jsonDataList.Odors)
                {
                    
                    EditorGUILayout.LabelField("Odor name: ", data.Odor);
                    EditorGUILayout.LabelField("Odor description: ", data.Description);
                    GUILayout.Space(10);
                    float temp = 0f;
                    if(float.TryParse(data.Threshold, out temp))
                    {
                        Debug.Log("Creating " + data.Odor);
                        CreateOdor(data.Odor, temp, data.Description);                        
                    } else
                    {
                        Debug.Log("Could not create odor");
                    }
                    
                }
            } else
            {
                Debug.Log("No JSON data found");
            }
        }
        GUILayout.Space(10);

        EditorGUILayout.TextArea(jsonContent, GUILayout.Height(position.height - 100));
    }

    private OdorImportList ReadJsonFile(string filePath)
    {
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            return jsonDataList = JsonUtility.FromJson<OdorImportList>(jsonContent);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading or parsing JSON file: {e.Message}");
            return null;
        }
    }
    public static void CreateOdor(string name, float threshold, string description)
    {
        Stank asset = ScriptableObject.CreateInstance<Stank>();
        
        asset.name = name;
        asset.Name = name;
        asset.Description = description;
        AssetDatabase.CreateAsset(asset, "Assets/STANK/SOStank/Stanks/"+name+".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
