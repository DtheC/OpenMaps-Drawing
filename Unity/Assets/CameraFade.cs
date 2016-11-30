using UnityEngine;
using System.Collections;
using Gamelogic.Extensions;
using System;

public class CameraFade : Singleton<CameraFade> {
    
    public float TimerSpeed = 60f; 
    public float FadeSpeed = 0.00001f;

    public Camera[] Cameras;
    public Material FadeMaterial;

    public bool DisableFade = false;
    [SerializeField]
    private int _currentCamera = 0;
    [SerializeField]
    private bool _fadingIn = false;
    [SerializeField]
    private bool _fadingOut = false;


    public bool FadingIn
    {
        get
        {
            return _fadingIn;
        }

        set
        {
            _fadingIn = value;
        }
    }

    public bool FadingOut
    {
        get
        {
            return _fadingOut;
        }

        set
        {
            _fadingOut = value;
        }
    }

    void Start()
    {
        foreach (Camera c in Cameras)
        {
            c.enabled = false;
        }
        Cameras[0].enabled = true;
        FadingIn = true;
    }

    void Update()
    {
        if (FadingIn)
        {
            FadingOut = false;
            FadeIn();
        }

        if (FadingOut)
        {
            FadingIn = false;
            FadeOut();
        }

        if (Input.GetKeyDown("1"))
        {
            QuickSwitch(0);
        }
        if (Input.GetKeyDown("2"))
        {
            QuickSwitch(1);
        }
        if (Input.GetKeyDown("3"))
        {
            QuickSwitch(2);
        }
        if (Input.GetKeyDown("4"))
        {
            QuickSwitch(3);
        }

        if (Input.GetKeyDown("0"))
        {
            DisableFade = !DisableFade;
        }

        if (DisableFade)
        {
            QuickSwitch(_currentCamera);
            
            StopAllCoroutines();
        }

    }

    void QuickSwitch(int cam)
    {
        StopAllCoroutines();
        Cameras[_currentCamera].enabled = false;
        _currentCamera = cam;
        if (_currentCamera >= Cameras.Length)
        {
            _currentCamera = 0;
        }
        Cameras[_currentCamera].enabled = true;

        FadingIn = false;
        FadingOut = false;
        var c = FadeMaterial.color;
        c.a = 0;
        FadeMaterial.color = c;

        StartCoroutine(waitThenCallback(TimerSpeed, () =>
        { FadingIn = true; }));
    }

    void FadeIn() //Make Opaque
    {

        if (FadeMaterial.color.a < 1.0f)
        {
            var c = FadeMaterial.color;
            c.a += FadeSpeed;
            FadeMaterial.color = c;
        }
        else
        {
            var c = FadeMaterial.color;
            c.a = 1.0f;
            FadeMaterial.color = c;
            FadingIn = false;
            SwitchCamera();
        }
        
    }

    void FadeOut() //Make transparent
    {
        if (FadeMaterial.color.a > 0)
        {
            var c = FadeMaterial.color;
            c.a -= FadeSpeed;
            FadeMaterial.color = c;
        }
        else
        {
            var c = FadeMaterial.color;
            c.a = 0;
            FadeMaterial.color = c;
            FadingOut = false;
            StartCoroutine(waitThenCallback(TimerSpeed, () =>
            { FadingIn = true; }));
        }
    }

    private void SwitchCamera()
    {
        Cameras[_currentCamera].enabled = false;
        _currentCamera++;
        if (_currentCamera >= Cameras.Length)
        {
            _currentCamera = 0;
        }
        Cameras[_currentCamera].enabled = true;

        StartCoroutine(waitThenCallback(2, () =>
        {FadingOut = true; }));
        
        //FadingOut = true;
    }
    
    private IEnumerator waitThenCallback(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

}