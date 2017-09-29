using UnityEngine;
using System.Collections;
using Assets;

public class TileInfo : MonoBehaviour
{
    public int PosX;
    public int PosY;
    private TileCreator _tileCreator;
    public GameObject[] Adjacents = new GameObject[4];
    public int Status = 0;
    //0 = normal, 1 = infected, 2 = safe

    void OnMouseUp()
    {
        Debug.Log("Clicked: " + PosX + " - " + PosY);
        Debug.Log("status is : " + Status);
        if(Status == 0)
            if (_tileCreator.OnTileClick(PosX, PosY))
            {
                Debug.Log("Setting Safe: " + PosX + " - " + PosY);
                SetSafe();
            }
                
        
    }

    public void SetTileCreator(TileCreator t)
    {
        _tileCreator = t;
    }

    public void SetNormal()
    {
        Status = 0;
        GetComponent<SpriteRenderer>().color = Color.green;
        ResetObject();
    }

    private void ResetObject()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void SetDead()
    {
        if (Status == 3)
            Debug.Log("Failed");

        Status = 1;
        GetComponent<SpriteRenderer>().color = Color.red;
        ResetObject();
    }

    public void SetSafe()
    {
        Status = 2;
        GetComponent<SpriteRenderer>().color = Color.grey;
        ResetObject();
    }

    public void SetTarget()
    {
        Status = 3;
        GetComponent<SpriteRenderer>().color = Color.magenta;
        ResetObject();
    }
}
