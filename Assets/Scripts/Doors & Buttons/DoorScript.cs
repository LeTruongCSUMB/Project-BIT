using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool opened;
    public bool automatic;
    public bool isLocked;

    public GameObject door;

    public List<float> doorCoordinates;

    // Single Door
    private float doorOneOpenYRot;
    private float doorOneOpenXPos;
    private float doorOneOpenYPos;
    private float doorOneOpenZPos;

    private float doorOneCloseYRot;
    private float doorOneCloseXPos;
    private float doorOneCloseYPos;
    private float doorOneCloseZPos;

    private float waitTime;
    private float waitTimeRef;

    private float l;
    private bool isOpened;

    // Start is called before the first frame update
    void Start()
    {
        opened = false;

        if(door == null)
        {
            door = transform.gameObject;
        }

        doorOneCloseYRot = door.transform.localRotation.y;

        doorOneCloseXPos = door.transform.localPosition.x;
        doorOneCloseYPos = door.transform.localPosition.y;
        doorOneCloseZPos = door.transform.localPosition.z;

        if (doorCoordinates.Count != 0)
        {
            doorOneOpenYRot = doorCoordinates[0];

            doorOneOpenXPos = doorCoordinates[1];
            doorOneOpenYPos = doorCoordinates[2];
            doorOneOpenZPos = doorCoordinates[3];

            if (automatic)
            {
                if (doorCoordinates.Count == 5)
                {
                    waitTimeRef = doorCoordinates[4];
                    waitTime = 0.0f;
                }
                else
                {
                    waitTimeRef = 0.0f;
                    waitTime = 0.0f;
                }
            }
        }
        else
        {
            doorOneOpenYRot = door.transform.localRotation.y;

            doorOneOpenXPos = door.transform.localPosition.x;
            doorOneOpenYPos = door.transform.localPosition.y;
            doorOneOpenZPos = door.transform.localPosition.z;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Opening door if closed or half way
        if (!isLocked)
        {
            if (opened && !isOpened)
            {
                door.transform.localRotation = Quaternion.Euler(0.0f, Mathf.Lerp(doorOneCloseYRot, doorOneOpenYRot, l), 0.0f);
                door.transform.localPosition = new Vector3(Mathf.Lerp(doorOneCloseXPos, doorOneOpenXPos, l), Mathf.Lerp(doorOneCloseYPos, doorOneOpenYPos, l), Mathf.Lerp(doorOneCloseZPos, doorOneOpenZPos, l));
                l += 0.95f * Time.deltaTime;
                if (l > 1.0f)
                {
                    l = 0.0f;
                    door.transform.localRotation = Quaternion.Euler(0.0f, doorOneOpenYRot, 0.0f);
                    door.transform.localPosition = new Vector3(doorOneOpenXPos, doorOneOpenYPos, doorOneOpenZPos);
                    isOpened = true;
                }
            }
            // Closing if fully opened
            else if (!opened && isOpened && !automatic)
            {
                door.transform.localRotation = Quaternion.Euler(0.0f, Mathf.Lerp(doorOneOpenYRot, doorOneCloseYRot, l), 0.0f);
                door.transform.localPosition = new Vector3(Mathf.Lerp(doorOneOpenXPos, doorOneCloseXPos, l), Mathf.Lerp(doorOneOpenYPos, doorOneCloseYPos, l), Mathf.Lerp(doorOneOpenZPos, doorOneCloseZPos, l));
                l += 0.95f * Time.deltaTime;
                if (l > 1.0f)
                {
                    l = 0.0f;
                    door.transform.localRotation = Quaternion.Euler(0.0f, doorOneCloseYRot, 0.0f);
                    door.transform.localPosition = new Vector3(doorOneCloseXPos, doorOneCloseYPos, doorOneCloseZPos);
                    isOpened = false;
                }
            }
        }

        if(isOpened && automatic)
        {
            waitTime += Time.deltaTime;
            //print("Wait Time = " + waitTime);
        }

        // Closing if fully opened
        if (waitTime > waitTimeRef && isOpened && automatic || isLocked)
        {
            door.transform.localRotation = Quaternion.Euler(0.0f, Mathf.Lerp(doorOneOpenYRot, doorOneCloseYRot, l), 0.0f);
            door.transform.localPosition = new Vector3(Mathf.Lerp(doorOneOpenXPos, doorOneCloseXPos, l), Mathf.Lerp(doorOneOpenYPos, doorOneCloseYPos, l), Mathf.Lerp(doorOneOpenZPos, doorOneCloseZPos, l));
            l += 0.95f * Time.deltaTime;
            if (l > 1.0f)
            {
                l = 0.0f;
                door.transform.localRotation = Quaternion.Euler(0.0f, doorOneCloseYRot, 0.0f);
                door.transform.localPosition = new Vector3(doorOneCloseXPos, doorOneCloseYPos, doorOneCloseZPos);
                waitTime = 0.0f;
                isOpened = false;
                opened = false;
            }
        }
    }
}
