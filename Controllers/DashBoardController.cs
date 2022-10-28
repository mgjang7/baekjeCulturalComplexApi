using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using BaekjeCulturalComplexApi.Data;
using BaekjeCulturalComplexApi.Models;
using BaekjeCulturalComplexApi.Models.ViewModels;
using System.Collections;

namespace BaekjeCulturalComplexApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly BaekjeDbContext _dbCon;
        private readonly IConfiguration _config;

        public DashBoardController(BaekjeDbContext context, IConfiguration config)
        {
            _dbCon = context;
            _config = config;
        }

        
        /// <summary>
        /// 대시보드 누적 현황 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("cumulativeStatus")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetCumulativeStatus(string searchType)
        {
            try
            {
                // 전년 //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DashBoardViewModel prevModel = new DashBoardViewModel();

                if (searchType == "toDay") // 금일
                {
                    // 누적 입장객
                    prevModel.PayComplexDayAccumulate = 500; // 유료 - 단지 - 주간 - 누적 입장객
                    prevModel.PayComplexNightAccumulate = 200; // 유료 - 단지 - 야간 - 누적 입장객
                    prevModel.PayHistoryhallAccumulate = 300; // 유료 - 역사관 - 누적 입장객

                    prevModel.FreeComplexAccumulate = 134; // 무료 - 단지 - 누적 입장객
                    prevModel.FreeHistoryhallAccumulate = 270; // 무료 - 역사관 - 누적 입장객

                    // 누적 매출
                    prevModel.PayComplexDayAccumulateSales = 5000000; // 단지 - 주간 - 누적 매출
                    prevModel.PayComplexNightAccumulateSales = 2000000; // 단지 - 야간 - 누적 매출
                    prevModel.PayHistoryhallAccumulateSales = 3000000; // 유료 - 역사관 - 누적 매출

                    // 입장객 매출
                    prevModel.VisitorSales = prevModel.PayComplexDayAccumulateSales + prevModel.PayComplexNightAccumulateSales + prevModel.PayHistoryhallAccumulateSales; 
                    prevModel.IncidentalSales = 330000; // 부대 매출
                    prevModel.RentalSales = 350000; // 임대 매출
                    prevModel.GoodsSales = 200000; // 상품 매출
                }
                else if (searchType == "monthly") // 월간
                {
                    // 누적 입장객
                    prevModel.PayComplexDayAccumulate = 500 * 30; // 유료 - 단지 - 주간 - 누적 입장객
                    prevModel.PayComplexNightAccumulate = 200 * 30; // 유료 - 단지 - 야간 - 누적 입장객
                    prevModel.PayHistoryhallAccumulate = 300 * 30; // 유료 - 역사관 - 누적 입장객

                    prevModel.FreeComplexAccumulate = 134 * 30; // 무료 - 단지 - 누적 입장객
                    prevModel.FreeHistoryhallAccumulate = 270 * 30; // 무료 - 역사관 - 누적 입장객

                    // 누적 매출
                    prevModel.PayComplexDayAccumulateSales = 5000000 * 30; // 단지 - 주간 - 누적 매출
                    prevModel.PayComplexNightAccumulateSales = 2000000 * 30; // 단지 - 야간 - 누적 매출
                    prevModel.PayHistoryhallAccumulateSales = 3000000 * 30; // 유료 - 역사관 - 누적 매출

                    // 입장객 매출
                    prevModel.VisitorSales = prevModel.PayComplexDayAccumulateSales + prevModel.PayComplexNightAccumulateSales + prevModel.PayHistoryhallAccumulateSales;
                    prevModel.IncidentalSales = 330000 * 30; // 부대 매출
                    prevModel.RentalSales = 350000 * 30; // 임대 매출
                    prevModel.GoodsSales = 200000 * 30; // 상품 매출
                }
                else if (searchType == "yearly") // 연간
                {
                    // 누적 입장객
                    prevModel.PayComplexDayAccumulate = 500 * 30 * 12; // 유료 - 단지 - 주간 - 누적 입장객
                    prevModel.PayComplexNightAccumulate = 200 * 30 * 12; // 유료 - 단지 - 야간 - 누적 입장객
                    prevModel.PayHistoryhallAccumulate = 300 * 30 * 12; // 유료 - 역사관 - 누적 입장객

                    prevModel.FreeComplexAccumulate = 134 * 30 * 12; // 무료 - 단지 - 누적 입장객
                    prevModel.FreeHistoryhallAccumulate = 270 * 30 * 12; // 무료 - 역사관 - 누적 입장객

                    // 누적 매출
                    prevModel.PayComplexDayAccumulateSales = 5000000 * 30 * 12; // 단지 - 주간 - 누적 매출
                    prevModel.PayComplexNightAccumulateSales = 2000000 * 30 * 12; // 단지 - 야간 - 누적 매출
                    prevModel.PayHistoryhallAccumulateSales = 3000000 * 30 * 12; // 유료 - 역사관 - 누적 매출

                    // 입장객 매출
                    prevModel.VisitorSales = prevModel.PayComplexDayAccumulateSales + prevModel.PayComplexNightAccumulateSales + prevModel.PayHistoryhallAccumulateSales;
                    prevModel.IncidentalSales = 330000 * 30 * 12; // 부대 매출
                    prevModel.RentalSales = 350000 * 30 * 12; // 임대 매출
                    prevModel.GoodsSales = 200000 * 30 * 12; // 상품 매출
                }

                // 총 누적 입장객
                prevModel.AccumulateVisitorTotal = prevModel.PayComplexDayAccumulate + prevModel.PayComplexNightAccumulate + prevModel.PayHistoryhallAccumulate +
                    prevModel.FreeComplexAccumulate + prevModel.FreeHistoryhallAccumulate;

                // 단지 - 누적 매출
                prevModel.ComplexSalesTotal = prevModel.PayComplexDayAccumulateSales + prevModel.PayComplexNightAccumulateSales;

                // 총 누적 매출
                prevModel.AccumulateSalesTotal = prevModel.PayComplexDayAccumulateSales + prevModel.PayComplexNightAccumulateSales + prevModel.PayHistoryhallAccumulateSales +
                    prevModel.VisitorSales + prevModel.IncidentalSales + prevModel.RentalSales + prevModel.GoodsSales; 



                // 금년 //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                DashBoardViewModel viewModel = new DashBoardViewModel();

                if (searchType == "toDay") // 금일
                {
                    // 누적 입장객
                    viewModel.PayComplexDayAccumulate = 600; // 유료 - 단지 - 주간 - 누적 입장객
                    viewModel.PayComplexNightAccumulate = 300; // 유료 - 단지 - 야간 - 누적 입장객
                    viewModel.PayHistoryhallAccumulate = 400; // 유료 - 역사관 - 누적 입장객

                    viewModel.FreeComplexAccumulate = 234; // 무료 - 단지 - 누적 입장객
                    viewModel.FreeHistoryhallAccumulate = 370; // 무료 - 역사관 - 누적 입장객

                    // 누적 매출
                    viewModel.PayComplexDayAccumulateSales = 5500000; // 단지 - 주간 - 누적 매출
                    viewModel.PayComplexNightAccumulateSales = 3000000; // 단지 - 야간 - 누적 매출
                    viewModel.ComplexSalesTotal = viewModel.PayComplexDayAccumulateSales + viewModel.PayComplexNightAccumulateSales; // 단지 - 누적 매출
                    viewModel.PayHistoryhallAccumulateSales = 4000000; // 유료 - 역사관 - 누적 매출

                    // 입장객 매출
                    viewModel.VisitorSales = viewModel.PayComplexDayAccumulateSales + viewModel.PayComplexNightAccumulateSales + viewModel.PayHistoryhallAccumulateSales;
                    // 부대 매출
                    viewModel.IncidentalSales = 430000;
                    // 임대 매출
                    viewModel.RentalSales = 450000;
                    // 상품 매출
                    viewModel.GoodsSales = 300000; 
                }
                else if (searchType == "monthly") // 월간
                {
                    // 누적 입장객
                    viewModel.PayComplexDayAccumulate = 600 * 30; // 유료 - 단지 - 주간 - 누적 입장객
                    viewModel.PayComplexNightAccumulate = 300 * 30; // 유료 - 단지 - 야간 - 누적 입장객
                    viewModel.PayHistoryhallAccumulate = 400 * 30; // 유료 - 역사관 - 누적 입장객

                    viewModel.FreeComplexAccumulate = 234 * 30; // 무료 - 단지 - 누적 입장객
                    viewModel.FreeHistoryhallAccumulate = 370 * 30; // 무료 - 역사관 - 누적 입장객

                    // 누적 매출ㅁ
                    viewModel.PayComplexDayAccumulateSales = 5500000 * 30; // 단지 - 주간 - 누적 매출
                    viewModel.PayComplexNightAccumulateSales = 3000000 * 30; // 단지 - 야간 - 누적 매출
                    viewModel.ComplexSalesTotal = viewModel.PayComplexDayAccumulateSales + viewModel.PayComplexNightAccumulateSales; // 단지 - 누적 매출
                    viewModel.PayHistoryhallAccumulateSales = 4000000 * 30; // 유료 - 역사관 ㅁ- 누적 매출

                    // 입장객 매출
                    viewModel.VisitorSales = viewModel.PayComplexDayAccumulateSales + viewModel.PayComplexNightAccumulateSales + viewModel.PayHistoryhallAccumulateSales;
                    // 부대 매출
                    viewModel.IncidentalSales = 430000 * 30;
                    // 임대 매출
                    viewModel.RentalSales = 450000 * 30;
                    // 상품 매출
                    viewModel.GoodsSales = 300000 * 30;
                }
                else if (searchType == "yearly") // 연간
                {
                    // 누적 입장객
                    viewModel.PayComplexDayAccumulate = 600 * 30 * 12; // 유료 - 단지 - 주간 - 누적 입장객
                    viewModel.PayComplexNightAccumulate = 300 * 30 * 12; // 유료 - 단지 - 야간 - 누적 입장객
                    viewModel.PayHistoryhallAccumulate = 400 * 30 * 12; // 유료 - 역사관 - 누적 입장객

                    viewModel.FreeComplexAccumulate = 234 * 30 * 12; // 무료 - 단지 - 누적 입장객
                    viewModel.FreeHistoryhallAccumulate = 370 * 30 * 12; // 무료 - 역사관 - 누적 입장객

                    // 누적 매출
                    viewModel.PayComplexDayAccumulateSales = 5500000 * 30 * 12; // 단지 - 주간 - 누적 매출
                    viewModel.PayComplexNightAccumulateSales = 3000000 * 30 * 12; // 단지 - 야간 - 누적 매출
                    viewModel.ComplexSalesTotal = viewModel.PayComplexDayAccumulateSales + viewModel.PayComplexNightAccumulateSales; // 단지 - 누적 매출
                    viewModel.PayHistoryhallAccumulateSales = 4000000 * 30 * 12; // 유료 - 역사관 - 누적 매출

                    // 입장객 매출
                    viewModel.VisitorSales = viewModel.PayComplexDayAccumulateSales + viewModel.PayComplexNightAccumulateSales + viewModel.PayHistoryhallAccumulateSales;
                    // 부대 매출
                    viewModel.IncidentalSales = 430000 * 30 * 12;
                    // 임대 매출
                    viewModel.RentalSales = 450000 * 30 * 12;
                    // 상품 매출
                    viewModel.GoodsSales = 300000 * 30 * 12;
                }

                viewModel.AccumulateVisitorTotal = viewModel.PayComplexDayAccumulate + viewModel.PayComplexNightAccumulate + viewModel.PayHistoryhallAccumulate +
                    viewModel.FreeComplexAccumulate + viewModel.FreeHistoryhallAccumulate; // 총 누적 입장객

                // 총 누적 입장객 증감률
                float idVisitor = viewModel.AccumulateVisitorTotal - prevModel.AccumulateVisitorTotal;
                viewModel.VisitorTotalIDRate = (idVisitor / (float)prevModel.AccumulateVisitorTotal) * 100;
                // 총 누적 입장객 증가여부
                viewModel.IsVisitorTotalIncrease = !viewModel.VisitorTotalIDRate.ToString().Contains("-");

                // 단지 - 주간 - 입장객 수 전년대비 증감율
                float complexDayID = viewModel.PayComplexDayAccumulate - prevModel.PayComplexDayAccumulate;
                viewModel.ComplexDayIDRate = (complexDayID / (float)prevModel.PayComplexDayAccumulate) * 100;
                // 단지 - 주간 - 입장객 수 전년대비 증가 여부 
                viewModel.IsComplexDayIncrease = !viewModel.ComplexDayIDRate.ToString().Contains("-");


                // 단지 - 야간 - 입장객 수 전년대비 증감율
                float complexNightID = viewModel.PayComplexNightAccumulate - prevModel.PayComplexNightAccumulate;
                viewModel.ComplexNightIDRate = (complexNightID / (float)prevModel.PayComplexNightAccumulate) * 100;
                // 단지 - 야간 - 입장객 수 전년대비 증가 여부
                viewModel.IsComplexNightIncrease = !viewModel.ComplexNightIDRate.ToString().Contains("-");


                // 역사관 - 입장객 수 전년대비 증감율
                float historyhallID = viewModel.PayHistoryhallAccumulate - prevModel.PayHistoryhallAccumulate;
                viewModel.HistoryhallIDRate = (historyhallID / (float)prevModel.PayHistoryhallAccumulate) * 100;
                // 역사관 - 입장객 수 전년대비 증가 여부
                viewModel.IsHistoryhallIncrease = !viewModel.HistoryhallIDRate.ToString().Contains("-");

                // 단지 - 입장객 수 전년대비 증감율
                float complexID = (viewModel.PayComplexDayAccumulate + viewModel.PayComplexNightAccumulate + viewModel.FreeComplexAccumulate) 
                    - (prevModel.PayComplexDayAccumulate + prevModel.PayComplexNightAccumulate + prevModel.FreeComplexAccumulate);
                viewModel.ComplexIDRate = (complexID / ((float)prevModel.PayComplexDayAccumulate + (float)prevModel.PayComplexNightAccumulate + (float)prevModel.FreeComplexAccumulate)) * 100;
                // 단지 - 입장객 수 전년대비 증가여부
                viewModel.IsComplexIncrease = !viewModel.ComplexIDRate.ToString().Contains("-");


                // 단지 - 주간 - 매출 전년대비 증감율
                var complexDaySalesID = viewModel.PayComplexDayAccumulateSales - prevModel.PayComplexDayAccumulateSales;
                viewModel.ComplexDayIDRateSales = (complexDaySalesID / (float)prevModel.PayComplexDayAccumulateSales) * 100;
                // 단지 - 주간 - 매출 전년대비 증가 여부
                viewModel.IsComplexDayIncreaseSales = !viewModel.ComplexDayIDRateSales.ToString().Contains("-");

                // 단지 - 야간 - 매출 전년대비 증감율
                var complexNightSalesID = viewModel.PayComplexNightAccumulateSales - prevModel.PayComplexNightAccumulateSales;
                viewModel.ComplexNightIDRateSales = (complexNightSalesID / (float)prevModel.PayComplexNightAccumulateSales) * 100;
                // 단지 - 야간 - 매출 전년대비 증가 여부
                viewModel.IsComplexNightIncreaseSales = !viewModel.ComplexNightIDRateSales.ToString().Contains("-");

                // 역사관 - 매출 전년대비 증감율
                var historyhallSalesID = viewModel.PayHistoryhallAccumulateSales - prevModel.PayHistoryhallAccumulateSales;
                viewModel.HistoryhallIDRateSales = (historyhallSalesID / (float)prevModel.PayHistoryhallAccumulateSales) * 100;
                // 역사관 - 매출 전년대비 증가 여부
                viewModel.IsHistoryhallIncreaseSales = !viewModel.HistoryhallIDRateSales.ToString().Contains("-");

                // 단지 - 매출 전년대비 증감율
                var complexSalesID = viewModel.ComplexSalesTotal - prevModel.ComplexSalesTotal;
                viewModel.ComplexSalesTotalIDRate = (complexSalesID / (float)prevModel.ComplexSalesTotal) * 100;
                // 단지 - 매출 전년대비 증가 여부
                viewModel.IsComplexSalesTotalIncrease = !viewModel.ComplexSalesTotalIDRate.ToString().Contains("-");


                viewModel.AccumulateSalesTotal = viewModel.PayComplexDayAccumulateSales + viewModel.PayComplexNightAccumulateSales + viewModel.PayHistoryhallAccumulateSales +
                    viewModel.VisitorSales + viewModel.IncidentalSales + viewModel.RentalSales + viewModel.GoodsSales; // 총 누적 매출

                // 총 누적 매출 증감률
                var salesTotalID = viewModel.AccumulateSalesTotal - prevModel.AccumulateSalesTotal;
                viewModel.SalesTotalIDRate = (salesTotalID / (float)prevModel.AccumulateSalesTotal) * 100;
                // 총 누적 매출 증가여부
                viewModel.IsSalesTotalIncrease = !viewModel.SalesTotalIDRate.ToString().Contains("-");

                return new JsonResult(viewModel);
            }
            catch (Exception ex)
            {
                return new JsonResult(null);
            }
        }
    }
}