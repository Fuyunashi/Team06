using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField]
    private GameObject crashEffectPref;
    private GameObject m_crashEffect;
    [SerializeField]
    private GameObject destroyEffectPref;
    private GameObject m_destroyEffect;
    [SerializeField]
    private GameObject getSetEffectPref;
    private GameObject m_getSetEffect;

    private Vector3 basePosition;
    private Vector3 baseScale;

    private Vector3 value;
    private bool isPositionMove;
    private bool isScaleMove;
    private bool isHitObj;

    private Shooter shoter;

    // Use this for initialization
    void Start()
    {
        isPositionMove = false;
        isHitObj = false;
        basePosition = transform.parent.transform.parent.position;
        baseScale = transform.parent.localScale;

        shoter = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPositionMove == true)
        {
            PositionMove();
        }
        if (isScaleMove == true)
        {
            ScaleMove();
        }
    }

    public void PositionShiftStart(Vector3 originValue)
    {
        value = originValue;
        isPositionMove = true;
    }

    private void PositionMove()
    {
        if (isHitObj == false)
        {
            transform.parent.transform.parent.position = Vector3.MoveTowards(transform.parent.transform.parent.position, value, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.parent.transform.parent.position, value) == 0)
            {
                DeleteOutline();
                basePosition = transform.parent.transform.parent.position;
                shoter.MovingEnd();
                isPositionMove = false;
            }
        }
    }

    public void ScaleShiftStart(Vector3 originValue)
    {
        value = originValue;
        isScaleMove = true;
    }

    private void ScaleMove()
    {
        if (isHitObj == false)
        {
            transform.parent.localScale = Vector3.MoveTowards(transform.parent.localScale, value, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.parent.localScale, value) == 0)
            {
                DeleteOutline();
                baseScale = transform.parent.localScale;
                shoter.MovingEnd();
                isScaleMove = false;
            }
        }
    }

    public void SetOutline()
    {
        this.gameObject.AddComponent<Outline>();
        this.gameObject.GetComponent<Outline>().OutlineColor = new Color(255, 0, 0);
        this.gameObject.GetComponent<Outline>().OutlineWidth = 10.0f;
    }

    public void DeleteOutline()
    {
        Destroy(this.gameObject.GetComponent<Outline>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPos=Vector3.zero;
        foreach(ContactPoint point in collision.contacts)
        {
            hitPos = point.point;
        }
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "GravityObj" && collision.gameObject.tag != "Bullet")
        {
            m_destroyEffect = Instantiate(destroyEffectPref, hitPos,Quaternion.identity);
            Destroy(m_destroyEffect, 1.0f);
            SoundManager.GetInstance.PlaySE("Break_SE");
            isHitObj = true;
            isPositionMove = false;
            isScaleMove = false;
            DeleteOutline();
            LeanTween.alpha(gameObject, 0.0f, 1.0f).setOnComplete(() =>
              {
                  shoter.MovingEnd();
                  transform.parent.transform.parent.position = basePosition;
                  transform.parent.localScale = baseScale;

                  LeanTween.alpha(gameObject, 1.0f, 2.0f).setOnComplete(() => { isHitObj = false; });
                  SoundManager.GetInstance.PlaySE("Born_SE");
              });
        }
        else if(collision.gameObject.tag!="Player" && collision.gameObject.tag != "GravityObj" && collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "ChangeObject")
        {
            m_crashEffect = Instantiate(crashEffectPref, hitPos, Quaternion.identity);
            Destroy(m_crashEffect, 1.0f);
            SoundManager.GetInstance.PlaySE("Crash_SE");
        }
    }

    public void GetSetEffect()
    {
        m_getSetEffect = Instantiate(getSetEffectPref, this.transform.position, Quaternion.identity);
        Destroy(m_getSetEffect, 1.0f);
    }

}
