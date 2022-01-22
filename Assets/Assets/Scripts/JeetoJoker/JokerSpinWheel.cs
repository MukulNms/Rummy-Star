using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JokerSpinWheel : MonoBehaviour
{
    [SerializeField] private int currentImageIndex;
    [SerializeField] private int lastImageIndex;

    public AnimationCurve animationCurve;

    [SerializeField] private GameObject outerWheel;
    [SerializeField] private GameObject innerWheel;

    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject[] _awardImages;
    public static JokerSpinWheel instane;
    public Action onSpinComplete;
    public float spaceBetweenTwoImages = 100;
    bool isSpinning;
    public Transform centerObj;

    private void Awake()
    {
        instane = this;
    }

    public float maxValue;
    public float minValue;
    private void Start()
    {
        outerWheel = gameObject;
        innerWheel = gameObject;
        lastImageIndex = 0;
        currentImageIndex = 0;

    }
    int[] angles = { 0, 36, 72, 108, -216, -180, -144, -108, -72, -36 };

    //public void Spin(int wheelNo, string imageXfactor)
    public void Spin(int wheelNo)
    {
        if (!isSpinning)
        {
            desireNo = wheelNo;
            isSpinning = true;
            //  RearangeAwardImages(imageXfactor);
            CalculateAngle();
            SoundManager.instance?.PlayClip("spinwheel");
            initailDistance = centerObj.transform.position.x;
            //initalposX = _awardImages[currentImageIndex].transform.position.x;
        }

    }
    float temp = 0f;
    void Update()
    {
        if (isSpinning)
            if (anglesUntillNow < totalAngles)
            {
                float angle = speed * t;


                Angle = angle;
                anglesUntillNow += Math.Abs(angle);

                float mapTime = animationCurve.Evaluate(
                    NumberMapping(anglesUntillNow, 0, totalAngles,
                        rateOfChangeOfSpeed, initialTime));

                float distanceLeft = NumberMapping(anglesUntillNow, 0, totalAngles,
                        initalposX, initailDistance);
                remainDistance = distanceLeft;
                m = mapTime;
                t = mapTime;
                //move images
                _awardImages[currentImageIndex].transform.position = new Vector2(distanceLeft,
                     _awardImages[currentImageIndex].transform.position.y);
                //rotate wheel
                outerWheel.transform.Rotate(0f, 0f, -angle);
               innerWheel.transform.Rotate(0f, 0f, angle);
                temp += Math.Abs(angle);
                if (temp > 360)
                {
                    currentRound++;
                    temp = 0;
                }
            }
            else
            {
                currentNo = desireNo;
                AfterSpin(currentNo);
            }
    }

    void AfterSpin(int wheelNo)
    {
       outerWheel.transform.eulerAngles = new Vector3(0, 0, angles[wheelNo]);
       innerWheel.transform.eulerAngles = new Vector3(0, 0, angles[wheelNo]);
        isSpinning = false;
        SoundManager.instance?.PlayClip("spinwheelend");
        lastImageIndex = currentImageIndex;
        RemoveParents();
        if (onSpinComplete != null)
            onSpinComplete();
    }

    private void RemoveParents()
    {
        for (int i = 0; i < _awardImages.Length; i++)
        {
            if (i == currentImageIndex) continue;
            _awardImages[i].transform.parent = _content.transform;
        }
    }



    public int desireNo = 5;
    public float speed;

    public float rateOfChangeOfSpeed = 0.01f;
    public int maxRounds = 5;
    public float singleAngle = 360 / 10;
    public float totalAngles;
    public float anglesUntillNow = 0f;
    public float initialTime = 1;
    public float t = 2f;
    public float m = 2f;
    public int currentNo = 0;
    public int currentRound = 0;
    public float offset = .10f;
    public float Angle;
    public float initailDistance;
    public float initalposX;
    public float remainDistance;
    public float moveTime;
    public bool useDeltaTime;
    private void CalculateAngle()
    {
        anglesUntillNow = 0f;
        currentRound = 1;
        t = initialTime;
        if (currentNo == desireNo)
        {
            totalAngles = maxRounds * 360;
        }
        else if (currentNo < desireNo)
        {
            totalAngles = (10 - Math.Abs(currentNo - desireNo)) * singleAngle + maxRounds * 360;
        }
        else
        {
            totalAngles = Math.Abs(currentNo - desireNo) * singleAngle + maxRounds * 360;

        }
    }



    float NumberMapping(float num, float in_min, float in_max, float out_min, float out_max)
    {
        return (num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    public void SetWheelInitialAngle(int wheelNo, string xfactor)
    {
        print("set initialangle ");
       outerWheel.transform.eulerAngles = new Vector3(0, 0, angles[wheelNo]);
        innerWheel.transform.eulerAngles = new Vector3(0, 0, angles[wheelNo]);
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
        currentNo = wheelNo;
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
            iTween.Stop(outerWheel);
            iTween.Stop(innerWheel);
            iTween.Stop(_awardImages[currentImageIndex], true);
            outerWheel.transform.eulerAngles = new Vector3(0, 0, 0);
            innerWheel.transform.eulerAngles = new Vector3(0, 0, 0);
            lastImageIndex = currentImageIndex;

            for (int i = 0; i < _awardImages.Length; i++)
            {
                _awardImages[i].transform.parent = _content.transform;
                _awardImages[i].transform.localPosition = new Vector3(200, _awardImages[i].transform.localPosition.y, 0);
            }
            _awardImages[currentImageIndex].transform.localPosition = new Vector3(0, _awardImages[currentImageIndex].transform.localPosition.y, 0);
            isSpinning = false;
        }
    }

}