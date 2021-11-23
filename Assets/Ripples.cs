using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripples : MonoBehaviour
{
    public Sprite[] sprites;

    public float lifetime = 1f;
    private int _spriteIndex;
    private bool _changeSprite;
    private bool _death;
    private SpriteRenderer _renderer;
    private float _birthtime;
    // Start is called before the first frame update
    void Start()
    {
        _birthtime = Time.time;
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        int oldIndex = _spriteIndex;
        _spriteIndex = (int) Math.Floor(((Time.time - _birthtime) / lifetime)*sprites.Length);
        if (oldIndex != _spriteIndex)
        {
            _changeSprite = true;
        }

        if (_spriteIndex >= sprites.Length)
        {
            _death = true;
        }
    }

    private void FixedUpdate()
    {
        if (_death)
        {
            Destroy(gameObject);
        } else if (_changeSprite)
        {
            _changeSprite = false;
            _renderer.sprite = sprites[_spriteIndex];
        }
    }
}
