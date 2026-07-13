using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitFromStartWindow : EditorWindow
{
    private const string PrefKey = "AlexTools_InitFromStart_LastScene";
    private SceneAsset startScene;
    private string startScenePath;
    private string previousScenePath;

    [MenuItem("Tools/Inciar desde escena")]
    public static void ShowWindow()
    {
        GetWindow<InitFromStartWindow>("Inciar desde escena");
    }

    private void OnEnable()
    {
        // Cargar la escena guardada previamente
        startScenePath = EditorPrefs.GetString(PrefKey, "");
        if (!string.IsNullOrEmpty(startScenePath))
        {
            startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScenePath);
        }
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Escena de inicio:", EditorStyles.boldLabel);
        var newScene = (SceneAsset)EditorGUILayout.ObjectField(startScene, typeof(SceneAsset), false);

        if (newScene != startScene)
        {
            startScene = newScene;
            startScenePath = AssetDatabase.GetAssetPath(startScene);
            EditorPrefs.SetString(PrefKey, startScenePath);
        }

        GUI.enabled = startScene != null && !EditorApplication.isPlayingOrWillChangePlaymode;
        if (GUILayout.Button("Iniciar desde esta escena"))
        {
            previousScenePath = SceneManager.GetActiveScene().path;
            EditorPrefs.SetString("AlexTools_InitFromStart_PreviousScene", previousScenePath);

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(startScenePath);
                EditorApplication.isPlaying = true;
            }
        }
        GUI.enabled = true;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Al salir del modo Play, volver a la escena original
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            var prevScene = EditorPrefs.GetString("AlexTools_InitFromStart_PreviousScene", "");
            if (!string.IsNullOrEmpty(prevScene) && prevScene != startScenePath)
            {
                EditorPrefs.DeleteKey("AlexTools_InitFromStart_PreviousScene");
                EditorSceneManager.OpenScene(prevScene);
            }
        }
    }
}