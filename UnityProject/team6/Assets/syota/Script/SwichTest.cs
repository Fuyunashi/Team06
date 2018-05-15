using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AxisOfRotation { x, y, z, }
enum RotateDire { Right, Left, }

public class SwichTest : MonoBehaviour
{
    //オブジェクトそれぞれに座標を指定
    [System.Serializable]
    private struct ObjectANDAxis
    {
        [SerializeField, Header("回転軸")]
        public AxisOfRotation m_axis;
        [SerializeField, Header("回転方向")]
        public RotateDire m_rote_dir;
        [SerializeField]
        public GameObject m_rotate_obj;
    }

    [SerializeField, Header("オブジェクトの数")]
    private ObjectANDAxis[] targets_;

    //回転角度の変数
    private float m_rote_angle;
    //回転フラグ
    private bool m_rote_flag;

    void Start()
    {
        m_rote_angle = .0f;
        m_rote_flag = false;
    }
    private void Update()
    {
        //フラグがONでなければ元の値に戻す
        if (!m_rote_flag)
        {
            if (m_rote_angle >= .0f)
                m_rote_angle -= 0.5f;
        }
        ObjectRotato();
    }

    void OnTriggerStay(Collider other)
    {        
        if (other.gameObject.CompareTag("SwichOn") || other.gameObject.CompareTag("Player"))
        {
            //接触していれば回転する
            if (m_rote_angle <= 90.0f)
                m_rote_angle += 0.8f;
            //フラグをONにする
            m_rote_flag = true;

            Debug.Log("hit");
        }
        else
        {
            if (m_rote_angle >= .0f)
                m_rote_angle -= 0.5f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SwichOn") || other.gameObject.CompareTag("Player"))
        {
            m_rote_flag = false;
        }
    }

    void ObjectRotato()
    {
        foreach (var obj in targets_)
        {
            if (obj.m_rote_dir == RotateDire.Left)
                m_rote_angle *= -1;
            switch (obj.m_axis)
            {
                case AxisOfRotation.x:
                    obj.m_rotate_obj.transform.rotation = Quaternion.Euler(m_rote_angle, .0f, .0f);
                    break;
                case AxisOfRotation.y:
                    obj.m_rotate_obj.transform.rotation = Quaternion.Euler(.0f, m_rote_angle, .0f);
                    break;
                case AxisOfRotation.z:
                    obj.m_rotate_obj.transform.rotation = Quaternion.Euler(.0f, .0f, m_rote_angle);
                    break;
                default:
                    return;
            }
        }
    }
}
