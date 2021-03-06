﻿using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {
    public float ScreenScale = 1.8f;

    GameObject mainCamera;
    GameObject targetScreen;
	// Use this for initialization
	void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.transform.LookAt(new Vector3(0,0,0));

        targetScreen = GameObject.FindGameObjectWithTag("TargetScreen");
        targetScreen.transform.localPosition = new Vector3(0,0,20);
        targetScreen.transform.LookAt(mainCamera.transform.position);
        targetScreen.transform.LookAt(targetScreen.transform.position + mainCamera.transform.up);
        targetScreen.transform.localScale = new Vector3(1.62f, 1.08f, 1.00f) * ScreenScale;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
            Screen.fullScreen = !Screen.fullScreen;
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameObject kinectManager = GameObject.FindGameObjectWithTag("KinectManagers");
            kinectManager.GetComponent<BodySourceView>().Visible = !kinectManager.GetComponent<BodySourceView>().Visible;
        }

    }
}
