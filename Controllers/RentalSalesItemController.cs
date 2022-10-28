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

namespace BaekjeCulturalComplexApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalSalesItemController : ControllerBase
    {
        private readonly BaekjeDbContext _dbCon;
        private readonly IConfiguration _config;

        public RentalSalesItemController(BaekjeDbContext context, IConfiguration config)
        {
            _dbCon = context;
            _config = config;
        }

        /// <summary>
        /// 임대매출 항목 등록
        /// </summary>
        /// <param name="addData"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("CorsPolicyName")]
        public JsonResult Post(RentalSalesItem addData)
        {
            try
            {
                RentalSalesItem addItem = new RentalSalesItem();

                addItem.Seq = new Guid();
                addItem.DivisionCode = addData.DivisionCode;
                addItem.DivisionName = _dbCon.CodeItems.Where(p => p.Code == addData.DivisionCode).Select(n => n.CodeName).FirstOrDefault();
                addItem.ItemName = addData.ItemName;
                addItem.IsUse = addData.IsUse;
                addItem.Note = addData.Note;
                addItem.RegDate = DateTime.Now;
                addItem.UpdateDate = DateTime.Now;

                _dbCon.RentalSalesItems.Add(addItem);
                _dbCon.SaveChanges();

                return new JsonResult("Add Success");
            }
            catch (Exception ex)
            {
                return new JsonResult("Add Error Ouccuerd : " + ex.Message);
            }
        }

        /// <summary>
        /// 임대매출 항목 구분 코드 데이터 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("rentalSalesItemTypes")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetRentalSalesItemTypes()
        {
            try
            {
                var codes = _dbCon.CodeItems.Where(p => p.GroupCode == "rentalSales").ToList();

                if (codes.Count > 0)
                {
                    return new JsonResult(codes);
                }
                else
                {
                    return new JsonResult("");
                }

            }
            catch (Exception ex)
            {
                return new JsonResult("");
            }
        }

        /// <summary>
        /// 임대매출 항목 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("rentalSalesItems")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetRentalSalesItems(string divisionCode, string searchKeyword)
        {
            try
            {
                var datas = _dbCon.RentalSalesItems.ToList();

                // 선택 매출구분 조회
                if (divisionCode != "all")
                {
                    datas = datas.Where(p => p.DivisionCode == divisionCode).ToList();
                }

                // 검색 항목명 조회
                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    datas = datas.Where(p => p.ItemName.Contains(searchKeyword)).ToList();
                }

                if (datas.Count > 0)
                {
                    List<int> orders = new List<int>();

                    for (var i = datas.Count; i >= 1; i--)
                    {
                        orders.Add(i);
                    }

                    // datas = datas.OrderBy(p => p.DivisionName).OrderBy(o => o.ItemName).ToList();
                    datas = datas.OrderByDescending(p => p.RegDate).ToList();

                    List<RentalSalesItemViewModel> list = new List<RentalSalesItemViewModel>();

                    for (var i = 0; i < datas.Count; i++)
                    {
                        RentalSalesItemViewModel viewModel = new RentalSalesItemViewModel();

                        viewModel.Order = orders[i];
                        viewModel.Seq = datas[i].Seq;
                        viewModel.DivisionCode = datas[i].DivisionCode;
                        viewModel.DivisionName = datas[i].DivisionName;
                        viewModel.ItemName = datas[i].ItemName;
                        viewModel.IsUse = datas[i].IsUse;
                        viewModel.Note = datas[i].Note;
                        viewModel.RegDate = datas[i].RegDate.ToString("yyyy-MM-dd");
                        viewModel.UpdateDate = datas[i].UpdateDate.ToString("yyyy-MM-dd");

                        list.Add(viewModel);
                    }

                    return new JsonResult(list);
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
        /// 임대매출 항목 상세 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetRentalSalesItem(Guid id)
        {
            try
            {
                var data = _dbCon.RentalSalesItems.Where(p => p.Seq == id).FirstOrDefault();

                if (data != null)
                {
                    return new JsonResult(data);
                }
                else
                {
                    return new JsonResult("");
                }
            }
            catch (Exception ex)
            {
                return new JsonResult("");
            }
        }

        /// <summary>
        /// 임대매출 항목 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="editData"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [EnableCors("CorsPolicyName")]
        public JsonResult Put(Guid id, RentalSalesItem editData)
        {
            try
            {
                RentalSalesItem targetData = _dbCon.RentalSalesItems.Where(p => p.Seq == id).FirstOrDefault();

                if (targetData != null)
                {
                    targetData.DivisionCode = editData.DivisionCode;
                    targetData.DivisionName = _dbCon.CodeItems.Where(p => p.Code == editData.DivisionCode).Select(n => n.CodeName).FirstOrDefault();
                    targetData.ItemName = editData.ItemName;
                    targetData.IsUse = editData.IsUse;
                    targetData.Note = editData.Note;
                    targetData.UpdateDate = DateTime.Now;

                    _dbCon.SaveChanges();
                }
                else
                {
                    return new JsonResult("Update Fail");
                }

                return new JsonResult("Update Success");
            }
            catch (Exception ex)
            {
                return new JsonResult("Update Fail");
            }
        }

        /// <summary>
        /// 임대매출 > 거래처등록 > 등록 대상 매출항목 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("rentalSalesItemsByAccount")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetRentalSalesItemsByAccount()
        {
            try
            {
                // var list = _dbCon.RentalSalesItems.Where(p => p.IsUse == true).ToList();
                var list = _dbCon.RentalSalesItems.ToList();

                if (list.Count > 0)
                {
                    return new JsonResult(list);
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
        /// 임대매출관리 > 거래처 조회
        /// </summary>
        /// <param name="divisionCode"></param>
        /// <param name="salesItem"></param>
        /// <param name="searchKeyword"></param>
        /// <returns></returns>
        [HttpGet("rentalSalesAccountItems")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetRentalSalesAccountItems(string divisionCode, string salesItem, string searchKeyword)
        {
            try
            {
                var datas = _dbCon.RentalSalesAccounts.ToList();

                if (datas.Count > 0)
                {
                    // 매출항목 대분류 필터
                    if (divisionCode != "all")
                    {
                        datas = datas.Where(p => p.DivisionCode.Contains(divisionCode)).ToList();
                    }

                    // 매출항목 소분류 필터
                    if (salesItem != "all")
                    {
                        var arr = salesItem.Split("_");
                        string itemName = arr[1];
                        datas = datas.Where(p => p.ItemName.Contains(itemName)).ToList();
                    }

                    // 거래처 검색 키워드 필터
                    if (!string.IsNullOrWhiteSpace(searchKeyword))
                    {
                        datas = datas.Where(p => p.AccountName.Contains(searchKeyword)).ToList();
                    }

                    if (datas.Count > 0)
                    {
                        List<int> orders = new List<int>();

                        for (var i = datas.Count; i >= 1; i--)
                        {
                            orders.Add(i);
                        }

                        datas = datas.OrderByDescending(p => p.RegDate).ToList();

                        List<RentalSalesAccountViewModel> list = new List<RentalSalesAccountViewModel>();

                        for (var i = 0; i < datas.Count; i++)
                        {
                            RentalSalesAccountViewModel viewModel = new RentalSalesAccountViewModel();

                            viewModel.Order = orders[i];
                            viewModel.Seq = datas[i].Seq;
                            viewModel.AccountName = datas[i].AccountName;
                            viewModel.TransactionStartDate = datas[i].TransactionStartDate.ToString("yyyy-MM-dd");
                            viewModel.TransactionEndDate = datas[i].TransactionEndDate.ToString("yyyy-MM-dd");
                            viewModel.TransactionPeriod = datas[i].TransactionStartDate.ToString("yyyy-MM-dd") + " ~ " + datas[i].TransactionEndDate.ToString("yyyy-MM-dd");

                            List<string> codeNames = new List<string>();
                            var arr = datas[i].DivisionCode.Split(",");
                            for (var j = 0; j < arr.Length; j++)
                            {
                                string codeName = _dbCon.CodeItems.Where(p => p.Code == arr[j]).Select(n => n.CodeName).FirstOrDefault();
                                codeNames.Add(codeName);
                            }

                            string codeNameStr = string.Join(",", codeNames);

                            viewModel.DivisionCode = codeNameStr;
                            //viewModel.ItemName = datas[i].ItemName;
                            var arr2 = datas[i].ItemName.Split(","); // lease_임대비,maintenanceCost_공사비 
                            List<string> itemNames = new List<string>();
                            foreach (var str in arr2)
                            {
                                var arr3 = str.Split("_"); // arr[0] : lease, arr[1] : 임대비
                                itemNames.Add(arr3[1]); // ["임대비","공사비"]
                            }

                            string itemNameStr = string.Join(",", itemNames);
                            viewModel.ItemName = itemNameStr;

                            viewModel.IsUse = datas[i].IsUse;
                            viewModel.Note = datas[i].Note;
                            viewModel.RegDate = datas[i].RegDate.ToString("yyyy-MM-dd");
                            viewModel.UpdateDate = datas[i].UpdateDate.ToString("yyyy-MM-dd");

                            list.Add(viewModel);
                        }

                        return new JsonResult(list);
                    }
                    else
                    {
                        return new JsonResult("");
                    }
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
        /// 임대매출 현황 > 매출 등록 > 선택 거래처별 매출항목 조회
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        [HttpGet("rentalSalesItemAddByAccount")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetRentalSalesItemAddByAccount(string accountName)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(accountName))
                {
                    return new JsonResult("");
                }
                else 
                {
                    var data = _dbCon.RentalSalesAccounts.Where(p => p.AccountName == accountName).FirstOrDefault();

                    RentalSalesItemByAccount item = new RentalSalesItemByAccount();

                    List<string> divisions = new List<string>();

                    // 대분류
                    if (data.DivisionCode.Contains(","))
                    {
                        var divisionCodes = data.DivisionCode.Split(",");


                        foreach (var divisionCode in divisionCodes)
                        {
                            string divisionName = _dbCon.CodeItems.Where(p => p.Code == divisionCode).Select(p => p.CodeName).FirstOrDefault();

                            divisions.Add(divisionCode + "_" + divisionName);
                        }

                        item.Division = string.Join(",", divisions);
                    }
                    else
                    {
                        string divisionName = _dbCon.CodeItems.Where(p => p.Code == data.DivisionCode).Select(p => p.CodeName).FirstOrDefault();
                        item.Division = data.DivisionCode + "_" + divisionName;
                    }

                    // 소분류
                    item.Item = data.ItemName;

                    return new JsonResult(item);
                }       

            }
            catch (Exception ex)
            {
                return new JsonResult(null);
            }
        }
    }

    public class RentalSalesItemByAccount 
    {
        //public List<string> Divisions { get; set; }
        //public List<string> Items { get; set; }
        public string Division { get; set; }
        public string Item { get; set; }
    }
}