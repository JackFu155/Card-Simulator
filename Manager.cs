using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script deals with the main scene manager.
/// This scrip holds nearly all major assets in the scene and well as most functions that deal with them
/// Functions from the deck include: Assigning, Drawing random cards, and Refilling
/// Functions from the hand include: Adding Cards to the Hand, Discarding, and Titling the cards based on given rotation value
/// Functions from the discard pile include: Recieving cards, managing the contents of the pile, and transferring the pile to the deck
/// </summary>
public class Manager : MonoBehaviour
{
    public GameObject deck;
    public GameObject deckSpawnPoint;
    public GameObject discardPile;
    public GameObject energyCounterObject;
    public List<GameObject> deckCards;
    public List<GameObject> handCards;
    public List<GameObject> discardedCards;
    public GameObject card;
    public List<GameObject> damageCards;
    public int damageSelectionNumber;
    public List<GameObject> shieldCards;
    public int shieldSelectionNumber;
    public List<GameObject> strengthCards;
    public int strengthSelectionNumber;
    public int NumCardsInDeck;
    public int NumCardsInHand;
    public int discarded;
    public int energyCounter;
    public int energyCounterLimit;
    public TextAsset textJSON;
    public float cardTilt;
    public TMP_Text energyReading;
    public float cardSpace;
    public TMP_Text DeckCounter;
    public TMP_Text DiscardCounter;
    public Text beginText;
    public GameObject drawAnimation;
    public GameObject discardAnimation;
    public GameObject returnAnimation;
    public bool drawOrReturn;

    //Classes that allow for data to be parsed from the JSON file
    //Main class designed to get data from each card
    [System.Serializable]
    public class Cards
    {
        public string name;
        public int cost;
        public int image_id;
        public string type;
        public int value;
        public string target;
        public Effect[] effects;
    }

    //Sub class that allows for data from the sub-array Effects to be copied
    [System.Serializable]
    public class Effect
    {
        public string type;
        public int value;
        public string target;
    }

    //Main class that will hold the copied data from the JSON file
    [System.Serializable]
    public class CardList
    {
        public Cards[] cards;
    }

    public CardList cardList = new CardList();
    // Start is called before the first frame update
    //At runtime, the data is parsed and copied from the JSON file and saved in a Cards array
    //The deck is then formed by sorting each card by type and picking the cards randomly based on 4/3/2 ratio (though it is adjustable)
    //The deck is then added to the scene
    void Start()
    {
        cardList = JsonUtility.FromJson<CardList>(textJSON.text);
        LoadCardData(cardList);
        AssignDeck();
        CenterDeck();
        ArrangeHand();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateHandAndDeck();
        //ArrangeHand();
        //ForceDiscard();
        //Draws four cards and changes center text
        if(drawOrReturn == true)
        {
            DrawMultiple(4);
            beginText.text = "Press Enter to Draw";
        }
        else if(drawOrReturn == false)
        {
            beginText.text = "If the card has enough value, drag it to the circle to play it, Press End Turn to end your turn";
        }
    }

    //Function for reading in card data from JSON File
    public void LoadCardData(CardList cardlist)
    {
        //Divides up all the cards by type and saves them in three lists based on that type
        //Then loops through each individual type array and randomly picks out a number with those maximum values to add to the main deck
        for(int i = 0; i < cardList.cards.Length; i++)
        {
            GameObject newCard = Instantiate(card, new Vector3(deckSpawnPoint.transform.position.x, deckSpawnPoint.transform.position.y + (0.01f * i), deckSpawnPoint.transform.position.z), Quaternion.identity);
            newCard.GetComponent<Card>().cardName = cardList.cards[i].name;
            newCard.GetComponent<Card>().cardCost = cardList.cards[i].cost;
            newCard.GetComponent<Card>().ImageID = cardList.cards[i].image_id;
            newCard.GetComponent<Card>().cardType = cardList.cards[i].type;
            newCard.GetComponent<Card>().cardText = cardList.cards[i].effects[0].type  + " " + cardList.cards[i].effects[0].target + " " + cardList.cards[i].effects[0].value;
            if(cardList.cards[i].effects[0].type == "damage")
            {
                damageCards.Add(newCard);
            }
            else if (cardList.cards[i].effects[0].type == "shield")
            {
                shieldCards.Add(newCard);
            }
            else if (cardList.cards[i].effects[0].type == "strength")
            {
                strengthCards.Add(newCard);
            }
        }
    }

