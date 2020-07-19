using System;
using System.Collections.Generic;
using System.Text;

namespace DSS_Coding_Challenge
{
    static class PenaltyMinimizer
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        static DateTime startDate = new DateTime(2020, 7, 20, 6, 0, 0);
        static Order[]  orders;
        static double timeLeftSum; // Lehetne használni

        public static void MinimizePenalty(Order[] givenOrders)
        {
            orders = givenOrders;
            if (orders.Length > 1)
            {
                _log.Info("Megrendelések sorbarendezése.");
                OrderFirstByTimeAndPenaltyRatio(0, orders.Length - 1);

                _log.Info("Kötbér minimalizálás.");
                Minimizer(CalculateCurrentPenaltyAmmount());
            }

            _log.Info("Minimalizált összkötbér: " + CalculateCurrentPenaltyAmmount().ToString("N0") + " Ft");
            StoreStartAndEndDates();
            DebugOrders();
        }

        public static void DebugOrders()
        {
            for (int i = 0; i < orders.Length; i++)
            {
                _log.Debug(orders[i]);
            }
        }

        private static void StoreStartAndEndDates()
        {
            DateTime lastEndDate = orders[0].CalculateEndDateByStartDate(startDate);
            orders[0].startDate = startDate;
            orders[0].endDate = lastEndDate;

            for (int i = 1; i < orders.Length; i++)
            {
                orders[i].startDate = lastEndDate;
                lastEndDate = orders[i].CalculateEndDateByStartDate(lastEndDate);
                orders[i].endDate = lastEndDate;
            }
        }

        private static void Minimizer(double lastPenaltyAmmount)
        {
            double currentPenaltyAmmount = lastPenaltyAmmount;
            double changedPenaltyAmmount;

            for (int i = 0; i < orders.Length; i++)
            {
                for (int k = orders.Length - 1; k >= 0; k--)
                {
                    if (i == k) continue;
                    ExchangeOrders(i, k);
                    changedPenaltyAmmount = CalculateCurrentPenaltyAmmount();

                    if (changedPenaltyAmmount > currentPenaltyAmmount)
                        ExchangeOrders(i, k);
                    else
                    {
                        if (changedPenaltyAmmount < currentPenaltyAmmount)
                            k = orders.Length - 1;
                        currentPenaltyAmmount = changedPenaltyAmmount;
                    }
                }

                if (lastPenaltyAmmount != currentPenaltyAmmount)
                {
                    lastPenaltyAmmount = currentPenaltyAmmount;
                    i = -1;
                }
            }
        }

        private static double CalculateCurrentPenaltyAmmount()
        {
            double penaltySum    = orders[0].CalculatePenaltyByStartDate(startDate);
            DateTime lastEndDate = orders[0].CalculateEndDateByStartDate(startDate);
            timeLeftSum = orders[0].TimeRemainingAfterJobIsDone(startDate);

            for (int i = 1; i < orders.Length; i++)
            {
                penaltySum += orders[i].CalculatePenaltyByStartDate(lastEndDate);
                lastEndDate = orders[i].CalculateEndDateByStartDate(lastEndDate);
                timeLeftSum += orders[i].TimeRemainingAfterJobIsDone(startDate);
            }

            return penaltySum;
        }

        private static void OrderFirstByTimeAndPenaltyRatio(int l, int r)
        {
            int i, j;
            double x;

            i = l;
            j = r;

            x = orders[(l + r) / 2].EstimatedTimeAndPenaltyRatio();
            while (true)
            {
                while (i < r && orders[i].EstimatedTimeAndPenaltyRatio() < x)
                    i++;
                while (x < orders[j].EstimatedTimeAndPenaltyRatio())
                    j--;
                if (i <= j)
                {
                    ExchangeOrders(i, j);
                    i++;
                    j--;
                }
                if (i > j)
                    break;
            }
            if (l < j)
                OrderFirstByTimeAndPenaltyRatio(l, j);
            if (i < r)
                OrderFirstByTimeAndPenaltyRatio(i, r);
        }

        private static void ExchangeOrders(int i, int j)
        {
            Order tmp = orders[i];
            orders[i] = orders[j];
            orders[j] = tmp;
        }
    }
}
