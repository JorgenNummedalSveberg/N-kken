using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Flicker : MonoBehaviour
{
    public float interval = 2f;
    public float maxIntensity = 1f;
    public float minIntensity = 0.5f;
    private bool _up;
    private float _counter;

    private Light2D _light2D;
    // Start is called before the first frame update
    void Start()
    {
        _light2D = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _counter += Time.deltaTime;
        float intensity = (_counter / interval)*(maxIntensity-minIntensity);
        _light2D.intensity = _up ? minIntensity + intensity : maxIntensity - intensity;
        if (_counter > interval)
        {
            _up = !_up;
            _counter = 0;
        }
    }
}
