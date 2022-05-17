using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DropZone script is designed to manage the DropZone's collision for testing purposes.
/// If the object collides with the DropZone it will change color and subsequently turn back when it it removed
/// </summary>
public class DropZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Functions designed to deal with the trigger zone for playing cards so long as the object collides with the zone
    //When the card enters the zone, the dropzone turns magenta
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Card")
        {
            GetComponent<SpriteRenderer>().color = Color.magenta;

        }
    }
    //When the card exits the zone, the dropzone turns white
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Card")
        {
            GetComponent<SpriteRenderer>().color = Color.white;

        }
    }
}
