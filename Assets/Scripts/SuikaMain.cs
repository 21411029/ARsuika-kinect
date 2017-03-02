using UnityEngine;
using System.Collections;

public class SuikaMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if( col.tag == "Stick")
            this.transform.position = new Vector3(Random.Range(1.0f, 1.5f), 1.5f, -Random.Range(1.0f, 1.5f));
    }
}
