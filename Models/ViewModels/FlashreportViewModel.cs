namespace BaekjeCulturalComplexApi.Models.ViewModels
{
    public class FlashreportViewModel
    {
        /// <summary>
        /// 총 입장객/매출 구분
        /// </summary>
        public string TotalDivision { get; set; }
        /// <summary>
        /// 구분
        /// </summary>
        public string Division { get; set; }
        /// <summary>
        /// 항목
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        /// 목표 - 월
        /// </summary>
        public long TargetMonth { get; set; }
        /// <summary>
        /// 목표 -년
        /// </summary>
        public long TargetYear { get; set; }
        /// <summary>
        /// 누계(년)
        /// </summary>
        public long TargetTotal { get; set; }
        /// <summary>
        /// 금일 실적
        /// </summary>
        public long TodaySales { get; set; }
        /// <summary>
        /// 월누계 - 전년
        /// </summary>
        public long MonthTotalPreviousYear { get; set; }
        /// <summary>
        /// 월누계 - 금년
        /// </summary>
        public long MonthTotalThisYear { get; set; }
        /// <summary>
        /// 월누계 - 증가율
        /// </summary>
        public float MonthTotalRateIncrease { get; set; }
        /// <summary>
        /// 월누계 - 목표대비
        /// </summary>
        public float MonthTotalAgainstTarget { get; set; }
        /// <summary>
        /// 년누계 - 전년
        /// </summary>
        public long YearTotalPreviousYear { get; set; }
        /// <summary>
        /// 년누계 - 금년
        /// </summary>
        public long YearTotalThisYear { get; set; }
        /// <summary>
        /// 년누계 - 증가율
        /// </summary>
        public float YearTotalRateIncrease { get; set; }

        /// <summary>
        /// 년누계 - 목표대비
        /// </summary>
        public float YearTotalAgainstTarget { get; set; }
    }
}

