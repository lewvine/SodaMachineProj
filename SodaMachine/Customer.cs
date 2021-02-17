using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class Customer
    {
        //Member Variables (Has A)
        public Wallet Wallet;
        public Backpack Backpack;

        //Constructor (Spawner)
        public Customer()
        {
            Wallet = new Wallet();
            Backpack = new Backpack();
        }
        //Member Methods (Can Do)

        //This method will be the main logic for a customer to retrieve coins form their wallet.
        //Takes in the selected can for price reference
        //Will need to get user input for coins they would like to add.
        //When all is said and done this method will return a list of coin objects that the customer will use a payment for their soda.
        public List<Coin> GatherCoinsFromWallet(Can selectedCan)
        {
            List<Coin> coins = new List<Coin>();
            double price = selectedCan.Price;
            double coinTotal = 0;

            while (coinTotal < price && Wallet.Coins != null)
            {
                Console.WriteLine("Which coins would you like to use?" +
                    "\n 1 - Quarter" +
                    "\n 2 - Dime" +
                    "\n 3 - Nickel" +
                    "\n 4 - Penny");

                string input = Console.ReadLine();
                string output = null;
                switch (input)
                {
                    case "1":
                        output = "quarter";
                        break;
                    case "2":
                        output = "dime";
                        break;
                    case "3":
                        output = "nickel";
                        break;
                    case "4":
                        output = "penny";
                        break;

                }

                Coin coin = GetCoinFromWallet(output);

                if (coin == null)
                {
                    Console.WriteLine($"You don't have any {output}s.  Please select a different coin.");
                    GatherCoinsFromWallet(selectedCan);
                }
                else
                {
                    coinTotal += coin.Value;
                    Wallet.Coins.Remove(coin);
                    coins.Add(coin);
                    Console.WriteLine($"You've taken a {output} from your wallet.");
                }
            }
            if(Wallet.Coins != null)
            {
                Console.WriteLine($"You've taken a total of ${coinTotal} from your wallet, which" +
                    $" is enough to buy a {selectedCan.Name}.");
                Console.ReadLine();
                return coins;
            }
            else
            {
                Console.WriteLine($"You don't have enough coins to buy this drink.");
                Console.ReadLine();
                return coins;
            }
        }

        //Returns a coin object from the wallet based on the name passed into it.
        //Returns null if no coin can be found
        public Coin GetCoinFromWallet(string coinName)
        {
            if((Wallet.Coins.Find(coin => coin.Name.ToLower() == coinName) != null))
            {
                return Wallet.Coins.Find(coin => coin.Name.ToLower() == coinName);
            }
            else
            {
                return null;
            }
        }
        //Takes in a list of coin objects to add into the customers wallet.
        public void AddCoinsIntoWallet(List<Coin> coinsToAdd)
        {
            foreach(Coin coin in coinsToAdd)
            {
                Wallet.Coins.Add(coin);
            }   
        }
        //Takes in a can object to add to the customers backpack.
        public void AddCanToBackpack(Can purchasedCan)
        {
            Backpack.cans.Add(purchasedCan);
        }
    }
}
