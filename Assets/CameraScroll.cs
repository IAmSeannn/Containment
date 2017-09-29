using UnityEngine;
using System.Collections;
using Assets;

public class CameraScroll : MonoBehaviour
{

    public TileCreator TileCreator;

	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.UpArrow))
	    {
	        var limit = TileCreator.CameraLimitY;
            if (transform.position.y < limit)
                transform.Translate(Vector3.up);
	    }


	    if (Input.GetKeyDown(KeyCode.DownArrow))
	    {
            var limit = TileCreator.CameraLimitY;
            if (transform.position.y > (limit*-1.0f))
	            transform.Translate(Vector3.down);
	    }

	    if (Input.GetKeyDown(KeyCode.RightArrow))
	    {
            var limit = TileCreator.CameraLimitX;
            if (transform.position.x < limit)
	            transform.Translate(Vector3.right);
	    }

	    if (Input.GetKeyDown(KeyCode.LeftArrow))
	    {
            var limit = TileCreator.CameraLimitX;
            if (transform.position.x > (limit*-1.0f))
	            transform.Translate(Vector3.left);
	    }
	}

}
