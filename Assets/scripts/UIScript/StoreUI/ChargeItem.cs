using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ChargeItem : IUserInterface 
    {
        NumMoney numMoney;
        ChargeUI chargeUI;
        public void SetCharge(ChargeUI cui, NumMoney nm){
            chargeUI = cui;
            numMoney = nm;

            Name.text = string.Format("[ff9500]晶石 *{0}    {1}元[-]", numMoney.num, numMoney.money);
        }

        UILabel Name;
        void Awake() {
            SetCallback("Info", OnBuy);
            Name = GetLabel("Name");
        }
        void OnBuy() {
            GameInterface_Backpack.Charge(numMoney); 
        }
    }

}