using UnityEngine;
using System.Collections;

public class testdraw : MonoBehaviour {


	public Vector3 a;
	public Vector3 b;

	// Use this for initialization
	void Start () {
		Color randomCol = new Color (Random.Range (0.00f, 1.00f),Random.Range (0.00f, 1.00f),Random.Range (0.00f, 1.00f));


		/*
		Vector3 v = b - a;

		Vector3 c = new Vector3 (-v.y, 0, v.x);
		c = c / Mathf.Sqrt (Mathf.Pow (v.x, 2) + Mathf.Pow (v.y, 2)) * 1;

		Vector3 d = new Vector3 (-v.y, 0, v.x);
		d = d / Mathf.Sqrt (Mathf.Pow (v.x, 2) + Mathf.Pow (v.y, 2)) * -1;
		*/

			//(-v.y, v.x) / Sqrt(v.x^2 + v.y^2) * h;

		//float angle = Vector3.Angle (a, b);
		//angle -= 90;

		//var rot = Quaternion.AngleAxis(angle,Vector3.up);
		// that's a local direction vector that points in forward direction but also 45 upwards.
		//var lDirection = rot * Vector3.forward;

		//Quaternion rot = Quaternion.Euler (0, angle, 0);


		//Vector3 d = rot * b;

		Vector3 newVec = a - b;
		Vector3 newVector = Vector3.Cross (newVec, Vector3.down);
		newVector.Normalize ();

		Vector3 c = 1 * newVector + a;
		Vector3 d = -1 * newVector + a;

		Vector3 e = 1 * newVector + b;
		Vector3 f = -1 * newVector + b;




		Debug.DrawLine(a, b, randomCol, 2000, false);
		 randomCol = new Color (Random.Range (0.00f, 1.00f),Random.Range (0.00f, 1.00f),Random.Range (0.00f, 1.00f));
		//Debug.DrawLine(b, d, randomCol, 2000, false);
		//Debug.DrawLine(a, c, randomCol, 2000, false);
		Debug.DrawLine(c, d, randomCol, 2000, false);
		Debug.DrawLine(e, f, randomCol, 2000, false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
