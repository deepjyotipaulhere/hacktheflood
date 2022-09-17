using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanState {Come, Go, Stop};

public class Human : MonoBehaviour
{
    public float speed = 0.001f;
    public Transform PathContainer;
    private Transform[] _points;
    private int _currentTargetIdx;
    private Player _player;
    private ParticleSystem _particlesSelected;
    private ParticleSystem _particlesCollision;

    private HumanState state = HumanState.Stop;

    void Start()
    {
        _player = GameObject.Find("MRTK XR Rig").GetComponent<Player>();
        foreach (Transform eachChild in transform) {
            if (eachChild.name == "ParticlesSelected") {
                 _particlesSelected = eachChild.GetComponentInChildren<ParticleSystem>();
            }
            if (eachChild.name == "ParticlesCollision") {
                 _particlesCollision = eachChild.GetComponentInChildren<ParticleSystem>();
            }
        }
        _points = PathContainer.GetComponentsInChildren<Transform>();
        transform.position = _points[0].position;
    }

    public void Select(){
        _particlesSelected.Play();
    }

    public void Unselect(){
        _particlesSelected.Stop();
    }

    public void ChangeState(HumanState newState){
        state = newState;
    }

    void Update(){
        float dist = Time.deltaTime * speed;

        if (state == HumanState.Come){
            
            if (_points == null || _points.Length == 0) return;
            var distance = Vector3.Distance(transform.position, _points[_currentTargetIdx].position);
            if (Mathf.Abs(distance) < 0.05f){
                _currentTargetIdx++;
            }
            transform.position = Vector3.MoveTowards(transform.position, _points[_currentTargetIdx].position, speed * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter(Collider collision){
        if (collision.gameObject.CompareTag("Human")){
            state = HumanState.Stop;
            _particlesCollision.Play();
        }
    }
}
