using System;
using System.Collections.Generic;

namespace Spotify
{
    public class ChartReport
    {
        public string country { get; set; }
        public DateTime weekStart { get; set; }
        public DateTime weekEnd { get; set; }
        public List<ChartReportTrack> chartReport { get; set; }
        public ChartReport()
        {
            chartReport = new List<ChartReportTrack>();
        }
    }
}
