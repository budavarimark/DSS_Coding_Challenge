using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DSS_Coding_Challenge
{
    static class FileWriter
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public static void Write(string outputDir, Order[] orders, List<Schedule> schedules)
        {
            string orderFirstLine    = "Megrendelésszám;Profit összesen;Levont kötbér;Munka megkezdése;Készre jelentése ideje;Megrendelés eredeti határideje";
            string scheduleFirstLine = "Dátum;Gép;Kezdő időpont;Záró időpont;Megrendelésszám";

            try
            {
                _log.Info("A megrendelések CSV fájlba mentése.");
                File.WriteAllText(
                    outputDir + "/" + "megrendelesek.csv",
                    GenerateString(orderFirstLine, orders)
                );

                _log.Info("A munkarendek CSV fájlba mentése.");
                File.WriteAllText(
                    outputDir + "/" + "munkarend.csv",
                    GenerateString(scheduleFirstLine, schedules.ToArray())
                );
            }
            catch
            {
                _log.Fatal("Nem sikerült elmenteni a generált adatokat.");
            }
        }

        private static string GenerateString<T>(string firstLine, T[] objects)
        {
            string tmp = firstLine+"\n";

            for(int i = 0; i < objects.Length; i++) 
                if(i+1 == objects.Length)
                    tmp += objects[i];
                else
                    tmp += objects[i]+"\n";

            return tmp;
        }
    }
}
