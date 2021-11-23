using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Nøkken : MonoBehaviour
{
    public Sprite[] sprites;

    public float spawnTime = 1f;
    private int _spriteIndex;
    private bool _changeSprite;
    private bool _spawnRipple;
    private bool _death;
    private SpriteRenderer _renderer;
    public float _birthtime;
    private Tilemap _water;
    private Tilemap _land;

    private GameObject _player;
    public float maxDistance = 3;
    public float minDistance = 1;
    public bool withinDistance;
    public float despawnTime = 2;
    public float despawnTimer;
    public float magnitude;
    public float moveSpeed = 0.01f;
    private FeleSpilling _fele;
    private (Transform, Transform, Transform, Transform) _eyes;

    

    public GameObject ripple;
    // Start is called before the first frame update
    void Start()
    {
        _fele = GetComponent<FeleSpilling>();
        _birthtime = Time.time;
        _renderer = GetComponent<SpriteRenderer>();
        _eyes = (transform.Find("SmallLeft"), transform.Find("SmallRight"), transform.Find("BigLeft"), transform.Find("BigRight"));
        _player = SceneManager.GetActiveScene().GetRootGameObjects().Single(x => x.name.Equals("Player"));
        GameObject grid = SceneManager.GetActiveScene().GetRootGameObjects().Single(x => x.name.Equals("Grid"));
        _water = grid.transform.Find("Water").GetComponent<Tilemap>();
        _land = grid.transform.Find("Land").GetComponent<Tilemap>();
        transform.position = RandomValidPosition();
    }

    public bool ValidPosition(Vector3 newPosition)
    {
        float mag = Math.Abs((newPosition - _player.transform.position).magnitude);
        magnitude = mag;
        return mag < maxDistance && mag > minDistance && !OnLand(newPosition) && OverWater(newPosition);
    }

    public Vector3Int RandomValidPosition()
    {
        var bounds = _water.cellBounds;
        Vector3Int newPosition = Vector3Int.zero;
        bool valid = false;
        while (!valid)
        {
            newPosition = new Vector3Int(Random.Range(bounds.xMin, bounds.xMax), Random.Range(bounds.yMin, bounds.yMax), 0);
            valid = ValidPosition(newPosition);
        }
        return newPosition;
    }

    public bool OnLand(Vector3 pos)
    {
        Vector3Int newPos = Vector3Int.RoundToInt(pos - new Vector3(0.5f, 0.5f));
        TileBase landTile = _land.GetTile(newPos);
        return landTile != null;
    }
    public bool OverWater(Vector3 pos)
    {
        Vector3Int newPos = Vector3Int.RoundToInt(pos - new Vector3(0.5f, 0.5f));
        TileBase waterTile = _water.GetTile(newPos);
        return waterTile != null;
    }

    // Update is called once per frame
    void Update()
    { 
        _fele.SetFreq(Math.Min(magnitude, maxDistance), maxDistance);
        int oldIndex = _spriteIndex;
        _spriteIndex = (int) Math.Floor(((Time.time - _birthtime) / spawnTime)*sprites.Length);
        if (oldIndex != _spriteIndex)
        {
            _spawnRipple = true;
            if (_spriteIndex <= sprites.Length-1) 
            {
                _changeSprite = true; 
            }
        }
        withinDistance = ValidPosition(transform.position);

        if (withinDistance)
        {
            despawnTimer = Math.Max(0, despawnTimer - Time.deltaTime);
        }
        else
        {
            despawnTimer += Time.deltaTime;
            _birthtime = Time.time - spawnTime + despawnTimer;
        }
        if (despawnTimer > despawnTime)
        {
            _death = true;
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventSystem.current.GetComponent<GameController>().Die();
        }
    }

    private void FixedUpdate()
    {
        if (_death)
        {
            _death = false;
            gameObject.SetActive(false);
            _birthtime = Time.time;
            transform.position = RandomValidPosition();
            gameObject.SetActive(true);
        } else
        {
            Vector3 newPos = transform.position +
                             (_player.transform.position - transform.position).normalized * moveSpeed;
            if (ValidPosition(newPos) && Time.time > _birthtime + spawnTime + 1)
            {
                transform.position = newPos;
            }
            if (_changeSprite)
            {
                _changeSprite = false;
                _renderer.sprite = sprites[_spriteIndex];
                switch (_spriteIndex)
                {
                    case 2:
                        _eyes.Item1.gameObject.SetActive(true);
                        _eyes.Item2.gameObject.SetActive(true);
                        _eyes.Item3.gameObject.SetActive(false);
                        _eyes.Item4.gameObject.SetActive(false);
                        break;
                    case 3:
                        _eyes.Item1.gameObject.SetActive(false);
                        _eyes.Item2.gameObject.SetActive(false);
                        _eyes.Item3.gameObject.SetActive(true);
                        _eyes.Item4.gameObject.SetActive(true);
                        break;
                    default:
                        _eyes.Item1.gameObject.SetActive(false);
                        _eyes.Item2.gameObject.SetActive(false);
                        _eyes.Item3.gameObject.SetActive(false);
                        _eyes.Item4.gameObject.SetActive(false);
                        break;
                }
            }

            if (_spawnRipple)
            {
                _spawnRipple = false;
                GameObject rip = Instantiate(ripple);
                rip.transform.position = transform.position;
            }
        }
    }
}
