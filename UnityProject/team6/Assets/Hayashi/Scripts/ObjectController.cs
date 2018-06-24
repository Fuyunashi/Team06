using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    public Vector3 basePosition;
    public Vector3 baseScale;

    private Vector3 value;
    public bool isPositionMove { get; set; }
    public bool isScaleMove { get; set; }
    public bool isHitObj { get; set; }

    public Shooter shoter;

    // Use this for initialization
    void Start()
    {
        isPositionMove = false;
        isHitObj = false;
        basePosition = transform.parent.parent.position;
        baseScale = transform.parent.parent.localScale;
        shoter = null;

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == SceneName.SelectScene.ToString()) return;
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            if (shoter == null)
            {
                shoter = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooter>();
            }
        }
        else shoter = null;

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
            transform.parent.parent.position = Vector3.MoveTowards(transform.parent.parent.position, value, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.parent.parent.position, value) == 0)
            {
                DeleteOutline();
                basePosition = transform.parent.parent.position;
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
            transform.parent.parent.localScale = Vector3.MoveTowards(transform.parent.parent.localScale, value, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.parent.parent.localScale, value) == 0)
            {
                DeleteOutline();
                baseScale = transform.parent.parent.localScale;
                shoter.MovingEnd();
                isScaleMove = false;
            }
        }
    }

    public void SetOutline()
    {
        this.gameObject.AddComponent<Outline>();
        this.gameObject.GetComponent<Outline>().OutlineColor = new Color(1, 0, 0,1f);
        this.gameObject.GetComponent<Outline>().OutlineWidth = 10.0f;
    }

    public void DeleteOutline()
    {
        Destroy(this.gameObject.GetComponent<Outline>());
    }
}
