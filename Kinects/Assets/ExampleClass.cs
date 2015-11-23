// Translate, rotate and scale a mesh. Try varying
// the parameters in the inspector while running
// to see the effect they have.

using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public Vector3 translation;
    public Vector3 eulerAngles;
    public Vector3 scale = new Vector3(1, 1, 1);
    private MeshFilter mf;
    private Vector3[] origVerts;
    private Vector3[] newVerts;
    public Matrix4x4 m = Matrix4x4.identity;
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        origVerts = mf.mesh.vertices;
        newVerts = new Vector3[origVerts.Length];
    }

    void Update()
    {
        Quaternion rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        m.SetTRS(translation, rotation, scale);

        Debug.Log(MatrixFunk.ExtractRotationFromMatrix(ref m).eulerAngles);
        int i = 0;
        while (i < origVerts.Length)
        {
            newVerts[i] = m.MultiplyPoint3x4(origVerts[i]);
            i++;
        }
        mf.mesh.vertices = newVerts;
    }
}
