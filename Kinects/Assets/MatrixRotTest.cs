using UnityEngine;
using System.Collections;

public class MatrixRotTest : MonoBehaviour
{


    public Matrix4x4 TransformMatrix4X4; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{

	    transform.rotation = MatrixFunk.ExtractRotationFromMatrix(ref TransformMatrix4X4);

        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(0,90,0);
        TransformMatrix4X4.SetTRS(Vector3.zero, rotation, Vector3.one);



	
	}
}