    //Once the deck list has been assigned, the cards in the deck list are added to the scene
    //The deck is instantiated by spawning the cards in y values of 0.01 going up so it can be displayed properly
    public void AssignDeck()
    {
        //Selecting Random Damage cards based on designated number
        for (int i = 0; i < damageSelectionNumber; i++)
        {
            int random = Random.Range(0, damageCards.Count);
            GameObject selectedCard = damageCards[random];
            deckCards.Add(selectedCard);
            damageCards.Remove(selectedCard);            
        }

        //Selecting Random Shield cards based on designated number
        for (int i = 0; i < shieldSelectionNumber; i++)
        {
            int random = Random.Range(0, shieldCards.Count);
            GameObject selectedCard = shieldCards[random];
            deckCards.Add(selectedCard);
            shieldCards.Remove(selectedCard);
        }

        //Selecting Random Strength cards based on designated number
        for (int i = 0; i < strengthSelectionNumber; i++)
        {
            int random = Random.Range(0, strengthCards.Count);
            GameObject selectedCard = strengthCards[random];
            deckCards.Add(selectedCard);
            strengthCards.Remove(selectedCard);
        }
    }

    //This function properly arranges the position of cards stacked on top of each other on the deck
    //It also makes sure that the cards in the deck cannot be selected while in deck
    //The ArrangeHand function makes sure that the the cards become usable if possible when eventually added to the hand
    public void CenterDeck()
    {
        for(int i = 0; i < deckCards.Count; i++)
        {
            deckCards[i].transform.position = new Vector3(deck.transform.position.x, deck.transform.position.y, deck.transform.position.z + (i * -0.01f));
            deckCards[i].GetComponent<Card>().usable = false;
        }
    }

    //Function for displaying proper number of cards in the deck, hand, and discard pile
    //Sets the approprate values for each element then displays them in both the UI and inspectors
    public void CalculateHandAndDeck()
    {
        int deck = deckCards.Count;
        int hand = handCards.Count;
        int discard = discardedCards.Count;

        NumCardsInDeck = deck;
        NumCardsInHand = hand;
        discarded = discard;

        energyReading.text = energyCounter.ToString() + "/" + energyCounterLimit.ToString();
        DeckCounter.text = "Deck: " + deckCards.Count.ToString();
        DiscardCounter.text = "Discard: " + discardedCards.Count.ToString();
    }

