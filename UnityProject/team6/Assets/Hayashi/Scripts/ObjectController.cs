using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    private Material defaultMat;

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
        defaultMat = GetComponent<Renderer>().material;
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
                ResetMaterial();
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
            if (Vector3.Distance(transform.parent.transform.parent.localScale, value) == 0)
            {
                ResetMaterial();
                baseScale = transform.parent.localScale;
                shoter.MovingEnd();
                isPositionMove = false;
            }
        }
    }

    public void ChangeMaterial(Material mat)
    {
        this.GetComponent<Renderer>().material = mat;
    }

    public void ResetMaterial()
    {
        this.GetComponent<Renderer>().material = defaultMat;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "moveObject" && collision.gameObject.tag!="Player")
        {
            isHitObj = true;
            isPositionMove = false;
            isScaleMove = false;
            LeanTween.alpha(gameObject, 0.0f, 0.5f).setOnComplete(() =>
              {
                  transform.parent.transform.parent.position = basePosition;
                  transform.parent.localScale = baseScale;
                  ResetMaterial();
                  shoter.MovingEnd();
                  LeanTween.alpha(gameObject, 1.0f, 0.5f).setOnComplete(()=> { isHitObj = false; });

              });
        }
    }


}
