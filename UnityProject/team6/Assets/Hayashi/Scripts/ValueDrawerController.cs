using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//数字ポップアップ
public class ValueDrawerController : MonoBehaviour
{
    private Text m_Text;
    private GameObject target;
    private Quaternion targetRotate;
    private GameObject drawObj;

    [SerializeField]
    private GameObject numDrawStartPos;
    private GameObject m_numDrawStartPos;

    [SerializeField]
    float width = 0.2f;
    [SerializeField]
    private GameObject[] numbers;

    private GameObject[] numberObjects;
    private Dictionary<char, GameObject> dic_Objects;

    private Quaternion defaultRotation;

    private string str_value;
    private float m_value;
    private float prevValue = 0.0f;
    private float currentValue = 0.0f;
    private bool isTween;

    private void Awake()
    {
        dic_Objects = new Dictionary<char, GameObject>()
        {
            {'0',numbers[0] },
            {'1',numbers[1] },
            {'2',numbers[2] },
            {'3',numbers[3] },
            {'4',numbers[4] },
            {'5',numbers[5] },
            {'6',numbers[6] },
            {'7',numbers[7] },
            {'8',numbers[8] },
            {'9',numbers[9] },
            {'.',numbers[10] },
            {'-',numbers[11] },
        };
        defaultRotation = transform.localRotation;
    }


    // Use this for initialization
    void Start()
    {
        //m_Text = transform.Find("Canvas").Find("ValueText").GetComponent<Text>();
        target = GameObject.FindGameObjectWithTag("Player");
        isTween = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentValue != prevValue)
        {
            if (m_numDrawStartPos != null)
                Destroy(m_numDrawStartPos);
            m_numDrawStartPos = Instantiate(numDrawStartPos, transform.position + new Vector3(0.5f, 0.0f, 0.0f), Quaternion.identity);
            if (numberObjects != null)
            {
                for (int i = 0; i < numberObjects.Length; i++)
                {
                    Destroy(numberObjects[i]);
                }
            }
            numberObjects = new GameObject[m_value.ToString("f2").Length];
            transform.localRotation = defaultRotation;
            for (var i = 0; i < numberObjects.Length; ++i)
            {
                GameObject numberObj = Instantiate(
                    dic_Objects[str_value[i]],
                    m_numDrawStartPos.transform.position + new Vector3((float)i * width, 0, 0),
                    dic_Objects[str_value[i]].transform.localRotation)
                    as GameObject;
                numberObj.transform.SetParent(m_numDrawStartPos.transform);
                numberObjects[i] = numberObj;

            }
            m_numDrawStartPos.transform.localEulerAngles = new Vector3(0, 180, 0);
            m_numDrawStartPos.transform.SetParent(this.transform);

            prevValue = currentValue;
        }
        if (target != null)
        {
            targetRotate = Quaternion.LookRotation(target.transform.position - transform.localPosition);
            transform.localRotation = Quaternion.Slerp(transform.rotation, targetRotate, 1);
        }

        if (isTween == false)
            transform.position = new Vector3(drawObj.transform.parent.position.x, drawObj.transform.parent.position.y, drawObj.transform.parent.position.z)+
                                 new Vector3(0,drawObj.transform.localScale.y/2+0.5f,0);

    }

    public void GetDrawBaseObj(GameObject m_obj)
    {
        drawObj = m_obj;
    }

    public void ObjectAxisValue(float value)
    {
        str_value = value.ToString("f2");
        m_value = value;
        currentValue = value;
        //m_Text.text = value.ToString("f2");
    }

    public void IsTween(bool tween)
    {
        isTween = tween;
    }
}
