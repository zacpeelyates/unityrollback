using UnityEngine;
using UnityEngine.TextCore.Text;
using System.IO;


namespace UnityEditor.TextCore.Text
{
    static class TextSettingsCreationMenu
    {
        [MenuItem("Assets/Create/Text/Text Settings", false, 300, true)]
        internal static void CreateTextSettingsAsset()
        {
            string filePath;
            if (Selection.assetGUIDs.Length == 0)
            {
                // No asset selected.
                filePath = "Assets";
            }
            else
            {
                // Get the path of the selected folder or asset.
                filePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                // Get the file extension of the selected asset as it might need to be removed.
                string fileExtension = Path.GetExtension(filePath);
                if (fileExtension != "")
                {
                    filePath = Path.GetDirectoryName(filePath);
                }
            }

            string filePathWithName = AssetDatabase.GenerateUniqueAssetPath(filePath + "/Text Settings.asset");

            // Create new TextSettings asset
            TextSettings textSettings = ScriptableObject.CreateInstance<TextSettings>();
            AssetDatabase.CreateAsset(textSettings, filePathWithName);

            // Not sure if this is still necessary in newer versions of Unity.
            EditorUtility.SetDirty(textSettings);

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(textSettings);
        }
    }
}
