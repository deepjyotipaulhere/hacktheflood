using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float maxDistance = 20;

    private Camera _camera;
    private Human _lookingAt;

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
            if (other){
                if (other.CompareTag("Human") && other != _lookingAt){
                    _lookingAt = other.GetComponent<Human>();
                    _lookingAt.Select();
                }
            }
        } else if (_lookingAt){
            _lookingAt.Unselect();
            _lookingAt = null;
        }

        // Get hand position
        // var handRay = InputRayUtils.TryGetHand();
    }
    private void Update(){
        if (_lookingAt){
            if (Input.GetKeyDown("1")){
                _lookingAt.ChangeState(HumanState.Come);
            } else if (Input.GetKeyDown("2")){
                _lookingAt.ChangeState(HumanState.Stop);
            } else if (Input.GetKeyDown("3")){
                _lookingAt.ChangeState(HumanState.Go);
            }
        }
    }
}
