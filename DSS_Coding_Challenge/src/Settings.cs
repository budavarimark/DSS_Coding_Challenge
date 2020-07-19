using System;
using System.Collections.Generic;
using System.Text;

namespace DSS_Coding_Challenge
{
    static class Settings
    {
        public const int WORK_HOUR_PER_DAY      = 16;
        public const int WORK_DAY_STARTING_HOUR = 6;
        public const int WORK_DAY_ENDING_HOUR   = 22;
        public const int ONE_HOUR_IN_MINUTE     = 60;

        public static int[] availableMachineQuantity = { 6, 2, 3, 1, 4, 3 };

        public static int[,] minuteCostsByProduct = {
            // { Vágás, Hajlítás, Hegesztés, Tesztelés, Festés, Csomagolás }
            { 5, 10, 8, 5, 12, 10 },    // Kid                          | 50
            { 6, 15, 10, 5, 15, 12 },   // Adolescent                   | 63
            { 8, 16, 12, 5, 20, 15},    // Adult                        | 76
        };

        // Optimizer osztály által generált adat
        public static int[,] alreadyOptimizedMachineQuantity =
        {
            {1, 2, 2, 1, 3, 2}, // Gyerek
            {1, 2, 2, 1, 2, 2}, // Serdülő
            {1, 2, 2, 1, 3, 2}  // Felnőtt
        };
    }
}
