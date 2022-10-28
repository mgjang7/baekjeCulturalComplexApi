namespace BaekjeCulturalComplexApi.Models.ViewModels
{
    public class DashBoardViewModel
    {
        /// <summary>
        /// 유료 - 단지 - 주간 - 누적 입장객 수
        /// </summary>
        public int PayComplexDayAccumulate { get; set; }

        /// <summary>
        /// /// 유료 - 단지 - 야간 - 누적 입장객 수
        /// </summary>
        public int PayComplexNightAccumulate { get; set; }

        /// <summary>
        /// 유료 - 역사관 - 누적 입장객 수
        /// </summary>
        public int PayHistoryhallAccumulate { get; set; }

        /// <summary>
        /// 무료 - 단지 - 누적 입장객 수
        /// </summary>
        public int FreeComplexAccumulate { get; set; }

        /// <summary>
        /// 무료 - 역사관 - 누적 입장객 수
        /// </summary>
        public int FreeHistoryhallAccumulate { get; set; }

        /// <summary>
        /// 단지 - 주간 - 입장객 수 전년대비 증감율
        /// </summary>
        public float ComplexDayIDRate { get; set; }

        /// <summary>
        /// 단지 - 주간 - 입장객 수 전년대비 증가/감소 여부
        /// </summary>
        public bool IsComplexDayIncrease { get; set; }

        /// <summary>
        /// 단지 - 야간 - 입장객 수 전년대비 증감율
        /// </summary>
        public float ComplexNightIDRate { get; set; }

        /// <summary>
        /// 단지 - 야간 - 입장객 수 전년대비 증가/감소 여부
        /// </summary>
        public bool IsComplexNightIncrease { get; set; }

        /// <summary>
        /// 역사관 - 입장객 수 전년대비 증감율
        /// </summary>
        public float HistoryhallIDRate { get; set; }

        /// <summary>
        /// 역사관 - 입장객 수 전년대비 증가/감소 여부
        /// </summary>
        public bool IsHistoryhallIncrease { get; set; }

        //////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// 유료 - 단지 - 주간 - 누적 매출
        /// </summary>
        public long PayComplexDayAccumulateSales { get; set; }

        /// <summary>
        /// /// 유료 - 단지 - 야간 - 누적 매출
        /// </summary>
        public long PayComplexNightAccumulateSales { get; set; }

        /// <summary>
        /// 유료 - 역사관 - 누적 매출
        /// </summary>
        public long PayHistoryhallAccumulateSales { get; set; }

        /// <summary>
        /// 단지 - 주간 - 매출 전년대비 증감율
        /// </summary>
        public float ComplexDayIDRateSales { get; set; }

        /// <summary>
        /// 단지 - 주간 - 매출 전년대비 증가/감소 여부
        /// </summary>
        public bool IsComplexDayIncreaseSales { get; set; }

        /// <summary>
        /// 단지 - 야간 - 매출 전년대비 증감율
        /// </summary>
        public float ComplexNightIDRateSales { get; set; }

        /// <summary>
        /// 단지 - 야간 - 매출 전년대비 증가/감소 여부
        /// </summary>
        public bool IsComplexNightIncreaseSales { get; set; }

        /// <summary>
        /// 역사관 - 매출 전년대비 증감율
        /// </summary>
        public float HistoryhallIDRateSales { get; set; }

        /// <summary>
        /// 역사관 - 매출 전년대비 증가/감소 여부
        /// </summary>
        public bool IsHistoryhallIncreaseSales { get; set; }

        /// <summary>
        /// 입장객 매출
        /// </summary>
        public long VisitorSales { get; set; }

        /// <summary>
        /// 부대 매출
        /// </summary>
        public long IncidentalSales { get; set; }

        /// <summary>
        /// 임대 매출
        /// </summary>
        public long RentalSales { get; set; }

        /// <summary>
        /// 상품 매출
        /// </summary>
        public long GoodsSales { get; set; }

        //////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// 총 누적 입장객
        /// </summary>
        public int AccumulateVisitorTotal { get; set; }
        /// <summary>
        /// 총 누적 입장객 증감률
        /// </summary>
        public float VisitorTotalIDRate { get; set; }
        /// <summary>
        /// 총 누적 입장객 증가여부
        /// </summary>
        public bool IsVisitorTotalIncrease { get; set; }

        /// <summary>
        /// 총 누적 매출
        /// </summary>
        public long AccumulateSalesTotal { get; set; }
        /// <summary>
        /// 총 누적 매출 증감률
        /// </summary>
        public float SalesTotalIDRate { get; set; }
        /// <summary>
        /// 총 누적 매출 증가여부
        /// </summary>
        public bool IsSalesTotalIncrease { get; set; }

        /// <summary>
        /// 단지 - 누적 입장객 수
        /// </summary>
        public int ComplexAccumulate { get; set; }
        /// <summary>
        /// 단지 - 전년대비 증감률
        /// </summary>
        public float ComplexIDRate { get; set; }
        /// <summary>
        /// 단지 - 전년대비 증가여부
        /// </summary>
        public bool IsComplexIncrease { get; set; }


        /// <summary>
        /// 단지 - 누적 매출
        /// </summary>
        public long ComplexSalesTotal { get; set; }
        /// <summary>
        /// 단지 - 누적 매출 증감률
        /// </summary>
        public float ComplexSalesTotalIDRate { get; set; }
        /// <summary>
        /// 단지 - 누적 매출 증가여부
        /// </summary>
        public bool IsComplexSalesTotalIncrease { get; set; }
    }
}
