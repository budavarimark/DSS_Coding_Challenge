using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DSS_Coding_Challenge
{
    /*
     * Segédosztály. Az optimális géphasználati felosztás kiszámításához.
     */
    class Optimizer
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        int[] wantedMachineQuantitySum = { 0, 0, 0, 0, 0, 0 };
        int TESTING_STEP_INDEX = (int)Machine.Tester;

        public class OrderToOptimize
        {
            public Order    order { get; set; }
            public int[]    wantedMachineQuantity { get; set; }
            public double[] speedTable { get; set; }
            public bool     isOptimized { get; set; }

            public double   maxValue;
            public int      maxIndex;

            public OrderToOptimize(Order order)
            {
                this.order            = order;
                wantedMachineQuantity = new [] { 1, 1, 1, 1, 1, 1};
                speedTable            = new double[] { 0, 0, 0, 0, 0, 0 };
                isOptimized           = false;
            }
        }

        List<OrderToOptimize> ordersToOptimize = new List<OrderToOptimize>();

        public OrderToOptimize AddOrderToOptimize(Order order)
        {
            if(ordersToOptimize.Count == 2)
            {
                throw new Exception(message: "Túl sok megrendelés az optimalizáláshoz." +
                    "Kérem próbáljon egy vagy kettő megrendelés optimalizálni egyszerre.");
            }
            var oto = new OrderToOptimize(order);
            ordersToOptimize.Add(oto);
            return oto;
        }

        private void SumWantedMachineQuantity()
        {
            wantedMachineQuantitySum = new[] { 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < ordersToOptimize.Count; i++)
                for (int k = 0; k < ordersToOptimize[i].wantedMachineQuantity.Length; k++)
                    wantedMachineQuantitySum[k] += ordersToOptimize[i].wantedMachineQuantity[k];
        }

        private void SetSpeedTable(int index)
        {
            foreach (var orderToOptimize in ordersToOptimize)
            {
                double minuteCost = (double)Settings.minuteCostsByProduct[(int)orderToOptimize.order.product, index];
                double wantedMachineQuantity = (double)orderToOptimize.wantedMachineQuantity[index];

                orderToOptimize.speedTable[index] = minuteCost / wantedMachineQuantity;
            }
        }

        private void SetSpeedTableIfThereIsNoEnoughMachine(int howManyMachine)
        {
            foreach (var orderToOptimize in ordersToOptimize)
                orderToOptimize.speedTable[TESTING_STEP_INDEX] = 
                    Settings.minuteCostsByProduct[(int)orderToOptimize.order.product, TESTING_STEP_INDEX] * howManyMachine;
        }

        private void ProcessWantedMachineQuantity()
        {
            SumWantedMachineQuantity();

            for (int i = 0; i < wantedMachineQuantitySum.Length; i++)
            {
                if (Settings.availableMachineQuantity[i] >= wantedMachineQuantitySum[i])
                    SetSpeedTable(i);
                else if (i == TESTING_STEP_INDEX)
                    SetSpeedTableIfThereIsNoEnoughMachine(wantedMachineQuantitySum[i]);
                else
                    throw new Exception(message: "Valami hiba történt az optimalizálóban");
            }
        }

        // A jelenlegi gyártási sebességgel hány terméket gyártunk percenként?
        private double CalculateOptimizedFunctionValue()
        {
            ProcessWantedMachineQuantity();
            double optimizedFunctionValue = 0;

            foreach (var orderToOptimize in ordersToOptimize)
                optimizedFunctionValue += 1 / orderToOptimize.speedTable.Max();

            return optimizedFunctionValue;
        }

        private bool IsThereAOptPossibility()
        {
            bool    foundAWayToOptimize = false;
            int     maxIndex;
            double  maxValue;

            foreach (var orderToOptimize in ordersToOptimize) {
                if (!orderToOptimize.isOptimized)
                {
                    maxIndex = 0;
                    maxValue = orderToOptimize.speedTable[0];

                    for (int k = 1; k < orderToOptimize.speedTable.Length; k++)
                    {
                        if (orderToOptimize.speedTable[k] >  maxValue ||
                           (orderToOptimize.speedTable[k] == maxValue &&
                            Settings.availableMachineQuantity[k]   >= wantedMachineQuantitySum[k] + 1))
                        {
                            maxIndex = k;
                            maxValue = orderToOptimize.speedTable[k];
                        }
                    }
            
                    if (Settings.availableMachineQuantity[maxIndex] < wantedMachineQuantitySum[maxIndex] + 1)
                        orderToOptimize.isOptimized = true;
                    else
                    {
                        orderToOptimize.maxValue = maxValue;
                        orderToOptimize.maxIndex = maxIndex;
                        foundAWayToOptimize = true;
                    }
                }
            }

            return foundAWayToOptimize;
        }

        private void OptimizeWantedMachineQuantity()
        {
            double  maxValue = -1;
            int     maxIndex = -1;
            OrderToOptimize maxOrder = ordersToOptimize[0];

            foreach (var orderToOptimize in ordersToOptimize)
            {
                if (!orderToOptimize.isOptimized)
                {
                    if (orderToOptimize.maxValue > maxValue)
                    {
                        maxValue = orderToOptimize.maxValue;
                        maxOrder = orderToOptimize;
                        maxIndex = orderToOptimize.maxIndex;
                    }
                }
            }

            maxOrder.wantedMachineQuantity[maxIndex] = maxOrder.wantedMachineQuantity[maxIndex] + 1;
        }

        /*
         * KidBicycleFrame:         {1, 2, 2, 1, 3, 2}
         * AdolescentBicycleFrame:  {1, 2, 2, 1, 2, 2}
         * AdultBicycleFrame:       {1, 2, 2, 1, 3, 2}
         */
        public void DebugWantedMachineQuantity()
        {
            foreach (var orderToOptimize in ordersToOptimize)
            {
                _log.Debug(orderToOptimize.order.product+": {");
                for (int i = 0; i < orderToOptimize.wantedMachineQuantity.Length; i++)
                {
                    if(i+1 == orderToOptimize.wantedMachineQuantity.Length)
                        _log.Debug(orderToOptimize.wantedMachineQuantity[i]);
                    else
                        _log.Debug(orderToOptimize.wantedMachineQuantity[i] + ", ");
                }
                _log.Debug("}");
            }
        }

        /*
         * Az optimalizálás során maximalizáltuk a gépek használatát, hogy minimalizáljuk a bottleneck maximumát.
         * De igazából nem kell minden gép, hogy elérjük azt a keresett értéket. 
         * Tehát addig csökkentjük a gépek számát, ameddig a minimalizált maximum érték állandó.
         */
        private void MinimizeMachineQuantity(double maxValue)
        {
            foreach(var orderToOptimize in ordersToOptimize)
            {
                for (int i = 0; i < orderToOptimize.wantedMachineQuantity.Length; i++) {
                    orderToOptimize.wantedMachineQuantity[i]--;
                    if(CalculateOptimizedFunctionValue() != maxValue)
                    {
                        orderToOptimize.wantedMachineQuantity[i]++;
                    }
                }
            }
        }

        private void DebugLog()
        {
            foreach (var orderToOptimize in ordersToOptimize)
            {
                _log.Debug("speedTable.Max(): " + orderToOptimize.speedTable.Max());
            }
        }

        // Maximalizáljuk a gyártási sebességet a bottleneck minimalizálásával
        public double Optimize()
        {
            double  maxValue       = CalculateOptimizedFunctionValue();
            bool    stop           = !IsThereAOptPossibility();
            double  currentValue;

            while (!stop)
            {
                OptimizeWantedMachineQuantity();
                currentValue = CalculateOptimizedFunctionValue();

                if (maxValue <= currentValue)
                    maxValue = currentValue;

                stop = !IsThereAOptPossibility();
            }

            DebugLog();
            MinimizeMachineQuantity(maxValue);
            return maxValue;
        }
    }
}
