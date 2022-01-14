using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelManager : MonoBehaviour
{
    public List<SpinItem> items = new List<SpinItem>();
    //  public Transform img;
    public Transform wheel;
    public Transform handle;
    public Button startButton;
    // public ParticleSystem happyParticle;
    public AudioSource beepAudio;
    public AudioSource winAudio;

    public bool reverseWheelRotation = false;
    public bool reverseHandleRotation = false;
    public float spinSpeed = 5;
    public float minSpinSpeed = 2f;
    public int spinRounds = 7;

    public GameObject autoGenerateParent;
    public Sprite circleSprite;
    public Sprite pinSprite;
    public bool randomColor = true;
    public bool hasItemSpace = true;
    public float spaceSize = 5f;
    public Color spaceColor = Color.white;
    public bool generateItemsContent = true;
    public bool generateItemsText = true;
    public float itemsTextPosition = 100;
    public Color itemsTextColor = Color.white;
    public int itemsTextSize = 35;
    public TextAnchor itemsTextAlignment = TextAnchor.MiddleCenter;
    public bool itemsHasOutline = true;
    public Color itemsOutlineColor = Color.black;
    public bool generateItemsIcon = true;
    public float itemsIconPosition = 190;
    public float itemsIconSize = 45;

    private bool isSpinning = false;
    private bool isSpinningFianl = false;
    private float itemDegree = 0;
    private int mSpinCount = 0;
    private float rotationSpin = 0;
    private bool tickSFXPlayed = false;
    private float finalRotation;
    private float mFinalSpinSpeed;
    [SerializeField]
    public int selectedItem;
    public virtual void OnFinishedSpin()
    {
        
    }
    public virtual void OnSpinButtonClick()
    {
        if (!IsWheelSpinning())
        {
            InitMove();
            Spin();
        }
    }
    public bool IsWheelSpinning()
    {
        if (isSpinning || isSpinningFianl) return true;
        return false;
    }
    public void AddSlice(string text, Sprite icon, int chance, Color color)
    {
        SpinItem item = new SpinItem();
        item.text = text;
        item.icon = icon;
        item.chance = chance;
        item.color = color;
        items.Add(item);

        rotationSpin = 0;
        wheel.eulerAngles = new Vector3(0, 0, 0);
        AutoGenerateSpin();
    }
    public void RemoveSlice(int index)
    {
        items.RemoveAt(index);

        rotationSpin = 0;
        wheel.eulerAngles = new Vector3(0, 0, 0);
        AutoGenerateSpin();
    }
    public virtual void Start()
    {
        itemDegree = (float)(360f / items.Count);
    }
    public virtual void Update()
    {
        int direction = (reverseWheelRotation) ? -1 : 1;
        if (isSpinning) // Normal spinning with linear speed
        {
            float speed = spinSpeed;

            rotationSpin -= (speed * Time.deltaTime / 2f);
            if (rotationSpin <= -360f)
            {
                rotationSpin += 360f;
                mSpinCount++;
                if (mSpinCount == spinRounds - 2)
                {
                    rotationSpin = 0;
                    isSpinning = false;
                    mFinalSpinSpeed = spinSpeed;
                    isSpinningFianl = true;

                }
            }
            //wheel.transform.Rotate(Vector2.up * angle * Time.fixedDeltaTime * direction, Space.Self);
            wheel.eulerAngles = new Vector3(0, 0, rotationSpin * direction);
            //wheel.eulerAngles = new Vector3(0, 0, rotationSpin * direction * Time.deltaTime * 30);
            GoldCoinMove();
            AdjustHandleRotation();

        }
        else if (isSpinningFianl) // 2 final rounds with decreasing speed 
        {
            float currentProgress = ((2 - (spinRounds - mSpinCount)) * 360) - rotationSpin;
            float finalProgress = 720 - finalRotation;
            float speedMult = 1 - (currentProgress / finalProgress);
            mFinalSpinSpeed = ((spinSpeed - minSpinSpeed) * (speedMult)) + minSpinSpeed;

            rotationSpin -= (mFinalSpinSpeed * Time.deltaTime / 2);
            GoldCoinMove();
            if (mSpinCount < spinRounds)
            {
                if (rotationSpin <= -360f)
                {
                    rotationSpin += 360f;
                    mSpinCount++;
                }
            }
            else
            {
                if (rotationSpin <= finalRotation)
                {
                    isSpinningFianl = false;
                    OnFinishedSpin();
                    OnFInishedScroll();
                }
            }
            wheel.eulerAngles = new Vector3(0, 0, rotationSpin * direction);
            // wheel.eulerAngles = new Vector3(0, 0, rotationSpin * direction*Time.deltaTime * 20);

            if (handle)
            {
                AdjustHandleRotation();
            }
        }
    }
    public int totalRounds = 4;
    public float angle = 1;
    int totalAngles;
    float angleUntilNow = 0;

    float imgInitial_X_Pos;

    CentralImageController GameTest;
    void InitMove()
    {
        Debug.Log("Selected Item " + selectedItem);
        GameTest = FindObjectOfType<CentralImageController>();
        totalAngles = (totalRounds * 360) - ((10 - selectedItem) * 36);
        angleUntilNow = 0;
        // imgXPos = 0;
        imgInitial_X_Pos = GameTest.img.position.x;
        GameTest.img.position = GameTest.ImgPos;
        imgInitial_X_Pos = GameTest.ImgPos.x;
        GameTest.InitImage();
        // GameTest.img2.gameObject.SetActive(true);
    }
    void GoldCoinMove()
    {
        angleUntilNow += angle;

        float result = NumberMapping(angleUntilNow, 0, totalAngles,
            imgInitial_X_Pos, wheel.position.x);
        GameTest.img.transform.position = new Vector2(result, GameTest.img.position.y);

        //GameTest.img2.transform.position = new Vector2(result, GameTest.img2.position.y);
    }
    void OnFInishedScroll()
    {

        GameTest.transform.position = new Vector2(-56.5f, GameTest.transform.position.y);
        Debug.Log("OnFinished Called " + imgInitial_X_Pos);
        // GameTest.img2.gameObject.SetActive(false);
        GameTest.SetImage();
    }
    float NumberMapping(float num, float in_min, float in_max, float out_min, float out_max)
    {
        // Debug.Log("Result....." + (num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
        return (num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    public bool isHandleallowed = true;
    private void AdjustHandleRotation()
    {
        if (!isHandleallowed) return;
        float x = rotationSpin % (-itemDegree);
        x = (-x);
        if (x > (itemDegree / 2f))
        {
            x = (itemDegree / 2f);
            tickSFXPlayed = false;
        }

        int direction = (reverseHandleRotation) ? -1 : 1;

        if (x <= (itemDegree / 4f))
        {
            handle.eulerAngles = new Vector3(0, 0, (x / (itemDegree / 4f)) * 45f * direction);
            if (!tickSFXPlayed)
            {
                tickSFXPlayed = true;
            }
        }
        else
        {
            handle.eulerAngles = new Vector3(0, 0, (45f - ((x - (itemDegree / 4f)) / (itemDegree / 4f)) * 45f) * direction);
        }
    }
    private void Spin()
    {
        wheel.eulerAngles = Vector3.zero;
        handle.eulerAngles = Vector3.zero;
        mSpinCount = 0;
        rotationSpin = 0;
        //selectedItem = 8;
        int allChances = 0;
        for (int i = 0; i < items.Count; i++)
        {
            allChances += items[i].chance;
        }

        float chancePart = 1000f / allChances;
        float checkedChances = 0f;

        for (int i = 0; i < items.Count; i++)
        {
            checkedChances += chancePart * items[i].chance;
            if (selectedItem < checkedChances)
            {
                // selectedItem = i;
                break;
            }
        }

        finalRotation = -(selectedItem * itemDegree) - (itemDegree / 2f) + 18;
        tickSFXPlayed = false;
        isSpinningFianl = false;
        isSpinning = true;
        Debug.Log("itemDegree: " + itemDegree);
        Debug.Log("selectedItem: " + selectedItem);
        Debug.Log("finalRotation: " + finalRotation);
    }

    int[] angles = { 0, 36, 72, 108, -216, -180, -144, -108, -72, -36 };
    public virtual void SetNumber(int no)
    {
        selectedItem = no;
        float angle = angles[no];
        wheel.eulerAngles = new Vector3(0, 0, angle);
    }
    public void AutoGenerateSpin()
    {
        if (items.Count <= 2)
        {
            Debug.LogError("Minimum Items Count Is 3.");
            return;
        }

        bool allChancesZero = true;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].chance < 0)
            {
                Debug.LogError("Items chance can't be negative.");
                return;
            }
            else if (items[i].chance > 0)
            {
                allChancesZero = false;
            }

            if (!randomColor && items[i].color.a == 0)
            {
                items[i].color.a = 1;
            }
        }

        if (allChancesZero)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].chance = 1;
            }
        }

        for (int i = autoGenerateParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(autoGenerateParent.transform.GetChild(i).gameObject);
        }

        itemDegree = (float)(360f / items.Count);

        GameObject slices = new GameObject("slices");
        slices.transform.SetParent(autoGenerateParent.transform);
        slices.transform.localScale = Vector3.one;
        slices.transform.localPosition = Vector3.zero;

        for (int i = 0; i < items.Count; i++)
        {
            GameObject slice = new GameObject("slice " + i);
            slice.transform.SetParent(slices.transform);
            slice.transform.localScale = Vector3.one;
            slice.transform.localPosition = Vector3.zero;
            slice.AddComponent<Image>();
            slice.GetComponent<Image>().sprite = circleSprite;
            slice.GetComponent<Image>().type = Image.Type.Filled;
            slice.GetComponent<Image>().fillAmount = 1f / items.Count;
            slice.GetComponent<Image>().fillOrigin = (int)Image.Origin360.Top;
            SetRectSize(slice.GetComponent<RectTransform>(), 470, 470);
            slice.transform.eulerAngles = new Vector3(0, 0, (i + 1) * itemDegree);
            if (randomColor)
            {
                slice.GetComponent<Image>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            }
            else
            {
                slice.GetComponent<Image>().color = items[i].color;
            }
        }

        if (hasItemSpace)
        {
            GameObject spaces = new GameObject("spaces");
            spaces.transform.SetParent(autoGenerateParent.transform);
            spaces.transform.localScale = Vector3.one;
            spaces.transform.localPosition = Vector3.zero;

            for (int i = 0; i < items.Count; i++)
            {
                GameObject space = new GameObject("space " + i);
                space.transform.SetParent(spaces.transform);
                space.transform.localScale = Vector3.one;
                space.transform.localPosition = Vector3.zero;
                space.AddComponent<Image>();
                space.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                SetRectSize(space.GetComponent<RectTransform>(), spaceSize, 234);
                space.transform.eulerAngles = new Vector3(0, 0, i * itemDegree);
                space.GetComponent<Image>().color = spaceColor;
            }
        }

        GameObject pins = new GameObject("pins");
        pins.transform.SetParent(autoGenerateParent.transform);
        pins.transform.localScale = Vector3.one;
        pins.transform.localPosition = Vector3.zero;

        for (int i = 0; i < items.Count; i++)
        {
            GameObject pin = new GameObject("pin " + i);
            pin.transform.SetParent(pins.transform);
            pin.transform.localScale = Vector3.one;
            pin.transform.localPosition = Vector3.zero;

            GameObject image = new GameObject("image");
            image.transform.SetParent(pin.transform);
            image.transform.localScale = Vector3.one;
            image.transform.localPosition = Vector3.zero;
            image.AddComponent<Image>();
            image.GetComponent<Image>().sprite = pinSprite;
            SetRectSize(image.GetComponent<RectTransform>(), 30, 30);
            image.transform.localPosition = new Vector3(0, 255, 0);

            pin.transform.eulerAngles = new Vector3(0, 0, i * itemDegree);
        }

        if (generateItemsContent)
        {
            GameObject texts = new GameObject("items");
            texts.transform.SetParent(autoGenerateParent.transform);
            texts.transform.localScale = Vector3.one;
            texts.transform.localPosition = Vector3.zero;

            for (int i = 0; i < items.Count; i++)
            {
                GameObject item = new GameObject("item " + i);
                item.transform.SetParent(texts.transform);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;

                if (generateItemsText)
                {
                    GameObject text = new GameObject("item text " + i);
                    text.transform.SetParent(item.transform);
                    text.transform.localScale = Vector3.one;
                    text.transform.localPosition = Vector3.zero;
                    text.AddComponent<Text>();
                    text.transform.localPosition = new Vector3(itemsTextPosition, 0, 0);
                    SetRectSize(text.GetComponent<RectTransform>(), 1, 1);
                    text.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                    text.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
                    text.GetComponent<Text>().color = itemsTextColor;
                    text.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    text.GetComponent<Text>().alignment = itemsTextAlignment;
                    if (items[i].text.Length > 0)
                        text.GetComponent<Text>().text = items[i].text;
                    else
                        text.GetComponent<Text>().text = "item " + i;
                    text.GetComponent<Text>().fontSize = itemsTextSize;
                    if (itemsHasOutline)
                    {
                        text.AddComponent<Outline>();
                        text.GetComponent<Outline>().effectDistance = new Vector2(2, -2);
                        text.GetComponent<Outline>().effectColor = itemsOutlineColor;
                    }
                }

                if (generateItemsIcon)
                {
                    GameObject image = new GameObject("item icon " + i);
                    image.transform.SetParent(item.transform);
                    image.transform.localScale = Vector3.one;
                    image.AddComponent<Image>();
                    SetRectSize(image.GetComponent<RectTransform>(), itemsIconSize, itemsIconSize);
                    image.GetComponent<Image>().preserveAspect = true;
                    if (items[i].icon)
                        image.GetComponent<Image>().sprite = items[i].icon;
                    else
                        image.GetComponent<Image>().sprite = circleSprite;
                    image.transform.localPosition = new Vector3(itemsIconPosition, 0, 0);
                }

                item.transform.eulerAngles = new Vector3(0, 0, (90 - itemDegree) + (itemDegree / 2) + ((i + 1) * itemDegree));
            }
        }
    }
    private void SetRectSize(RectTransform rect, float newWidth, float newHeight)
    {
        Vector2 newSize = new Vector2(newWidth, newHeight);
        Vector2 oldSize = rect.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        rect.offsetMin = rect.offsetMin - new Vector2(deltaSize.x * rect.pivot.x, deltaSize.y * rect.pivot.y);
        rect.offsetMax = rect.offsetMax + new Vector2(deltaSize.x * (1f - rect.pivot.x), deltaSize.y * (1f - rect.pivot.y));
    }
    public void TestAddRandomSlice()
    {
        AddSlice("test" + Random.Range(0, 1000), (items.Count > 0) ? items[Random.Range(0, items.Count)].icon : null, 1, new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
    }
    public void TestRemoveLastSlice()
    {
        if (items.Count > 0)
        {
            RemoveSlice(items.Count - 1);
        }
    }

    [System.Serializable]
    public class SpinItem
    {
        public string text;
        public Sprite icon;
        public int chance = 1;
        public Color color = Color.white;
    }
}
