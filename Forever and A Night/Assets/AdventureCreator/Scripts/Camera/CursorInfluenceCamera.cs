using UnityEngine;

namespace AC
{

	/** A subclass of _Camera that allows for the cursor position to tweak the camera's rotation */
	public abstract class CursorInfluenceCamera : _Camera
	{

		/** If True, then the camera will rotate towards the cursor's position on-screen */
		public bool followCursor = false;
		/** The influence that the cursor's position has on rotation, if followCursor = True */
		public Vector2 cursorInfluence = new Vector2 (0.3f, 0.1f);
		/** If True, and followCursor = True, then camera rotation according to the cursor's X position will be limited */
		public bool constrainCursorInfluenceX = false;
		/** The lower and upper limits, if constrainCursorInfluenceX = True */
		public Vector2 limitCursorInfluenceX;
		/** If True, and followCursor = True, then camera rotation according to the cursor's Y position will be limited */
		public bool constrainCursorInfluenceY = false;
		/** The lower and upper limits, if constrainCursorInfluenceY = True */
		public Vector2 limitCursorInfluenceY;

		private Vector2 actualCursorOffset;


		public override Vector2 CreateRotationOffset ()
		{
			if (followCursor)
			{
				if (KickStarter.stateHandler.IsInGameplay ())
				{
					Vector2 mousePosition = KickStarter.playerInput.GetMousePosition ();
					Vector2 mouseOffset = new Vector2 (mousePosition.x / (Screen.width / 2) - 1, mousePosition.y / (Screen.height / 2) - 1);
					float distFromCentre = mouseOffset.sqrMagnitude;

					if (distFromCentre < 1.96f)
					{
						if (constrainCursorInfluenceX)
						{
							mouseOffset.x = Mathf.Clamp (mouseOffset.x, limitCursorInfluenceX[0], limitCursorInfluenceX[1]);
						}
						if (constrainCursorInfluenceY)
						{
							mouseOffset.y = Mathf.Clamp (mouseOffset.y, limitCursorInfluenceY[0], limitCursorInfluenceY[1]);
						}
					}

					Vector2 targetCursorOffset = new Vector2 (mouseOffset.x * cursorInfluence.x, mouseOffset.y * cursorInfluence.y);
					actualCursorOffset = Vector2.Lerp (actualCursorOffset, targetCursorOffset, Time.deltaTime * 3f);
				}

				return actualCursorOffset;
			}
			return Vector2.zero;
		}

	}

}