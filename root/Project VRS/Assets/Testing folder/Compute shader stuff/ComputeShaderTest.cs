using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderTest : MonoBehaviour
{
    public ComputeShader shader;

    public RenderTexture texture;

    public GameObject testing;

    public float Shift = 0f;

    // Start is called before the first frame update
    void Start()
    {
        texture = new RenderTexture(256, 256, 24);
        texture.enableRandomWrite = true;
        texture.Create();
    }

    void DispatchShader()
    {
        shader.SetTexture(0, "Result", texture);
        shader.SetFloat("Resolution", texture.width);
        shader.SetFloat("Shifter", Shift);
        shader.Dispatch(0, texture.width / 8, texture.height / 8, 1);

        testing.GetComponent<Renderer>().material.mainTexture = texture;
    }


    private void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 200,50), "Test Compute Shader"))
        {
            DispatchShader();
        }
    }
}
