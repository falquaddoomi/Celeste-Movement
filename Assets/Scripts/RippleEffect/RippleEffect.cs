﻿using UnityEngine;
using System.Collections;

public class RippleEffect : MonoBehaviour
{
    public AnimationCurve waveform = new AnimationCurve(
        new Keyframe(0.00f, 0.50f, 0, 0),
        new Keyframe(0.05f, 1.00f, 0, 0),
        new Keyframe(0.15f, 0.10f, 0, 0),
        new Keyframe(0.25f, 0.80f, 0, 0),
        new Keyframe(0.35f, 0.30f, 0, 0),
        new Keyframe(0.45f, 0.60f, 0, 0),
        new Keyframe(0.55f, 0.40f, 0, 0),
        new Keyframe(0.65f, 0.55f, 0, 0),
        new Keyframe(0.75f, 0.46f, 0, 0),
        new Keyframe(0.85f, 0.52f, 0, 0),
        new Keyframe(0.99f, 0.50f, 0, 0)
    );

    [Range(0.01f, 1.0f)]
    public float refractionStrength = 0.5f;

    public Color reflectionColor = Color.gray;

    [Range(0.01f, 1.0f)]
    public float reflectionStrength = 0.7f;

    [Range(1.0f, 5.0f)]
    public float waveSpeed = 1.25f;

    [Range(0.0f, 2.0f)]
    public float dropInterval = 0.5f;

    [SerializeField, HideInInspector]
    Shader shader;

    class Droplet
    {
        Vector2 position;
        float time;

        public Droplet()
        {
            time = 1000;
        }

        public void Reset(Vector2 pos)
        {
			position = pos;
            time = 0;
        }

        public void Update()
        {
            time += Time.deltaTime * 2;
        }

        public Vector4 MakeShaderParameter(float aspect)
        {
            return new Vector4(position.x * aspect, position.y, time, 0);
        }
    }

    Droplet[] droplets;
    Texture2D gradTexture;
    Material material;
    float timer;
    int dropCount;
    private Camera c;
    private static readonly int GradTex = Shader.PropertyToID("_GradTex");
    private static readonly int Drop1 = Shader.PropertyToID("_Drop1");
    private static readonly int Drop2 = Shader.PropertyToID("_Drop2");
    private static readonly int Drop3 = Shader.PropertyToID("_Drop3");
    private static readonly int Reflection = Shader.PropertyToID("_Reflection");
    private static readonly int Params1 = Shader.PropertyToID("_Params1");
    private static readonly int Params2 = Shader.PropertyToID("_Params2");

    void UpdateShaderParameters()
    {
        material.SetVector(Drop1, droplets[0].MakeShaderParameter(c.aspect));
        material.SetVector(Drop2, droplets[1].MakeShaderParameter(c.aspect));
        material.SetVector(Drop3, droplets[2].MakeShaderParameter(c.aspect));

        material.SetColor(Reflection, reflectionColor);
        material.SetVector(Params1, new Vector4(c.aspect, 1, 1 / waveSpeed, 0));
        material.SetVector(Params2, new Vector4(1, 1 / c.aspect, refractionStrength, reflectionStrength));
    }

    void Awake() {
        c = GetComponent<Camera>();
        
        droplets = new Droplet[3];
        droplets[0] = new Droplet();
        droplets[1] = new Droplet();
        droplets[2] = new Droplet();

        gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false) {
            wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear
        };
        for (var i = 0; i < gradTexture.width; i++)
        {
            var x = 1.0f / gradTexture.width * i;
            var a = waveform.Evaluate(x);
            gradTexture.SetPixel(i, 0, new Color(a, a, a, a));
        }
        gradTexture.Apply();

        material = new Material(shader) {hideFlags = HideFlags.DontSave};
        material.SetTexture(GradTex, gradTexture);

        UpdateShaderParameters();
    }

    void Update()
    {
        if (dropInterval > 0)
        {
            timer += Time.deltaTime;
            while (timer > dropInterval)
            {
                //Emit();
                timer -= dropInterval;
            }
        }

        foreach (var d in droplets) d.Update();

        UpdateShaderParameters();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

    public void Emit(Vector2 pos)
    {
        droplets[dropCount++ % droplets.Length].Reset(pos);
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(.3f);

    }

}
