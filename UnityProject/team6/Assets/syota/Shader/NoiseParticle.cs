using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseParticle : MonoBehaviour
{
    enum PerformanceMode
    {
        Title,
        Gun,
    }

    PerformanceMode performanceMode;

    Shader m_shader;
    Material m_mat;
    [Range(0, 1)]
    public float horizonValue;
    [SerializeField]
    GameObject title;
    [SerializeField]
    GameObject gun_model;
    [SerializeField]
    GameObject ligth;

    float light_time;
    float Title_time;
    bool title_frag;
    bool gun_frag;

    private void Start()
    {
        title_frag = true;
        gun_frag = true;
        Title_time = 0;
        light_time = 0;
        horizonValue = 0;
        LeanTween.alpha(title, 0.0f, 2);
        LeanTween.alpha(gun_model, 1.0f, 2);
    }
    private void Update()
    {
        if (light_time <= 1.0f)
        {
            light_time += Time.deltaTime;
        }
        ligth.GetComponent<Light>().intensity = light_time;
        if (Title_time < 120) { Title_time++; return; }


        switch (performanceMode)
        {
            case PerformanceMode.Title:
                if (title_frag) LeanTween.alpha(title, 0.0f, 2); title_frag = false;
                horizonValue += Time.deltaTime * 2;
                if (horizonValue >= 0.5f)
                {
                    performanceMode = PerformanceMode.Gun;
                }
                break;
            case PerformanceMode.Gun:
                if (gun_frag) LeanTween.alpha(gun_model, 0.0f, 1); gun_frag = false;
                if (horizonValue >= 0)
                    horizonValue -= 0.1f;
                else horizonValue = 0;
                break;
        }




    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (m_mat == null)
        {
            m_shader = Shader.Find("Unlit/NoiseParticle");
            Debug.Log("動いてる" + m_shader);
            m_mat = new Material(m_shader);
            m_mat.hideFlags = HideFlags.DontSave;
        }

        //ランダム値を動かすことで乱数を動かす
        m_mat.SetInt("_Seed", Time.frameCount);
        //左右にずらす値をセット
        m_mat.SetFloat("_HorizonValue", horizonValue);
        Graphics.Blit(src, dest, m_mat);
    }
}
