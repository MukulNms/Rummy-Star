using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SampleProjectSpinManager : SpinWheelManager
{
    public int setNo;
    private Dropdown dropDownMenu;

    

    public override void Start()
    {
        base.Start();
    }   

    public override void OnFinishedSpin()
    {
        base.OnFinishedSpin();
    }

    public override void OnSpinButtonClick()
    {
        if (!IsWheelSpinning())
        {
            if (UseCoin(200))
            {
                base.OnSpinButtonClick();
                selectedItem = dropDownMenu.value;
            }
        }
    }

    public void Spin(int no)
    {
        selectedItem = no;
        base.OnSpinButtonClick();
    }
    private int GetCoins()
    {
        return PlayerPrefs.GetInt("coin", 1000);
    }

    public override void SetNumber(int no)
    {
        base.SetNumber(no);
    }
    private void AddCoin(int value)
    {
        int coins = GetCoins();
        PlayerPrefs.SetInt("coin", coins + value);
    }

    private bool UseCoin(int value)
    {
        int coins = GetCoins();
        if(coins >= value)
        {
            PlayerPrefs.SetInt("coin", GetCoins() - value);
            return true;
        }

        return false;
    }

    private int GetHearts()
    {
        return PlayerPrefs.GetInt("heart", 0);
    }

    private void AddHeart(int value)
    {
        int hearts = GetHearts();
        PlayerPrefs.SetInt("heart", hearts + value);
    }

 
}
