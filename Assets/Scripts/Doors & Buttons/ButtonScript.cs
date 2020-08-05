using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public bool clicked;
    public bool isSlider;
    public bool isSensor;
    public GameObject buttonOffState;
    public GameObject buttonOnState;
    public GameObject buttonSlider;
    public Transform sliderMax;
    public Transform sliderMin;
    public Vector3 player;
    public float timerMax;
    private float timer;

    public List<GameObject> lightList;
    public List<Collider> ColliderList;
    

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        clicked = false;
        if (!isSlider)
        {
            buttonOffState = transform.GetChild(0).gameObject;
            buttonOnState = transform.GetChild(1).gameObject;
        }
        else
        {
            buttonSlider = transform.GetChild(0).gameObject;
            sliderMax = transform.GetChild(1).gameObject.transform;
            sliderMin = transform.GetChild(2).gameObject.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSlider)
        {
            if (clicked)
            {
                // DO SLIDERS
            }
            else
            {
                // DO SLIDERS
            }
        }
        else
        {
            if (clicked)
            {
                buttonOnState.SetActive(true);
                buttonOffState.SetActive(false);
                TurnOnLights();
            }
            else
            {
                buttonOffState.SetActive(true);
                buttonOnState.SetActive(false);
                TurnOffLights();
            }
        }
        if(isSensor)
        {
            if (clicked)
            {
                player = GameObject.Find("Player").transform.position;
                CheckMovement();
            }
        }
    }

    void CheckMovement()
    {
        foreach (Collider sensors in ColliderList)
        {
            if(sensors.bounds.Contains(player))
            {
                timer = 0.0f;
                print("Inbound");
            }
        }
        timer += Time.deltaTime / 3600;
        print(timer);

        if(timer >= timerMax)
        {
            timer = 0.0f;
            clicked = false;
            print("maxed timer");
        }
    }

    void TurnOnLights()
    {
        foreach(GameObject light in lightList)
        {
            if(light.GetComponent<Light>() != null)
            {
                light.GetComponent<Light>().enabled = true;
            }
            else
            {
                print("ERROR: ButtonScript: lightList " + light.name + " is missing componenet light");
            }
        }
    }

    void TurnOffLights()
    {
        foreach (GameObject light in lightList)
        {
            if (light.GetComponent<Light>() != null)
            {
                light.GetComponent<Light>().enabled = false;
            }
            else
            {
                print("ERROR: ButtonScript: lightList " + light.name + " is missing componenet light");
            }
        }
    }
}
