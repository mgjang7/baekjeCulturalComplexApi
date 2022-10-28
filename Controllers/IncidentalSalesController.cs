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
    public class IncidentalSalesController : ControllerBase
    {
        private readonly BaekjeDbContext _dbCon;
        private readonly IConfiguration _config;

        public IncidentalSalesController(BaekjeDbContext context, IConfiguration config)
        {
            _dbCon = context;
            _config = config;
        }

        /// <summary>
        /// 부대매출  등록
        /// </summary>
        /// <param name="addData"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("CorsPolicyName")]
        public JsonResult Post(IncidentalSales addData)
        {
            try
            {
                IncidentalSales addItem = new IncidentalSales();

                addItem.Seq = new Guid();
                addItem.DivisionCode = addData.DivisionCode;
                addItem.ItemName = addData.ItemName;
                addItem.SalesDate = addData.SalesDate;
                addItem.SupplyPrice = addData.SupplyPrice;
                addItem.Surtax = addData.Surtax;
                addItem.SumPrice = addData.SumPrice;
                addItem.Note = addData.Note;
                addItem.RegDate = DateTime.Now;
                addItem.UpdateDate = DateTime.Now;


                _dbCon.IncidentalSales.Add(addItem);
                _dbCon.SaveChanges();

                return new JsonResult("Add Success");
            }
            catch (Exception ex)
            {
                return new JsonResult(null);
                //return new JsonResult("Add Error Ouccuerd : " + ex.Message);
            }
        }

        /// <summary>
        /// 부대매출 현황 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("incidentalSalesStatus")]
        [EnableCors("CorsPolicyName")]
        // public JsonResult GetincidentalSalesStatus(string vatParam, int settlementYearParam, string divisionCodeParam) //Hashtable
        public Hashtable GetincidentalSalesStatus(string vatParam, int settlementYearParam, string divisionCodeParam) //Hashtable
        {
            try
            {
                Hashtable ht = new Hashtable();

                List<IncidentalSales> datas;

                if (divisionCodeParam == "all")
                {
                    datas = _dbCon.IncidentalSales.ToList();
                }
                else 
                {
                    datas = _dbCon.IncidentalSales.Where(p => p.DivisionCode == divisionCodeParam).ToList();
                }

                // 매출항목 사용여부 체크
                var noUseSalesItems = _dbCon.IncidentalSalesItems.Where(p => p.IsUse == false).ToList();

                List<IncidentalSales> noUseItems = new List<IncidentalSales>();

                if (noUseSalesItems.Count > 0) 
                {
                    foreach (var noUseSalesItem in noUseSalesItems) 
                    {
                        var targetItems = datas.Where(p => p.DivisionCode == noUseSalesItem.DivisionCode && p.ItemName == noUseSalesItem.ItemName).ToList();

                        foreach (var targetItem in targetItems) 
                        {
                            if (targetItem.SalesDate.Year != settlementYearParam)
                            {
                                noUseItems.Add(targetItem);
                            }
                        }
                    }
                }

                if (noUseItems.Count > 0) 
                {
                    foreach (var noUseItem in noUseItems) 
                    {
                        datas.Remove(noUseItem);
                    }
                }

                if (datas.Count > 0)
                {
                    int targetYear = settlementYearParam;

                    List<IncidentalSalesViewModel>  viewModels = new List<IncidentalSalesViewModel>();

                    
                    var groupByList = datas.GroupBy(g => g.ItemName).ToList();

                    List<IncidentalSales> targetList;
                    int supplyPrice = 0;
                    int tax = 0;
                    int sumPrice = 0;

                    foreach (var groupByItem in groupByList)
                    {
                        var key = groupByItem.Key;

                        IncidentalSalesViewModel addModel = new IncidentalSalesViewModel();

                        //DivisionName
                        var divisionCode = _dbCon.IncidentalSales.Where(p => p.ItemName == key).Select(n => n.DivisionCode).FirstOrDefault();
                        var divisionName = _dbCon.CodeItems.Where(p => p.Code == divisionCode).Select(n => n.CodeName).FirstOrDefault();
                        addModel.DivisionName = divisionName;
                        //DivisionCode
                        addModel.DivisionCode = divisionCode;
                        //ItemName
                        addModel.ItemName = key;

                        var targetItemList = _dbCon.IncidentalSales.Where(p => p.ItemName == key).ToList();

                        //January
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 1).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.January = supplyPrice;
                        addModel.January = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //February
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 2).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.February = supplyPrice;
                        addModel.February = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //March
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 3).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.March = supplyPrice;
                        addModel.March = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //April
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 4).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.April = supplyPrice;
                        addModel.April = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //May
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 5).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.May = supplyPrice;
                        addModel.May = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //June
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 6).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.June = supplyPrice;
                        addModel.June = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //July
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 7).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.July = supplyPrice;
                        addModel.July = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //August
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 8).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.August = supplyPrice;
                        addModel.August = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //September
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 9).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.September = supplyPrice;
                        addModel.September = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //October
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 10).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.October = supplyPrice;
                        addModel.October = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //November
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 11).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.November = supplyPrice;
                        addModel.November = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //December
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 12).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        // addModel.December = supplyPrice;
                        addModel.December = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //Total
                        addModel.Total = addModel.January + addModel.February + addModel.March + addModel.April + addModel.May + addModel.June
                            + addModel.July + addModel.August + addModel.September + addModel.October + addModel.November + addModel.December;

                        viewModels.Add(addModel);
                    }

                    viewModels = viewModels.OrderBy(o => o.DivisionName).ToList();


                    if (divisionCodeParam == "all") // 전체일때 항목별 소계, 합계 row 생성, 매출 구분 컬럼 merge
                    {
                        // 소계 row 생성
                        var groupByDivisionList = viewModels.GroupBy(g => g.DivisionName).ToList();

                        List<IncidentalSalesViewModel> subTotalList = new List<IncidentalSalesViewModel>();

                        foreach (var groupByDivision in groupByDivisionList)
                        {
                            string divisionKey = groupByDivision.Key;

                            IncidentalSalesViewModel subRowModel = new IncidentalSalesViewModel();

                            int month1Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.January);
                            int month2Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.February);
                            int month3Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.March);
                            int month4Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.April);
                            int month5Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.May);
                            int month6Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.June);
                            int month7Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.July);
                            int month8Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.August);
                            int month9Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.September);
                            int month10Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.October);
                            int month11Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.November);
                            int month12Sum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.December);

                            int subToal = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.Total);

                            subRowModel.DivisionName = divisionKey;
                            subRowModel.DivisionCode = "";
                            subRowModel.ItemName = "소계";

                            subRowModel.January = month1Sum;
                            subRowModel.February = month2Sum;
                            subRowModel.March = month3Sum;
                            subRowModel.April = month4Sum;
                            subRowModel.May = month5Sum;
                            subRowModel.June = month6Sum;
                            subRowModel.July = month7Sum;
                            subRowModel.August = month8Sum;
                            subRowModel.September = month9Sum;
                            subRowModel.October = month10Sum;
                            subRowModel.November = month11Sum;
                            subRowModel.December = month12Sum;
                            subRowModel.Total = subToal;

                            subTotalList.Add(subRowModel);
                        }

                        // 소계 row 합치기
                        List<IncidentalSalesViewModel> mergeList = new List<IncidentalSalesViewModel>();

                        string beforeDivisionName = viewModels[0].DivisionName;

                        foreach (var model in viewModels)
                        {
                            if (model.DivisionName == beforeDivisionName)
                            {
                                mergeList.Add(model);
                            }
                            else
                            {
                                foreach (var subItem in subTotalList)
                                {
                                    if (subItem.DivisionName == beforeDivisionName)
                                    {
                                        mergeList.Add(subItem);
                                    }
                                }

                                mergeList.Add(model);

                                beforeDivisionName = model.DivisionName;
                            }
                        }

                        foreach (var subItem in subTotalList)
                        {
                            if (subItem.DivisionName == beforeDivisionName)
                            {
                                mergeList.Add(subItem);
                            }
                        }

                        // 합계 row 생성
                        IncidentalSalesViewModel allTotalRow = new IncidentalSalesViewModel();

                        allTotalRow.DivisionName = "합계";
                        allTotalRow.DivisionCode = "";
                        allTotalRow.ItemName = "합계";
                        allTotalRow.January = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.January);
                        allTotalRow.February = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.February);
                        allTotalRow.March = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.March);
                        allTotalRow.April = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.April);
                        allTotalRow.May = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.May);
                        allTotalRow.June = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.June);
                        allTotalRow.July = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.July);
                        allTotalRow.August = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.August);
                        allTotalRow.September = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.September);
                        allTotalRow.October = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.October);
                        allTotalRow.November = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.November);
                        allTotalRow.December = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.December);
                        allTotalRow.Total = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.Total);

                        mergeList.Add(allTotalRow);

                        // 소계 row index
                        List<int> subTotalRowIndexs = new List<int>();
                        for (var i = 0; i < mergeList.Count; i++) 
                        {
                            if (mergeList[i].ItemName == "소계") 
                            {
                                subTotalRowIndexs.Add(i);
                            }
                        }

                        List<SetMerge> setMergeList = new List<SetMerge>();
                        bool isFirst = true;
                        int beforeIndex = 0;
                        foreach (var groupByDivision in groupByDivisionList)
                        {
                            if (isFirst)
                            {
                                string targetDivisionName = mergeList.Where(p => p.DivisionName == groupByDivision.Key).Select(p => p.DivisionName).FirstOrDefault();
                                int startIndex = 0;
                                int itemCount = mergeList.Where(p => p.DivisionName == targetDivisionName).Count();
                                beforeIndex += itemCount;

                                SetMerge data = new SetMerge();
                                data.DivisionName = targetDivisionName;
                                data.StartIndex = startIndex;
                                data.ItemCount = itemCount;
                                setMergeList.Add(data);

                                isFirst = false;
                            }
                            else 
                            {
                                string targetDivisionName = mergeList.Where(p => p.DivisionName == groupByDivision.Key).Select(p => p.DivisionName).FirstOrDefault();
                                int startIndex = beforeIndex;
                                int itemCount = mergeList.Where(p => p.DivisionName == targetDivisionName).Count();

                                SetMerge data = new SetMerge();
                                data.DivisionName = targetDivisionName;
                                data.StartIndex = startIndex;
                                data.ItemCount = itemCount;
                                setMergeList.Add(data);

                                beforeIndex += itemCount;
                            }
                        }

                        foreach (var item in setMergeList) 
                        {
                            int delStartIndex = item.StartIndex + 1;
                            int itemCount = item.ItemCount;

                            item.DelIndexList = new List<int>();

                            for (var i = delStartIndex; i <= (delStartIndex + itemCount - 2); i++) 
                            {
                                item.DelIndexList.Add(i);
                            }
                        }

                        ht.Add("data", mergeList);
                        ht.Add("subTotalRowIndexs", subTotalRowIndexs);
                        ht.Add("setMergeData", setMergeList);

                        return ht;
                    }
                    else 
                    {
                        IncidentalSalesViewModel totalRow = new IncidentalSalesViewModel();

                        totalRow.DivisionName = "합계";
                        totalRow.DivisionCode = "";
                        totalRow.ItemName = "합계";

                        totalRow.January = viewModels.Sum(p => p.January);
                        totalRow.February = viewModels.Sum(p => p.February);
                        totalRow.March = viewModels.Sum(p => p.March);
                        totalRow.April = viewModels.Sum(p => p.April);
                        totalRow.May = viewModels.Sum(p => p.May);
                        totalRow.June = viewModels.Sum(p => p.June);
                        totalRow.July = viewModels.Sum(p => p.July);
                        totalRow.August = viewModels.Sum(p => p.August);
                        totalRow.September = viewModels.Sum(p => p.September);
                        totalRow.October = viewModels.Sum(p => p.October);
                        totalRow.November = viewModels.Sum(p => p.November);
                        totalRow.December = viewModels.Sum(p => p.December);

                        totalRow.Total = viewModels.Sum(p => p.Total);

                        viewModels.Add(totalRow);

                        ht.Add("data", viewModels);
                        ht.Add("subTotalRowIndexs", null);
                        return ht;
                    }
                }
                else
                {
                    ht.Add("data", "");
                    ht.Add("subTotalRowIndexs", null);
                    return ht;
                }
            }
            catch (Exception ex)
            {
                Hashtable ht = new Hashtable();
                // return new JsonResult(null);
                ht.Add("data", null);
                ht.Add("subTotalRowIndexs", null);
                return ht;
            }
        }

        /// <summary>
        /// 부대매출 현황 월별 상세 조회
        /// </summary>
        /// <param name="monthParam"></param>
        /// <returns></returns>
        [HttpGet("incidentalSalesStatus/detail")]
        [EnableCors("CorsPolicyName")]
        public Hashtable GetincidentalSalesStatusDetail(string startDateParam, string endDateParam)
        {
            try
            {
                Hashtable ht = new Hashtable();

                DateTime startDate = Convert.ToDateTime(startDateParam);
                DateTime endDate = Convert.ToDateTime(endDateParam);

                int targetYear = startDate.Year;

                var datas = _dbCon.IncidentalSales.Where(p => p.SalesDate.Year == targetYear && 
                    DateTime.Compare(p.SalesDate, startDate) >= 0 && DateTime.Compare(p.SalesDate, endDate) <= 0).ToList();

                if (datas.Count > 0)
                {
                    List<IncidentalSalesDetailViewModel> viewModels = new List<IncidentalSalesDetailViewModel>();

                    var groupByList = datas.GroupBy(g => g.ItemName).ToList();

                    foreach (var groupItem in groupByList) 
                    {
                        string itemNameKey = groupItem.Key;

                        IncidentalSalesDetailViewModel viewModel = new IncidentalSalesDetailViewModel();

                        viewModel.DivisionCode = datas.Where(p => p.ItemName == itemNameKey).Select(p => p.DivisionCode).FirstOrDefault();
                        viewModel.DivisionName = _dbCon.CodeItems.Where(p => p.Code == viewModel.DivisionCode).Select(p => p.CodeName).FirstOrDefault();
                        viewModel.ItemName = itemNameKey;

                        int supplyPriceTotal = datas.Where(p => p.ItemName == itemNameKey).Sum(p => p.SupplyPrice);
                        int surtaxTotal = datas.Where(p => p.ItemName == itemNameKey).Sum(p => p.Surtax);
                        // int sumPriceTotal = datas.Where(p => p.ItemName == itemNameKey).Sum(p => p.SumPrice);
                        int sumPriceTotal = supplyPriceTotal + surtaxTotal;

                        viewModel.SupplyPrice = supplyPriceTotal;
                        viewModel.Surtax = surtaxTotal;
                        viewModel.SumPrice = sumPriceTotal;

                        var noteList = datas.Where(p => p.ItemName == itemNameKey).Select(p => p.Note).ToList();
                        var noteStr = String.Join(", ", noteList);
                        if (noteStr == ", ") 
                        {
                            noteStr = "";
                        }
                        viewModel.Note = noteStr;

                        viewModels.Add(viewModel);
                    }

                    viewModels = viewModels.OrderBy(o => o.DivisionName).ToList();
                    
                    // 소계 row 생성
                    var groupByDivisionList = viewModels.GroupBy(g => g.DivisionName).ToList();

                    List<IncidentalSalesDetailViewModel> subTotalList = new List<IncidentalSalesDetailViewModel>();

                    foreach (var groupByDivision in groupByDivisionList)
                    {
                        string divisionKey = groupByDivision.Key;

                        IncidentalSalesDetailViewModel subRowModel = new IncidentalSalesDetailViewModel();

                        int supplyPriceSum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.SupplyPrice);
                        int surtaxSum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.Surtax);
                        int sumPriceSum = viewModels.Where(p => p.DivisionName == divisionKey).Sum(p => p.SumPrice);
                        
                        subRowModel.DivisionName = divisionKey;
                        subRowModel.DivisionCode = "";
                        subRowModel.ItemName = "소계";

                        subRowModel.SupplyPrice = supplyPriceSum;
                        subRowModel.Surtax = surtaxSum;
                        subRowModel.SumPrice = sumPriceSum;

                        subRowModel.Note = "";


                        subTotalList.Add(subRowModel);
                    }

                    // 소계 row 합치기
                    List<IncidentalSalesDetailViewModel> mergeList = new List<IncidentalSalesDetailViewModel>();

                    string beforeDivisionName = viewModels[0].DivisionName;

                    foreach (var model in viewModels)
                    {
                        if (model.DivisionName == beforeDivisionName)
                        {
                            mergeList.Add(model);
                        }
                        else
                        {
                            foreach (var subItem in subTotalList)
                            {
                                if (subItem.DivisionName == beforeDivisionName)
                                {
                                    mergeList.Add(subItem);
                                }
                            }

                            mergeList.Add(model);

                            beforeDivisionName = model.DivisionName;
                        }
                    }

                    foreach (var subItem in subTotalList)
                    {
                        if (subItem.DivisionName == beforeDivisionName)
                        {
                            mergeList.Add(subItem);
                        }
                    }

                    // 합계 row 생성
                    IncidentalSalesDetailViewModel allTotalRow = new IncidentalSalesDetailViewModel();

                    allTotalRow.DivisionName = "합계(체험+대관/대여+상품)";
                    allTotalRow.DivisionCode = "";
                    allTotalRow.ItemName = "합계(체험+대관/대여+상품)";

                    allTotalRow.SupplyPrice = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.SupplyPrice);
                    allTotalRow.Surtax = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.Surtax);
                    allTotalRow.SumPrice = mergeList.Where(p => p.ItemName == "소계").Sum(p => p.SumPrice);

                    allTotalRow.Note = "";

                    mergeList.Add(allTotalRow);

                    // 소계 row index
                    List<int> subTotalRowIndexs = new List<int>();
                    for (var i = 0; i < mergeList.Count; i++)
                    {
                        if (mergeList[i].ItemName == "소계")
                        {
                            subTotalRowIndexs.Add(i);
                        }
                    }

                    List<SetMerge> setMergeList = new List<SetMerge>();
                    bool isFirst = true;
                    int beforeIndex = 0;
                    foreach (var groupByDivision in groupByDivisionList)
                    {
                        if (isFirst)
                        {
                            string targetDivisionName = mergeList.Where(p => p.DivisionName == groupByDivision.Key).Select(p => p.DivisionName).FirstOrDefault();
                            int startIndex = 0;
                            int itemCount = mergeList.Where(p => p.DivisionName == targetDivisionName).Count();
                            beforeIndex += itemCount;

                            SetMerge data = new SetMerge();
                            data.DivisionName = targetDivisionName;
                            data.StartIndex = startIndex;
                            data.ItemCount = itemCount;
                            setMergeList.Add(data);

                            isFirst = false;
                        }
                        else
                        {
                            string targetDivisionName = mergeList.Where(p => p.DivisionName == groupByDivision.Key).Select(p => p.DivisionName).FirstOrDefault();
                            int startIndex = beforeIndex;
                            int itemCount = mergeList.Where(p => p.DivisionName == targetDivisionName).Count();

                            SetMerge data = new SetMerge();
                            data.DivisionName = targetDivisionName;
                            data.StartIndex = startIndex;
                            data.ItemCount = itemCount;
                            setMergeList.Add(data);

                            beforeIndex += itemCount;
                        }
                    }

                    foreach (var item in setMergeList)
                    {
                        int delStartIndex = item.StartIndex + 1;
                        int itemCount = item.ItemCount;

                        item.DelIndexList = new List<int>();

                        for (var i = delStartIndex; i <= (delStartIndex + itemCount - 2); i++)
                        {
                            item.DelIndexList.Add(i);
                        }
                    }

                    ht.Add("data", mergeList);
                    ht.Add("subTotalRowIndexs", subTotalRowIndexs);
                    ht.Add("setMergeData", setMergeList);

                    return ht;
                }
                else 
                {
                    ht.Add("data", "");
                    return ht;
                }
            }
            catch (Exception ex) 
            {
                Hashtable ht = new Hashtable();
                ht.Add("data", null);
                return ht;
            }
        }
    }

    public class SetMerge {
        // 구분명
        // 시작 index
        // 항목수

        public string DivisionName { get; set; }
        public int StartIndex { get; set; }
        public int ItemCount { get; set; }
        public List<int> DelIndexList { get; set; }

    }
}