    //Arranges the cards in proper order with the slight tilt.
    //Written in a way that it can be used with as many cards as possible
    public void ArrangeHand()
    {
        //Calculates the middle card value, rounds down if not a whole number
        int middle = CalculateMiddle(handCards);
        float rotationVal = cardTilt;
        //If we have an odd number of cards in the hand
        if (handCards.Count % 2 != 0)
        {
            //Needs to take into account the card in the middle
            int numberOfCardsBetweenMiddleAndEdge = middle - 1;
            float xStartingPoint = cardSpace * (numberOfCardsBetweenMiddleAndEdge + 1);
            float rotationHeft = rotationVal / (numberOfCardsBetweenMiddleAndEdge + 1);
            float ZStartingPoint = -0.05f;
            float currentRot = -cardTilt;
            for (int i = 0; i < handCards.Count; i++)
            {
                handCards[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, currentRot);
                handCards[i].transform.position = new Vector3(xStartingPoint, -3.5f, ZStartingPoint);
                currentRot += rotationHeft;
                xStartingPoint -= cardSpace;
                ZStartingPoint -= 0.05f;
                if(energyCounter >= handCards[i].GetComponent<Card>().cardCost)
                {
                    handCards[i].GetComponent<Card>().usable = true;
                }
                else if (energyCounter < handCards[i].GetComponent<Card>().cardCost)
                {
                    handCards[i].GetComponent<Card>().usable = false;
                }
                handCards[i].GetComponent<Card>().startingMovePos = handCards[i].transform.position;
            }
        }
        //If we have an even number of cards in the hand
        else if (handCards.Count % 2 == 0)
        {
            //Has no card in the middle to take into account
            int numberOfCardsBetweenMiddleAndEdge = middle;
            float xStartingPoint = cardSpace * (numberOfCardsBetweenMiddleAndEdge);
            float rotationHeft = rotationVal / numberOfCardsBetweenMiddleAndEdge;
            float currentRot = -cardTilt;
            float startingY = -0.05f;
            for (int i = 0; i < middle; i++)
            {
                //Rotates at main rotation value first then divides rotation by the number of cards between edge and center
                //Current rotation increases by calculated division value with each iteration
                handCards[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, currentRot);
                handCards[i].transform.position = new Vector3(xStartingPoint, -3.5f, startingY);
                currentRot += rotationHeft;
                xStartingPoint -= cardSpace;
                startingY -= 0.05f;
                //Sets the usable bool of the cards to true or false depending on the card's cost compared to the m
                if (energyCounter >= handCards[i].GetComponent<Card>().cardCost)
                {
                    handCards[i].GetComponent<Card>().usable = true;
                }
                else if (energyCounter < handCards[i].GetComponent<Card>().cardCost)
                {
                    handCards[i].GetComponent<Card>().usable = false;
                }
                handCards[i].GetComponent<Card>().startingMovePos = handCards[i].transform.position;
            }
            //Adds on outside of loop in order to compensate for lack of center card
            currentRot += rotationHeft;
            xStartingPoint -= cardSpace;
            for (int i = middle; i < handCards.Count; i++)
            {
                //Rotates at main rotation value first then divides rotation by the number of cards between edge and center
                //Current rotation increases by calculated division value with each iteration
                handCards[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, currentRot);
                handCards[i].transform.position = new Vector3(xStartingPoint, -3.5f, startingY);
                currentRot += rotationHeft;
                xStartingPoint -= cardSpace;
                startingY -= 0.05f;
                if (energyCounter >= handCards[i].GetComponent<Card>().cardCost)
                {
                    handCards[i].GetComponent<Card>().usable = true;
                }
                handCards[i].GetComponent<Card>().startingMovePos = handCards[i].transform.position;
            }
        }

        if(handCards.Count == 1)
        {
            handCards[0].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            handCards[0].GetComponent<Card>().startingMovePos = handCards[0].transform.position;
            if (energyCounter >= handCards[0].GetComponent<Card>().cardCost)
            {
                handCards[0].GetComponent<Card>().usable = true;
            }
            else if (energyCounter < handCards[0].GetComponent<Card>().cardCost)
            {
                handCards[0].GetComponent<Card>().usable = false;
            }
        }
    }

    //Basic function for drawing cards
    //Because the code for drawing cards is used in multiple functions, it is kept in a separate function
    //The function takes in whether or not the player has cards in the deck and properly responds to it
    //If the deck is out of cards, the deck is refilled and the function continues soon after
    //Otherwise the card is drawn as normal
    //Because this function draws a single card, it can be properly used even if the function is called multiple times
    //as the deck refills when empty before another card is drawn
    public void DrawCard()
    {
        if (deckCards.Count > 0)
        {
            DrawFunction();
        }
        else if (deckCards.Count == 0)
        {
            RefillDeck();
            //Wait(2);
            DrawFunction();
        }
    }

