using UnityEngine;
using System.Collections;

public class ButtonRaycast : MonoBehaviour
{
	public float timer = 0;
	
	// Update is called once per frame
	void Update ()
	{
		RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit))
		{
			if (hit.transform.tag == "Button")
			{
				if (timer >= 1)
					hit.transform.gameObject.GetComponent<Buttons>().Press();
				else
					timer += Time.deltaTime;
			}
			else
				timer = 0;
		}
		else
			timer = 0;
	}
}
