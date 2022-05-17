using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Due to some complications involving Unity's UI, this script creates an EndTurn button out of a standard gameobject that accomplishes the same task
/// </summary>
public class Button : MonoBehaviour
{
    public int duration = 0;
    public int durationlimit;
    public GameObject functionObject;
    public bool over = false;
    /// <summary>
    /// Due to Unity's own buttons failing to respond to the mouse, I decided to create my own
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Duration is used to determine how long the button remains a color after clicking, can be adjusted
        //When the button is clicked, the duration counter goes up
        if(duration < durationlimit)
        {
            if (Input.GetMouseButton(0) && over == true)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
                duration += 1;
            }
        }
    }

    //This function calls the EndTurn function located in the manager
    public void CallFunction()
    {
        functionObject.GetComponent<Manager>().EndTurn();
    }

    //When the mouse button is clicked, the function is called
    private void OnMouseDown()
    {
        CallFunction();
    }

    //When the mouse button goes over the button, it turns magenta
    //Over bool is used to prevent the button from changing color unless the mouse button is hovering over it
    private void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().color = Color.magenta;
        over = true;
    }

    //When the mouse button is released, the button changes back to its hovered color and the duration counter is reset
    private void OnMouseUp()
    {
        GetComponent<SpriteRenderer>().color = Color.magenta;
        duration = 0;
    }

    //When the mouse exits the button, it changes back to its original color
    //Over bool is used to prevent the button from changing color unless the mouse button is hovering over it
    private void OnMouseExit()
    {
        over = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
