using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class SodaMachine
    {
        //Member Variables (Has A)
        private List<Coin> _register;
        private List<Can> _inventory;

        //Constructor (Spawner)
        public SodaMachine()
        {
            _register = new List<Coin>();
            _inventory = new List<Can>();

            RootBeer rootBeer = new RootBeer();
            Cola cola = new Cola();
            OrangeSoda orangeSoda = new OrangeSoda();

            FillInventory(20, rootBeer);
            FillInventory(20, cola);
            FillInventory(20, orangeSoda);

            Quarter quarter = new Quarter();
            Dime dime = new Dime();
            Nickel nickel = new Nickel();
            Penny penny = new Penny();
            FillRegister(20, quarter);
            FillRegister(10, dime);
            FillRegister(20, nickel);
            FillRegister(50, penny);
        }

        //Member Methods (Can Do)

        //A method to fill the sodamachines register with coin objects.
        public void FillRegister(int x, Coin type)
        {
            for(int i = 0; i < x; i++)
            {
                _register.Add(type);
            }
        }
        //A method to fill the sodamachines inventory with soda can objects.
        public void FillInventory(int x, Can type)
        {
            for (int i = 0; i < x; i++)
            {
                _inventory.Add(type);
            }
        }
        //Method to be called to start a transaction.
        //Takes in a customer which can be passed freely to which ever method needs it.
        public void BeginTransaction(Customer customer)
        {
            bool willProceed = UserInterface.DisplayWelcomeInstructions(_inventory);
            if (willProceed)
            {
                Transaction(customer);
            }
        }
        
        //This is the main transaction logic think of it like "runGame".  This is where the user will be prompted for the desired soda.
        //grab the desired soda from the inventory.
        //get payment from the user.
        //pass payment to the calculate transaction method to finish up the transaction based on the results.
        private void Transaction(Customer customer)
        {
            
            string sodaChoice = UserInterface.SodaSelection(_inventory);
            Can soda = GetSodaFromInventory(sodaChoice);

            List<Coin> payment = customer.GatherCoinsFromWallet(soda);

            CalculateTransaction(payment, soda, customer);
        }
        //Gets a soda from the inventory based on the name of the soda.
        private Can GetSodaFromInventory(string nameOfSoda)
        {
            Can soda = _inventory.Find(item => item.Name == nameOfSoda);
            return soda;
        }

        //This is the main method for calculating the result of the transaction.
        //It takes in the payment from the customer, the soda object they selected, and the customer who is purchasing the soda.
        //This is the method that will determine the following:
        //If the payment is greater than the price of the soda, and if the sodamachine has enough change to return: Despense soda, and change to the customer.
        //If the payment is greater than the cost of the soda, but the machine does not have ample change: Despense payment back to the customer.
        //If the payment is exact to the cost of the soda:  Despense soda.
        //If the payment does not meet the cost of the soda: despense payment back to the customer.
        private void CalculateTransaction(List<Coin> payment, Can chosenSoda, Customer customer)
        {
            double totalPayment = TotalCoinValue(payment);
            double price = chosenSoda.Price;
            double totalChange = DetermineChange(totalPayment, price);
            List<Coin> returnCoins = GatherChange(totalChange);
            double totalRegister = TotalCoinValue(_register);
            Can soda = GetSodaFromInventory(chosenSoda.Name);

            //If the customer has put in enough money, do first IF:
            if (totalPayment >= price)
            {
                Console.Clear();
                Console.WriteLine("Processing transaction, please wait.");
                Console.ReadLine();
                Console.Clear();

                //If the soda machine has enough in the register to return change, do the second IF:
                if (totalChange <= totalRegister)
                {
                    _inventory.Remove(soda);
                    customer.Backpack.cans.Add(soda);
                    Console.WriteLine($"Thank you for shopping with us today!  Here is your {soda.Name}.  You have $0.{totalChange} in change.");
                    foreach (Coin coin in returnCoins)
                    {
                        customer.Wallet.Coins.Add(coin);
                    }
                }
                else
                //There is not enough money in the register to give change.
                {
                    foreach (Coin coin in payment)
                    {
                        customer.Wallet.Coins.Add(coin);
                    }
                    Console.WriteLine("Sorry.  Our coin register is almost empty; we don't have enough money to give you change!" +
                        "Please come back again later.!");
                }
            }
            else
            {
                foreach (Coin coin in payment)
                {
                    customer.Wallet.Coins.Add(coin);
                }
                Console.WriteLine("Sorry.  You haven't put in enough coins.  Come back when you have the correct amount!");
            }
        }
        //Takes in the value of the amount of change needed.
        //Attempts to gather all the required coins from the sodamachine's register to make change.
        //Returns the list of coins as change to despense.
        //If the change cannot be made, return null.
        private List<Coin> GatherChange(double changeValue)
        {
            double totalChange = 0;
            List<Coin> change = new List<Coin>();
            List<String> coinNames = new List<string>() { "quarter", "dime", "nickel", "penny" };

            while (changeValue > totalChange)
            {
                for(int i = 0; i <= coinNames.Count; i++) 
                {
                    if(RegisterHasCoin(coinNames[i]))
                    {
                        Coin coin = GetCoinFromRegister(coinNames[i]);
                        while(coin.Value >= changeValue)
                        {
                            change.Add(coin);
                            _register.Remove(coin);
                            totalChange += coin.Value;
                            changeValue -= coin.Value;
                        }
                    }

                }
            }
            return change;
        }


        //Reusable method to check if the register has a coin of that name.
        //If it does have one, return true.  Else, false.
        private bool RegisterHasCoin(string name)
        {
            Coin coin = _register.Find(Coin => Coin.Name.ToLower() == name);
            if (coin != null)
            {
                return true;
            }
            return false;
        }


        //Reusable method to return a coin from the register.
        //Returns null if no coin can be found of that name.
        private Coin GetCoinFromRegister(string name)
        {
            Coin coin = _register.Find(Coin => Coin.Name.ToLower() == name);
            if (coin != null)
            {
                return coin;
            }
            return null;
        }


        //Takes in the total payment amount and the price of can to return the change amount.
        private double DetermineChange(double totalPayment, double canPrice)
        {
            double change = totalPayment - canPrice;
            return change;
        }


        //Takes in a list of coins to return the total value of the coins as a double.
        private double TotalCoinValue(List<Coin> payment)
        {
            double coinTotal = 0;

           foreach (Coin coin in payment)
            {
                coinTotal += coin.Value;    
            }

            return coinTotal;
        }


        //Puts a list of coins into the soda machines register.
        private void DepositCoinsIntoRegister(List<Coin> coins)
        {
            foreach (Coin coin in coins)
            {
                _register.Add(coin);
            }
        }
    }
}
