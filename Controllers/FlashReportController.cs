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
using System.Collections;
using BaekjeCulturalComplexApi.Models.ViewModels;
using Newtonsoft.Json;

namespace BaekjeCulturalComplexApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashReportController : ControllerBase
    {
        private readonly BaekjeDbContext _dbCon;
        private readonly IConfiguration _config;

        public FlashReportController(BaekjeDbContext context, IConfiguration config)
        {
            _dbCon = context;
            _config = config;
        }

        /// <summary>
        /// 영업속보 > 연간목표 등록
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("CorsPolicyName")]
        public JsonResult Post(FlashreportTargetYearAddModel addData)
        {
            try
            {
                int targetYear = addData.TargetYear;
                string jsonStr = addData.json;

                var flashreportTargetYearDatas = JsonConvert.DeserializeObject<List<FlashreportTargetYearViewModel>>(jsonStr);

                // 등록 대상연도에 이미 등록되어 있는 데이터가 있을때 기존 데이터 수정
                var checkDatas = _dbCon.FlashreportTargetYears.Where(p => p.TargetYear == targetYear).ToList();

                if (checkDatas.Count > 0)
                {
                    foreach (var item in flashreportTargetYearDatas) 
                    {
                        var checkData = checkDatas.Where(p => p.TargetYear == targetYear && p.TargetMonth == item.month).FirstOrDefault();

                        if (checkData != null)
                        {
                            checkData.PayComplexDayVisitor = item.col1;
                            checkData.PayComplexNightVisitor = item.col2;
                            checkData.PayHistoryhallVisitor = item.col3;
                            checkData.FreeComplexVisitor = item.col4;
                            checkData.FreeHistoryhallVisitor = item.col5;
                            checkData.PayComplexDaySales = item.col6;
                            checkData.PayComplexNightSales = item.col7;
                            checkData.PayHistoryhallSales = item.col8;
                            checkData.IncidentalSabiroSales = item.col9;
                            checkData.IncidentalExperienceSales = item.col10;
                            checkData.IncidentalRentalSales = item.col11;
                            checkData.IncidentalGoodsSales = item.col12;
                            checkData.IncidentalRmeSales = item.col13;
                            checkData.RentalSales = item.col14;

                            checkData.UpdateDate = DateTime.Now;

                            _dbCon.SaveChanges();
                        }
                        else 
                        {
                            return new JsonResult(null);
                        }
                    }

                    return new JsonResult("Add Success");
                }
                else // 새로 등록
                {
                    if (flashreportTargetYearDatas.Count > 0)
                    {
                        foreach (var item in flashreportTargetYearDatas)
                        {
                            FlashreportTargetYear model = new FlashreportTargetYear();

                            model.Seq = new Guid();
                            model.TargetYear = targetYear;
                            model.TargetMonth = item.month;

                            model.PayComplexDayVisitor = item.col1;
                            model.PayComplexNightVisitor = item.col2;
                            model.PayHistoryhallVisitor = item.col3;
                            model.FreeComplexVisitor = item.col4;
                            model.FreeHistoryhallVisitor = item.col5;
                            model.PayComplexDaySales = item.col6;
                            model.PayComplexNightSales = item.col7;
                            model.PayHistoryhallSales = item.col8;
                            model.IncidentalSabiroSales = item.col9;
                            model.IncidentalExperienceSales = item.col10;
                            model.IncidentalRentalSales = item.col11;
                            model.IncidentalGoodsSales = item.col12;
                            model.IncidentalRmeSales = item.col13;
                            model.RentalSales = item.col14;

                            model.RegDate = DateTime.Now;
                            model.UpdateDate = DateTime.Now;

                            _dbCon.FlashreportTargetYears.Add(model);
                            _dbCon.SaveChanges();
                        }
                    }

                    return new JsonResult("Add Success");
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(null);
            }
        }

        /// <summary>
        /// 영업속보 > 연간목표 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("flashreportTargetYear")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetFlashreportTargetYear(int targetYear)
        {
            try
            {
                var datas = _dbCon.FlashreportTargetYears.Where(p => p.TargetYear == targetYear).ToList();

                if (datas.Count > 0)
                {
                    List<FlashreportTargetYearViewModel> vModels = new List<FlashreportTargetYearViewModel>();

                    foreach (var item in datas) 
                    {
                        FlashreportTargetYearViewModel vModel = new FlashreportTargetYearViewModel();

                        vModel.month = item.TargetMonth;

                        vModel.col1 = item.PayComplexDayVisitor;
                        vModel.col2 = item.PayComplexNightVisitor;
                        vModel.col3 = item.PayHistoryhallVisitor;
                        vModel.col4 = item.FreeComplexVisitor;
                        vModel.col5 = item.FreeHistoryhallVisitor;
                        vModel.col6 = item.PayComplexDaySales;
                        vModel.col7 = item.PayComplexNightSales;
                        vModel.col8 = item.PayHistoryhallSales;
                        vModel.col9 = item.IncidentalSabiroSales;
                        vModel.col10 = item.IncidentalExperienceSales;
                        vModel.col11 = item.IncidentalRentalSales;
                        vModel.col12 = item.IncidentalGoodsSales;
                        vModel.col13 = item.IncidentalRmeSales;
                        vModel.col14 = item.RentalSales;

                        vModels.Add(vModel);
                    }

                    return new JsonResult(vModels);
                }
                else
                {
                    return new JsonResult("");
                }

            }
            catch (Exception ex)
            {
                return new JsonResult(null);
            }
        }

        /// <summary>
        /// 영업속보 조회
        /// </summary>
        /// <param name="targetDate"></param>
        /// <returns></returns>
        [HttpGet("flashreport")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetFlashreport(string targetDate) 
        {
            try
            {
                DateTime tDate = Convert.ToDateTime(targetDate);

                var datas = _dbCon.FlashreportTargetYears.Where(p => p.TargetYear == tDate.Year).ToList();

                if (datas.Count > 0)
                {
                    List<FlashreportViewModel> viewModels = new List<FlashreportViewModel>();

                    long visitorTargetMonthTotal = 0; // 입장객 목표(월) 합계
                    long visitorTargetYearTotal = 0; // 입장객 목표(년) 합계
                    long visitorTargetTotalTotal = 0; // 입장객 누계(년) 합계

                    long salesTargetMonthTotal = 0; // 매출 목표(월) 합계
                    long salesTargetYearTotal = 0; // 매출 목표(년) 합계
                    long salesTargetTotalTotal = 0; // 매출 누계(년) 합계



                    long payComplexVisitor1 = 0; // 유료 단지 입장객 수(목표(월))
                    long payComplexSales1 = 0; // 유료 단지 매출(목표(월))

                    long payComplexVisitor2 = 0; // 유료 단지 입장객 수(목표(년))
                    long payComplexSales2 = 0; // 유료 단지 매출(목표(년))

                    long payComplexVisitor3 = 0; // 유료 단지 입장객 수(누계(년))
                    long payComplexSales3 = 0; // 유료 단지 매출(누계(년))

                    long payHistoryhallVisitor1 = 0; // 유료 역사관 입장객 수(목표(월))
                    long payHistoryhallSales1 = 0; // 유료 역사관 매출(목표(월))

                    long payHistoryhallVisitor2 = 0; // 유료 역사관 입장객 수(목표(년))
                    long payHistoryhallSales2 = 0; // 유료 역사관 매출(목표(년))

                    long payHistoryhallVisitor3 = 0; // 유료 역사관 입장객 수(누계(년))
                    long payHistoryhallSales3 = 0; // 유료 역사관 매출(누계(년))

                    FlashreportViewModel viewModel = new FlashreportViewModel();
                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "유료입장객";
                    viewModel.Item = "단지";
                    viewModel.TargetMonth = 0;
                    viewModel.TargetYear = 0;
                    viewModel.TargetTotal = 0;
                    viewModel.TodaySales = 0;
                    viewModel.MonthTotalPreviousYear = 0;
                    viewModel.MonthTotalThisYear = 0;
                    viewModel.MonthTotalRateIncrease = 0;
                    viewModel.MonthTotalAgainstTarget = 0;
                    viewModel.YearTotalPreviousYear = 0;
                    viewModel.YearTotalThisYear = 0;
                    viewModel.YearTotalRateIncrease = 0;
                    viewModel.YearTotalAgainstTarget = 0;
                    viewModels.Add(viewModel);


                    viewModel = new FlashreportViewModel();
                    
                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "유료입장객";
                    viewModel.Item = "ㄴ 주간";

                    viewModel.TargetMonth =  datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.PayComplexDayVisitor).FirstOrDefault();
                    payComplexVisitor1 += viewModel.TargetMonth;

                    viewModel.TargetYear = datas.Sum(p => p.PayComplexDayVisitor);
                    payComplexVisitor2 += viewModel.TargetYear;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.PayComplexDayVisitor);
                    payComplexVisitor3 += viewModel.TargetTotal;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);


                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "유료입장객";
                    viewModel.Item = "ㄴ 야간";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.PayComplexNightVisitor).FirstOrDefault();
                    payComplexVisitor1 += viewModel.TargetMonth;

                    viewModel.TargetYear = datas.Sum(p => p.PayComplexNightVisitor);
                    payComplexVisitor2 += viewModel.TargetYear;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.PayComplexNightVisitor);
                    payComplexVisitor3 += viewModel.TargetTotal;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);


                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "유료입장객";
                    viewModel.Item = "역사관";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.PayHistoryhallVisitor).FirstOrDefault();
                    payHistoryhallVisitor1 += viewModel.TargetMonth;

                    viewModel.TargetYear = datas.Sum(p => p.PayHistoryhallVisitor);
                    payHistoryhallVisitor2 += viewModel.TargetYear;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.PayHistoryhallVisitor);
                    payHistoryhallVisitor3 += viewModel.TargetTotal;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);



                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "유료입장객";
                    viewModel.Item = "소계";

                    var targetMonthData = datas.Where(p => p.TargetMonth == tDate.Month).FirstOrDefault();

                    viewModel.TargetMonth = targetMonthData.PayComplexDayVisitor + targetMonthData.PayComplexNightVisitor + targetMonthData.PayHistoryhallVisitor;
                    viewModel.TargetYear = datas.Sum(p => p.PayComplexDayVisitor) + datas.Sum(p => p.PayComplexNightVisitor) + datas.Sum(p => p.PayHistoryhallVisitor);
                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.PayComplexDayVisitor) +
                           datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.PayComplexNightVisitor) +
                           datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.PayHistoryhallVisitor);

                    visitorTargetMonthTotal += viewModel.TargetMonth;
                    visitorTargetYearTotal += viewModel.TargetYear;
                    visitorTargetTotalTotal += viewModel.TargetTotal;

                    viewModel.TodaySales = 15000;
                    viewModel.MonthTotalPreviousYear = 300000;
                    viewModel.MonthTotalThisYear = 330000;
                    viewModel.MonthTotalRateIncrease = 30;
                    viewModel.MonthTotalAgainstTarget = 15;
                    viewModel.YearTotalPreviousYear = 300000;
                    viewModel.YearTotalThisYear = 330000;
                    viewModel.YearTotalRateIncrease = 30;
                    viewModel.YearTotalAgainstTarget = 15;

                    viewModels.Add(viewModel);


                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "무료입장객";
                    viewModel.Item = "단지";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.FreeComplexVisitor).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => p.FreeComplexVisitor);
                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.FreeComplexVisitor);

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);


                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "무료입장객";
                    viewModel.Item = "역사관";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.FreeHistoryhallVisitor).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => p.FreeHistoryhallVisitor);
                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.FreeHistoryhallVisitor);

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);


                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "무료입장객";
                    viewModel.Item = "소계";


                    viewModel.TargetMonth = targetMonthData.FreeComplexVisitor + targetMonthData.FreeHistoryhallVisitor;
                    viewModel.TargetYear = datas.Sum(p => p.FreeComplexVisitor) + datas.Sum(p => p.FreeHistoryhallVisitor);
                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.FreeComplexVisitor) +
                           datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => p.FreeHistoryhallVisitor);

                    visitorTargetMonthTotal += viewModel.TargetMonth;
                    visitorTargetYearTotal += viewModel.TargetYear;
                    visitorTargetTotalTotal += viewModel.TargetTotal;

                    viewModel.TodaySales = 1000;
                    viewModel.MonthTotalPreviousYear = 200000;
                    viewModel.MonthTotalThisYear = 220000;
                    viewModel.MonthTotalRateIncrease = 20;
                    viewModel.MonthTotalAgainstTarget = 10;
                    viewModel.YearTotalPreviousYear = 200000;
                    viewModel.YearTotalThisYear = 220000;
                    viewModel.YearTotalRateIncrease = 20;
                    viewModel.YearTotalAgainstTarget = 10;

                    viewModels.Add(viewModel);


                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 입장객(명)";
                    viewModel.Division = "합계";
                    viewModel.Item = "합계";

                    viewModel.TargetMonth = visitorTargetMonthTotal;
                    viewModel.TargetYear = visitorTargetYearTotal;
                    viewModel.TargetTotal = visitorTargetTotalTotal;

                    viewModel.TodaySales = 0;
                    viewModel.MonthTotalPreviousYear = 0;
                    viewModel.MonthTotalThisYear = 0;
                    viewModel.MonthTotalRateIncrease = 0;
                    viewModel.MonthTotalAgainstTarget = 0;
                    viewModel.YearTotalPreviousYear = 0;
                    viewModel.YearTotalThisYear = 0;
                    viewModel.YearTotalRateIncrease = 0;
                    viewModel.YearTotalAgainstTarget = 0;

                    viewModels.Add(viewModel);

                    // 총 매출 ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "유료입장객매출";
                    viewModel.Item = "단지";

                    viewModel.TargetMonth = 0;
                    viewModel.TargetYear = 0;
                    viewModel.TargetTotal = 0;

                    viewModel.TodaySales = 0;
                    viewModel.MonthTotalPreviousYear = 0;
                    viewModel.MonthTotalThisYear = 0;
                    viewModel.MonthTotalRateIncrease = 0;
                    viewModel.MonthTotalAgainstTarget = 0;
                    viewModel.YearTotalPreviousYear = 0;
                    viewModel.YearTotalThisYear = 0;
                    viewModel.YearTotalRateIncrease = 0;
                    viewModel.YearTotalAgainstTarget = 0;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "유료입장객매출";
                    viewModel.Item = "ㄴ 주간";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.PayComplexDaySales).FirstOrDefault();
                    payComplexSales1 += viewModel.TargetMonth;

                    viewModel.TargetYear = datas.Sum(p => (long)p.PayComplexDaySales);
                    payComplexSales2 += viewModel.TargetYear;


                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.PayComplexDaySales);
                    payComplexSales3 += viewModel.TargetTotal;


                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "유료입장객매출";
                    viewModel.Item = "ㄴ 야간";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.PayComplexNightSales).FirstOrDefault();
                    payComplexSales1 += viewModel.TargetMonth;

                    viewModel.TargetYear = datas.Sum(p => (long)p.PayComplexNightSales);
                    payComplexSales2 += viewModel.TargetYear;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.PayComplexNightSales);
                    payComplexSales3 += viewModel.TargetTotal;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "유료입장객매출";
                    viewModel.Item = "평균단가";

                    viewModel.TargetMonth = payComplexSales1 / payComplexVisitor1;
                    viewModel.TargetYear = payComplexSales2 / payComplexVisitor2;
                    viewModel.TargetTotal = payComplexSales3 / payComplexVisitor3;

                    viewModel.TodaySales = 0;
                    viewModel.MonthTotalPreviousYear = 0;
                    viewModel.MonthTotalThisYear = 0;
                    viewModel.MonthTotalRateIncrease = 0;
                    viewModel.MonthTotalAgainstTarget = 0;
                    viewModel.YearTotalPreviousYear = 0;
                    viewModel.YearTotalThisYear = 0;
                    viewModel.YearTotalRateIncrease = 0;
                    viewModel.YearTotalAgainstTarget = 0;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "유료입장객매출";
                    viewModel.Item = "역사관";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.PayHistoryhallSales).FirstOrDefault();
                    payHistoryhallSales1 += viewModel.TargetMonth;

                    viewModel.TargetYear = datas.Sum(p => (long)p.PayHistoryhallSales);
                    payHistoryhallSales2 += viewModel.TargetYear;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.PayHistoryhallSales);
                    payHistoryhallSales3 += viewModel.TargetTotal;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "유료입장객매출";
                    viewModel.Item = "평균단가";

                    viewModel.TargetMonth = payHistoryhallSales1 / payHistoryhallVisitor1;
                    viewModel.TargetYear = payHistoryhallSales2 / payHistoryhallVisitor2;
                    viewModel.TargetTotal = payHistoryhallSales3 / payHistoryhallVisitor3;

                    viewModel.TodaySales = 0;
                    viewModel.MonthTotalPreviousYear = 0;
                    viewModel.MonthTotalThisYear = 0;
                    viewModel.MonthTotalRateIncrease = 0;
                    viewModel.MonthTotalAgainstTarget = 0;
                    viewModel.YearTotalPreviousYear = 0;
                    viewModel.YearTotalThisYear = 0;
                    viewModel.YearTotalRateIncrease = 0;
                    viewModel.YearTotalAgainstTarget = 0;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "유료입장객매출";
                    viewModel.Item = "소계";

                    viewModel.TargetMonth = targetMonthData.PayComplexDaySales + targetMonthData.PayComplexNightSales + targetMonthData.PayHistoryhallSales;


                    viewModel.TargetYear = datas.Sum(p => (long)p.PayComplexDaySales) + datas.Sum(p => (long)p.PayComplexNightSales) + datas.Sum(p => (long)p.PayHistoryhallSales);
                    //sum = 0;
                    //foreach (var data in datas)
                    //{
                    //    sum += data.PayComplexDaySales + data.PayComplexNightSales + data.PayHistoryhallSales;
                    //}
                    //viewModel.TargetYear = sum;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.PayComplexDaySales) +
                           datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.PayComplexNightSales) +
                           datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.PayHistoryhallSales);

                    //sum = 0;
                    // list = datas.Where(p => p.TargetMonth <= tDate.Month).ToList();
                    //foreach (var item in list) 
                    //{
                    //    sum += item.PayComplexDaySales + item.PayComplexNightSales + item.PayHistoryhallSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    salesTargetMonthTotal += viewModel.TargetMonth;
                    salesTargetYearTotal += viewModel.TargetYear;
                    salesTargetTotalTotal += viewModel.TargetTotal;

                    viewModel.TodaySales = 15000;
                    viewModel.MonthTotalPreviousYear = 300000;
                    viewModel.MonthTotalThisYear = 330000;
                    viewModel.MonthTotalRateIncrease = 30;
                    viewModel.MonthTotalAgainstTarget = 15;
                    viewModel.YearTotalPreviousYear = 300000;
                    viewModel.YearTotalThisYear = 330000;
                    viewModel.YearTotalRateIncrease = 30;
                    viewModel.YearTotalAgainstTarget = 15;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "부대매출";
                    viewModel.Item = "사비로열차";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalSabiroSales).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => (long)p.IncidentalSabiroSales);
                    //sum = 0;
                    //foreach (var data in datas) 
                    //{
                    //    sum += data.IncidentalSabiroSales;
                    //}
                    //viewModel.TargetYear = sum;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalSabiroSales);
                    // list = datas.Where(p => p.TargetMonth <= tDate.Month).ToList();
                    //sum = 0;
                    //foreach (var item in list) 
                    //{
                    //    sum += item.IncidentalSabiroSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "부대매출";
                    viewModel.Item = "대관/대여";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalRentalSales).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => (long)p.IncidentalRentalSales);
                    //sum = 0;
                    //foreach (var data in datas)
                    //{
                    //    sum += data.IncidentalRentalSales;
                    //}
                    //viewModel.TargetYear = sum;
                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalRentalSales);
                    // list = datas.Where(p => p.TargetMonth <= tDate.Month).ToList();
                    //sum = 0;
                    //foreach (var item in list)
                    //{
                    //    sum += item.IncidentalRentalSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "부대매출";
                    viewModel.Item = "임대관리비";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalRmeSales).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => (long)p.IncidentalRmeSales);
                    //sum = 0;
                    //foreach (var data in datas)
                    //{
                    //    sum += data.IncidentalRmeSales;
                    //}
                    //viewModel.TargetYear = sum;
                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalRmeSales);
                    //sum = 0;
                    //foreach (var item in list)
                    //{
                    //    sum += item.IncidentalRmeSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "부대매출";
                    viewModel.Item = "상품";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalGoodsSales).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => (long)p.IncidentalGoodsSales);
                    //sum = 0;
                    //foreach (var data in datas)
                    //{
                    //    sum += data.IncidentalGoodsSales;
                    //}
                    //viewModel.TargetYear = sum;
                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalGoodsSales);
                    //sum = 0;
                    //foreach (var item in list)
                    //{
                    //    sum += item.IncidentalGoodsSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "부대매출";
                    viewModel.Item = "체험(기타)";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalExperienceSales).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => (long)p.IncidentalExperienceSales);
                    //sum = 0;
                    //foreach (var data in datas)
                    //{
                    //    sum += data.IncidentalExperienceSales;
                    //}
                    //viewModel.TargetYear = sum;
                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalExperienceSales);
                    //sum = 0;
                    //foreach (var item in list)
                    //{
                    //    sum += item.IncidentalExperienceSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "부대매출";
                    viewModel.Item = "소계";

                    viewModel.TargetMonth =
                        datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalSabiroSales).FirstOrDefault() +
                        datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalRentalSales).FirstOrDefault() +
                        datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalRmeSales).FirstOrDefault() +
                        datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalGoodsSales).FirstOrDefault() +
                        datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.IncidentalExperienceSales).FirstOrDefault();


                    viewModel.TargetYear =
                        datas.Sum(p => (long)p.IncidentalSabiroSales) +
                        datas.Sum(p => (long)p.IncidentalRentalSales) +
                        datas.Sum(p => (long)p.IncidentalRmeSales) +
                        datas.Sum(p => (long)p.IncidentalGoodsSales) +
                        datas.Sum(p => (long)p.IncidentalExperienceSales);

                    //sum = 0;
                    //foreach (var data in datas) 
                    //{
                    //    sum += data.IncidentalSabiroSales + data.IncidentalRentalSales + data.IncidentalRmeSales +
                    //           data.IncidentalGoodsSales + data.IncidentalExperienceSales;
                    //}
                    //viewModel.TargetYear = sum;


                    viewModel.TargetTotal =
                        datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalSabiroSales) +
                        datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalRentalSales) +
                        datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalRmeSales) +
                        datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalGoodsSales) +
                        datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.IncidentalExperienceSales);


                    //sum = 0;
                    //// list = datas.Where(p => p.TargetMonth <= tDate.Month).ToList();
                    //foreach (var item in list)
                    //{
                    //    sum += item.IncidentalSabiroSales + item.IncidentalRentalSales + item.IncidentalRmeSales +
                    //           item.IncidentalGoodsSales + item.IncidentalExperienceSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    salesTargetMonthTotal += viewModel.TargetMonth;
                    salesTargetYearTotal += viewModel.TargetYear;
                    salesTargetTotalTotal += viewModel.TargetTotal;

                    viewModel.TodaySales = 0;
                    viewModel.MonthTotalPreviousYear = 0;
                    viewModel.MonthTotalThisYear = 0;
                    viewModel.MonthTotalRateIncrease = 0;
                    viewModel.MonthTotalAgainstTarget = 0;
                    viewModel.YearTotalPreviousYear = 0;
                    viewModel.YearTotalThisYear = 0;
                    viewModel.YearTotalRateIncrease = 0;
                    viewModel.YearTotalAgainstTarget = 0;
                    
                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "임대매출";
                    viewModel.Item = "임대매출";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.RentalSales).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => (long)p.RentalSales);
                    //sum = 0;
                    //foreach (var data in datas) 
                    //{
                    //    sum += data.RentalSales;
                    //}
                    //viewModel.TargetYear = sum;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.RentalSales);
                    // list = datas.Where(p => p.TargetMonth <= tDate.Month).ToList();
                    //sum = 0;
                    //foreach (var item in list) 
                    //{
                    //    sum += item.RentalSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);

                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "임대매출";
                    viewModel.Item = "소계";

                    viewModel.TargetMonth = datas.Where(p => p.TargetMonth == tDate.Month).Select(p => p.RentalSales).FirstOrDefault();
                    viewModel.TargetYear = datas.Sum(p => (long)p.RentalSales);
                    //sum = 0;
                    //foreach (var data in datas) 
                    //{
                    //    sum += data.RentalSales;
                    //}
                    //viewModel.TargetYear = sum;

                    viewModel.TargetTotal = datas.Where(p => p.TargetMonth <= tDate.Month).Sum(p => (long)p.RentalSales);
                    // list = datas.Where(p => p.TargetMonth <= tDate.Month).ToList();
                    //sum = 0;
                    //foreach (var item in list)
                    //{
                    //    sum += item.RentalSales;
                    //}
                    //viewModel.TargetTotal = sum;

                    salesTargetMonthTotal += viewModel.TargetMonth;
                    salesTargetYearTotal += viewModel.TargetYear;
                    salesTargetTotalTotal += viewModel.TargetTotal;

                    viewModel.TodaySales = 5000;
                    viewModel.MonthTotalPreviousYear = 100000;
                    viewModel.MonthTotalThisYear = 110000;
                    viewModel.MonthTotalRateIncrease = 10;
                    viewModel.MonthTotalAgainstTarget = 5;
                    viewModel.YearTotalPreviousYear = 100000;
                    viewModel.YearTotalThisYear = 110000;
                    viewModel.YearTotalRateIncrease = 10;
                    viewModel.YearTotalAgainstTarget = 5;

                    viewModels.Add(viewModel);


                    viewModel = new FlashreportViewModel();

                    viewModel.TotalDivision = "총 매출(원)";
                    viewModel.Division = "합계";
                    viewModel.Item = "합계";

                    viewModel.TargetMonth = salesTargetMonthTotal;
                    viewModel.TargetYear = salesTargetYearTotal;
                    viewModel.TargetTotal = salesTargetTotalTotal;

                    viewModel.TodaySales = 0;
                    viewModel.MonthTotalPreviousYear = 0;
                    viewModel.MonthTotalThisYear = 0;
                    viewModel.MonthTotalRateIncrease = 0;
                    viewModel.MonthTotalAgainstTarget = 0;
                    viewModel.YearTotalPreviousYear = 0;
                    viewModel.YearTotalThisYear = 0;
                    viewModel.YearTotalRateIncrease = 0;
                    viewModel.YearTotalAgainstTarget = 0;

                    viewModels.Add(viewModel);


                    return new JsonResult(viewModels);
                }
                else 
                {
                    return new JsonResult("");
                }
            }
            catch (Exception ex) 
            {
                return new JsonResult(null);
            }
        }
    }

}
