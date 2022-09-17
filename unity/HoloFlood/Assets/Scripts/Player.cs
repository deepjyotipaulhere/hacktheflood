using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float maxDistance = 100;

    private Camera _camera;
    private Human _lookingAt;

    private HumanState _currentCommand = HumanState.Stop;

    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
    }
 
    void FixedUpdate(){
        // Raycast to find what player is looking at (which human)
        RaycastHit hit;
        Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, maxDistance);
        if (hit.collider){
            Human other = hit.collider.gameObject.GetComponent<Human>();
            if (other != null){
                if (_lookingAt == null || other.GetInstanceID() != _lookingAt.GetInstanceID()){
                    _lookingAt = other;
                    other.Select();
                    other.ChangeState(_currentCommand);
                }
            }
        } else if (_lookingAt){
            _lookingAt.Unselect();
            _lookingAt = null;
        }
    }
    
    public void OnPalmUp(){
        // var game = GameObject.Find("Game").GetComponent<Game>();
        // game.TakePhoto();
        _currentCommand = HumanState.Come;
        if (_lookingAt){
            _lookingAt.ChangeState(_currentCommand);
        }
    }

    public void OnPalmDown(){
        _currentCommand = HumanState.Go;
        if (_lookingAt){
            _lookingAt.ChangeState(HumanState.Stop);
        }
    }

}
