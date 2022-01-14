
using UnityEngine;
using UnityEngine.UI;

public class WheelTester : MonoBehaviour
{
    public int whellNo;
    public Button _1xBtn;
    public Button _2xBtn;
    public Button _4xBtn;

    private void Start()
    {
        _1xBtn.onClick.AddListener(() => TestWheell("1x"));
        _2xBtn.onClick.AddListener(() => TestWheell("2x"));
        _4xBtn.onClick.AddListener(() => TestWheell("4x"));
    }

    void TestWheell(string xfactor)
    {
        //SpinWheelWithoutPlugin.instane.Spin(whellNo, xfactor)
        SpinWheelWithoutPlugin.instane.Spin(whellNo);
    }
}
