using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public enum AxisOption
	{
		// Options for which axes to use
		Both, // Use both
		OnlyHorizontal, // Only horizontal
		OnlyVertical // Only vertical
	}

	public int MovementRange = 100;
	public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
	public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
	public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

	public bool controlsCamera;
	public Camera camera;
	public GameObject player;
	private bool shouldRotateCamera;
	private Vector2 rotateVector;
	private float rotationSpeed;

	Vector3 m_StartPos;
	bool m_UseX; // Toggle for using the x axis
	bool m_UseY; // Toggle for using the Y axis

	void OnEnable()
	{
		
	}

	void Start()
	{
		m_StartPos = transform.position;
		shouldRotateCamera = false;

		m_UseX = true;
		m_UseY = true;

		rotationSpeed = 2.5f;
	}

	void Update(){
		if(shouldRotateCamera){
			rotateCamera();
		}
	}


	public void OnDrag(PointerEventData data)
	{
		Vector3 newPos = Vector3.zero;

		if (m_UseX)
		{
			int delta = (int)(data.position.x - m_StartPos.x);
			delta = Mathf.Clamp(delta, - MovementRange, MovementRange);
			newPos.x = delta;
		}

		if (m_UseY)
		{
			int delta = (int)(data.position.y - m_StartPos.y);
			delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
			newPos.y = delta;
		}

		transform.position = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + m_StartPos;		// move Joystick

		shouldRotateCamera = true;
		rotateVector = new Vector2(newPos.x, newPos.y);
		rotateVector.Normalize();

	}

	private void rotateCamera(){
		/*
		player.transform.Rotate(player.transform.up, rotateVector.x * -rotationSpeed);	// rotate camera along x axis
		player.transform.Rotate(player.transform.right, rotateVector.y * -rotationSpeed);	// rotate camera along y axis
		*/
		player.transform.Rotate(rotateVector.y * -rotationSpeed, rotateVector.x * rotationSpeed, 0, Space.Self);
		float z = player.transform.eulerAngles.z;
		player.transform.Rotate(0, 0, -z);
	}


	public void OnPointerUp(PointerEventData data)
	{
		transform.position = m_StartPos;
		shouldRotateCamera = false;
	}


	public void OnPointerDown(PointerEventData data) { }

	void OnDisable()
	{
		// remove the joysticks from the cross platform input

	}
}

