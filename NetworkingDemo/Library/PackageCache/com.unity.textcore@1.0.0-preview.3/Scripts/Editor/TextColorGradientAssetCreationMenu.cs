using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace UnityEditor.TextCore.Text
{
    internal static class TextColorGradientAssetCreationMenu
    {
        #if TEXTMESHPRO_4_0_OR_NEWER
        [MenuItem("Assets/Create/TextMeshPro/Color Gradient", false, 250)]
        #endif
        [MenuItem("Assets/Create/Text/Color Gradient", false, 250)]
        internal static void CreateColorGradient(MenuCommand context)
        {
            string filePath;

            if (Selection.assetGUIDs.Length == 0)
                filePath = "Assets/New TMP Color Gradient.asset";
            else
                filePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

            if (Directory.Exists(filePath))
            {
                filePath += "/New Text Color Gradient.asset";
            }
            else
            {
                filePath = Path.GetDirectoryName(filePath) + "/New Text Color Gradient.asset";
            }

            filePath = AssetDatabase.GenerateUniqueAssetPath(filePath);

            // Create new Color Gradient Asset.
            TextColorGradient colorGradient = ScriptableObject.CreateInstance<TextColorGradient>();

            // Create Asset
            AssetDatabase.CreateAsset(colorGradient, filePath);

            //EditorUtility.SetDirty(colorGradient);

            AssetDatabase.SaveAssets();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(colorGradient));

            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(colorGradient);
        }
    }
}
