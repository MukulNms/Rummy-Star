using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheel : MonoBehaviour
{
    [SerializeField] public float customAngle;
    [SerializeField] private int wheelTime;
    [SerializeField] private int currentWheelNo;
    [SerializeField] private int nextWheelNo;
    [SerializeField] private int noOfRounds;
    [SerializeField] private int currentImageIndex;
    [SerializeField] private int lastImageIndex;
    [HideInInspector] public bool isStarted;


    [SerializeField] private GameObject _fortuneWheel;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject[] _awardImages;
    public iTween.EaseType easetype;
    public static SpinWheel instane;
    public Action onSpinComplete;
    public float spaceBetweenTwoImages;
    public bool isSpinning;
    public Transform centerObj;
    [SerializeField] private Vector3 _centerPos;

    private void Awake()
    {
        instane = this;
    }
    private void Start()
    {
        _centerPos = centerObj.position;
        _fortuneWheel = gameObject;
        lastImageIndex = 0;
        currentImageIndex = 0;
    }
    //int[] angles = { 0, 36, 72, 108, -216, -180, -144, -108, -72, -36 };
    int[] angles = { -20, 16, 52, 88, -196, -160, -124, -88, -52, -16 };
    public void SpinTheWheel(int wheelNo, string xfactor) => StartCoroutine(Spin(wheelNo, xfactor));
     IEnumerator Spin(int wheelNo, string imageXfactor)
    {
        nextWheelNo = wheelNo;
        if (currentWheelNo == nextWheelNo)
        {
            customAngle = 0;
        }
        else if (currentWheelNo > nextWheelNo)
        {
            customAngle = Mathf.Abs(currentWheelNo - nextWheelNo) / 10f;
        }
        else
        {
            customAngle = Mathf.Abs(10 - (nextWheelNo - currentWheelNo)) / 10f;

        }

        if (!isStarted)
        {
            RearangeAwardImages(imageXfactor);
            isStarted = true;
            customAngle = noOfRounds + customAngle;
            SoundManager.instance?.PlayClip("spinwheel");
            iTween.RotateBy(_fortuneWheel, iTween.Hash("z", -customAngle, "time", wheelTime, "easetype", easetype));
            iTween.MoveTo(_awardImages[currentImageIndex], iTween.Hash("position", _centerPos, "time", wheelTime, "easetype", easetype));
            isSpinning = true;
            yield return new WaitForSeconds(wheelTime);
            _fortuneWheel.transform.eulerAngles = new Vector3(0, 0, angles[wheelNo]);
            isSpinning = false;
            SoundManager.instance?.PlayClip("spinwheelend");
            lastImageIndex = currentImageIndex;
            customAngle = noOfRounds - customAngle;
            currentWheelNo = nextWheelNo;
        }
        for (int i = 0; i < _awardImages.Length; i++)
        {
            if (i == currentImageIndex) continue;
            _awardImages[i].transform.parent = _content.transform;
        }
        isStarted = false;
        if (onSpinComplete != null)
            onSpinComplete();
    }
    public void SetWheelInitialAngle(int wheelNo, string xfactor)
    {
        print("set initialangle ");
        _fortuneWheel.transform.eulerAngles = new Vector3(0, 0, angles[wheelNo]);
        currentWheelNo = wheelNo;
        lastImageIndex = 0;
        if (xfactor == "4x")
        {
            lastImageIndex = _awardImages.Length - 1;
        }
        else if (xfactor == "2x")
        {
            lastImageIndex = _awardImages.Length - 2;
        }
        for (int i = 0; i < _awardImages.Length; i++)
        {
            _awardImages[i].transform.parent = _content.transform;
            _awardImages[i].transform.localPosition = new Vector3(200, _awardImages[i].transform.localPosition.y, 0);
        }
        _awardImages[lastImageIndex].transform.localPosition = new Vector3(0, _awardImages[currentImageIndex].transform.localPosition.y, 0);

    }
    private void RearangeAwardImages(string img_xFactor)
    {
        print("rearange xfactor is " + img_xFactor);
        if (img_xFactor == "1x")
        {
            while (lastImageIndex == currentImageIndex)
            {
                currentImageIndex = UnityEngine.Random.Range(0, _awardImages.Length - 4);
            }
        }
        else if (img_xFactor == "2x")
        {
            currentImageIndex = lastImageIndex == _awardImages.Length - 4 ? _awardImages.Length - 2 : _awardImages.Length - 4;
        }
        else if (img_xFactor == "4x")
        {
            currentImageIndex = lastImageIndex == _awardImages.Length - 3 ? _awardImages.Length - 1 : _awardImages.Length - 3;
        }
        _awardImages[currentImageIndex].transform.SetAsLastSibling();
        _awardImages[lastImageIndex].transform.SetAsFirstSibling();
        float finalXCoordinate = (_awardImages[currentImageIndex].GetComponent<RectTransform>().rect.width * (_awardImages.Length - 1) + spaceBetweenTwoImages * (_awardImages.Length - 1));
        _awardImages[currentImageIndex].transform.localPosition = new Vector2(-finalXCoordinate, _awardImages[currentImageIndex].transform.localPosition.y);

        int index = 0;
        foreach (Transform image in _content.transform)
        {

            float xCoordinate = 0;
            if (index == 0)
            {
                xCoordinate = 0f;
            }
            else xCoordinate = (100 * index + spaceBetweenTwoImages * index);
            image.transform.localPosition = new Vector2(-xCoordinate, image.transform.localPosition.y);
            index++;
        }
        for (int i = 0; i < _awardImages.Length; i++)
        {
            if (i == currentImageIndex) continue;
            _awardImages[i].transform.parent = _awardImages[currentImageIndex].transform;
        }
    }

    public void ForceFullyStopWheel()
    {
        if (isSpinning)
        {
            iTween.Stop(_fortuneWheel);
            iTween.Stop(_awardImages[currentImageIndex],true);
            _fortuneWheel.transform.eulerAngles = new Vector3(0, 0, 0);
            lastImageIndex = currentImageIndex;

            for (int i = 0; i < _awardImages.Length; i++)
            {
                _awardImages[i].transform.parent = _content.transform;
                _awardImages[i].transform.localPosition = new Vector3(200, _awardImages[i].transform.localPosition.y, 0);
            }
                _awardImages[currentImageIndex].transform.localPosition = new Vector3(0, _awardImages[currentImageIndex].transform.localPosition.y, 0);
            isSpinning = false;
            isStarted = false;
        }
    }
}
