using System;
using System.Collections.Generic;
using System.Text;

namespace DSS_Coding_Challenge
{
    static class WorkScheduleGenerator
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        static public List<Schedule> scheduleList = new List<Schedule>();

        static public void GenerateFromOrders(Order[] orders)
        {
            foreach (Order order in orders)
            {
                _log.Info(order.id + " megrendelés munkarendjének generálása.");
                foreach (Machine machine in Enum.GetValues(typeof(Machine)))
                    GenerateScheduleByOrderAndMachineType(order, machine);
            }

            scheduleList.Sort(new Schedule.Comperator());
        }

        static private void GenerateScheduleByOrderAndMachineType(Order order, Machine machine)
        {
            DateTime currentDate    = order.startDate;
            int      howManyDayAway = order.endDate.Subtract(order.startDate).Days;
            int      machineCount   = Settings.alreadyOptimizedMachineQuantity[(int)order.product, (int)machine];
            double   startingHour;
            double   startingMinute;
            DateTime dayEnd;

            if (howManyDayAway > 0)
            {
                for (int i = 0; i < howManyDayAway; i++) {
                    startingHour   = currentDate.Hour;
                    startingMinute = currentDate.Minute;

                    dayEnd = currentDate
                        .AddHours(Settings.WORK_DAY_ENDING_HOUR - startingHour - 1)
                        .AddMinutes(60 - startingMinute);

                    for (int k = 1; k <= machineCount; k++)
                        scheduleList.Add(new Schedule(currentDate, dayEnd, machine.GetString() + "-" + k, order.id));
                    
                    currentDate = dayEnd.AddHours(24-Settings.WORK_HOUR_PER_DAY);
                }
            }

            for (int k = 1; k <= machineCount; k++)
                scheduleList.Add(new Schedule(currentDate, order.endDate, machine.GetString() + "-" + k, order.id));
        }
    }
}
