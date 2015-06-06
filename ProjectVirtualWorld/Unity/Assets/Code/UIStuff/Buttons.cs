using UnityEngine;
using System.Collections;

public class Buttons : MonoBehaviour
{
	public enum ButtonFunction {Reset, Exit};
	public ButtonFunction buttonFunction = ButtonFunction.Reset;
	
	public void Press ()
	{
		switch (buttonFunction)
		{
			case ButtonFunction.Reset:
				Application.LoadLevel("Main2");
				break;
			
			case ButtonFunction.Exit:
				Application.Quit();
				break;
		}
	}
}
