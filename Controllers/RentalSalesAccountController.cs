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
using Newtonsoft.Json;

namespace BaekjeCulturalComplexApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalSalesAccountController : ControllerBase
    {
        private readonly BaekjeDbContext _dbCon;
        private readonly IConfiguration _config;

        public RentalSalesAccountController(BaekjeDbContext context, IConfiguration config)
        {
            _dbCon = context;
            _config = config;
        }

        /// <summary>
        /// 임대매출관리 > 거래처 등록
        /// </summary>
        /// <param name="addData"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("CorsPolicyName")]
        public JsonResult RentalSalesAccountPost(RentalSalesAccountViewModel addData)
        {
            try
            {
                var salesItems = JsonConvert.DeserializeObject<List<string>>(addData.SalesItemsJson); // lease_임대비

                string divisionCodeStr = string.Empty;
                string itemNameStr = string.Empty;

                List<string> divisionCodes = new List<string>();
                List<string> itemNames = new List<string>();

                var rentalSalesCodes = _dbCon.CodeItems.Where(p => p.GroupCode == "rentalSales").Select(n => n.Code).ToList();

                foreach (var salesItem in salesItems) 
                {
                    var arr = salesItem.Split("_"); // arr[0] : lease, arr[1] : 임대비
                    bool isExist = rentalSalesCodes.Contains(arr[0]);

                    if (!divisionCodes.Contains(arr[0])) 
                    {
                        divisionCodes.Add(arr[0]);
                    }

                    itemNames.Add(salesItem);
                }

                divisionCodeStr = string.Join(",", divisionCodes);
                itemNameStr = string.Join(",", itemNames);

                RentalSalesAccount addItem = new RentalSalesAccount();

                addItem.Seq = new Guid();
                addItem.AccountName = addData.AccountName;
                addItem.TransactionStartDate = Convert.ToDateTime(addData.TransactionStartDate);
                addItem.TransactionEndDate = Convert.ToDateTime(addData.TransactionEndDate);
                addItem.DivisionCode = divisionCodeStr;
                addItem.ItemName = itemNameStr;
                addItem.IsUse = addData.IsUse;
                addItem.Note = addData.Note;
                addItem.RegDate = DateTime.Now;
                addItem.UpdateDate = DateTime.Now;

                _dbCon.RentalSalesAccounts.Add(addItem);
                _dbCon.SaveChanges();

                return new JsonResult("Add Success");
            }
            catch (Exception ex)
            {
                // return new JsonResult("Add Error Ouccuerd : " + ex.Message);
                return new JsonResult(null);
            }
        }

        /// <summary>
        /// 거래처 상세 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetRentalSalesAccountItem(Guid id)
        {
            try
            {
                var data = _dbCon.RentalSalesAccounts.Where(p => p.Seq == id).FirstOrDefault();

                if (data != null)
                {
                    return new JsonResult(data);
                }
                else
                {
                    return new JsonResult(null);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(null);
            }
        }

        /// <summary>
        /// 임대매출관리 > 거래처 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="addData"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [EnableCors("CorsPolicyName")]
        public JsonResult RentalSalesAccountPut(Guid id, RentalSalesAccountViewModel editData)
        {
            try
            {
                var targetData = _dbCon.RentalSalesAccounts.Where(p => p.Seq == id).FirstOrDefault();

                if (targetData != null) 
                {
                    var salesItems = JsonConvert.DeserializeObject<List<string>>(editData.SalesItemsJson); // lease_임대비

                    string divisionCodeStr = string.Empty;
                    string itemNameStr = string.Empty;

                    List<string> divisionCodes = new List<string>();
                    List<string> itemNames = new List<string>();

                    var rentalSalesCodes = _dbCon.CodeItems.Where(p => p.GroupCode == "rentalSales").Select(n => n.Code).ToList();

                    foreach (var salesItem in salesItems)
                    {
                        var arr = salesItem.Split("_"); // arr[0] : lease, arr[1] : 임대비
                        bool isExist = rentalSalesCodes.Contains(arr[0]);

                        if (!divisionCodes.Contains(arr[0]))
                        {
                            divisionCodes.Add(arr[0]);
                        }

                        itemNames.Add(salesItem);
                    }

                    divisionCodeStr = string.Join(",", divisionCodes);
                    itemNameStr = string.Join(",", itemNames);

                    targetData.AccountName = editData.AccountName;
                    targetData.TransactionStartDate = Convert.ToDateTime(editData.TransactionStartDate);
                    targetData.TransactionEndDate = Convert.ToDateTime(editData.TransactionEndDate);
                    targetData.DivisionCode = divisionCodeStr;
                    targetData.ItemName = itemNameStr;
                    targetData.IsUse = editData.IsUse;
                    targetData.Note = editData.Note;
                    targetData.UpdateDate = DateTime.Now;

                    _dbCon.SaveChanges();

                    return new JsonResult("Add Success");
                }
                else 
                {
                    return new JsonResult(null);
                }
            }
            catch (Exception ex)
            {
                // return new JsonResult("Add Error Ouccuerd : " + ex.Message);
                return new JsonResult(null);
            }
        }
    }
}