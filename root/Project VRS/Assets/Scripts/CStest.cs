using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStest : MonoBehaviour
{
    public ComputeShader computeShader;
    public ComputeBuffer buffer;

    public RenderTexture RenderTexture;

    private void Start()
    {
        RenderTexture = new RenderTexture(256, 256, 24);
        RenderTexture.enableRandomWrite = true;
        RenderTexture.Create();

        computeShader.SetTexture(0, "Result", RenderTexture);
        computeShader.Dispatch(0,RenderTexture.width /8, RenderTexture.height /8, 1);
        //buffer.GetData()
    }
}
