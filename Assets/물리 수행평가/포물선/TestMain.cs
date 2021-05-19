using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMain : MonoBehaviour {

    //public ThrowSimulator simul;
    public Transform bullet;
    public Transform startPoint;
    public Transform endPoint;
    public float g;
    public Transform heightGo;


    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        if (GUILayout.Button("발사", GUILayout.Width(100), GUILayout.Height(60)))
        {
            var clone = Instantiate(bullet);

            //this.simul.Shoot(clone, startPoint.position, endPoint.position, g, heightGo.position.y, OnCompleteThrowSimulation);
        }
    }

    private void OnCompleteThrowSimulation()
    {
        Debug.Log("도착");
    }
}
