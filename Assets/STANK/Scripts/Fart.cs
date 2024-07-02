using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fart : Smeller
{
    public float lingerDuration = 10.0f;
    float lingerTimer = 0.0f;
    public float velocity = 10.0f;
    public AnimationCurve lingerCurve;

    // Start is called before the first frame update
    void Start()
    {
        lingerTimer = lingerDuration;
    }

    // Update is called once per frame
    void Update()
    {
        lingerTimer -= Time.deltaTime;
        if(lingerTimer <= 0.0f){
            Destroy(gameObject);
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, (velocity * lingerCurve.Evaluate(lingerTimer / lingerDuration)));

    }
}
