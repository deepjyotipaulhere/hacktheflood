using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanState {Go, Stop};

public class Human : MonoBehaviour
{
    private float speed = 1.25f;

    [ SerializeField ]
    private GameObject[] _templates;

    private Transform[] _points;
    private int _currentTargetIdx = 0;
    private Player _player;
    private ParticleSystem _particlesSelected;
    private ParticleSystem _particlesCollision;

    private HumanState _state = HumanState.Stop;

    private GameObject _model;
    private Animator _animator;

    public Game game;
    private AudioSource stepsAudio;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        _player = GameObject.Find("MRTK XR Rig").GetComponent<Player>();
        foreach (Transform eachChild in transform) {
            if (eachChild.name == "ParticlesSelected") {
                 _particlesSelected = eachChild.GetComponentInChildren<ParticleSystem>();
            }
            if (eachChild.name == "ParticlesCollision") {
                 _particlesCollision = eachChild.GetComponentInChildren<ParticleSystem>();
            }
        }

        // Randomly pick a model
        int randomIndex = Random.Range(0, _templates.Length);
        var pos = transform.position; //+ new Vector3(0, 1, 0);
        // var pos = new Vector3(UnityEngine.Random.Range(1000, 100000), 0, 0);
        _model = Instantiate(_templates[randomIndex], pos, Quaternion.identity, GetComponent<Transform>());
        _model.transform.localScale *= 2.15f;
        _animator = _model.GetComponent<Animator>();
        _animator.Play("Land");
        transform.rotation = Quaternion.LookRotation(_points[_currentTargetIdx].position - _points[_currentTargetIdx - 1].position, new Vector3(0,1,0));
    }

    public void Select(){
        _particlesSelected.Play();
    }

    public void Unselect(){
        _particlesSelected.Stop();
    }

    public void ChangeState(HumanState newState){
        _state = newState;
        if (newState == HumanState.Stop && stepsAudio){
            stepsAudio.Stop();
        } else if (newState == HumanState.Go){
            _animator.Play("Movement");
            if (stepsAudio){
                stepsAudio.Play();
            }
        }
    }

    void Update(){
        float dist = Time.deltaTime * speed;
        if (_state == HumanState.Go){
            if (_points == null || _points.Length == 0) return;
            var distance = Vector3.Distance(transform.position, _points[_currentTargetIdx].position);
            if (Mathf.Abs(distance) < 0.05f && _currentTargetIdx < _points.Length - 1){
                _currentTargetIdx++;
                transform.rotation = Quaternion.LookRotation(_points[_currentTargetIdx].position - _points[_currentTargetIdx - 1].position, new Vector3(0,1,0));
            }
            transform.position = Vector3.MoveTowards(transform.position, _points[_currentTargetIdx].position, speed * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter(Collider collision){
        if (collision.gameObject.CompareTag("Human")){
            if (game == null){
                game = GameObject.Find("Game").GetComponent<Game>();
            }
            game.AddCollision();

            _state = HumanState.Stop;
            if (_particlesCollision && stepsAudio){
                stepsAudio.Stop();
                _particlesCollision.Play();
                _animator.Play("Movement");
            }
        } if (collision.gameObject.CompareTag("Goal")){
            game.AddScore();
        }
    }

    public void SetPath(Transform path){
        _currentTargetIdx = 2;
        _points = path.GetComponentsInChildren<Transform>();
        transform.position = _points[1].position;
        
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(3);
        
        stepsAudio = GetComponent<AudioSource>();
        _state = HumanState.Go;
        stepsAudio.volume = Random.Range(0.05f, 0.1f);
        stepsAudio.time = Random.Range(0f, 0.5f);
        stepsAudio.pitch = Random.Range(0.9f, 1.1f);
        stepsAudio.Play();
    }
}
