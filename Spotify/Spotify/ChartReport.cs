using System;
using System.Collections.Generic;

namespace Spotify
{
    public class ChartReport
    {
        public string country { get; set; }
        public DateTime weekStart { get; set; }
        public DateTime weekEnd { get; set; }
        public List<ReportTrack> chartReport { get; set; }
        public ChartReport()
        {
            chartReport = new List<ReportTrack>();
        }
    }
}
