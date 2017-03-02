using UnityEngine;
using System.Collections;

public class SuikaMain : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Hit");
        if (col.gameObject.tag == "Stick")
        {
            BodySourceView body = GameObject.FindGameObjectWithTag("KinectManagers").GetComponent<BodySourceView>();
            Debug.Log(body.isSwinging());
            if (body.isSwinging())
            {
                this.transform.position = new Vector3(Random.Range(1.0f, 1.5f), 3.0f, -Random.Range(1.0f, 1.5f));
            }
        }
    }
}
