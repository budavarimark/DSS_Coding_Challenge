using System;
using System.Collections.Generic;
using System.Text;

namespace DSS_Coding_Challenge
{
    class Order
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        public string   id { get; }
        public Product  product;
        int             quantity;
        int             profitPerPiece;
        int             penaltyForDelayPerDay;
        public double   estimatedMinuteNeeded { get; }

        public DateTime deadline { get; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public Order(string id, Product product, int quantity, DateTime deadline, int profitPerPiece, int penaltyForDelayPerDay)
        {
            this.id                     = id;
            this.product                = product;
            this.quantity               = quantity;
            this.deadline               = deadline;
            this.profitPerPiece         = profitPerPiece;
            this.penaltyForDelayPerDay  = penaltyForDelayPerDay;
            estimatedMinuteNeeded       = CalculateEstimatedMinuteToManufacture();

            _log.Info(id + " azonosítóval rendelkező rendelés feldolgozva.");
        }

        public DateTime CalculateEndDateByStartDate(DateTime startDate)
        {
            double divided = estimatedMinuteNeeded / Settings.ONE_HOUR_IN_MINUTE / Settings.WORK_HOUR_PER_DAY;
            double howManyDayIsThat = Math.Truncate(divided);
            double remainingMinutes = (divided - howManyDayIsThat) 
                * Settings.WORK_HOUR_PER_DAY 
                * Settings.ONE_HOUR_IN_MINUTE;

            DateTime endDate = startDate
                .AddDays(howManyDayIsThat)
                .AddMinutes(remainingMinutes);

            if(endDate.Hour >= Settings.WORK_DAY_ENDING_HOUR || endDate.Hour < Settings.WORK_DAY_STARTING_HOUR)
                endDate = endDate.AddHours(24-Settings.WORK_HOUR_PER_DAY);

            return endDate;
        }

        public double EstimatedTimeAndPenaltyRatio()
        {
            return (estimatedMinuteNeeded / 100) * penaltyForDelayPerDay;
        }

        public double TimeRemainingAfterJobIsDone(DateTime startDate)
        {
            return deadline.Subtract(CalculateEndDateByStartDate(startDate)).TotalMinutes;
        }

        public double CalculatePenaltyByStartDate(DateTime startDate)
        {
            double timeLeft = TimeRemainingAfterJobIsDone(startDate);
            if(timeLeft < 0)
            {
                double day = CalculateEndDateByStartDate(startDate).Subtract(deadline).Days + 1;
                return day * penaltyForDelayPerDay;
            }

            return 0;
        }

        public double CalculatePenaltyByTimeRemainingAfterJobIsDone(double timeLeft)
        {
            if (timeLeft < 0)
            {
                double day = CalculateEndDateByStartDate(startDate).Subtract(deadline).Days + 1;
                return day * penaltyForDelayPerDay;
            }

            return 0;
        }

        /* 
         * A számok az optimalizáló osztályból jöttek
         * Gyerek:  0.2 db/perc        (5 perces bottleneck),    50 perc az első darab elkészülésééig
         * Serdülő: 0.13333 db/perc    (7.5 perces bottleneck),  63 perc
         * Felnőtt: 0.125 db/perc      (8 perces bottleneck),    76 perc
         * 
         * A bottleneck miatt nincs sok értelme egyszerre többfélét gyártani.
         */
        private double CalculateEstimatedMinuteToManufacture() => product switch
        {
            // ((quantity - 1) / AVG_MINUTE_TO_MAKE_AFTER_THE_FIRST_ONE) + TIME_TO_MAKE_THE_FIRST_ONE
            Product.KidBicycleFrame        => ((quantity - 1) * 5) + 50,
            Product.AdolescentBicycleFrame => ((quantity - 1) * 7.5) + 63,
            Product.AdultBicycleFrame      => ((quantity - 1) * 8) + 76,
            _ => throw new ArgumentException(message: "Hibás enum érték"),
        };


        private string ConvertDateToOutputDateFormat(DateTime dateTime)
        {
            string month    = dateTime.Month.ToString("D2");
            string day      = dateTime.Day.ToString("D2");
            string hour     = dateTime.Hour.ToString("D2");
            string minute   = dateTime.Minute.ToString("D2");

            return month + "." + day + ". " + hour + ":" + minute; ;
        }

        // Megrendelésszám; Profit összesen; Levont kötbér; Munka megkezdése; Készre jelentése ideje; Megrendelés eredeti határideje
        public override string ToString()
        {
            double penalty          = CalculatePenaltyByStartDate(startDate);
            double profit           = (profitPerPiece * quantity) - penalty;
            string startDateString  = ConvertDateToOutputDateFormat(startDate);
            string endDateString    = ConvertDateToOutputDateFormat(endDate);
            string deadlineString   = ConvertDateToOutputDateFormat(deadline);

            return id + ";" + profit + ";" + penalty + ";" + startDateString + ";" + endDateString + ";" + deadlineString;
        }
    }
}