    //Special function for drawing more than one card
    //Repeats the function based on the number passed into the function
    //Then sets the parameters to prevent players from drawing additional cards until the turn ends
    public void DrawMultiple(int multiple)
    {
        //Function uses Return key to ease user use in order to better demonstrate the loop
        //Requires the drawOrReturn value to be true in order to ensure that the function cannot be called multiple times
        if (Input.GetKeyDown(KeyCode.Return) && drawOrReturn == true)
        {
            for (int i = 0; i < multiple; i++)
            {
                DrawCard();
            }
            beginText.text = "If the card has enough value, drag it to the circle to play it, Press End Turn to end your turn";
            drawOrReturn = false;
        }
    }

    //The main reusable bit of code which draws a card from the deck
    //Removes a random card from the main deck list and adds it to the hand
    public void DrawFunction()
    {
        int random = Random.Range(0, deckCards.Count);
        deckCards[random].GetComponent<Card>().Flip();
        handCards.Add(deckCards[random]);
        deckCards.Remove(deckCards[random]);
        GameObject ani = Instantiate(drawAnimation);
        ani.GetComponent<CardAni>().go = true;
        ArrangeHand();
    }

    //Function which discards all cards from the hand
    public void ForceDiscard()
    {
        //First the function adds all the hand cards to the discarded cards list
        //Then it subsequently moves the cards from the hand to the discard pile
        //This uses an algorithm idential to the used for assigning the deck which stacks the cards on top of each other
        //Though unlike the deck, the cards are placed face up much like a real discard pile on a tapletop card game
        for (int i = 0; i < handCards.Count; i++)
        {
            discardedCards.Add(handCards[i]);
            handCards[i].transform.position = new Vector3(discardPile.transform.position.x, discardPile.transform.position.y, discardPile.transform.position.z + (-0.01f * i));
            handCards[i].GetComponent<Card>().usable = false;
            GameObject ani = Instantiate(discardAnimation);
            ani.GetComponent<CardAni>().go = true;
        }
        //The cards added to the discarded list are taken out of the hand list via a reverse loop
        for (int i = handCards.Count - 1; i > -1; i--)
        {
            handCards.Remove(handCards[i]);
        }
        //This function then compensates for the tilted nature of the hand cards by looping through the discard pile and
        //straightening each card out
        for(int i = 0; i < discardedCards.Count; i++)
        {
            discardedCards[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    //Function for moving all cards from the discard pile back to the deck
    public void RefillDeck()
    {
        //First each card is then flipped face down 
        //Then the cards are moved back to the deck
        //The cards are then added to the deck list
        //Animations representing the cards moving back to the deck are then played
        for (int i = 0; i < discardedCards.Count; i++)
        {
            discardedCards[i].GetComponent<Card>().Flip();
            discardedCards[i].transform.position = new Vector3(deck.transform.position.x, deck.transform.position.y, deck.transform.position.z + (i * -0.01f));
            deckCards.Add(discardedCards[i]);
            GameObject ani = Instantiate(returnAnimation);
            ani.GetComponent<CardAni>().go = true;
        }
        //Afterwards, the discarded cards list is then cleared
        discardedCards.Clear();
    }

    //Function for the custom End Turn Button
    //Activates functions which discard all cards in the hand
    //Then changes boolean to allow for four new cards to be drawn
    //It then resets the energy counter for testing purposes
    public void EndTurn()
    {
        ForceDiscard();
        drawOrReturn = true;
        energyCounter = 4;
    }

    //Calculates the middle value of a list, and since it saves as an int it rounds down to the nearest whole number, it is able to calculate which gameObject is in the middle
    //Can be used for any list with gameobjects and returns an int representing which GameObject is the middle one
    //Used in this program for managing hand arrangement in ArrangeHand()
    public int CalculateMiddle(List<GameObject> cards)
    {
        int numberOfCards = cards.Count;
        int middle = (0 + numberOfCards) / 2;
        return middle;
    }    
}
