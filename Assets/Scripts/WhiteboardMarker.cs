using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WhiteboardMarker : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private int penSize = 5;

    private Renderer _renderer;
    private Color[] _colors;
    public float _tipHeight;

    private RaycastHit _touch;
    private WhiteBoard _whiteboard;
    private Vector2 _touchPos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;

    public GameObject tipColour;

    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, penSize * penSize).ToArray();
        //_tipHeight = _tip.localScale.y;
    }

    
    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        if (Physics.Raycast(_tip.position,transform.up, out _touch, _tipHeight))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if(_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<WhiteBoard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (penSize / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (penSize / 2));

                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) 
                    return;

                if (_touchedLastFrame)
                {
                    _whiteboard.texture.SetPixels(x, y, penSize, penSize, _colors);

                    for(float f = 0.01f ; f < 1.00f; f += 0.07f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard.texture.SetPixels(lerpX, lerpY, penSize, penSize, _colors);
                    }

                    transform.rotation = _lastTouchRot;
                    
                    Highlight();

                    _whiteboard.texture.Apply();

                    
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }
        _whiteboard = null;
        _touchedLastFrame = false;
    }

    void Highlight()
    {
        
    }
}
