using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRotate : MonoBehaviour {
    private float varies;
	// Use this for initialization
	void Start () {
        varies = Random.Range(0f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.Lerp(Vector3.zero,new Vector3(Random.Range(0, 1f), 1f, Random.Range(0, 1f)),Time.deltaTime));
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.03f * Mathf.Sin(Time.time + varies) * transform.localScale.x, transform.position.z);
    }
}
