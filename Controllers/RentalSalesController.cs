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

namespace BaekjeCulturalComplexApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalSalesController : ControllerBase
    {
        private readonly BaekjeDbContext _dbCon;
        private readonly IConfiguration _config;

        public RentalSalesController(BaekjeDbContext context, IConfiguration config)
        {
            _dbCon = context;
            _config = config;
        }

        /// <summary>
        /// 임대매출 등록
        /// </summary>
        /// <param name="addData"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("CorsPolicyName")]
        public JsonResult Post(RentalSales addData)
        {
            try
            {
                RentalSales addItem = new RentalSales();

                addItem.Seq = new Guid();
                addItem.AccountName = addData.AccountName;
                addItem.SalesDate = addData.SalesDate;
                addItem.DivisionCode = addData.DivisionCode;
                addItem.ItemName = addData.ItemName;
                addItem.SupplyPrice = addData.SupplyPrice;
                addItem.Surtax = addData.Surtax;
                addItem.SumPrice = addData.SumPrice;
                addItem.Note = addData.Note;
                addItem.RegDate = DateTime.Now;
                addItem.UpdateDate = DateTime.Now;

                _dbCon.RentalSales.Add(addItem);
                _dbCon.SaveChanges();

                return new JsonResult("Add Success");
            }
            catch (Exception ex)
            {
                return new JsonResult(null);
            }
        }

        /// <summary>
        /// 임대매출 현황 조회
        /// </summary>
        /// <param name="vatParam"></param>
        /// <param name="settlementYearParam"></param>
        /// <param name="divisionCodeParam"></param>
        /// <returns></returns>
        [HttpGet("rentalSalesStatus")]
        [EnableCors("CorsPolicyName")]

        public Hashtable GetRentalSalesStatus(string vatParam, int settlementYearParam, string divisionCodeParam)
        {
            try
            {
                Hashtable ht = new Hashtable();

                var datas = _dbCon.RentalSales.ToList();

                if (divisionCodeParam != "all") 
                {
                    datas = datas.Where(p => p.DivisionCode == divisionCodeParam).ToList();
                }

                // 매출항목 사용여부 체크
                var noUseSalesItems = _dbCon.RentalSalesItems.Where(p => p.IsUse == false).ToList();

                List<RentalSales> noUseItems = new List<RentalSales>();

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

                // 거래처 사용여부 체크
                var noUseAccounts = _dbCon.RentalSalesAccounts.Where(p => p.IsUse == false).ToList();

                List<RentalSales> noUseAccountItems = new List<RentalSales>();

                if (noUseAccounts.Count > 0)
                {
                    foreach (var noUseAccount in noUseAccounts)
                    {
                        var targetItems = datas.Where(p => p.AccountName == noUseAccount.AccountName).ToList();

                        foreach (var targetItem in targetItems)
                        {
                            if (targetItem.SalesDate.Year != settlementYearParam)
                            {
                                noUseAccountItems.Add(targetItem);
                            }
                        }
                    }
                }

                if (noUseAccountItems.Count > 0)
                {
                    foreach (var noUseItem in noUseAccountItems)
                    {
                        datas.Remove(noUseItem);
                    }
                }


                if (datas.Count > 0)
                {
                    int targetYear = settlementYearParam;

                    List<RentalSalesViewModel> viewModels = new List<RentalSalesViewModel>();

                    // var groupByList = datas.GroupBy(g => g.AccountName).ToList();
                    var groupByList = datas.GroupBy(x => new { x.AccountName, x.ItemName }).ToList();

                    List<RentalSales> targetList;
                    int supplyPrice = 0;
                    int tax = 0;
                    int sumPrice = 0;

                    foreach (var groupByItem in groupByList)
                    {
                        var key = groupByItem.Key;

                        RentalSalesViewModel addModel = new RentalSalesViewModel();

                        //AccountName
                        addModel.AccountName = _dbCon.RentalSales.Where(p => p.ItemName == key.ItemName && p.AccountName == key.AccountName).Select(p => p.AccountName).FirstOrDefault();
                        //ItemName
                        addModel.ItemName = key.ItemName;

                        var targetItemList = _dbCon.RentalSales.Where(p => p.ItemName == key.ItemName && p.AccountName == key.AccountName).ToList();

                        //January
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 1).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.January = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //February
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 2).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.February = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //March
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 3).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.March = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //April
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 4).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.April = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //May
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 5).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.May = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //June
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 6).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.June = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //July
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 7).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.July = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //August
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 8).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.August = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //September
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 9).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.September = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //October
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 10).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.October = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //November
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 11).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.November = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //December
                        targetList = targetItemList.Where(p => p.SalesDate.Year == targetYear && p.SalesDate.Month == 12).ToList();
                        supplyPrice = targetList.Sum(p => p.SupplyPrice);
                        tax = targetList.Sum(p => p.Surtax);
                        sumPrice = targetList.Sum(p => p.SumPrice);
                        addModel.December = vatParam == "1" ? (supplyPrice + tax) : supplyPrice;

                        //Total
                        addModel.Total = addModel.January + addModel.February + addModel.March + addModel.April + addModel.May + addModel.June
                            + addModel.July + addModel.August + addModel.September + addModel.October + addModel.November + addModel.December;

                        viewModels.Add(addModel);
                    }

                    viewModels = viewModels.OrderBy(o => o.AccountName).ToList();

                    
                    // 소계 row 생성
                    var groupByAccountList = viewModels.GroupBy(g => g.AccountName).ToList();

                    List<RentalSalesViewModel> subTotalList = new List<RentalSalesViewModel>();

                    foreach (var groupByAccount in groupByAccountList)
                    {
                        string accountKey = groupByAccount.Key;

                        RentalSalesViewModel subRowModel = new RentalSalesViewModel();

                        int month1Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.January);
                        int month2Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.February);
                        int month3Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.March);
                        int month4Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.April);
                        int month5Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.May);
                        int month6Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.June);
                        int month7Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.July);
                        int month8Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.August);
                        int month9Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.September);
                        int month10Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.October);
                        int month11Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.November);
                        int month12Sum = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.December);

                        int subToal = viewModels.Where(p => p.AccountName == accountKey).Sum(p => p.Total);

                        subRowModel.AccountName = accountKey;
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
                    List<RentalSalesViewModel> mergeList = new List<RentalSalesViewModel>();

                    string beforeAccountName = viewModels[0].AccountName;

                    foreach (var model in viewModels)
                    {
                        if (model.AccountName == beforeAccountName)
                        {
                            mergeList.Add(model);
                        }
                        else
                        {
                            foreach (var subItem in subTotalList)
                            {
                                if (subItem.AccountName == beforeAccountName)
                                {
                                    mergeList.Add(subItem);
                                }
                            }

                            mergeList.Add(model);

                            beforeAccountName = model.AccountName;
                        }
                    }

                    foreach (var subItem in subTotalList)
                    {
                        if (subItem.AccountName == beforeAccountName)
                        {
                            mergeList.Add(subItem);
                        }
                    }

                    // 합계 row 생성
                    RentalSalesViewModel allTotalRow = new RentalSalesViewModel();

                    allTotalRow.AccountName = "합계";
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
                    foreach (var groupByAccount in groupByAccountList)
                    {
                        if (isFirst)
                        {
                            string targetAccountName = mergeList.Where(p => p.AccountName == groupByAccount.Key).Select(p => p.AccountName).FirstOrDefault();
                            int startIndex = 0;
                            int itemCount = mergeList.Where(p => p.AccountName == targetAccountName).Count();
                            beforeIndex += itemCount;

                            SetMerge data = new SetMerge();
                            data.DivisionName = targetAccountName;
                            data.StartIndex = startIndex;
                            data.ItemCount = itemCount;
                            setMergeList.Add(data);

                            isFirst = false;
                        }
                        else
                        {
                            string targetAccountName = mergeList.Where(p => p.AccountName == groupByAccount.Key).Select(p => p.AccountName).FirstOrDefault();
                            int startIndex = beforeIndex;
                            int itemCount = mergeList.Where(p => p.AccountName == targetAccountName).Count();

                            SetMerge data = new SetMerge();
                            data.DivisionName = targetAccountName;
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
                    ht.Add("subTotalRowIndexs", null);
                    return ht;
                }
            }
            catch (Exception ex) 
            {
                Hashtable ht = new Hashtable();
                ht.Add("data", null);
                ht.Add("subTotalRowIndexs", null);
                return ht;
            }
        }


        /// <summary>
        /// 임대매출 현황 상세 조회
        /// </summary>
        /// <param name="startDateParam"></param>
        /// <param name="endDateParam"></param>
        /// <returns></returns>
        [HttpGet("rentalSalesStatus/detail")]
        [EnableCors("CorsPolicyName")]
        public Hashtable GetRentalSalesStatusDetail(string startDateParam, string endDateParam)
        {
            try
            {
                Hashtable ht = new Hashtable();

                DateTime startDate = Convert.ToDateTime(startDateParam);
                DateTime endDate = Convert.ToDateTime(endDateParam);

                int targetYear = startDate.Year;

                var datas = _dbCon.RentalSales.Where(p => p.SalesDate.Year == targetYear &&
                    DateTime.Compare(p.SalesDate, startDate) >= 0 && DateTime.Compare(p.SalesDate, endDate) <= 0).ToList();

                if (datas.Count > 0)
                {
                    List<RentalSalesDetailViewModel> viewModels = new List<RentalSalesDetailViewModel>();

                    // var groupByList = datas.GroupBy(g => g.ItemName).ToList();
                    var groupByList = datas.GroupBy(x => new { x.DivisionCode, x.AccountName, x.ItemName }).ToList();

                    foreach (var groupItem in groupByList)
                    {
                        var key = groupItem.Key;

                        RentalSalesDetailViewModel viewModel = new RentalSalesDetailViewModel();

                        viewModel.DivisionCode = datas.Where(p => p.DivisionCode == key.DivisionCode 
                            && p.AccountName == key.AccountName 
                            && p.ItemName == key.ItemName)
                            .Select(p => p.DivisionCode).FirstOrDefault();

                        viewModel.DivisionName = _dbCon.CodeItems.Where(p => p.Code == viewModel.DivisionCode).Select(p => p.CodeName).FirstOrDefault();

                        viewModel.AccountName = key.AccountName;

                        viewModel.ItemName = key.ItemName;

                        int supplyPriceTotal = datas.Where(p => p.DivisionCode == key.DivisionCode 
                            && p.AccountName == key.AccountName 
                            && p.ItemName == key.ItemName)
                            .Sum(p => p.SupplyPrice);

                        int surtaxTotal = datas.Where(p => p.DivisionCode == key.DivisionCode
                            && p.AccountName == key.AccountName
                            && p.ItemName == key.ItemName)
                            .Sum(p => p.Surtax);

                        int sumPriceTotal = supplyPriceTotal + surtaxTotal;

                        viewModel.SupplyPrice = supplyPriceTotal;
                        viewModel.Surtax = surtaxTotal;
                        viewModel.SumPrice = sumPriceTotal;

                        var noteList = datas.Where(p => p.DivisionCode == key.DivisionCode
                            && p.AccountName == key.AccountName
                            && p.ItemName == key.ItemName)
                            .Select(p => p.Note).ToList();

                        var noteStr = String.Join(", ", noteList);
                        if (noteStr == ", ")
                        {
                            noteStr = "";
                        }
                        viewModel.Note = noteStr;

                        viewModels.Add(viewModel);
                    }

                    viewModels = viewModels.OrderBy(x => x.DivisionCode).ThenBy(x => x.AccountName).ThenBy(x => x.ItemName).ToList();

                    List<RentalSalesDetailViewModel> mergeModels = new List<RentalSalesDetailViewModel>();

                    var divisionCodeGroups = viewModels.GroupBy(x => x.DivisionCode).ToList();

                    foreach (var key in divisionCodeGroups) 
                    {
                        string divisionCodeKey = key.Key;

                        var targetModels = viewModels.Where(p => p.DivisionCode == divisionCodeKey).ToList();

                        var accountGroups = targetModels.GroupBy(x => x.AccountName).ToList();

                        foreach (var aKey in accountGroups) 
                        {
                            string accountKey = aKey.Key;

                            foreach (var vModel in viewModels) 
                            {
                                if (vModel.DivisionCode == divisionCodeKey && vModel.AccountName == accountKey) 
                                {
                                    RentalSalesDetailViewModel addModel = new RentalSalesDetailViewModel();

                                    addModel.DivisionCode = vModel.DivisionCode;
                                    addModel.DivisionName = vModel.DivisionName;
                                    addModel.AccountName = vModel.AccountName;
                                    addModel.ItemName = vModel.ItemName;
                                    addModel.SupplyPrice = vModel.SupplyPrice;
                                    addModel.Surtax = vModel.Surtax;
                                    addModel.SumPrice = vModel.SumPrice;
                                    addModel.Note = vModel.Note;

                                    mergeModels.Add(addModel);
                                }
                            }

                            // 소계 Row Add
                            RentalSalesDetailViewModel subTotalMergeModel = new RentalSalesDetailViewModel();

                            subTotalMergeModel.DivisionCode = divisionCodeKey;
                            subTotalMergeModel.DivisionName = _dbCon.CodeItems.Where(p => p.Code == divisionCodeKey).Select(p => p.CodeName).FirstOrDefault();
                            subTotalMergeModel.AccountName = accountKey;
                            subTotalMergeModel.ItemName = "소계";

                            subTotalMergeModel.SupplyPrice = viewModels.Where(p => p.DivisionCode == divisionCodeKey 
                                && p.AccountName == accountKey)
                                .Sum(p => p.SupplyPrice);

                            subTotalMergeModel.Surtax = viewModels.Where(p => p.DivisionCode == divisionCodeKey
                                && p.AccountName == accountKey)
                                .Sum(p => p.Surtax);

                            subTotalMergeModel.SumPrice = viewModels.Where(p => p.DivisionCode == divisionCodeKey
                                && p.AccountName == accountKey)
                                .Sum(p => p.SumPrice);

                            subTotalMergeModel.Note = "";

                            mergeModels.Add(subTotalMergeModel);
                        }

                        // 임대/관리비 매출 합계 Row Add
                        RentalSalesDetailViewModel salesTotalMergeModel = new RentalSalesDetailViewModel();

                        salesTotalMergeModel.DivisionCode = divisionCodeKey;
                        string salesTotalMergeAccountName = _dbCon.CodeItems.Where(p => p.Code == divisionCodeKey).Select(p => p.CodeName).FirstOrDefault();
                        salesTotalMergeModel.DivisionName = salesTotalMergeAccountName;
                        salesTotalMergeModel.AccountName = salesTotalMergeAccountName + " 합계";
                        salesTotalMergeModel.ItemName = salesTotalMergeAccountName + " 합계";
                        salesTotalMergeModel.SupplyPrice = mergeModels.Where(p => p.ItemName == "소계" && p.DivisionCode == divisionCodeKey).Sum(p => p.SupplyPrice);
                        salesTotalMergeModel.Surtax = mergeModels.Where(p => p.ItemName == "소계" && p.DivisionCode == divisionCodeKey).Sum(p => p.Surtax);
                        salesTotalMergeModel.SumPrice = mergeModels.Where(p => p.ItemName == "소계" && p.DivisionCode == divisionCodeKey).Sum(p => p.SumPrice);
                        salesTotalMergeModel.Note = "";

                        mergeModels.Add(salesTotalMergeModel);
                    }

                    // 총 합계 Row Add
                    RentalSalesDetailViewModel salesAllTotalModel = new RentalSalesDetailViewModel();

                    salesAllTotalModel.DivisionCode = "";
                    salesAllTotalModel.DivisionName = "총 계(임대 + 관리비매출)";
                    salesAllTotalModel.AccountName = "총 계(임대 + 관리비매출)";
                    salesAllTotalModel.ItemName = "총 계(임대 + 관리비매출)";
                    salesAllTotalModel.SupplyPrice = mergeModels.Where(p => p.AccountName.Contains("합계")).Sum(p => p.SupplyPrice);
                    salesAllTotalModel.Surtax = mergeModels.Where(p => p.AccountName.Contains("합계")).Sum(p => p.Surtax);
                    salesAllTotalModel.SumPrice = mergeModels.Where(p => p.AccountName.Contains("합계")).Sum(p => p.SumPrice);
                    salesAllTotalModel.Note = "";

                    mergeModels.Add(salesAllTotalModel);

                    // 소계 row index 설정
                    List<int> subTotalRowIndexs = new List<int>();
                    for (var i = 0; i < mergeModels.Count; i++)
                    {
                        if (mergeModels[i].ItemName == "소계")
                        {
                            subTotalRowIndexs.Add(i);
                        }
                    }

                    // 임대/관리비매출 합계 row index 설정
                    List<int> salesTotalRowIndexs = new List<int>();
                    for (var i = 0; i < mergeModels.Count; i++)
                    {
                        if (mergeModels[i].AccountName.Contains("합계"))
                        {
                            salesTotalRowIndexs.Add(i);
                        }
                    }



                    // 거래처 merge index 설정
                    var divisionAccountGroups = mergeModels.GroupBy(x => new { x.DivisionName, x.AccountName }).ToList();

                    List<SetMerge> setAccountMergeList = new List<SetMerge>();
                    bool isFirst = true;
                    int beforeIndex = 0;
                    foreach (var groupItem in divisionAccountGroups)
                    {
                        if (isFirst)
                        {
                            string targetDivisionName = mergeModels.Where(p => p.DivisionName == groupItem.Key.DivisionName &&
                                p.AccountName == groupItem.Key.AccountName)
                                .Select(p => p.DivisionName).FirstOrDefault();

                            int startIndex = 0;

                            int itemCount = mergeModels.Where(p => p.DivisionName == targetDivisionName &&
                                p.AccountName == groupItem.Key.AccountName)
                                .Count();


                            beforeIndex += itemCount;

                            SetMerge data = new SetMerge();
                            data.DivisionName = targetDivisionName;
                            data.StartIndex = startIndex;
                            data.ItemCount = itemCount;
                            setAccountMergeList.Add(data);

                            isFirst = false;
                        }
                        else
                        {
                            string targetDivisionName = mergeModels.Where(p => p.DivisionName == groupItem.Key.DivisionName &&
                                p.AccountName == groupItem.Key.AccountName)
                                .Select(p => p.DivisionName).FirstOrDefault();


                            int startIndex = beforeIndex;
                            int itemCount = mergeModels.Where(p => p.DivisionName == targetDivisionName &&
                                p.AccountName == groupItem.Key.AccountName)
                                .Count();

                            SetMerge data = new SetMerge();
                            data.DivisionName = targetDivisionName;
                            data.StartIndex = startIndex;
                            data.ItemCount = itemCount;
                            setAccountMergeList.Add(data);

                            beforeIndex += itemCount;
                        }
                    }

                    foreach (var item in setAccountMergeList)
                    {
                        int delStartIndex = item.StartIndex + 1;
                        int itemCount = item.ItemCount;

                        item.DelIndexList = new List<int>();

                        for (var i = delStartIndex; i <= (delStartIndex + itemCount - 2); i++)
                        {
                            item.DelIndexList.Add(i);
                        }
                    }

                    // 매출구분 merge index 설정
                    var divisionGroups = mergeModels.GroupBy(x => x.DivisionName).ToList();

                    List<SetMerge> setDivisionMergeList = new List<SetMerge>();
                    isFirst = true;
                    beforeIndex = 0;
                    foreach (var groupItem in divisionGroups)
                    {
                        if (isFirst)
                        {
                            string targetDivisionName = mergeModels.Where(p => p.DivisionName == groupItem.Key)
                                .Select(p => p.DivisionName).FirstOrDefault();

                            int startIndex = 0;

                            int itemCount = mergeModels.Where(p => p.DivisionName == targetDivisionName).Count();

                            beforeIndex += itemCount;

                            SetMerge data = new SetMerge();
                            data.DivisionName = targetDivisionName;
                            data.StartIndex = startIndex;
                            data.ItemCount = itemCount;
                            setDivisionMergeList.Add(data);

                            isFirst = false;
                        }
                        else
                        {
                            string targetDivisionName = mergeModels.Where(p => p.DivisionName == groupItem.Key)
                                .Select(p => p.DivisionName).FirstOrDefault();

                            int startIndex = beforeIndex;
                            int itemCount = mergeModels.Where(p => p.DivisionName == targetDivisionName).Count();

                            SetMerge data = new SetMerge();
                            data.DivisionName = targetDivisionName;
                            data.StartIndex = startIndex;
                            data.ItemCount = itemCount;
                            setDivisionMergeList.Add(data);

                            beforeIndex += itemCount;
                        }
                    }

                    foreach (var item in setDivisionMergeList)
                    {
                        int delStartIndex = item.StartIndex + 1;
                        int itemCount = item.ItemCount;

                        item.DelIndexList = new List<int>();

                        for (var i = delStartIndex; i <= (delStartIndex + itemCount - 2); i++)
                        {
                            item.DelIndexList.Add(i);
                        }
                    }



                    ht.Add("data", mergeModels);
                    ht.Add("subTotalRowIndexs", subTotalRowIndexs);
                    ht.Add("salesTotalRowIndexs", salesTotalRowIndexs);

                    ht.Add("setAccountMergeData", setAccountMergeList);
                    ht.Add("setDivisionMergeData", setDivisionMergeList);

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














        /// <summary>
        /// 임대매출 항목 등록
        /// </summary>
        /// <param name="addData"></param>
        /// <returns></returns>
        //[HttpPost]
        //[EnableCors("CorsPolicyName")]
        //public JsonResult Post(RentalSalesItem addData)
        //{
        //    try
        //    {
        //        RentalSalesItem addItem = new RentalSalesItem();

        //        addItem.Seq = new Guid();
        //        addItem.DivisionCode = addData.DivisionCode;
        //        addItem.DivisionName = _dbCon.CodeItems.Where(p => p.Code == addData.DivisionCode).Select(n => n.CodeName).FirstOrDefault();
        //        addItem.ItemName = addData.ItemName;
        //        addItem.IsUse = addData.IsUse;
        //        addItem.Note = addData.Note;
        //        addItem.RegDate = DateTime.Now;
        //        addItem.UpdateDate = DateTime.Now;

        //        _dbCon.RentalSalesItems.Add(addItem);
        //        _dbCon.SaveChanges();

        //        return new JsonResult("Add Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult("Add Error Ouccuerd : " + ex.Message);
        //    }
        //}







        /// <summary>
        /// 임대매출 항목 구분 코드 데이터 조회
        /// </summary>
        /// <returns></returns>
        //[HttpGet("rentalSalesItemTypes")]
        //[EnableCors("CorsPolicyName")]
        //public JsonResult GetRentalSalesItemTypes()
        //{
        //    try
        //    {
        //        var codes = _dbCon.CodeItems.Where(p => p.GroupCode == "rentalSales").ToList();

        //        if (codes.Count > 0)
        //        {
        //            return new JsonResult(codes);
        //        }
        //        else
        //        {
        //            return new JsonResult("");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult("");
        //    }
        //}

        /// <summary>
        /// 임대매출 항목 조회
        /// </summary>
        /// <returns></returns>
        //[HttpGet("rentalSalesItems")]
        //[EnableCors("CorsPolicyName")]
        //public JsonResult GetRentalSalesItems(string divisionCode, string searchKeyword)
        //{
        //    try
        //    {
        //        var datas = _dbCon.RentalSalesItems.ToList();

        //        // 선택 매출구분 조회
        //        if (divisionCode != "all")
        //        {
        //            datas = datas.Where(p => p.DivisionCode == divisionCode).ToList();
        //        }

        //        // 검색 항목명 조회
        //        if (!string.IsNullOrWhiteSpace(searchKeyword))
        //        {
        //            datas = datas.Where(p => p.ItemName.Contains(searchKeyword)).ToList();
        //        }

        //        if (datas.Count > 0)
        //        {
        //            List<int> orders = new List<int>();

        //            for (var i = datas.Count; i >= 1; i--)
        //            {
        //                orders.Add(i);
        //            }

        //            // datas = datas.OrderBy(p => p.DivisionName).OrderBy(o => o.ItemName).ToList();
        //            datas = datas.OrderByDescending(p => p.RegDate).ToList();

        //            List<RentalSalesItemViewModel> list = new List<RentalSalesItemViewModel>();

        //            for (var i = 0; i < datas.Count; i++)
        //            {
        //                RentalSalesItemViewModel viewModel = new RentalSalesItemViewModel();

        //                viewModel.Order = orders[i];
        //                viewModel.Seq = datas[i].Seq;
        //                viewModel.DivisionCode = datas[i].DivisionCode;
        //                viewModel.DivisionName = datas[i].DivisionName;
        //                viewModel.ItemName = datas[i].ItemName;
        //                viewModel.IsUse = datas[i].IsUse;
        //                viewModel.Note = datas[i].Note;
        //                viewModel.RegDate = datas[i].RegDate.ToString("yyyy-MM-dd");
        //                viewModel.UpdateDate = datas[i].UpdateDate.ToString("yyyy-MM-dd");

        //                list.Add(viewModel);
        //            }

        //            return new JsonResult(list);
        //        }
        //        else
        //        {
        //            return new JsonResult("");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(null);
        //    }
        //}

        /// <summary>
        /// 임대매출 항목 상세 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        //[EnableCors("CorsPolicyName")]
        //public JsonResult GetRentalSalesItem(Guid id)
        //{
        //    try
        //    {
        //        var data = _dbCon.RentalSalesItems.Where(p => p.Seq == id).FirstOrDefault();

        //        if (data != null)
        //        {
        //            return new JsonResult(data);
        //        }
        //        else
        //        {
        //            return new JsonResult("");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult("");
        //    }
        //}

        /// <summary>
        /// 임대매출 항목 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="editData"></param>
        /// <returns></returns>
        //[HttpPut("{id}")]
        //[EnableCors("CorsPolicyName")]
        //public JsonResult Put(Guid id, RentalSalesItem editData)
        //{
        //    try
        //    {
        //        RentalSalesItem targetData = _dbCon.RentalSalesItems.Where(p => p.Seq == id).FirstOrDefault();

        //        if (targetData != null)
        //        {
        //            targetData.DivisionCode = editData.DivisionCode;
        //            targetData.DivisionName = _dbCon.CodeItems.Where(p => p.Code == editData.DivisionCode).Select(n => n.CodeName).FirstOrDefault();
        //            targetData.ItemName = editData.ItemName;
        //            targetData.IsUse = editData.IsUse;
        //            targetData.Note = editData.Note;
        //            targetData.UpdateDate = DateTime.Now;

        //            _dbCon.SaveChanges();
        //        }
        //        else
        //        {
        //            return new JsonResult("Update Fail");
        //        }

        //        return new JsonResult("Update Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult("Update Fail");
        //    }
        //}

        /// <summary>
        /// 임대매출 > 거래처등록 > 등록 대상 매출항목 조회
        /// </summary>
        /// <returns></returns>
        //[HttpGet("rentalSalesItemsByAccount")]
        //[EnableCors("CorsPolicyName")]
        //public JsonResult GetRentalSalesItemsByAccount()
        //{
        //    try
        //    {
        //        // var list = _dbCon.RentalSalesItems.Where(p => p.IsUse == true).ToList();
        //        var list = _dbCon.RentalSalesItems.ToList();

        //        if (list.Count > 0)
        //        {
        //            return new JsonResult(list);
        //        }
        //        else
        //        {
        //            return new JsonResult("");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(null);
        //    }
        //}

        /// <summary>
        /// 임대매출관리 > 거래처 조회
        /// </summary>
        /// <param name="divisionCode"></param>
        /// <param name="salesItem"></param>
        /// <param name="searchKeyword"></param>
        /// <returns></returns>
        //[HttpGet("rentalSalesAccountItems")]
        //[EnableCors("CorsPolicyName")]
        //public JsonResult GetRentalSalesAccountItems(string divisionCode, string salesItem, string searchKeyword)
        //{
        //    try
        //    {
        //        var datas = _dbCon.RentalSalesAccounts.ToList();

        //        if (datas.Count > 0)
        //        {
        //            // 매출항목 대분류 필터
        //            if (divisionCode != "all")
        //            {
        //                datas = datas.Where(p => p.DivisionCode.Contains(divisionCode)).ToList();
        //            }

        //            // 매출항목 소분류 필터
        //            if (salesItem != "all")
        //            {
        //                var arr = salesItem.Split("_");
        //                string itemName = arr[1];
        //                datas = datas.Where(p => p.ItemName.Contains(itemName)).ToList();
        //            }

        //            // 거래처 검색 키워드 필터
        //            if (!string.IsNullOrWhiteSpace(searchKeyword))
        //            {
        //                datas = datas.Where(p => p.AccountName.Contains(searchKeyword)).ToList();
        //            }

        //            if (datas.Count > 0)
        //            {
        //                List<int> orders = new List<int>();

        //                for (var i = datas.Count; i >= 1; i--)
        //                {
        //                    orders.Add(i);
        //                }

        //                datas = datas.OrderByDescending(p => p.RegDate).ToList();

        //                List<RentalSalesAccountViewModel> list = new List<RentalSalesAccountViewModel>();

        //                for (var i = 0; i < datas.Count; i++)
        //                {
        //                    RentalSalesAccountViewModel viewModel = new RentalSalesAccountViewModel();

        //                    viewModel.Order = orders[i];
        //                    viewModel.Seq = datas[i].Seq;
        //                    viewModel.AccountName = datas[i].AccountName;
        //                    viewModel.TransactionStartDate = datas[i].TransactionStartDate.ToString("yyyy-MM-dd");
        //                    viewModel.TransactionEndDate = datas[i].TransactionEndDate.ToString("yyyy-MM-dd");
        //                    viewModel.TransactionPeriod = datas[i].TransactionStartDate.ToString("yyyy-MM-dd") + " ~ " + datas[i].TransactionEndDate.ToString("yyyy-MM-dd");

        //                    List<string> codeNames = new List<string>();
        //                    var arr = datas[i].DivisionCode.Split(",");
        //                    for (var j = 0; j < arr.Length; j++)
        //                    {
        //                        string codeName = _dbCon.CodeItems.Where(p => p.Code == arr[j]).Select(n => n.CodeName).FirstOrDefault();
        //                        codeNames.Add(codeName);
        //                    }

        //                    string codeNameStr = string.Join(",", codeNames);

        //                    viewModel.DivisionCode = codeNameStr;
        //                    //viewModel.ItemName = datas[i].ItemName;
        //                    var arr2 = datas[i].ItemName.Split(","); // lease_임대비,maintenanceCost_공사비 
        //                    List<string> itemNames = new List<string>();
        //                    foreach (var str in arr2)
        //                    {
        //                        var arr3 = str.Split("_"); // arr[0] : lease, arr[1] : 임대비
        //                        itemNames.Add(arr3[1]); // ["임대비","공사비"]
        //                    }

        //                    string itemNameStr = string.Join(",", itemNames);
        //                    viewModel.ItemName = itemNameStr;

        //                    viewModel.IsUse = datas[i].IsUse;
        //                    viewModel.Note = datas[i].Note;
        //                    viewModel.RegDate = datas[i].RegDate.ToString("yyyy-MM-dd");
        //                    viewModel.UpdateDate = datas[i].UpdateDate.ToString("yyyy-MM-dd");

        //                    list.Add(viewModel);
        //                }

        //                return new JsonResult(list);
        //            }
        //            else
        //            {
        //                return new JsonResult("");
        //            }
        //        }
        //        else 
        //        {
        //            return new JsonResult("");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(null);
        //    }
        //}
    }
}
