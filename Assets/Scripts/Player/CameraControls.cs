using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float maxPitch;                      //The maximum pitch to go down
    public float minPitch;                      //The minimum pitch to go up
    public float cameraSpeed;                   //The speed of the camera movement
    public bool lockCursor = true;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float isPausedSpeed;
    private bool m_cursorIsLocked = true;

    public GameObject FirstPersonCamera;
    static float sightDistance;                         //Raycast length
    public static bool isHolding;                       //Checks if the player is already holding an object
    public static Transform playerHoldingPosition;      //Transform where the object will be held
    //public static Transform playerBodyPosition;      //Transform where the object will be held
    public static RaycastHit hit;

    public float timeBetweenClicks;
    private float timestamp;

    void Start()
    {
        playerHoldingPosition = GameObject.Find("Holding Pos").transform;
        //playerBodyPosition = GameObject.Find("Player").transform;
        FirstPersonCamera = GameObject.Find("Main Camera").gameObject;
        sightDistance = 4.5f;
        timestamp = 0.0f;
        isHolding = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerSees();
    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            //Force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
        {
            InternalLockUpdate();
        }
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0)) //&& !PauseMenuScript.gameIsPaused) READD WHEN PAUSE MENU IS IMPLMENTED
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void PlayerSees()
    {

        // USE WHEN PAUSE MENU IMPLEMENTED
        /*if (PauseMenuScript.gameIsPaused)
        {
            isPausedSpeed = 0.0f;
        }
        else
        {
            isPausedSpeed = 1.0f;
        }*/

        isPausedSpeed = 1.0f; // REMOVE WHEN PAUSE MENU IMPLEMENTED 

        yaw += cameraSpeed * isPausedSpeed * Input.GetAxis("Mouse X");                                  //Moves from left and right
        pitch -= cameraSpeed * isPausedSpeed * Input.GetAxis("Mouse Y");                                //Moves from up and down
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);                                                 //Gives limits/parameters to prevent revolving 

        playerHoldingPosition.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);                    //Turns the object in hand from up, down, left and right
        //playerBodyPosition.transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);                       //Turns the player's body in hand from up, down, left and right

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);                                          //Turns the transform of where this script is located from up, down, left, and right

        UpdateCursorLock();

        Vector3 forward = FirstPersonCamera.transform.TransformDirection(Vector3.forward);                  //Uses the FirstPersonCamera as the Ray
        Debug.DrawRay(FirstPersonCamera.transform.position, forward * sightDistance, Color.yellow);         //Debug draws the raycast lines

        if (Physics.Raycast(FirstPersonCamera.transform.position, forward, out hit, sightDistance))
        {
            float rayDistance = hit.distance;
            if (rayDistance <= sightDistance)
            {
                if (hit.transform.tag != "Player")
                {
                    //Pick Up physics Object
                    if (hit.transform.gameObject.GetComponent<Rigidbody>() != null && hit.transform.gameObject.layer == 8)
                    {
                        //Physically Carry the Object
                        if (!isHolding)
                        {
                            //If wanting to pick up & move object
                            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= timestamp)
                            {
                                CarryObject(hit.transform.gameObject);
                                timestamp = Time.time + timeBetweenClicks;
                            }
                        }
                        //Physically Drop the Object
                        else if (isHolding && (playerHoldingPosition.childCount > 0))
                        {
                            //if wanting to release object
                            if (Input.GetKeyUp(KeyCode.Mouse0) && Time.time >= timestamp)
                            {
                                DropObject(hit.transform.gameObject);
                                timestamp = Time.time + timeBetweenClicks;
                            }

                            if (Input.GetKey(KeyCode.Mouse1) && Time.time >= timestamp)
                            {

                                ThrowObject(hit.transform.gameObject, hit);
                                timestamp = Time.time + timeBetweenClicks;
                            }
                        }
                    }
                    // Interact with a button
                    if (hit.collider.tag.Equals("Button"))
                    {
                        if (Input.GetKey(KeyCode.E) && Time.time >= timestamp)
                        {
                            if (hit.transform.gameObject.GetComponent<ButtonScript>() != null)
                            {
                                if (!hit.transform.gameObject.GetComponent<ButtonScript>().clicked)
                                {
                                    hit.transform.gameObject.GetComponent<ButtonScript>().clicked = true;
                                }
                                else if (hit.transform.gameObject.GetComponent<ButtonScript>().clicked)
                                {
                                    hit.transform.gameObject.GetComponent<ButtonScript>().clicked = false;
                                }
                            }
                            timestamp = Time.time + timeBetweenClicks;
                        }
                    }
                    // Interact with a Door
                    if (hit.collider.tag.Equals("Door"))
                    {
                        if (Input.GetKey(KeyCode.E) && Time.time >= timestamp)
                        {
                            //print("I pressed door named " + hit.transform.gameObject.name);
                            if (hit.transform.gameObject.GetComponent<DoorScript>() != null)
                            {
                                if (!hit.transform.gameObject.GetComponent<DoorScript>().opened)
                                {
                                    //print("I opened the door named " + hit.transform.gameObject.name);
                                    hit.transform.gameObject.GetComponent<DoorScript>().opened = true;
                                    //print("opened = " + hit.transform.gameObject.GetComponent<DoorScript>().opened);
                                }
                                else if (hit.transform.gameObject.GetComponent<DoorScript>().opened)
                                {
                                    //print("I closed the door named " + hit.transform.gameObject.name);
                                    hit.transform.gameObject.GetComponent<DoorScript>().opened = false;
                                    //print("opened = " + hit.transform.gameObject.GetComponent<DoorScript>().opened);
                                }
                            }
                            timestamp = Time.time + timeBetweenClicks;
                        }
                    }
                }
            }
        }
    }

    //==========================
    //CARRY OBJECT
    //==========================
    public void CarryObject(GameObject hitObject)
    {     
        if (hitObject.transform.parent != null && !(hitObject.transform.parent.name.Equals(playerHoldingPosition.name)) && !(hitObject.transform.parent.gameObject.layer == 8))
        {
            hitObject = hitObject.transform.parent.gameObject;
        }

        hitObject.GetComponent<Rigidbody>().isKinematic = true;
        hitObject.GetComponent<Rigidbody>().useGravity = false;
        hitObject.transform.position = playerHoldingPosition.position;
        hitObject.transform.parent = playerHoldingPosition.transform;
        sightDistance = 3.5f;
        isHolding = true;
    }

    //==========================
    //DROP OBJECT
    //==========================
    public static void DropObject(GameObject hitObject)
    {
        if (hitObject.transform.parent != null && !(hitObject.transform.parent.name.Equals(playerHoldingPosition.name)) && !(hitObject.transform.parent.gameObject.layer == 8))
        {
            hitObject = hitObject.transform.parent.gameObject;
        }

        hitObject.GetComponent<Rigidbody>().isKinematic = false;
        hitObject.GetComponent<Rigidbody>().useGravity = true;
        hitObject.transform.parent = null;
        sightDistance = 7.0f;
        isHolding = false;
    }

    //==========================
    //THROW OBJECT
    //==========================
    public void ThrowObject(GameObject hitObject, RaycastHit hit)
    {
        if (hitObject.transform.parent != null && !(hitObject.transform.parent.name.Equals(playerHoldingPosition.name)) && !(hitObject.transform.parent.gameObject.layer == 8))
        {
            hitObject = hitObject.transform.parent.gameObject;
        }

        hitObject.GetComponent<Rigidbody>().isKinematic = false;
        hitObject.GetComponent<Rigidbody>().useGravity = true;
        hitObject.transform.parent = null;
        hit.rigidbody.AddForce(-hit.normal * 600);
        isHolding = false;
    }
}


