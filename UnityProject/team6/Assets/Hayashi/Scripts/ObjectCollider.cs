using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectCollider : MonoBehaviour {

    private GameObject crashEffectPref;
    private GameObject m_crashEffect;

    private GameObject destroyEffectPref;
    private GameObject m_destroyEffect;

    private ObjectController m_objCont;

    // Use this for initialization
    void Start () {
        crashEffectPref = Resources.Load("Particles/Particles Systems/BlockCollEff") as GameObject;
        destroyEffectPref = Resources.Load("Particles/Particles Systems/BleakEff") as GameObject;

        m_objCont = this.gameObject.GetComponent<ObjectController>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (SceneManager.GetActiveScene().name == SceneName.SelectScene.ToString()) return;
        Vector3 hitPos = Vector3.zero;
        foreach (ContactPoint point in collision.contacts)
        {
            hitPos = point.point;
        }
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "GravityObj")
        {
            m_destroyEffect = Instantiate(destroyEffectPref, transform.position, Quaternion.identity);
            Destroy(m_destroyEffect, 1.0f);
            SoundManager.GetInstance.PlaySE("Break_SE");
            m_objCont.isHitObj = true;
            m_objCont.isPositionMove = false;
            m_objCont.isScaleMove = false;
            m_objCont.DeleteOutline();
            LeanTween.alpha(gameObject, 0.0f, 1.0f).setOnComplete(() =>
            {
                m_objCont.shoter.MovingEnd();
                transform.parent.parent.position = m_objCont.basePosition;
                transform.parent.parent.localScale = m_objCont.baseScale;

                LeanTween.alpha(gameObject, 1.0f, 2.0f).setOnComplete(() => { m_objCont.isHitObj = false; });
                SoundManager.GetInstance.PlaySE("Born_SE");
            });
        }
        else if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "GravityObj" && collision.gameObject.tag != "ChangeObject")
        {
            m_crashEffect = Instantiate(crashEffectPref, hitPos, Quaternion.identity);
            Destroy(m_crashEffect, 1.0f);
            SoundManager.GetInstance.PlaySE("Crash_SE");
        }
    }
}
