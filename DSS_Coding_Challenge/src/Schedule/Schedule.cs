using System;
using System.Collections.Generic;
using System.Text;

namespace DSS_Coding_Challenge
{
    class Schedule
    {
        DateTime startingDate;
        DateTime stoppingDate;
        string machine;
        string orderId;

        public Schedule(DateTime startingDate, DateTime stoppingDate, string machine, string orderId)
        {
            this.startingDate = startingDate;
            this.stoppingDate = stoppingDate;
            this.machine      = machine;
            this.orderId      = orderId;
        }

        public class Comperator : IComparer<Schedule>
        {
            public int Compare(Schedule sch_a, Schedule sch_b)
            {
                return sch_a.startingDate.Subtract(sch_b.startingDate).TotalSeconds < 0 ? -1:0;
            }
        }

        // Dátum; Gép; Kezdő időpont; Záró időpont; Megrendelésszám
        public override string ToString()
        {
            string date      = startingDate.ToShortDateString().Replace(" ", "");
            string startTime = startingDate.Hour + ":" + startingDate.Minute.ToString("D2");
            string stopTime  = stoppingDate.Hour + ":" + stoppingDate.Minute.ToString("D2");

            return date + ";" + machine + ";" + startTime + ";" + stopTime + ";" + orderId;
        }
    }
}
