using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Graph : MonoBehaviour
{
    public int[] points;
    
    public Texture2D _texture;
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public RawImage rawImage;

    private int _at = 0;
    // Start is called before the first frame update
    void Start()
    {
        points = new int[0];
    }

    public void SetPoints(int[] pointList)
    {
        points = pointList;
    }
    
    

    // Update is called once per frame
    void Update()
    {
        TakeSnapshot();
    }
    
    public void TakeSnapshot()
    {
        _at += 1;
        Color fillColor = new Color(1, 0.5f, 0.5f);
        var fillColorArray =  _texture.GetPixels();
 
        for(var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = fillColor;
        }
        
        Color[] colorArray = new Color[10*10];
        for (var i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = Color.white;
        }
  
        _texture.SetPixels( fillColorArray );
        for (int i = 0; i < points.Length; i++)  {
            if (i % 2 == 1)
            {
                continue;
            }

            _texture.SetPixels((i)/2+9, (points[i])+175, 10, 10, colorArray );
        }
        _texture.Apply ();
        rawImage.texture = _texture;
        // _texture.ReadPixels (new Rect (0, 0, 1000, 1000), 0, 0);
        // for (int i = 0; i < 1000*.2f; i++) 
        // for (int j = 0; j < 1000;j++){
        //     _texture.SetPixel((_at+i),j, new Color(0,0,1) );
        // }
        // _texture.Apply ();
        
        RenderTexture.active = null;
        // gameObject.renderer.material.mainTexture = TakeSnapshot;
    }
}
