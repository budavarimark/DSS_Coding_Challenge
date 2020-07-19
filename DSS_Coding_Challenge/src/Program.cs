using System;
using System.IO;

namespace DSS_Coding_Challenge
{
    class Program
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private static string inputString;
        private static string outputDir;

        static void Main(string[] args)
        {
            ProcessArgs(args);

            Order[]  orders;
            DateTime now = DateTime.Now;

            try
            {
                orders = InputProcessor.ProcessOrdersFromString(inputString);
                PenaltyMinimizer.MinimizePenalty(orders);
                WorkScheduleGenerator.GenerateFromOrders(orders);
                FileWriter.Write(outputDir, orders, WorkScheduleGenerator.scheduleList);

                _log.Info("A program " + DateTime.Now.Subtract(now).TotalSeconds + " másodperc alatt lefutott.");
            }
            catch (Exception e)
            {
                LogFatal("Kritikus hiba történt a program futása közben, ezért nem folytatható a minimalizálás: \n" + e);
            }
        }

        static private void RunOptimizer(Order[] orders)
        {
           Optimizer optimizer = new Optimizer();
           optimizer.AddOrderToOptimize(orders[0]);
           Console.WriteLine(optimizer.Optimize());
           optimizer.DebugWantedMachineQuantity();
        }

        static private void LogFatal(string error)
        {
            _log.Fatal(error);
            Environment.Exit(0);
        }

        static private void ProcessArgs(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("A program futtatásához kérem adja meg az input fájl elérhetőségét és a kívánt kimeneti mappát argumentumként.");
                Console.WriteLine("program <bemeneti fájl> <kimeneti mappa>");
                Environment.Exit(0);
            }
            else
            {
                try
                {
                    _log.Info("Fájl beolvasása: " + args[0]);
                    inputString = File.ReadAllText(args[0]);
                    outputDir = args[1];
                }
                catch (Exception e)
                {
                    LogFatal("Nem sikerült az input fájl tartalmát kiolvasni! (" + e.Message + ")");
                }
            }
        }
    }
}
