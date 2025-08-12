using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] GameObject _gameObjectToFollow;
    [SerializeField] Vector3 offset = Vector3.zero;

    private void Awake(){
        if(_gameObjectToFollow == null) _gameObjectToFollow = FindObjectOfType<Player>().gameObject;
    }
    void Start(){
        //offset = transform.position + _gameObjectToFollow.transform.position;
    }

    void LateUpdate(){
        transform.position = _gameObjectToFollow.transform.position + offset;
    }
}