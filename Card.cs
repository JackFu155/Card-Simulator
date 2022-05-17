using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script is attached to the card object and deals with all general functions relating to the cards themselves and their data
/// Such functions include displaying data, discarding, collision, drag and drop, and flipping
/// </summary>
public class Card : MonoBehaviour
{
    public GameObject front;
    public List<GameObject> images;
    public string cardName;
    public int cardCost;
    public int ImageID;
    public string cardType;
    public string cardText;
    public TMP_Text type;
    public TMP_Text cost;
    public TMP_Text Name;
    public TMP_Text Text;
    public GameObject manager;
    public float hoverVal;
    public bool usable;
    public GameObject glow;
    public Vector3 startingMovePos;
    public Vector3 mousePosition;
    public bool clicked = false;
    public GameObject dropZone;
    public GameObject discard;
    public bool discardReady = false;
    // Start is called before the first frame update
    void Start()
    {
        //All three images are preattached to the card prefab, depending on the card's type based on the ImageID, the other two images are deleted
        //It also changes the color of the texture to make each card easier to differentiate
        //Can easily be added to once more images are created
        //If it is a damage card with an ID of 50
        if (ImageID == 50)
        {
            Destroy(images[1]);
            Destroy(images[2]);
            front.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            cardType = "Damage";
        }
        //If it is a damge card with an ID of 77
        else if (ImageID == 77)
        {
            Destroy(images[0]);
            Destroy(images[2]);
            front.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            cardType = "Shield";
        }
        //If it is a damge card with an ID of 18
        else if (ImageID == 18)
        {
            Destroy(images[0]);
            Destroy(images[1]);
            front.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            cardType = "Strength";
        }

        //When the card is created, it is by default flipped upside down to conceal its contents
        Flip();
    }

    // Update is called once per frame
    void Update()
    {
        //Displays the current parameters on the card's UI
        type.text = cardType;
        cost.text = cardCost.ToString();

        //Chnages the text of the card's cost and the glow of the card depending on whether or not the card can be used
        //The card's usability is determined in the Manager scripts ArrangeHand function
        //If the card is usable, the card's cost text is white and the card glows green
        //The card will also move up slightly when the mouse hovers over it (see OnMouseOver())
        if(usable == true)
        {
            cost.color = Color.white;
            glow.GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0067f, 1.0f);
        }
        //If the card can't be used, it's cost text will glow red and the card will be immobile
        else if(usable == false)
        {
            cost.color = Color.red;
            glow.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        Name.text = cardName;
        Text.text = cardText;
        //This function assists with clicking and dragging or cards by converting the mouseposition into the world space (see OnMouseDrag())
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    //Simple function that flips the card on the Y axis
    public void Flip()
    {
        transform.localEulerAngles += new Vector3(0.0f, 180.0f, 0.0f);
    }

    //Function that is meant to allow for a two second delay when the card is dropped on the DropZone before it is added to the discard pile
    //The energy cost is subsequently decreased by the cost and the hand updates its arrangement and the usability of all cards there
    IEnumerator Wait(int numberOfSeconds)
    {
        yield return new WaitForSeconds(numberOfSeconds);
        Discard();
        manager.GetComponent<Manager>().energyCounter -= cardCost;
        manager.GetComponent<Manager>().ArrangeHand();
    }

    //This function moves the card from the hand to the discard pile
    //Aftrwards, it adds the card to the discardCards list and removes it from the hand list
    public void Discard()
    {
        List<GameObject> discardCards = manager.GetComponent<Manager>().discardedCards;
        transform.position = new Vector3(discard.transform.position.x, discard.transform.position.y, discard.transform.position.z + (-0.01f * discardCards.Count));
        manager.GetComponent<Manager>().discardedCards.Add(this.gameObject);
        manager.GetComponent<Manager>().handCards.Remove(this.gameObject);
    }

    //When the mouse is moves over the card, it moves upward based on a set value on the card
    //Will only work if the card can be used and is not clicked on, otherwise it causes the card to end up at a different place when the mouse leaves it
    private void OnMouseEnter()
    {
        if(usable == true && clicked == false)
        {
            transform.position += new Vector3(0.0f, hoverVal, 0.0f);
        }
    }

    //When the mouse leaves the card, the card returns to its original position.
    //Will only work if the card can be used and is not clicked on, otherwise it causes the card to end up at a different place when the mouse leaves it
    private void OnMouseExit()
    {
        if(usable == true && clicked == false)
        {
            transform.position += new Vector3(0.0f, -hoverVal, 0.0f);
        }
        clicked = false;
    }

    //When the card is clicked for an instant, the card's clicked bool becomes true to activate other functions
    //Clicked needs to be false and as a safeguard the card must be usable
    public void OnMouseDown()
    {
        if(usable == true && clicked == false)
        {
            clicked = true;
        }
    }

    //When the card is clicked and dragged, it follows the mouse
    //The card must be usable and clicke don for this to work
    public void OnMouseDrag()
    {
        if(usable == true && clicked == true)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9.0f));
            gameObject.transform.position = mousePosition;
            //Debug.Log("Mouse Position: " + finalPos);
            //Debug.Log("Card Position: " + transform.position);
        }
    }

    //When the mouse button is released, different events occur based on whether or not the card is ready to be discarded
    //The discardedReady bool is activated if the card comes into contact with the dropZone
    public void OnMouseUp()
    {
        //If it fails to come into contact with the dropZone, the card returns to its original starting position
        if (usable == true && clicked == true && discardReady == false)
        {
            transform.position = startingMovePos;
        }
        //If it comes into contact, the card strightens out and makes its new position that of the dropzone
        //It then waits for two seconds before the card moves to the discard pile and the energy decreased by the card's cost
        else if (usable == true && clicked == true && discardReady == true)
        {
            usable = false;
            clicked = false;
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            transform.position = dropZone.transform.position;
            //manager.GetComponent<Manager>().energyCounter -= cardCost;
            StartCoroutine(Wait(2));
        }
    }

    //Functions designed to deal with the trigger zone for playing cards so long as the object collides with the zone
    //When the card enters the zone, the bool becomes true and ready for discarding
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DropZone")
        {
            discardReady = true;
        }
    }
    //When the card exits the zone, the bool becomes false again
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DropZone")
        {
            discardReady = false;
        }
    }

}
