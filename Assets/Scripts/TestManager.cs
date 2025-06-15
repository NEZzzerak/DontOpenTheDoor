using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			CutsceneManager.Instance.StartCutscene("First");
		}
	}
}
