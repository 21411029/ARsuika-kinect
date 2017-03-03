using UnityEngine;
using System.Collections;

public class SuikaMain : MonoBehaviour {

    public AudioClip audioHit;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter(Collider col)
    {
        if( col.tag == "Stick")
        {
            BodySourceView body = GameObject.FindGameObjectWithTag("KinectManagers").GetComponent<BodySourceView>();
            Debug.Log("Hit and Swing=" + body.isSwinging());
            if (body.isSwinging())
            {
                WiimoteController.Instance.SetRumble(0.2f);
                AudioSource stickAudio = col.GetComponent<AudioSource>();
                stickAudio.clip = audioHit;
                stickAudio.time = 0.12f;
                stickAudio.Play();
                this.transform.position = new Vector3(Random.Range(1.5f, 2.0f), 3.0f, -Random.Range(1.5f, 2.0f));
            }

        }
    }

}
