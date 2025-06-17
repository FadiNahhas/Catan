using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Catan.Editor.Tools
{
    public class QuickAccess : OdinEditorWindow
    {
        [MenuItem("Tools/Catan/Quick Access")]
        private static void OpenWindow()
        {
            GetWindow<QuickAccess>().Show();
        }

        #region Scene Methods

        [ButtonGroup("Scenes")]
        [Button(ButtonSizes.Large)]
        private void OpenMenuScene() => OpenScene("MainMenu");

        
        [ButtonGroup("Scenes")]
        [Button(ButtonSizes.Large)]
        private void OpenGameScene() => OpenScene("GameScene");
        
        private void OpenScene(string scene_name)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.Log("Cannot change scene while in play mode.");
                return;
            }   
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene($"Assets/Scenes/{scene_name}.unity");  
        }

        #endregion

        #region Folder Methods
        
        [ButtonGroup("Folders")]
        [Button(ButtonSizes.Large)]
        private void Prefabs() => NavigateTo("Assets/Prefabs/Network");
        
        [ButtonGroup("Folders")]
        [Button(ButtonSizes.Large)]
        private void Resources() => NavigateTo("Assets/Resources/Prefabs");
        
        private void NavigateTo(string path)
        {
            Object folder = AssetDatabase.LoadAssetAtPath<Object>(path);

            if (folder == null)
            {
                Debug.LogError($"Folder not found: {path}");
                return;
            }
            
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(folder);
            Selection.activeObject = folder;
            EditorApplication.ExecuteMenuItem("Assets/Open");
        }

        #endregion
    }
}