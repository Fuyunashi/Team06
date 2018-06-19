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
    GameObject[] gun_model;
    [SerializeField]
    GameObject[] ligth;

    //タイトルコントロールのスクリプト
    TitleCameraMove titleControll;

    float light_time;
    float Title_time;
    float Count;
    bool title_frag;
    bool gun_frag;

    public bool skipFrag;

    private void Start()
    {
        titleControll = GameObject.Find("TitleMainCamera").GetComponent<TitleCameraMove>();
        title_frag = true;
        gun_frag = true;
        Title_time = 0;
        light_time = 0;
        horizonValue = 0;
        Count = 0;
        performanceMode = PerformanceMode.Title;

    }
    private void Update()
    {

        if (skipFrag)
        {
            performanceMode = PerformanceMode.Gun;
            horizonValue = 0;
            LeanTween.alpha(gun_model[0], 1.0f, 0);
            LeanTween.alpha(gun_model[1], 1.0f, 0);
            LeanTween.alpha(title, 1.0f, 0);
            gun_model[0].transform.Rotate(0, 0, 20f * Time.deltaTime);

            return;
        }
        if (light_time <= 1.0f && Title_time > 50)
        {
            light_time += Time.deltaTime;
        }
        ligth[0].GetComponent<Light>().intensity = light_time;
        ligth[1].GetComponent<Light>().intensity = light_time;
        if (Title_time < 200) { Title_time++; return; }

        switch (performanceMode)
        {
            case PerformanceMode.Title:
                if (title_frag) LeanTween.alpha(title, 0.0f, 2).setOnComplete(() => { performanceMode = PerformanceMode.Gun; Count = 0; }); title_frag = false;
                horizonValue = Mathf.Lerp(0, 0.6f, Count / 120);
                Count++;
                break;
            case PerformanceMode.Gun:
                if (gun_frag)
                {
                    LeanTween.alpha(title, 1.0f, 1);
                    LeanTween.alpha(gun_model[0], 1.0f, 1);
                    LeanTween.alpha(gun_model[1], 1.0f, 1); gun_frag = false;
                }
                horizonValue = Mathf.Lerp(0.6f, 0.0f, Count / 120);
                Count++;
                if (Count >= 120)
                {
                    skipFrag = true;
                    horizonValue = 0;
                }
                if (Count >= 130)
                    gun_model[0].transform.Rotate(0, 0, 20f * Time.deltaTime);

                break;
        }
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (m_mat == null)
        {
            m_shader = Shader.Find("Unlit/NoiseParticle");
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
