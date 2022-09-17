using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanState {Come, Go, Stop};

public class Human : MonoBehaviour
{
    public float speed = 0.1f;
    private Player _player;
    private ParticleSystem _particlesSelected;

    private HumanState state = HumanState.Stop;

    void Start()
    {
        _player = GameObject.Find("MRTK XR Rig").GetComponent<Player>();
        _particlesSelected = GetComponentInChildren<ParticleSystem>();
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
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, dist);
        } else if (state == HumanState.Go){
            transform.position += (transform.position - _player.transform.position).normalized * dist;
        }
    }
}
