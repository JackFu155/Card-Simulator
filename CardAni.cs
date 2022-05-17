using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The CardAni script is used to manage the card animation shown when players draw, discard, or when the deck is refilled
/// These objects can be used as simple animations that vanish after a short period once the animation ends
/// </summary>
public class CardAni : MonoBehaviour
{
    public float counter = 0.0f;
    public float counterIncrease;
    public Vector3 start;
    public Vector3 end;
    public bool go;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //The go boolean is used to initate the lerp
        //This allows for stating cards to be placed in the scene without any motion for effect
        //When the object moves, the counter increases to increase the value of the lerp
        if(go == true)
        {
            DrawDiscardReturnAnimation(start, end, counter);
            counter += counterIncrease;
            //When the Lerp finished, the object is destroyed.
            if (counter >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    //functiuon for lerping the card object from one point to another
    public void DrawDiscardReturnAnimation(Vector3 start1, Vector3 end1, float interpolationvalue)
    {
        transform.position = Vector3.Lerp(start1, end1, interpolationvalue);
    }
}
