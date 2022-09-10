using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEffect : MonoBehaviour
{
    public ComputeShader waveCompute;
    public Material wave;

    public Transform target;

    public RenderTexture CurrState;
    public RenderTexture NextState; 
    public RenderTexture LastState;
    public Vector2Int resolution;


    // Start is called before the first frame update
    void Start()
    {
        InitTexture(ref CurrState);
        InitTexture(ref NextState);
        InitTexture(ref LastState);

        wave.mainTexture = CurrState;
    }

    void InitTexture(ref RenderTexture tex)
    {
        tex = new RenderTexture(resolution.x, resolution.y, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SNorm);
        tex.enableRandomWrite = true;
        tex.Create();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Graphics.CopyTexture(CurrState, LastState);
        Graphics.CopyTexture(NextState, CurrState);

        waveCompute.SetTexture(0, "CurrState", CurrState);
        waveCompute.SetTexture(0, "LastState", LastState);
        waveCompute.SetTexture(0, "NextState", NextState);
        Vector3 adjPos = new Vector3(target.position.x + 100, target.position.y + 100, 1) * 2f;

        waveCompute.SetVector("waveEffect", adjPos);
        waveCompute.SetVector("resolution", new Vector2(resolution.x, resolution.y));

        waveCompute.Dispatch(0, resolution.x/8, resolution.y/8, 1);
    }
}
