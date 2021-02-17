using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class Wallet
    {
        //Member Variables (Has A)
        public List<Coin> Coins;
        //Constructor (Spawner)
        public Wallet()
        {
            Coins = new List<Coin>();

            Quarter quarter = new Quarter();
            Dime dime = new Dime();
            Nickel nickel = new Nickel();
            Penny penny = new Penny();

            FillRegister(1, quarter);
            //FillRegister(5, dime);
            //FillRegister(12, nickel);
            //FillRegister(15, penny);

        }
        //Member Methods (Can Do)
        //Fills wallet with starting money
        private void FillRegister(int x, Coin type)
        {
            for (int i = 0; i < x; i++)
            {
                Coins.Add(type);
            }
        }
    }
}
