using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CentralImageController : MonoBehaviour
{
    public Transform wheel, img, img2;
    public Button spinBtn;
    public Button quitBtn;
    public int selectedItem;
    public GameObject spinManager;
    public GameObject[] images;
    public Sprite[] setImages;
    private GameObject tempGO, tempImg;
    public GameObject dropDown;
    public Dropdown dropDownMenu;
    public Image CenterPos;
    public Vector3 ImgPos;
    public int SelectedPrize;
    public SampleProjectSpinManager SampleProjectSpinManager;
    public void Start()
    {
        spinBtn.onClick.AddListener(() => OnSpin());
        float xOffset = -images.Length * 100;
        img.position = new Vector2(xOffset, img.position.y);
        ImgPos.x = -10f;
        ImgPos.y = img.position.y;
        SetInitalNumber(9, 2);
    }

    /// <summary>
    /// Application Quit
    /// </summary>
    /// <param name="change"></param>

    //Ouput the new value of the Dropdown into Text
    void DropdownValueChanged(Dropdown change)
    {
        SelectedPrize = change.value;

    }
    void SetInitalNumber(int no, int imageType)
    {
        SelectedPrize = imageType;
        spinManager.GetComponent<SampleProjectSpinManager>().SetNumber(no);

        InitImage();
    }

    public void InitImage()
    {
        if (SelectedPrize == 0) { images[0].GetComponent<Image>().sprite = setImages[0]; }
        if (SelectedPrize == 1) { images[0].GetComponent<Image>().sprite = setImages[1]; }
        if (SelectedPrize == 2) { images[0].GetComponent<Image>().sprite = setImages[2]; }
    }
    public void SetImage()
    {
        Debug.Log("Select Prize " + SelectedPrize);
        if (SelectedPrize == 0)
        {
            images[images.Length - 1].GetComponent<Image>().sprite = setImages[0];
            Debug.Log("It Works");
        }
        if (SelectedPrize == 1)
        {
            images[images.Length - 1].GetComponent<Image>().sprite = setImages[1];
            Debug.Log("It Works");
        }
        if (SelectedPrize == 2)
        {
            images[images.Length-1].GetComponent<Image>().sprite = setImages[2];
            Debug.Log("It Works");

        }
        img.transform.position= new Vector2(0,img.position.y);
    }
    void OnSpin()
    {
        Debug.Log("spin the wheel");
        SelectedPrize = 1;
        SampleProjectSpinManager.Spin(5);
    }
    public int totalRounds = 4;
    public float angle = 1;

    float NumberMapping(float num, float in_min, float in_max, float out_min, float out_max)
    {
        return (num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    public void Shuffle()
    {
        for (int i = 1; i < images.Length - 1; i++)
        {
            int rnd = Random.Range(1, images.Length);
            tempGO = images[rnd];
            images[rnd] = images[i];
            images[i] = tempGO;
            images[i].GetComponent<Transform>().SetSiblingIndex(i);
        }
    }
}
