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
            //FillRegister(20, quarter);
            //FillRegister(10, dime);
            //FillRegister(20, nickel);
            //FillRegister(50, penny);
        }

        //Member Methods (Can Do)

        public void FillRegister(int x, Coin type)
        {
            for(int i = 0; i < x; i++)
            {
                _register.Add(type);
            }
        }
        public void FillInventory(int x, Can type)
        {
            for (int i = 0; i < x; i++)
            {
                _inventory.Add(type);
            }
        }
        public void BeginTransaction(Customer customer)
        {
            bool willProceed = UserInterface.DisplayWelcomeInstructions(_inventory);
            if (willProceed)
            {
                Transaction(customer);
            }
        }


       
        //Get soda, take take, pass payment to CalculateTransaction method.
        private void Transaction(Customer customer)
        {
            string sodaChoice = UserInterface.SodaSelection(_inventory);
            Can soda = GetSodaFromInventory(sodaChoice);
            List<Coin> payment = customer.GatherCoinsFromWallet(soda);

            CalculateTransaction(payment, soda, customer);
        }

        //Takes string and returns Soda object.
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
            double totalChange = Math.Round(DetermineChange(totalPayment, price), 2);
            List<Coin> change = GatherChange(totalChange);

            DepositCoinsIntoRegister(_register, payment);
            double totalRegister = TotalCoinValue(_register);


            Can soda = GetSodaFromInventory(chosenSoda.Name);

            //Enough coins in machine to give change
            if (totalChange <= totalRegister)
            {
                //Received enough payment
                //Remove soda from machine.
                //Give change to wallet.
                //Add soda to backpack.
                if (totalPayment >= price)
                {
                    UserInterface.OutputText("Processing transaction, please wait.");

                    _inventory.Remove(soda);
                    DepositCoinsIntoRegister(customer.Wallet.Coins, change);
                    customer.Backpack.cans.Add(soda);

                    UserInterface.EndMessage(soda.Name, totalChange);
                }
                //Did not receive enough payment.
                //Return entire payment to wallet.
                else
                {
                    DepositCoinsIntoRegister(customer.Wallet.Coins, payment);
                    UserInterface.OutputText("Sorry.  You haven't put in enough coins.  Come back when you have the correct amount!");
                }
            }
            //Not enough coins in machine to give change.
            //Return entire payment to wallet.
            else
            {
                DepositCoinsIntoRegister(customer.Wallet.Coins, payment);
                UserInterface.OutputText("Sorry.  Our coin register is almost empty; we don't have " +
                    "enough money to give you change!  Please come back again later.!");
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
                for(int i = 0; i <= coinNames.Count - 1; i++) 
                {
                    if (RegisterHasCoin(coinNames[i]))
                    {
                        Coin coin = GetCoinFromRegister(coinNames[i]);
                        while (coin.Value <= Math.Round(changeValue,2))
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


        //Takes string of coin name and returns true or false
        private bool RegisterHasCoin(string name)
        {
            Coin coin = _register.Find(thing => thing.Name.ToLower() == name);
            if (coin != null)
            {
                return true;
            }
            return false;
        }

        //Takes string of coin name and returns coin object / null
        private Coin GetCoinFromRegister(string name)
        {
            Coin coin = _register.Find(item => item.Name.ToLower() == name);
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
        private void DepositCoinsIntoRegister(List<Coin> deposit, List<Coin> register)
        {
            foreach (Coin coin in deposit)
            {
                register.Add(coin);
            }
        }
    }
}
