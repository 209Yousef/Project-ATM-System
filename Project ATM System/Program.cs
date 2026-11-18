using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_ATM_System
{
    internal class Program
    {
        const string clientFileName = "Client-Atm-.txt";

        struct stDataClient
        {
            public string accountNumber;
            public string pinCode;
            public string fullName;
            public string phone;
            public double accountBalance;
        }

        static stDataClient currentClient;

        enum enWhatYouWantToDoAtmSystem
        {
            eQuickWithDraw = 1, eNormalWithDraw = 2, 
            eDeposit = 3, eCheckBalance = 4, eLogout = 5
        };

        static void ShowLoginScreen()
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("\tLogin Screen");
            Console.WriteLine("--------------------------------");
        }

        static string ReadAccountNumber()
        {
            string accountNumber;

            Console.Write("Enter AccountNumber? ");
            accountNumber = Console.ReadLine();

            return accountNumber;
        }

        static string ReadPinCode()
        {
            string pinCode;

            Console.Write("Enter PinCode? ");
            pinCode = Console.ReadLine();

            return pinCode;
        }

        static List<string> SplitString(string s, string delim = "#//#")
        {
            List<string> lWords = new List<string>();

            int pos;
            string sWord;

            while((pos = s.IndexOf(delim)) != -1)
            {
                sWord = s.Substring(0, pos);

                if(sWord != "")
                {
                    lWords.Add(sWord);
                }

                s = s.Remove(0, pos + delim.Length);
            }

            if(s != "")
                lWords.Add(s);

            return lWords;
        }

        static stDataClient ConvertLineToDataClient(string line, string delim = "#//#")
        {
            stDataClient dataClient = new stDataClient();
            List<string> lWords = SplitString(line, delim);

            dataClient.accountNumber = lWords[0];
            dataClient.pinCode = lWords[1];
            dataClient.fullName = lWords[2];
            dataClient.phone = lWords[3];
            dataClient.accountBalance = double.Parse(lWords[4]);

            return dataClient;
        }

        static List<stDataClient> LoadDataClientFromFile(string fileName)
        {
            List<stDataClient> lClients = new List<stDataClient>();
            stDataClient dataClient = new stDataClient();

            StreamReader myFile = new StreamReader(fileName);

            while(!myFile.EndOfStream)
            {
                string line = myFile.ReadLine();
                dataClient = ConvertLineToDataClient(line);
                lClients.Add(dataClient);
            }

            myFile.Close();

            return lClients;
        }

        static bool FindClinetByAccountNumberAndPinCode(string accountNumber, string pinCode, ref stDataClient dataClient)
        {
            List<stDataClient> lClients = LoadDataClientFromFile(clientFileName);

            for(int i = 0; i < lClients.Count; i++)
            {
                if (lClients[i].accountNumber == accountNumber && lClients[i].pinCode == pinCode)
                {
                    dataClient = lClients[i];
                    return true;
                }
            }

            return false;
        }

        static bool LoadClientinfo(string accountNumber, string pinCode)
        {
            if(FindClinetByAccountNumberAndPinCode(accountNumber, pinCode, ref currentClient))
            {
                return true;
            }

            return false;
        }

        static short ReadAtmMainMainMenue()
        {
            short number;

            Console.Write("Choose what do you want to do? [1 to 5] ? ");
            number = short.Parse(Console.ReadLine());

            return number;
        }

        static void GoBackToAtmMainMenue()
        {
            Console.Write("\n\nGo Back To ATM Main menue........");
            Console.ReadKey();
            ShowAtmMainMenueScreen();
        }

        static void CheckBalanceScreen()
        {
            Console.WriteLine("============================================");
            Console.WriteLine("\tCheck Balance Screen                      ");
            Console.WriteLine("============================================");

            Console.WriteLine($"Your balance is {currentClient.accountBalance}");
        }

        static short ReadQuickWihtDarwScreen()
        {
            short number;

            Console.Write("Choose what do you want to do? [1 to 9] ? ");
            number = short.Parse(Console.ReadLine());
            while(number < 1 || number > 9)
            {
                Console.Write("Choose what do you want to do? [1 to 9] ? ");
                number = short.Parse(Console.ReadLine());
            }

            return number;
        }

        static short GetQuickWithDrawAmount(short withDrawOption)
        {
            switch(withDrawOption)
            {
                case 1:
                    return 20;

                case 2:
                    return 50;

                case 3:
                    return 100;

                case 4:
                    return 200;

                case 5:
                    return 400;

                case 6:
                    return 600;

                case 7:
                    return 800;

                case 8:
                    return 1000;

                default:
                    return 0;
            }

        }

        static void SaveDataClientToFile(string fileName, List<stDataClient> lClients)
        {
            StreamWriter myFile = new StreamWriter(fileName, false);

            for(int i = 0; i < lClients.Count; i++)
            {
                string line = ConvertDataClientToLine(lClients[i]);

                myFile.WriteLine(line);
            }

            myFile.Close();
        }

        static string ConvertDataClientToLine(stDataClient dataClient, string delim = "#//#")
        {
            return dataClient.accountNumber + delim + dataClient.pinCode + delim + dataClient.fullName
                 + delim + dataClient.phone + delim + dataClient.accountBalance; 
        }

        static void DepositAmountToBalanceByAccountNumber(string accountNumber, int amount, List<stDataClient> lClients)
        {
            stDataClient dataClient = new stDataClient();

            Console.Write("\nAre you sure do you want perform transaction ? y/n ? ");
            char answer = char.Parse(Console.ReadLine());

            if (answer == 'y' || answer == 'Y')
            {
                for(int i = 0; i < lClients.Count; i++)
                {
                    if (lClients[i].accountNumber == accountNumber)
                    {
                        dataClient = lClients[i];
                        dataClient.accountBalance += amount;
                        lClients[i] = dataClient;

                        SaveDataClientToFile(clientFileName, lClients);

                        Console.WriteLine($"\nDone succusufully, Your new balance {lClients[i].accountBalance}");
                        return;
                    }
                }
            }
        }

        static void PerformQuickWihtDraw(short quickWithDrawOption)
        {
            if (quickWithDrawOption == 9)
                return;

            int quickWithDrawAmount = GetQuickWithDrawAmount(quickWithDrawOption);

            if(quickWithDrawAmount > currentClient.accountBalance)
            {
                Console.WriteLine("\nThe amount exeeds the balance, make another choose.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowQuickWihtDrawScreen();
                return;
            }

            List<stDataClient> lClients = LoadDataClientFromFile(clientFileName);
            DepositAmountToBalanceByAccountNumber(currentClient.accountNumber, quickWithDrawAmount *-1, lClients);

            currentClient.accountBalance -= quickWithDrawAmount;
        }

        static void ShowQuickWihtDrawScreen()
        {
            Console.Clear();

            Console.WriteLine("==============================================");
            Console.WriteLine("\tQuick WithDarw Screen                       ");
            Console.WriteLine("==============================================");

            Console.WriteLine("\t[1] 20\t\t [2] 50");
            Console.WriteLine("\t[3] 100\t\t [4] 200");
            Console.WriteLine("\t[5] 400\t\t [6] 600");
            Console.WriteLine("\t[7] 800\t\t [8] 1000");
            Console.WriteLine("\t[9] Exit");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Your balance is {currentClient.accountBalance}");

            PerformQuickWihtDraw(ReadQuickWihtDarwScreen());
        }

        static int ReadAmountForNormalWithDraw()
        {
            int amount;

            Console.Write("Enter an amount multiply of 5's? ");
            amount = int.Parse(Console.ReadLine());
            while(amount % 5 != 0)
            {
                Console.Write("Enter an amount multiply of 5's? ");
                amount = int.Parse(Console.ReadLine());
            }

            return amount;
        }

        static void PerformNormalWithDraw()
        {
            int amount = ReadAmountForNormalWithDraw();

            if(amount > currentClient.accountBalance)
            {
                Console.WriteLine("\nThe amount exeeds the balance, Make another choose.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowNormalWithDrawScreen();
                return;
            }

            List<stDataClient> lClients = LoadDataClientFromFile(clientFileName);

            DepositAmountToBalanceByAccountNumber(currentClient.accountNumber, amount*-1, lClients);
            currentClient.accountBalance -= amount;
        }

        static void ShowNormalWithDrawScreen()
        {
            Console.WriteLine("============================================");
            Console.WriteLine("\tNormal WithDarw Screen                    ");
            Console.WriteLine("============================================");

            PerformNormalWithDraw();    
        }

        static int ReadAmountForDeposit()
        {
            int amount;

            Console.Write("Enter a positive amount? ");
            amount = int.Parse(Console.ReadLine());

            while(amount <= 0)
            {
                Console.Write("Enter a positive amount? ");
                amount = int.Parse(Console.ReadLine());
            }

            return amount;
        }

        static void PerformDeposit()
        {
            int amount = ReadAmountForDeposit();

            List<stDataClient> lClients = LoadDataClientFromFile(clientFileName);
            DepositAmountToBalanceByAccountNumber(currentClient.accountNumber, amount, lClients);

            currentClient.accountBalance += amount;
        }

        static void ShowDepositScreen()
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("\tDeposit Screen                     ");
            Console.WriteLine("=====================================");

            PerformDeposit();
        }

        static void PerformAtmMainMenue(enWhatYouWantToDoAtmSystem whatYouWantToDoAtmSystem)
        {
            switch(whatYouWantToDoAtmSystem)
            {
                case enWhatYouWantToDoAtmSystem.eQuickWithDraw:
                    Console.Clear();
                    ShowQuickWihtDrawScreen();
                    GoBackToAtmMainMenue();
                    break;

                case enWhatYouWantToDoAtmSystem.eNormalWithDraw:
                    Console.Clear();
                    ShowNormalWithDrawScreen();
                    GoBackToAtmMainMenue();
                    break;

                case enWhatYouWantToDoAtmSystem.eDeposit:
                    Console.Clear();
                    ShowDepositScreen();
                    GoBackToAtmMainMenue();
                    break;

                case enWhatYouWantToDoAtmSystem.eCheckBalance:
                    Console.Clear();
                    CheckBalanceScreen();
                    GoBackToAtmMainMenue();
                    break;

                case enWhatYouWantToDoAtmSystem.eLogout:
                    Login();
                    break;
                           
            }
        }

        static void ShowAtmMainMenueScreen()
        {
            Console.Clear();

            Console.WriteLine("===============================================");
            Console.WriteLine("\tATM Main Menue Screen                        ");
            Console.WriteLine("===============================================");

            Console.WriteLine("\t[1] Quick WithDraw.");
            Console.WriteLine("\t[2] Normal WithDraw.");
            Console.WriteLine("\t[3] Deposit.");
            Console.WriteLine("\t[4] Check Balance.");
            Console.WriteLine("\t[5] Logout.");
            Console.WriteLine("===============================================");

            PerformAtmMainMenue((enWhatYouWantToDoAtmSystem)ReadAtmMainMainMenue());
        }

        static void Login()
        {
            bool loginFailed = false;

            do
            {
                Console.Clear();

                ShowLoginScreen();

                if(loginFailed)
                    Console.WriteLine("Invalid AccountNumber/PinCode!");

                string accountNumber = ReadAccountNumber();
                string PinCode = ReadPinCode();

                loginFailed = !LoadClientinfo(accountNumber, PinCode);

            }
            while(loginFailed);

            ShowAtmMainMenueScreen();
        }

        static void Main(string[] args)
        {
            Login();

            Console.ReadKey();
        }
    }
}
