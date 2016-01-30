using UnityEngine;
using System.Collections;

public class CameraFlip : MonoBehaviour {

	//public Vector3 basicEuler;
	public bool inverse;
	public Vector3 scale = new Vector3 (1, -1, 1);

	// void Update()
	// {
	// 	Quaternion rotation = Quaternion.Euler (basicEuler);
	// 
	// 	if (inverse)
	// 		rotation *= Quaternion.Euler (0, 0, 180);
	// 
	// 	transform.rotation = rotation;
	// }


	void OnPreCull()
	{		
		Camera.main.ResetWorldToCameraMatrix ();
		Camera.main.ResetProjectionMatrix ();
		if(inverse)
			Camera.main.projectionMatrix = Camera.main.projectionMatrix * Matrix4x4.Scale (scale);		
	}

	void OnPreRender () {
		if (inverse) {
			GL.SetRevertBackfacing (true);
		}
	}

	void OnPostRender () {
		if (inverse) {
			GL.SetRevertBackfacing (false);
		}
	}
}
