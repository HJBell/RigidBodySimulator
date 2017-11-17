using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Oscillator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var pos = this.transform.position;
        pos.x = Mathf.Sin(Time.time) * 2f;
        this.transform.position = pos;
	}
}
