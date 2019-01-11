using UnityEngine;
using UnityEditor;

namespace AC
{

	[CustomEditor (typeof (Player))]
	public class PlayerEditor : CharEditor
	{

		public override void OnInspectorGUI ()
		{
			Player _target = (Player) target;
			
			SharedGUIOne (_target);
			SharedGUITwo (_target);

			SettingsManager settingsManager = AdvGame.GetReferences ().settingsManager;
			if (settingsManager && (settingsManager.hotspotDetection == HotspotDetection.PlayerVicinity || settingsManager.playerSwitching == PlayerSwitching.Allow))
			{
				EditorGUILayout.BeginVertical ("Button");
				EditorGUILayout.LabelField ("Player settings", EditorStyles.boldLabel);

				if (settingsManager.hotspotDetection == HotspotDetection.PlayerVicinity)
				{
					_target.hotspotDetector = (DetectHotspots) CustomGUILayout.ObjectField <DetectHotspots> ("Hotspot detector child:", _target.hotspotDetector, true, "", "The DetectHotspots component to rely on for hotspot detection. This should be a child object of the Player.");
				}

				if (settingsManager.playerSwitching == PlayerSwitching.Allow)
				{
					_target.associatedNPCPrefab = (NPC) CustomGUILayout.ObjectField <NPC> ("Associated NPC prefab:", _target.associatedNPCPrefab, false, "", "The NPC counterpart of the Player, used as a stand-in when switching the active Player prefab");
				}

				EditorGUILayout.EndVertical ();
			}
			
			UnityVersionHandler.CustomSetDirty (_target);
		}

	}

}