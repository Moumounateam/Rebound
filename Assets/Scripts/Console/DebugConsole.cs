using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class DebugController : MonoBehaviour
{
	bool showConsole;
	public void OnToggleDebug(InputValue value)
	{
		showConsole = !showConsole;
	}

	private void OnGUI()
	{
		if (!showConsole) 
			return ;
		float y = 10;

		GUI.Box(new Rect(0, y, Screen.width, 30), "");
	}
}