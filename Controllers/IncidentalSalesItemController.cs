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
    public class IncidentalSalesItemController : ControllerBase
    {
        private readonly BaekjeDbContext _dbCon;
        private readonly IConfiguration _config;

        public IncidentalSalesItemController(BaekjeDbContext context, IConfiguration config)
        {
            _dbCon = context;
            _config = config;
        }

        /// <summary>
        /// 부대매출 항목 등록
        /// </summary>
        /// <param name="addData"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("CorsPolicyName")]
        public JsonResult Post(IncidentalSalesItem addData)
        {
            try
            {
                IncidentalSalesItem addItem = new IncidentalSalesItem();

                addItem.Seq = new Guid();
                addItem.DivisionCode = addData.DivisionCode;
                addItem.DivisionName = _dbCon.CodeItems.Where(p => p.Code == addData.DivisionCode).Select(n => n.CodeName).FirstOrDefault();
                // addItem.DivisionName = addData.DivisionName;
                addItem.ItemName = addData.ItemName;
                addItem.IsUse = addData.IsUse;
                addItem.Note = addData.Note;
                addItem.RegDate = DateTime.Now;
                addItem.UpdateDate = DateTime.Now;

                _dbCon.IncidentalSalesItems.Add(addItem);
                _dbCon.SaveChanges();

                return new JsonResult("Add Success");
            }
            catch (Exception ex)
            {
                return new JsonResult("Add Error Ouccuerd : " + ex.Message);
            }
        }

        /// <summary>
        /// 부대매출 항목 구분 코드 데이터 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("incidentalSalesItemTypes")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetincidentalSalesItemTypes()
        {
            try
            {
                var codes = _dbCon.CodeItems.Where(p => p.GroupCode == "incidentalSales").ToList();

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
        /// 부대매출 항목 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("incidentalSalesItems")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetincidentalSalesItems(string divisionCode, string searchKeyword)
        {
            try
            {
                var datas = _dbCon.IncidentalSalesItems.ToList();

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

                    List<IncidentalSalesItemViewModel> list = new List<IncidentalSalesItemViewModel>();

                    for (var i = 0; i < datas.Count; i++)
                    {
                        IncidentalSalesItemViewModel viewModel = new IncidentalSalesItemViewModel();

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
        /// 부대매출 항목 상세 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetincidentalSalesItem(Guid id)
        {
            try
            {
                var data = _dbCon.IncidentalSalesItems.Where(p => p.Seq == id).FirstOrDefault();

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
                return new JsonResult(null);
            }
        }

        /// <summary>
        /// 부대매출 항목 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="editData"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [EnableCors("CorsPolicyName")]
        public JsonResult Put(Guid id, IncidentalSalesItem editData)
        {
            try
            {
                IncidentalSalesItem targetData = _dbCon.IncidentalSalesItems.Where(p => p.Seq == id).FirstOrDefault();

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
                    return new JsonResult(null);
                }

                return new JsonResult("Update Success");
            }
            catch (Exception ex)
            {
                return new JsonResult(null);
            }
        }
    }
}