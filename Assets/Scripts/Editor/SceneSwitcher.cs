using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle LargeCommandButtonStyle;
		public static readonly GUIStyle CommandButtonStyle;
		public static readonly GUIStyle SmallCommandButtonStyle;
		public static readonly GUIStyle ExtraSmallCommandButtonStyle;
		public static readonly GUIStyle TextFieldCommandButtonStyle;

		static ToolbarStyles()
		{
			LargeCommandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold,
				fixedWidth = 160
			};
			
			CommandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold,
				fixedWidth = 120
			};
			
			SmallCommandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold,
				fixedWidth = 100
			};
			
			ExtraSmallCommandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold,
				fixedWidth = 70
			};
			
			TextFieldCommandButtonStyle  = new GUIStyle("toolbarTextField")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold,
				fixedHeight = 25,
				fixedWidth = 30
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitcher
	{
		public static event Action<Scene[]> OnScenesLoaded;
		static SceneSwitcher()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
			ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
		}
		
		static void OnLeftToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			if (Application.isPlaying)
			{
				if (GUILayout.Button(new GUIContent("RESTART", "Play from start"), ToolbarStyles.SmallCommandButtonStyle))
				{
					SceneHelper.PlayScene("_Preload");
				}
			}
			else
			{
				if (GUILayout.Button(new GUIContent("PLAY", "Play from start"), ToolbarStyles.ExtraSmallCommandButtonStyle))
				{
					SceneHelper.PlayScene("_Preload");
				}
			}
		
		}
		
		static void OnRightToolbarGUI()
		{
            if(Application.isPlaying || GameLevelsManager.Instance is null) return;
            
			if(GUILayout.Button(new GUIContent("Preload", "Open Main Title"), ToolbarStyles.SmallCommandButtonStyle))
				SceneHelper.OpenSceneWithPath("Assets/Scenes/_Preload.unity");

			List<string> options = new List<string>();
			List<Action> callbacks = new List<Action>();
			
			void LoadLevel(AssetReference levelSetting)
			{
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(levelSetting.editorAsset), OpenSceneMode.Single);
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(levelSetting.editorAsset), OpenSceneMode.Additive);
				BroadcastLoadedScenes();
			}

			options.Add($"Preload");
			callbacks.Add(
				() =>
				{
					EditorSceneManager.OpenScene("Assets/Scenes/_Preload.unity", OpenSceneMode.Single);
					BroadcastLoadedScenes();
				});

			var independentScenes = GameLevelsManager.Instance.GetIndependentScenes();

			foreach (AssetReference independentScene in independentScenes)
			{
				options.Add(independentScene.editorAsset.name);
				callbacks.Add(()=>
				{
					EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(independentScene.editorAsset), OpenSceneMode.Single);
					BroadcastLoadedScenes();
				});
			}
            
            foreach (var sceneAsset in GameLevelsManager.Instance.editorOnlyScenes)
            {
                options.Add($"EditorOnly/{sceneAsset.name}");
                callbacks.Add(()=>
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset), OpenSceneMode.Single);
                    BroadcastLoadedScenes();
                });
            }
            
			var levelSettings = GameLevelsManager.Instance.Levels;

            for (var index = 0; index < levelSettings.Count; index++)
            {
                var levelSetting = levelSettings[index];
                options.Add($"Levels/{levelSettings[index].editorAsset.name}");
                callbacks.Add(() => LoadLevel(levelSetting));
            }
            
            levelSettings = GameLevelsManager.Instance.ExtraLevels;
            
            for (var index = 0; index < levelSettings.Count; index++)
            {
                var levelSetting = levelSettings[index];
                options.Add($"Extra/{levelSettings[index].Asset.name}");
                callbacks.Add(() => LoadLevel(levelSetting));
            }

			int selectedIndex = -1;
			var activeScene = SceneManager.GetActiveScene();
			for (int i = 0; i < options.Count; i++)
			{
				if (activeScene.name == options[i])
				{
					selectedIndex = i;
					break;
				}
			}
			
			var selection = EditorGUILayout.Popup(selectedIndex, options.ToArray());
			if (selection != selectedIndex)
			{
				callbacks[selection].Invoke();
				//SceneHelper.OpenSceneWithPath(AssetDatabase.GetAssetPath(callbacks[selection]));
			}

			GUILayout.FlexibleSpace();

		}

		private static void BroadcastLoadedScenes()
		{
			List<Scene> scenes = new List<Scene>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
				scenes.Add(SceneManager.GetSceneAt(i));
			OnScenesLoaded?.Invoke(scenes.ToArray());
		}
	}

	static class SceneHelper
	{
		static string _sceneToOpen;

		public static void PlayScene(string sceneName)
		{
			if(EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			_sceneToOpen = sceneName;
			EditorApplication.update += OnUpdate;
		}

		public static void OpenScene(string scene)
		{
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo(); 
			EditorSceneManager.OpenScene(AssetDatabase.FindAssets("t:scene " + scene, null)[0]);
		}

		public static void OpenSceneWithPath(string scenePath)
		{
			if (!GameManager.disableLogs) Debug.Log(scenePath);
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo(); 
			EditorSceneManager.OpenScene(scenePath);
		}
		
		static void OnUpdate()
		{
			if (_sceneToOpen == null ||
			    EditorApplication.isPlaying || EditorApplication.isPaused ||
			    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			EditorApplication.update -= OnUpdate;

			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				// need to get scene via search because the path to the scene
				// file contains the package version so it'll change over time
				string[] guids = AssetDatabase.FindAssets("t:scene " + _sceneToOpen, null);
				if (guids.Length == 0)
				{
					if (!GameManager.disableLogs) Debug.LogWarning("Couldn't find scene file");
				}
				else
				{
					string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
					EditorSceneManager.OpenScene(scenePath);
					EditorApplication.isPlaying = true;
				}
			}
			_sceneToOpen = null;
		}

		public static SceneAsset[] FindAllScenes()
		{
			List<SceneAsset> assets = new List<SceneAsset>();
			string[] guids = AssetDatabase.FindAssets($"t:Scene");
			for (int i = 0; i < guids.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
				SceneAsset found = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
				if (found) assets.Add(found);
			}
			return assets.ToArray();
		}
	
	}
}
