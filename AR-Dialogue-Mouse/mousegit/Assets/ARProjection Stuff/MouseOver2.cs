using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseOver2 : MonoBehaviour
{
	private Color hoverColor = Color.yellow;
	private Color lockedColor = Color.magenta;
	private Renderer renderer;
	private float timetolock = 5f;
	bool over = false;
	float timeLeft;

	void Start()
	{
		renderer = GetComponent<Renderer>();
	}

	void OnMouseEnter()
	{
		renderer.material.color = hoverColor;
		timeLeft = timetolock;
		over = true;

	}

	void OnMouseExit()
	{
		over = false;
		renderer.material.color = Color.blue;
	}
	
	void Update()
    {
		if (over)
        {
			timeLeft -= Time.deltaTime;
        }
		if (timeLeft <= 0)
        {
			renderer.material.color = lockedColor;

		}
    }

}
