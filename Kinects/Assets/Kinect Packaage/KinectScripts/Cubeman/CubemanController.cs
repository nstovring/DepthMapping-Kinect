using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class CubemanController : NetworkBehaviour 
{
	public bool MoveVertically = false;
	public bool MirroredMovement = false;

    public Vector3 angleTest;

    //public GameObject debugText;

    public GameObject Hip_Center;
	public GameObject Spine;
	public GameObject Shoulder_Center;
	public GameObject Head;
	public GameObject Shoulder_Left;
	public GameObject Elbow_Left;
	public GameObject Wrist_Left;
	public GameObject Hand_Left;
	public GameObject Shoulder_Right;
	public GameObject Elbow_Right;
	public GameObject Wrist_Right;
	public GameObject Hand_Right;
	public GameObject Hip_Left;
	public GameObject Knee_Left;
	public GameObject Ankle_Left;
	public GameObject Foot_Left;
	public GameObject Hip_Right;
	public GameObject Knee_Right;
	public GameObject Ankle_Right;
	public GameObject Foot_Right;

	public LineRenderer SkeletonLine;

    public GameObject cubeRepresent;
	private GameObject[] bones; 
	private LineRenderer[] lines;
	private int[] parIdxs;

    [SyncVar] public Vector3 offset;
    [SyncVar] public float angleOffset;


    [SyncVar] public float player1AngleFromKinect;
    [SyncVar] public float angleFromKinect;
    [SyncVar] public float angleBetweenCameras;

    //public 
    private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Vector3 initialPosOffset = Vector3.zero;
	private uint initialPosUserID = 0;

    private KinectManager manager;

    void Start () 
	{
		//store bones in a list for easier access
		bones = new GameObject[] {
			Hip_Center, Spine, Shoulder_Center, Head,  // 0 - 3
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,  // 4 - 7
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,  // 8 - 11
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,  // 12 - 15
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right  // 16 - 19
		};

		parIdxs = new int[] {
			0, 0, 1, 2,
			2, 4, 5, 6,
			2, 8, 9, 10,
			0, 12, 13, 14,
			0, 16, 17, 18
		};
		
		// array holding the skeleton lines
		lines = new LineRenderer[bones.Length];
		
		if(SkeletonLine)
		{
			for(int i = 0; i < lines.Length; i++)
			{
				lines[i] = Instantiate(SkeletonLine) as LineRenderer;
				lines[i].transform.parent = transform;
			}
		}
		
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		//transform.rotation = Quaternion.identity;
	}

    private bool isCalibrated;
	// Update is called once per frame
    [Client]
	void Update () 
	{
        if (isLocalPlayer)
        {
            if (manager == null)
            {
                manager = KinectManager.Instance;
                ///manager.StartKinect();
            }
            else
            {
                GetAngleBetweenCameras();
                GetAngleFromKinect();
                angleOffset = Mathf.Abs(angleBetweenCameras) + Mathf.Abs(angleFromKinect) +
                              Mathf.Abs(player1AngleFromKinect);
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log(angleFromKinect + " Angle from kinect");
                    Debug.Log(angleBetweenCameras + " Angle Between Cameras");
                    Calibrate();
                    isCalibrated = true;
                }
                if (isCalibrated)
                {
                    Debug.Log("Current Pos " + transform.position);
                    isCalibrated = false;
                }

                if(manager)
                //MoveSkeleton();
                TestRotOffset();
            }
        }
	}

    void TestRotOffset()
    {
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;
        Vector3 posPointMan = manager.GetUserPosition(playerID);
        float yRotation = 90;
        posPointMan.z = !MirroredMovement ? -posPointMan.z : posPointMan.z;
        //Maybe remove this
        posPointMan.x *= 1;

        Vector3 directionVector3 = Quaternion.AngleAxis(yRotation, Vector3.up) * posPointMan;

        cubeRepresent.transform.position = directionVector3;

        //manager.kinectToWorld.MultiplyVector(directionVector3);
        //cubeRepresent.transform.eulerAngles = directionVector3;
        //Apply direction to movement of cube
    }

    private void Calibrate()
    {
        //Debug.Log("Last Pos " + transform.position);
        //Debug.Log("Offset "+ offset);
        //offset.y *= -1;
        Quaternion newAngleQuaternion = Quaternion.Euler(-manager.SensorAngle, angleTest.y, 0);
        manager.kinectToWorld.SetTRS(new Vector3(offset.x,offset.y + 1, offset.z), Quaternion.identity, Vector3.one);
        //manager.kinectToWorld.SetTRS(new Vector3(0f,0f,0f), Quaternion.identity, Vector3.one);
        //MoveSkeleton();
    }

    [Client]
    public void GetAngleFromKinect()
    {
        Vector3 targetDir = transform.position - Vector3.zero;
        Vector3 forward = Vector3.left;

        angleFromKinect = Vector3.Angle(targetDir, forward) - 90;

    }

    private void GetAngleBetweenCameras()
    {
        Vector3 positionVector3 = transform.position;
        Vector3 v0Vector3 = Vector3.zero;
        Vector3 vOffsetVector3 = offset;
        Vector3 r1Vector3 = positionVector3 - vOffsetVector3;
        Vector3 r2Vector3 = positionVector3 - v0Vector3;

        angleBetweenCameras = Vector3.Angle(r1Vector3, r2Vector3);
    }

    [Client]
    void MoveSkeleton() {
        // get 1st player
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;

        initialPosition = MatrixFunk.ExtractTranslationFromMatrix(ref manager.kinectToWorld);
        initialRotation = MatrixFunk.ExtractRotationFromMatrix(ref manager.kinectToWorld);

        if (playerID <= 0)
        {
            // reset the pointman position and rotation
            if (transform.position != initialPosition)
            {
                transform.position = initialPosition;
            }

            if (transform.rotation != initialRotation)
            {
                transform.rotation = initialRotation;
            }

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].gameObject.SetActive(true);

                bones[i].transform.localPosition = Vector3.zero;
                bones[i].transform.localRotation = Quaternion.identity;

                if (SkeletonLine)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }
            return;
        }

        // set the user position in space
        Vector3 posPointMan = manager.GetUserPosition(playerID);
        posPointMan.z = !MirroredMovement ? -posPointMan.z : posPointMan.z;
        //Maybe remove this
        posPointMan.x *= 1;

        transform.position = initialPosOffset + (MoveVertically ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));

        // update the local positions of the bones
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                int joint = MirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(i) : i;

                if (manager.IsJointTracked(playerID, joint))
                {
                    bones[i].gameObject.SetActive(true);

                    Vector3 posJoint = manager.GetJointPosition(playerID, joint);
                    posJoint.z = !MirroredMovement ? -posJoint.z : posJoint.z;

                    Quaternion rotJoint = manager.GetJointOrientation(playerID, joint, !MirroredMovement);
                    rotJoint = initialRotation * rotJoint;

                    posJoint -= posPointMan;

                    if (MirroredMovement)
                    {
                        posJoint.x = -posJoint.x;
                        posJoint.z = -posJoint.z;
                    }

                    bones[i].transform.localPosition = posJoint;
                    bones[i].transform.rotation = rotJoint;
                }
                else
                {
                    bones[i].gameObject.SetActive(false);
                }
            }
        }

        if (SkeletonLine)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                bool bLineDrawn = false;

                if (bones[i] != null)
                {
                    if (bones[i].gameObject.activeSelf)
                    {
                        Vector3 posJoint = bones[i].transform.position;

                        int parI = parIdxs[i];
                        Vector3 posParent = bones[parI].transform.position;

                        if (bones[parI].gameObject.activeSelf)
                        {
                            lines[i].gameObject.SetActive(true);

                            //lines[i].SetVertexCount(2);
                            lines[i].SetPosition(0, posParent);
                            lines[i].SetPosition(1, posJoint);

                            bLineDrawn = true;
                        }
                    }
                }

                if (!bLineDrawn)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
