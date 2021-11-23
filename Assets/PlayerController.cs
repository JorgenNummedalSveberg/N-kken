using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public float camerLerpSpeed = 1f;
    public float moveSpeed = 0.1f;
    public float animationInterval = 2f;
    public float soundDuration = 0.5f;
    public Sprite[] upSprites;
    public Sprite[] downSprites;
    public Sprite[] sideSprites;
    private Sprite[] _currentSprites;
    public Sprite stopSprite;
    private float _xMove;
    private float _yMove;
    private SpriteRenderer _renderer;
    public bool land = false;
    public AudioClip[] splashing;
    public AudioClip[] walking;
    private AudioClip[] _currentSounds;
    private int _spriteIndex;
    private int _soundIndex = 0;
    private AudioSource _soundPlayer;
    private bool _playSound = true;
    public GameObject ripples;
    private (Transform, Transform, Transform, Transform) _eyes;

    private Camera _cam;


    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _currentSprites = downSprites;
        _cam = transform.Find("Main Camera").GetComponent<Camera>();
        _currentSounds = walking;
        _soundPlayer = GetComponent<AudioSource>();
        _eyes.Item1 = transform.Find("LeftEye");
        _eyes.Item2 = transform.Find("RightEye");
        _eyes.Item3 = transform.Find("LeftLeftEye");
        _eyes.Item4 = transform.Find("RightRightEye");
    }

    // Update is called once per frame
    void Update()
    {
        if (!land)
        {
            _currentSounds = splashing;
        }
        else
        {
            _currentSounds = walking;
        }
        _xMove = Input.GetAxisRaw("Horizontal");
        _yMove = Input.GetAxisRaw("Vertical");
        var position = new Vector3(transform.position.x, transform.position.y, _cam.transform.position.z);
        _cam.transform.position = position;
        _spriteIndex = ((int) Math.Ceiling(((Time.time % animationInterval) / animationInterval) * _currentSprites.Length + 0.000000001f)) - 1;
        int oldSoundIndex = _soundIndex;
        _soundIndex = ((int) Math.Ceiling(((Time.time % (soundDuration*_currentSounds.Length)) / (soundDuration*_currentSounds.Length)) * _currentSounds.Length + 0.000000001f)) - 1;
        if (oldSoundIndex != _soundIndex)
        {
            _playSound = true;
        }

    }

    private void FixedUpdate()
    {
        Vector2 direction = new Vector2(_xMove, _yMove).normalized;
        transform.Translate(direction*moveSpeed);

        _renderer.flipX = direction.x > 0;
        if (direction.x.Equals(0) && direction.y.Equals(0))
        {
            _renderer.sprite = stopSprite;
            
            _eyes.Item1.gameObject.SetActive(true);
            _eyes.Item2.gameObject.SetActive(true);
            _eyes.Item3.gameObject.SetActive(false);
            _eyes.Item4.gameObject.SetActive(false);
        }
        else
        {
            if (!direction.x.Equals(0))
            {
                _currentSprites = sideSprites;
                
                _eyes.Item1.gameObject.SetActive(false);
                _eyes.Item2.gameObject.SetActive(false);
                _eyes.Item3.gameObject.SetActive(direction.x < 0);
                _eyes.Item4.gameObject.SetActive(direction.x > 0);
            }
            else
            {
                if (direction.y < 0)
                {
                    _currentSprites = downSprites;
                    _eyes.Item1.gameObject.SetActive(true);
                    _eyes.Item2.gameObject.SetActive(true);
                    _eyes.Item3.gameObject.SetActive(false);
                    _eyes.Item4.gameObject.SetActive(false);
                }
                else
                {
                    _currentSprites = upSprites;
                    
                    _eyes.Item1.gameObject.SetActive(false);
                    _eyes.Item2.gameObject.SetActive(false);
                    _eyes.Item3.gameObject.SetActive(false);
                    _eyes.Item4.gameObject.SetActive(false);
                }
            }
            _renderer.sprite = _currentSprites[_spriteIndex];

            if (_playSound)
            {
                _playSound = false;
                _soundPlayer.clip = _currentSounds[Random.Range(0, _currentSounds.Length-1)];
                _soundPlayer.Play();
                GameObject ripple = Instantiate(ripples);
                ripple.transform.position = transform.position;
            }
        }
        
    }
}
