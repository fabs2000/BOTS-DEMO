using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehaviour : MonoBehaviour
{
    public float TravelTime = 3f;
    public float MissileSpeed = 2f;
    
    private Vector3 _destPos;

    private void Start()
    {
        _destPos = transform.parent.position;
        
        StartCoroutine(MoveMissile());
    }
    
    private IEnumerator MoveMissile()
    {
        float elapsedTime = 0f;
        Vector3 currentPos = transform.position;

        while (elapsedTime < TravelTime)
        {
            transform.position = Vector3.Lerp(currentPos, _destPos, (elapsedTime/TravelTime) * MissileSpeed);
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        yield return null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
    
    
}
