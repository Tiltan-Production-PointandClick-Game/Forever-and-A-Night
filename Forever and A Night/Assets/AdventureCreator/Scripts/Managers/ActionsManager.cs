/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2018
 *	
 *	"ActionsManager.cs"
 * 
 *	This script handles the "Actions" tab of the Game Editor window.
 *	Custom actions can be added and removed by selecting them with this.
 * 
 */

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{

	/**
	 * Handles the "Actions" tab of the Game Editor window.
	 * All available Actions are listed here, and custom Actions can be added.
	 */
	[System.Serializable]
	public class ActionsManager : ScriptableObject
	{
		
		#if UNITY_EDITOR

		/** The folder path to any custom Actions */
		public string customFolderPath = "AdventureCreator/Scripts/Actions";

		#endif

		/** If True, then Actions can be displayed in an ActionList's Inspector window */
		public bool displayActionsInInspector = true;
		/** How Actions are arranged in the ActionList Editor window (ArrangedVertically, ArrangedHorizontally) */
		public DisplayActionsInEditor displayActionsInEditor = DisplayActionsInEditor.ArrangedVertically;
		/** If True, then multiple ActionList Editor windows can be opened at once */
		public bool allowMultipleActionListWindows = false;
		/** The effect the mouse scrollwheel has inside the ActionList Editor window (PansWindow, ZoomsWindow) */
		public ActionListEditorScrollWheel actionListEditorScrollWheel = ActionListEditorScrollWheel.PansWindow;
		/** If True, then panning is inverted in the ActionList Editor window (useful for Macbooks) */
		public bool invertPanning = false;
		/** The speed factor for panning/zooming */
		public float panSpeed = 1f;
		/** The index number of the default Action (deprecated) */
		public int defaultClass;
		/** The class name of the default Action */
		public string defaultClassName;
		/** A List of all Action classes found */
		public List<ActionType> AllActions = new List<ActionType>();

		#if UNITY_EDITOR

		private ActionType selectedClass = null;

		private bool showEditing = true;
		private bool showCustom = true;
		private bool showCategories = true;
		private bool showActionTypes = true;
		private bool showActionType = true;
		
		private int selectedCategoryInt = -1;
		private ActionCategory selectedCategory;

		#endif


		/**
		 * <summary>Gets the filename of the default Action.</summary>
		 * <returns>The filename of the default Action.</returns>
		 */
		public string GetDefaultAction ()
		{
			Upgrade ();

			if (!string.IsNullOrEmpty (defaultClassName))
			{
				return defaultClassName;
			}

			return "";
		}


		private void Upgrade ()
		{
			if (defaultClass >= 0 && AllActions.Count > 0 && AllActions.Count > defaultClass)
			{
				defaultClassName = AllActions[defaultClass].fileName;
				defaultClass = -1;
			}

			if (string.IsNullOrEmpty (defaultClassName) && AllActions.Count > 0)
			{
				defaultClassName = AllActions[0].fileName;
				defaultClass = -1;
			}
		}
		
		
		#if UNITY_EDITOR

		public void ShowGUI ()
		{
			ShowEditingGUI ();

			EditorGUILayout.Space ();

			ShowCustomGUI ();

			EditorGUILayout.Space ();

			if (AllActions.Count > 0)
			{
				Upgrade ();
				ShowCategoriesGUI ();

				if (selectedCategoryInt >= 0)
				{
					EditorGUILayout.Space ();
					ShowActionTypesGUI ();
					if (selectedClass != null)
					{
						EditorGUILayout.Space ();
						ShowActionTypeGUI ();
					}
				}
			}
			else
			{
				EditorGUILayout.HelpBox ("No Action subclass files found.", MessageType.Warning);
			}

			if (GUI.changed)
			{
				Upgrade ();
				EditorUtility.SetDirty (this);
			}
		}


		private int sideAction;
		private void SideMenu (int index)
		{
			sideAction = index;
			GenericMenu menu = new GenericMenu ();

			ActionType subclass = AllActions[index];
			if (!string.IsNullOrEmpty (defaultClassName) && subclass.fileName != defaultClassName)
											{
				menu.AddItem (new GUIContent ("Make default"), false, Callback, "Make default");
				menu.AddSeparator ("");
			}

			menu.AddItem (new GUIContent ("Search local instances"), false, Callback, "Search local instances");
			menu.AddItem (new GUIContent ("Search all instances"), false, Callback, "Search all instances");

			menu.ShowAsContext ();
		}


		private void Callback (object obj)
		{
			if (sideAction >= 0)
			{
				ActionType subclass = AllActions[sideAction];

				switch (obj.ToString ())
				{
					case "Make default":
						if (AllActions.Contains (subclass))
						{
							defaultClassName = subclass.fileName;
							subclass.isEnabled = true;
						}
						break;

					case "Search local instances":
						SearchForInstances (true, subclass);
						break;

					case "Search all instances":
						if (UnityVersionHandler.SaveSceneIfUserWants ())
						{
							SearchForInstances (false, subclass);
						}
						break;
				}
			}
		}


		private void ShowEditingGUI ()
		{
			EditorGUILayout.BeginVertical (CustomStyles.thinBox);
			showEditing = CustomGUILayout.ToggleHeader (showEditing, "ActionList editing settings");
			if (showEditing)
			{
				displayActionsInInspector = CustomGUILayout.ToggleLeft ("List Actions in Inspector window?", displayActionsInInspector, "AC.KickStarter.actionsManager.displayActionsInInspector", "If True, then Actions can be displayed in an ActionList's Inspector window");
				displayActionsInEditor = (DisplayActionsInEditor) CustomGUILayout.EnumPopup ("Actions in Editor are:", displayActionsInEditor, "AC.KickStarter.actionsManager.displayActionsInEditor", "How Actions are arranged in the ActionList Editor window");
				actionListEditorScrollWheel = (ActionListEditorScrollWheel) CustomGUILayout.EnumPopup ("Using scroll-wheel:", actionListEditorScrollWheel, "AC.KickStarter.actionsManager.actionListEditorScrollWheel", "The effect the mouse scrollwheel has inside the ActionList Editor window");

				if (actionListEditorScrollWheel == ActionListEditorScrollWheel.ZoomsWindow)
				{
					EditorGUILayout.HelpBox ("Panning is possible by holding down the middle-mouse button.", MessageType.Info);
				}

				panSpeed = CustomGUILayout.FloatField ((actionListEditorScrollWheel == ActionListEditorScrollWheel.PansWindow) ? "Panning speed:" : "Zoom speed:", panSpeed, "AC.KickStarter.actionsManager.panSpeed", "The speed factor for panning/zooming");
				invertPanning = CustomGUILayout.ToggleLeft ("Invert panning in ActionList Editor?", invertPanning, "AC.KickStarter.actionsManager.invertPanning", "If True, then panning is inverted in the ActionList Editor window (useful for Macbooks)");
				allowMultipleActionListWindows = CustomGUILayout.ToggleLeft ("Allow multiple ActionList Editor windows?", allowMultipleActionListWindows, "AC.KickStarter.actionsManager.allowMultipleActionListWindows", "If True, then multiple ActionList Editor windows can be opened at once");
			}
			EditorGUILayout.EndVertical ();
		}


		private void ShowCustomGUI ()
		{
			EditorGUILayout.BeginVertical (CustomStyles.thinBox);
			showCustom = CustomGUILayout.ToggleHeader (showCustom, "Custom Action scripts");
			if (showCustom)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Folder to search:", GUILayout.Width (110f));
				GUILayout.Label (customFolderPath, EditorStyles.textField);
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Set directory", EditorStyles.miniButtonLeft))
				{
					string path = EditorUtility.OpenFolderPanel ("Set custom Actions directory", "Assets", "");
					string dataPath = Application.dataPath;
					if (path.Contains (dataPath))
					{
						if (path == dataPath)
						{
							customFolderPath = "";
						}
						else
						{
							customFolderPath = path.Replace (dataPath + "/", "");
						}
					}
					else
					{
						ACDebug.LogError ("Cannot set new directory - be sure to select within the Assets directory.");
					}
				}
				if (GUILayout.Button ("Clear", EditorStyles.miniButtonRight))
				{
					customFolderPath = "";
				}
				GUILayout.EndHorizontal ();
			}
			EditorGUILayout.EndVertical ();
		}


		private void ShowCategoriesGUI ()
		{
			EditorGUILayout.BeginVertical (CustomStyles.thinBox);
			showCategories = CustomGUILayout.ToggleHeader (showCategories, "Action categories");
			if (showCategories)
			{
				ActionCategory[] categories = (ActionCategory[]) System.Enum.GetValues (typeof(ActionCategory));

				for (int i=0; i<categories.Length; i++)
				{
					if ((i % 4) == 0)
					{
						if (i > 0)
						{
							EditorGUILayout.EndHorizontal ();
						}
						EditorGUILayout.BeginHorizontal ();
					}

					if (GUILayout.Toggle (selectedCategoryInt == i, categories[i].ToString (), "Button", GUILayout.MinWidth (70f)))
					{
						if (selectedCategoryInt != i || selectedCategory != categories[i])
						{
							selectedCategoryInt = i;
							selectedCategory = categories[i];
							selectedClass = null;
						}
					}
				}

				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.EndVertical ();

			if (defaultClass > AllActions.Count - 1)
			{
				defaultClass = AllActions.Count - 1;
			}
		}


		private void ShowActionTypesGUI ()
		{
			EditorGUILayout.BeginVertical (CustomStyles.thinBox);
			showActionTypes = CustomGUILayout.ToggleHeader (showActionTypes, "Category: " + selectedCategory);
			if (showActionTypes)
			{
				ActionType[] actionTypes = GetActionTypesInCategory (selectedCategory);

				for (int i=0; i<actionTypes.Length; i++)
				{
					if (actionTypes[i] == null) continue;

					EditorGUILayout.BeginHorizontal ();

					string label = actionTypes[i].title;
					if (!string.IsNullOrEmpty (defaultClassName) && actionTypes[i].fileName == defaultClassName)
					{
						label += " (DEFAULT)";
					}
					else if (!actionTypes[i].isEnabled)
					{
						label += " (DISABLED)";
					}

					if (GUILayout.Toggle (actionTypes[i].IsMatch (selectedClass), label, "Button"))
					{
						selectedClass = actionTypes[i];
					}

					if (GUILayout.Button ("", CustomStyles.IconCog))
					{
						SideMenu (AllActions.IndexOf (actionTypes[i]));
					}
					EditorGUILayout.EndHorizontal ();
				}
			}
			EditorGUILayout.EndVertical ();
		}


		private void ShowActionTypeGUI ()
		{
			if (selectedClass == null) return;

			EditorGUILayout.BeginVertical (CustomStyles.thinBox);
			showActionType = CustomGUILayout.ToggleHeader (showActionType, selectedClass.GetFullTitle ());
			if (showActionType)
			{
				SpeechLine.ShowField ("Name:", selectedClass.GetFullTitle (), false);
				SpeechLine.ShowField ("Filename:", selectedClass.fileName + ".cs", false);
				SpeechLine.ShowField ("Description:", selectedClass.description, true);

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Node colour:", GUILayout.Width (85f));
				selectedClass.color = EditorGUILayout.ColorField (selectedClass.color);
				EditorGUILayout.EndHorizontal ();

				if (!string.IsNullOrEmpty (defaultClassName) && selectedClass.fileName == defaultClassName)
				{
					EditorGUILayout.HelpBox ("This is marked as the default Action", MessageType.Info);
				}
				else
				{
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Is enabled?", GUILayout.Width (85f));
					selectedClass.isEnabled = EditorGUILayout.Toggle (selectedClass.isEnabled);
					EditorGUILayout.EndHorizontal ();
				}
			}
			EditorGUILayout.EndVertical ();
		}


		private void SearchForInstances (bool justLocal, ActionType actionType)
		{
			if (justLocal)
			{
				SearchSceneForType ("", actionType);
				return;
			}
			
			// First look for lines that already have an assigned lineID
			string[] sceneFiles = AdvGame.GetSceneFiles ();
			if (sceneFiles == null || sceneFiles.Length == 0)
			{
				Debug.LogWarning ("Cannot search scenes - no enabled scenes could be found in the Build Settings.");
			}
			else
			{
				foreach (string sceneFile in sceneFiles)
				{
					SearchSceneForType (sceneFile, actionType);
				}
			}

			ActionListAsset[] allActionListAssets = AdvGame.GetReferences ().speechManager.GetAllActionListAssets ();
			foreach (ActionListAsset actionListAsset in allActionListAssets)
			{
				int[] foundIDs = SearchActionsForType (actionListAsset.actions, actionType);
				if (foundIDs != null && foundIDs.Length > 0)
				{
					ACDebug.Log ("(Asset: " + actionListAsset.name + ") Found " + foundIDs.Length + " instances of '" + actionType.GetFullTitle () + "' " + CreateIDReport (foundIDs), actionListAsset);
				}
			}
		}
		
		
		private void SearchSceneForType (string sceneFile, ActionType actionType)
		{
			string sceneLabel = "";
			
			if (sceneFile != "")
			{
				sceneLabel = "(Scene: " + sceneFile + ") ";
				UnityVersionHandler.OpenScene (sceneFile);
			}

			// Speech lines and journal entries
			ActionList[] actionLists = GameObject.FindObjectsOfType (typeof (ActionList)) as ActionList[];
			foreach (ActionList list in actionLists)
			{
				int[] foundIDs = SearchActionsForType (list.GetActions (), actionType);
				if (foundIDs != null && foundIDs.Length > 0)
				{
					ACDebug.Log (sceneLabel + " Found " + foundIDs.Length + " instances in '" + list.gameObject.name + "' " + CreateIDReport (foundIDs), list.gameObject);
				}
			}
		}


		private string CreateIDReport (int[] foundIDs)
		{
			string idLabel = "(IDs ";
			for (int i=0; i<foundIDs.Length; i++)
			{
				idLabel += foundIDs[i];
				if (i < (foundIDs.Length - 1))
				{
					idLabel += ", ";
				}
			}
			idLabel += ")";
			return idLabel;
		}
		
		
		private int[] SearchActionsForType (List<Action> actionList, ActionType actionType)
		{
			List<int> foundIDs = new List<int>();
			if (actionList != null)
			{
				foreach (Action action in actionList)
				{
					if ((action.category == actionType.category && action.title == actionType.title) ||
					    (action.GetType ().ToString () == actionType.fileName) ||
					    (action.GetType ().ToString () == "AC." + actionType.fileName))
					{
						int id = actionList.IndexOf (action);
						foundIDs.Add (id);
					}
				}
			}
			return foundIDs.ToArray ();
		}


		/** The folder path to the default Actions */
		public string FolderPath
		{
			get
			{
				return Resource.MainFolderPathRelativeToAssets + "/Scripts/Actions";
			}
		}


		public bool UsingCustomActionsFolder
		{
			get
			{
				return (customFolderPath != FolderPath);
			}
		}


		public string GetActionTypeLabel (Action _action, bool includeLabel)
		{
			int index = GetActionTypeIndex (_action);
			string suffix = (includeLabel) ? _action.SetLabel () : string.Empty;

			if (index >= 0 && AllActions != null && index < AllActions.Count)
			{
				return AllActions[index].GetFullTitle () + suffix;
			}
			return _action.category + ": " + _action.title + suffix;
		}
		
		#endif


		/**
		 * <summary>Gets the filename of an enabled Action.</summary>
		 * <param name = "i">The index number of the Action, in EnabledActions, to get the filename of</param>
		 * <returns>Gets the filename of the Action</returns>
		 */
		public string GetActionName (int i)
		{
			return (AllActions [i].fileName);
		}


		/**
		 * <summary>Checks if any enabled Actions have a specific filename.</summary>
		 * <param name = "_name">The filename to check for</param>
		 * <returns>True if any enabled Actions have the supplied filename</returns>
		 */
		public bool DoesActionExist (string _name)
		{
			foreach (ActionType actionType in AllActions)
			{
				if (_name == actionType.fileName || _name == ("AC." + actionType.fileName))
				{
					return true;
				}
			}
			return false;
		}
		

		/**
		 * <summary>Gets the number of enabled Actions.</summary>
		 * <returns>The number of enabled Actions</returns>
		 */
		public int GetActionsSize ()
		{
			return (AllActions.Count);
		}


		/**
		 * <summary>Gets all Action titles within EnabledActions.</summary>
		 * <returns>A string array of all Action titles within EnabledActions</returns>
		 */
		public string[] GetActionTitles ()
		{
			List<string> titles = new List<string>();
			
			foreach (ActionType type in AllActions)
			{
				titles.Add (type.title);
			}
			
			return (titles.ToArray ());
		}


		/**
		 * <summary>Gets the index number of an Action within EnabledActions.</summary>
		 * <param name = "_action">The Action to search for</param>
		 * <returns>The index number of the Action within EnabledActions</returns>
		 */
		public int GetActionTypeIndex (Action _action)
		{
			string className = _action.GetType ().ToString ();
			className = className.Replace ("AC.", "");
			foreach (ActionType actionType in AllActions)
			{
				if (actionType.fileName == className)
				{
					return AllActions.IndexOf (actionType);
				}
			}
			return defaultClass;
		}


		/**
		 * <summary>Gets the index number of an Action within EnabledActions.</summary>
		 * <param name = "_category">The category of the Action to search for</param>
		 * <param name = "subCategoryIndex">The index number of the Action in a list of all Actions that share its category</param>
		 * <returns>The index number of the Action within EnabledActions</returns>
		 */
		public int GetEnabledActionTypeIndex (ActionCategory _category, int subCategoryIndex)
		{
			List<ActionType> types = new List<ActionType>();
			foreach (ActionType type in AllActions)
			{
				if (type.category == _category && type.isEnabled)
				{
					types.Add (type);
				}
			}
			if (subCategoryIndex < types.Count)
			{
				return AllActions.IndexOf (types[subCategoryIndex]);
			}
			return 0;
		}


		/**
		 * <summary>Gets all found Action titles within a given ActionCategory.</summary>
		 * <param name = "_category">The category of the Actions to get the titles of.</param>
		 * <returns>A string array of all Action titles within the ActionCategory</returns>
		 */
		public string[] GetActionSubCategories (ActionCategory _category)
		{
			List<string> titles = new List<string>();

			foreach (ActionType type in AllActions)
			{
				if (type.category == _category)
				{
					if (type.isEnabled)
					{
						titles.Add (type.title);
					}
				}
			}
			
			return (titles.ToArray ());
		}
		

		/**
		 * <summary>Gets the ActionCategory of an Action within EnabledActions.</summary>
		 * <param name = "number">The index number of the Action's place in EnabledActions</param>
		 * <returns>The ActionCategory of the Action</returns>
		 */
		public ActionCategory GetActionCategory (int number)
		{
			if (AllActions == null || AllActions.Count == 0 || AllActions.Count < number)
			{
				return 0;
			}
			return AllActions[number].category;
		}
		

		/**
		 * <summary>Gets the index of an Action within a list of all Actions that share its category.</summary>
		 * <param name = "_action">The Action to get the index of</param>
		 * <returns>The index of the Action within a list of all Actions that share its category</returns>
		 */
		public int GetActionSubCategory (Action _action)
		{
			string fileName = _action.GetType ().ToString ().Replace ("AC.", "");
			ActionCategory _category = _action.category;
			
			// Learn category
			foreach (ActionType type in AllActions)
			{
				if (type.fileName == fileName)
				{
					_category = type.category;
				}
			}
			
			// Learn subcategory
			int i=0;
			foreach (ActionType type in AllActions)
			{
				if (type.category == _category)
				{
					if (type.fileName == fileName)
					{
						return i;
					}
					i++;
				}
			}
			
			ACDebug.LogWarning ("Error building Action " + _action);
			return 0;
		}


		/**
		 * <summary>Gets all found ActionType classes that belong in a given category</summary>
		 * <param name = "category">The category of ActionType classes to collect</param>
		 * <retuns>An array of all ActionType classes that belong in the given category</returns>
		 */
		public ActionType[] GetActionTypesInCategory (ActionCategory category)
		{
			List<ActionType> types = new List<ActionType>();
			foreach (ActionType type in AllActions)
			{
				if (type.category == category)
				{
					types.Add (type);
				}
			}
			return types.ToArray ();
		}


		public bool IsActionTypeEnabled (int index)
		{
			if (AllActions != null && index < AllActions.Count)
			{
				return AllActions[index].isEnabled;
			}
			return false;
		}


		public Color GetActionTypeColor (Action _action)
		{
			int index = GetActionTypeIndex (_action);

			if (index >= 0 && AllActions != null && index < AllActions.Count)
			{
				return GUI.color = AllActions[index].color;
			}
			return Color.white;
		}

	}
	
}