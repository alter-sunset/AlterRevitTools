using AlterTools.BatchExport.Utils;
using AlterTools.DriveFromOutside.Events;
using AlterTools.DriveFromOutside.Events.Detach;
using System.Diagnostics;

namespace AlterTools.RevitFileUpgrader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string initialFilePath = "D:\\test\\upgrader\\2021\\XXXX_ME_PD_AR_R21_Terminal_DGU.rvt";

            for (int i = 2022; i <= 2026; i++)
            {
                string tempPath = $"D:\\test\\upgrader\\{i}";

                DetachTaskConfig config = new()
                {
                    Config = new DetachConfig()
                    {
                        RemoveLinks = true,
                        CheckForEmptyView = false,
                        IsToRename = false,
                        Purge = false,
                        RemoveEmptyWorksets = false,
                        FolderPath = tempPath,
                        Files = [initialFilePath]
                    }
                };

                string taskFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"RevitListener\Tasks", $"task{i}.json");
                JsonHelper<DetachTaskConfig>.SerializeConfig(config, taskFileName);

                Process.Start(@$"C:\Program Files\Autodesk\Revit {i}\Revit.exe");
            }


            //Console.WriteLine("Добрый день!");
            //Console.WriteLine("Данное приложение позволит обновить модели Autodesk Revit до необходимой версии.");
            //Console.WriteLine("Что потребуется от вас:");
            //Console.WriteLine("- Иметь установленными все необходимые версии Revit.");
            //Console.WriteLine("- Иметь в каждой из них установленные плагины AlterTools (BatchExport и DriveFromOutside).");
            //Console.WriteLine("\nТеперь укажите первую версию Revit, с которой начнётся цикл обновления (не ниже 2019).");



            //Console.WriteLine("1. Greet Me");
            //Console.WriteLine("2. Show the Current Date and Time");
            //Console.WriteLine("3. Exit");

            //Console.Write("Enter your choice (1, 2, or 3): ");
            //string userInput = Console.ReadLine();

            //switch (userInput)
            //{
            //    case "1":
            //        GreetUser();
            //        break;
            //    case "2":
            //        ShowDateTime();
            //        break;
            //    case "3":
            //        ExitApp();
            //        break;
            //    default:
            //        Console.WriteLine("Invalid choice. Please run the program again and select a valid option.");
            //        break;
            //}
        }

        static void GreetUser()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine($"Hello, {name}! Nice to meet you.");
        }

        static void ShowDateTime()
        {
            DateTime now = DateTime.Now;
            Console.WriteLine($"The current date and time is: {now}");
        }

        static void ExitApp()
        {
            Console.WriteLine("Exiting the app. Goodbye!");
        }
    }

    public class DetachTaskConfig()
    {
        public static ExternalEvents Event => ExternalEvents.Detach;
        public DetachConfig Config { get; set; }
    }
}