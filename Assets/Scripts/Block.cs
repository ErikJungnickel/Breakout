using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Erik Jungnickel - http://backyard-dev.de
/// Represents a single block.
/// </summary>
public class Block : MonoBehaviour
{
    public event OnBlockHit onBlockHit;
    public delegate void OnBlockHit(Block block);

    public int hp;
    public int currentHp;

    public PowerUps? powerUp;
    public PowerDowns? powerDown;

    // Use this for initialization
    void Start()
    {
        currentHp = hp;
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        SetColor();
    }

    /// <summary>
    /// We set the blocks color depending on its currentHp;
    /// </summary>
    void SetColor()
    {
        if (currentHp == 1)
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        if (currentHp == 2)
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        if (currentHp == 3)
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    /// <summary>
    /// Called when something collides with this block. In this case we decrease its hp.
    /// If the hp reaches 0 we destroy the block and fire an event.
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {       
        currentHp--;

        onBlockHit(this);

        if (currentHp <= 0)
        {
            Destroy(gameObject);           
        }      
    }
}
