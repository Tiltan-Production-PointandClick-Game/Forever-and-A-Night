using UnityEngine;
using UnityEditor;

namespace AC
{

	[CustomEditor(typeof(GameCameraAnimated))]
	public class GameCameraAnimatedEditor : Editor
	{
		
		public override void OnInspectorGUI()
		{
			GameCameraAnimated _target = (GameCameraAnimated) target;

			if (_target.GetComponent <Animation>() == null)
			{
				EditorGUILayout.HelpBox ("This camera type requires an Animation component.", MessageType.Warning);
			}

			EditorGUILayout.BeginVertical ("Button");
			_target.animatedCameraType = (AnimatedCameraType) CustomGUILayout.EnumPopup ("Animated camera type:", _target.animatedCameraType, "", "The way in which animations are played");
			_target.clip = (AnimationClip) CustomGUILayout.ObjectField <AnimationClip> ("Animation clip:", _target.clip, false, "", "The animation to play when this camera is made active");

			if (_target.animatedCameraType == AnimatedCameraType.PlayWhenActive)
			{
				_target.loopClip = CustomGUILayout.Toggle ("Loop animation?", _target.loopClip, "", "If True, then the animation will loop");
				_target.playOnStart = CustomGUILayout.Toggle ("Play on start?", _target.playOnStart, "", "If True, then the animation will play when the scene begins, rather than waiting for it to become active");
			}
			else if (_target.animatedCameraType == AnimatedCameraType.SyncWithTargetMovement)
			{
				_target.pathToFollow = (Paths) CustomGUILayout.ObjectField <Paths> ("Path to follow:", _target.pathToFollow, true, "", "The Paths object to sync with animation");
				_target.targetIsPlayer = CustomGUILayout.Toggle ("Target is Player?", _target.targetIsPlayer, "", "If True, the camera will follow the active Player");
				
				if (!_target.targetIsPlayer)
				{
					_target.target = (Transform) CustomGUILayout.ObjectField <Transform> ("Target:", _target.target, true, "", "The object for the camera to follow");
				}
			}
			EditorGUILayout.EndVertical ();

			if (_target.animatedCameraType == AnimatedCameraType.SyncWithTargetMovement)
			{
				EditorGUILayout.Space ();
				EditorGUILayout.BeginVertical ("Button");
				EditorGUILayout.LabelField ("Cursor influence", EditorStyles.boldLabel);
				_target.followCursor = CustomGUILayout.Toggle ("Follow cursor?", _target.followCursor, "", "If True, then the camera will rotate towards the cursor's position on-screen");
				if (_target.followCursor)
				{
					_target.cursorInfluence = CustomGUILayout.Vector2Field ("Panning factor", _target.cursorInfluence, "", "The influence that the cursor's position has on rotation");
					_target.constrainCursorInfluenceX = CustomGUILayout.ToggleLeft ("Constrain panning in X direction?", _target.constrainCursorInfluenceX, "", "If True, then camera rotation according to the cursor's X position will be limited");
					if (_target.constrainCursorInfluenceX)
					{
						_target.limitCursorInfluenceX[0] = CustomGUILayout.Slider ("Minimum X:", _target.limitCursorInfluenceX[0], -1.4f, 0f, "", "The lower cursor-panning limit");
						_target.limitCursorInfluenceX[1] = CustomGUILayout.Slider ("Maximum X:", _target.limitCursorInfluenceX[1], 0f, 1.4f, "", "The upper cursor-panning limit");
					}
					_target.constrainCursorInfluenceY = CustomGUILayout.ToggleLeft ("Constrain panning in Y direction?", _target.constrainCursorInfluenceY, "", "If True, then camera rotation according to the cursor's Y position will be limited");
					if (_target.constrainCursorInfluenceY)
					{
						_target.limitCursorInfluenceY[0] = CustomGUILayout.Slider ("Minimum Y:", _target.limitCursorInfluenceY[0], -1.4f, 0f, "", "The lower cursor-panning limit");
						_target.limitCursorInfluenceY[1] = CustomGUILayout.Slider ("Maximum Y:", _target.limitCursorInfluenceY[1], 0f, 1.4f, "", "The upper cursor-panning limit");
					}

					if (Application.isPlaying && KickStarter.mainCamera != null && KickStarter.mainCamera.attachedCamera == _target)
					{
						EditorGUILayout.HelpBox ("Changes made to this panel will not be felt until the MainCamera switches to this camera again.", MessageType.Info);
					}
				}
				EditorGUILayout.EndVertical ();
			}

			UnityVersionHandler.CustomSetDirty (_target);
		}
	}

}