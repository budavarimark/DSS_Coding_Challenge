using System;
using System.Collections.Generic;
using System.Text;

namespace DSS_Coding_Challenge
{
    static class InputProcessor
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        static public Order[] ProcessOrdersFromString(string orderLines)
        {
            List<Order> orderList = new List<Order>();
            _log.Info("Fájl beolvasása megtörtént, feldolgozás.");

            string[] splittedOrderLines = orderLines.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            for(int i = 1; i < splittedOrderLines.Length; i++)
            {
                string orderLine = splittedOrderLines[i];
                if (orderLine.Length > 2)
                {
                    try
                    {
                        orderList.Add(CreateOrderFromString(orderLine));
                    }
                    catch
                    {
                        _log.Error("Nem sikerült feldolgozni a következő sort: " + orderLine);
                    }
                }
            }

            if (orderList.Count == 0)
                throw new Exception(message: "Egyetlen megrendelést sem sikerült értelmezni.");

            return orderList.ToArray();
        }

        // Példa: "MEGR001;GYB;1000;07.21. 13:00;1500;20 000"
        static private Order CreateOrderFromString(string orderLine)
        {
            string[] orderDetails = orderLine.Split(';');
            int penaltyForDelayPerDay = int.Parse(
                orderDetails[5].Replace(" ", string.Empty)
            );

            return new Order(
                orderDetails[0],
                GetProductFromString(orderDetails[1]),
                int.Parse(orderDetails[2]),
                StringToDateTime(orderDetails[3]),
                int.Parse(orderDetails[4]),
                penaltyForDelayPerDay
            );
        }

        /* 
         * A bemenetben a határidő "hh.nn. óó:pp" formátumban van megadva.
         * Jobb lenne, ha évszám is lenne, de hát ez van...
         */ 
        static private DateTime StringToDateTime(string dateString)
        {
            string[] seperatedBySpace = dateString.Split(' ');
            string[] monthAndDay = seperatedBySpace[0].Split('.', StringSplitOptions.RemoveEmptyEntries);
            string[] hourAndMinute = seperatedBySpace[1].Split(':');

            // TODO: Mi lesz a 2021-es megrendelésekkel? 
            return new DateTime(
                2020, 
                int.Parse(monthAndDay[0]),
                int.Parse(monthAndDay[1]), 
                int.Parse(hourAndMinute[0]),
                int.Parse(hourAndMinute[1]),
                0
            );
        }

        static private Product GetProductFromString(string productString) => productString switch
        {
            "GYB" => Product.KidBicycleFrame,
            "SB" => Product.AdolescentBicycleFrame,
            "FB" => Product.AdultBicycleFrame,
            _ => throw new ArgumentException(message: "Hibás enum érték"),
        };
    }
}
