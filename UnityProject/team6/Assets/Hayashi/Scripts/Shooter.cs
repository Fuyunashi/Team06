using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shooter : MonoBehaviour
{

    enum AxisType
    {
        Xaxis,
        Yaxis,
        Zaxis
    }

    enum ChangeType
    {
        Position,
        Scale,
        Rotate
    }

    enum ShotType
    {
        Getting,
        Shifting
    }

    private AxisType axisType = AxisType.Xaxis;
    private ChangeType changeType = ChangeType.Position;
    private ShotType shotType = ShotType.Getting;

    [SerializeField]
    private GameObject bulletPrefabs;
    private GameObject bullet;
    [SerializeField]
    private Transform bulletShotPos;

    [SerializeField]
    private Text axisText;
    [SerializeField]
    private Text changeText;
    [SerializeField]
    private Text shotText;

    private GameObject originObject;
    private GameObject targetObject;
    private Vector3 originPositionValue;
    private Vector3 targetPositionValue;
    private Vector3 originScaleValue;
    private Vector3 targetScaleValue;
    private Vector3 originRotateValue;
    private Vector3 targetRotateValue;

    private bool isChanging;
    private bool isMove;
    private bool getRTriggerDown;
    private bool getLTriggerDown;

    // Use this for initialization
    void Start()
    {
        getRTriggerDown = false;
        getLTriggerDown = false;

        Initialize();

        axisText.text = "axis:" + axisType.ToString();
        changeText.text = "change:" + changeType.ToString();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("RButton"))
        {
            if (bullet == null)
                bullet = Instantiate(bulletPrefabs, bulletShotPos.position, bulletShotPos.rotation);
            else
            {
                Destroy(bullet);
                bullet = Instantiate(bulletPrefabs, bulletShotPos.position, bulletShotPos.rotation);
            }
        }


        if (bullet == null && (Input.GetMouseButtonDown(1) || Input.GetButtonDown("LButton")))
        {
            switch (shotType)
            {
                case ShotType.Getting:
                    shotType = ShotType.Shifting;
                    shotText.text = "shot:" + shotType.ToString();
                    break;
                case ShotType.Shifting:
                    shotType = ShotType.Getting;
                    shotText.text = "shot:" + shotType.ToString();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) || (Input.GetAxisRaw("LRTrigger") <= -0.85f && getRTriggerDown == false))
        {
            getRTriggerDown = true;
            switch (axisType)
            {
                case AxisType.Xaxis:
                    axisType = AxisType.Yaxis;
                    axisText.text = "axis:" + axisType.ToString();
                    break;
                case AxisType.Yaxis:
                    axisType = AxisType.Zaxis;
                    axisText.text = "axis:" + axisType.ToString();
                    break;
                case AxisType.Zaxis:
                    axisType = AxisType.Xaxis;
                    axisText.text = "axis:" + axisType.ToString();
                    break;
            }
        }
        if (getRTriggerDown == true && Input.GetAxisRaw("LRTrigger") >= -0.1f) getRTriggerDown = false;

        if (Input.GetKeyDown(KeyCode.Q) || (Input.GetAxisRaw("LRTrigger") >= 0.85f && getLTriggerDown == false))
        {
            getLTriggerDown = true;
            switch (changeType)
            {
                case ChangeType.Position:
                    changeType = ChangeType.Scale;
                    changeText.text = "change:" + changeType.ToString();
                    break;
                case ChangeType.Scale:
                    changeType = ChangeType.Rotate;
                    changeText.text = "change:" + changeType.ToString();
                    break;
                case ChangeType.Rotate:
                    changeType = ChangeType.Position;
                    changeText.text = "change:" + changeType.ToString();
                    break;
            }
        }
        if (getLTriggerDown == true && Input.GetAxisRaw("LRTrigger") < 0.1f) getLTriggerDown = false;
    }

    public void HitBullet(GameObject hitObject)
    {
        if (isMove == false)
        {
            switch (shotType)
            {
                case ShotType.Getting:
                    originObject = hitObject.transform.parent.gameObject;
                    GetOriginAxisLength();
                    break;
                case ShotType.Shifting:
                    targetObject = hitObject.transform.parent.gameObject;
                    GetTargetAxisLength();
                    if (originObject != null)
                    {
                        ChangeObjectAxis();
                    }
                    break;
            }
        }
    }

    private void GetOriginAxisLength()
    {
        originPositionValue = originObject.transform.parent.localPosition;
        originScaleValue = originObject.transform.localScale;
        originRotateValue = originObject.transform.parent.localEulerAngles;
    }

    private void GetTargetAxisLength()
    {
        switch (changeType)
        {
            case ChangeType.Position:
                targetPositionValue = targetObject.transform.parent.localPosition;
                break;
            case ChangeType.Scale:
                targetScaleValue = targetObject.transform.localScale;
                break;
            case ChangeType.Rotate:
                targetRotateValue = targetObject.transform.parent.localEulerAngles;
                break;
        }
    }

    private void ChangeObjectAxis()
    {
        isMove = true;
        switch (changeType)
        {
            case ChangeType.Position:
                switch (axisType)
                {
                    case AxisType.Xaxis:
                        LeanTween.moveX(targetObject.transform.parent.gameObject, originPositionValue.x, 0.5f);
                        LeanTween.moveX(originObject.transform.parent.gameObject, targetPositionValue.x, 0.5f).setOnComplete(Initialize);
                        break;
                    case AxisType.Yaxis:
                        LeanTween.moveY(targetObject.transform.parent.gameObject, originPositionValue.y, 0.5f);
                        LeanTween.moveY(originObject.transform.parent.gameObject, targetPositionValue.y, 0.5f).setOnComplete(Initialize);
                        break;
                    case AxisType.Zaxis:
                        LeanTween.moveZ(targetObject.transform.parent.gameObject, originPositionValue.z, 0.5f);
                        LeanTween.moveZ(originObject.transform.parent.gameObject, targetPositionValue.z, 0.5f).setOnComplete(Initialize);
                        break;
                }
                break;
            case ChangeType.Scale:
                switch (axisType)
                {
                    case AxisType.Xaxis:
                        LeanTween.scaleX(targetObject, originScaleValue.x, 0.5f);
                        LeanTween.scaleX(originObject, targetScaleValue.x, 0.5f).setOnComplete(Initialize);
                        break;
                    case AxisType.Yaxis:
                        LeanTween.scaleY(targetObject, originScaleValue.y, 0.5f);
                        LeanTween.scaleY(originObject, targetScaleValue.y, 0.5f).setOnComplete(Initialize);
                        break;
                    case AxisType.Zaxis:
                        LeanTween.scaleZ(targetObject, originScaleValue.z, 0.5f);
                        LeanTween.scaleZ(originObject, targetScaleValue.z, 0.5f).setOnComplete(Initialize);
                        break;
                }
                break;
            case ChangeType.Rotate:
                switch (axisType)
                {
                    case AxisType.Xaxis:
                        LeanTween.rotateX(targetObject.transform.parent.gameObject, originRotateValue.x, 0.5f);
                        LeanTween.rotateX(originObject.transform.parent.gameObject, targetRotateValue.x, 0.5f).setOnComplete(Initialize);
                        break;
                    case AxisType.Yaxis:
                        LeanTween.rotateY(targetObject.transform.parent.gameObject, originRotateValue.y, 0.5f);
                        LeanTween.rotateY(originObject.transform.parent.gameObject, targetRotateValue.y, 0.5f).setOnComplete(Initialize);
                        break;
                    case AxisType.Zaxis:
                        LeanTween.rotateZ(targetObject.transform.parent.gameObject, originRotateValue.z, 0.5f);
                        LeanTween.rotateZ(originObject.transform.parent.gameObject, targetRotateValue.z, 0.5f).setOnComplete(Initialize);
                        break;
                }
                break;
        }
    }

    private void Initialize()
    {
        originObject = null;
        targetObject = null;
        originPositionValue = Vector3.zero;
        targetPositionValue = Vector3.zero;
        originScaleValue = Vector3.zero;
        targetScaleValue = Vector3.zero;
        originRotateValue = Vector3.zero;
        targetRotateValue = Vector3.zero;
        shotType = ShotType.Getting;
        shotText.text = "shot:" + shotType.ToString();
        isMove = false;
    }

}
