using IBatisNet.DataMapper;
using ezDMS.Class;
using ezDMS.Filter;
using ezDMS.Models.Common;
using ezDMS.Models.Dist;
using ezDMS.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ezDMS.Models.Log;

namespace ezDMS.Controllers
{
    [AuthFilter(limitRole = eRole.VENDER)]
    [ActionFilter]
    public class PartController : Controller
    {
        // GET: Part
        public ActionResult PartManage()
        {
            return View();
        }

        public ActionResult EoPartBom(int? eo_idx)
        {

            return View("Dialog/dlgEoPartBom", eo_idx);
        }


        public ActionResult PartBom(string part_no, string part_rev_no)
        {
            ViewBag.part_no = part_no;
            ViewBag.part_rev_no = part_rev_no;
            return View("Dialog/dlgPartBom");
        }


        public ActionResult distPartBom(int? dist_idx)
        {
            DistEoModel distEo = Mapper.Instance().QueryForObject<DistEoModel>("DIST.selDistEo", new DistEoModel() { dist_idx = dist_idx });
          
            ViewBag.dist_idx = dist_idx;
            ViewBag.eo_idx = distEo.eo_idx;

            return View("Dialog/dlgDistPartBom");
        }

        public ActionResult distPartList(string part_no, string part_rev_no)
        {
            ViewBag.part_no = part_no;
            ViewBag.part_rev_no = part_rev_no;
            return View("Dialog/dlgDistPartList");
        }


        public ActionResult PartFile(string part_no, string part_rev_no)
        {
            ViewBag.part_no = part_no;
            ViewBag.part_rev_no = part_rev_no;
            return View("Dialog/dlgPartFile");
        }

        public ActionResult distPartFile(string part_no, string part_rev_no)
        {
            ViewBag.part_no = part_no;
            ViewBag.part_rev_no = part_rev_no;
            return View("Dialog/dlgDistPartFile");
        }

        public ActionResult PartEo(string part_no, string part_rev_no)
        {
            ViewBag.part_no = part_no;
            ViewBag.part_rev_no = part_rev_no;
            return View("Dialog/dlgPartToEo");
        }

        public ActionResult FileHistory(int? file_idx)
        {
            var ActionList = Mapper.Instance().QueryForList<ActionHistoryModel>("Log.selActionHis", new ActionHistoryModel { isFileHistory = "Y", action_target_idx = file_idx });
         
            return View("Dialog/dlgFileHistory", ActionList);
        }

        public JsonResult GetPartBomList(string part_no, string part_rev_no)
        {
            try
            {
                var bomList = Mapper.Instance().QueryForList<ItfBomInfo>("Part.selPartBom", new ItfBomInfo { part_no = part_no, part_rev_no = part_rev_no });

                return Json (bomList);               
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetPartBomReverseList(string part_no, string part_rev_no)
        {
            try
            {
                var bomList = Mapper.Instance().QueryForList<ItfBomInfo>("Part.selPartBomReverse", new ItfBomInfo { part_no = part_no, part_rev_no = part_rev_no });

                return Json(bomList);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetEoBomList(int? eo_idx)
        {
            try
            {
                //ItfPartMaster partMaster = Mapper.Instance().QueryForObject<ItfPartMaster>("Part.selEoKeyPartInfo", eo_idx);
                //var bomList = Mapper.Instance().QueryForList<ItfBomInfo>("Part.selPartBom", new ItfBomInfo { part_no = partMaster.part_no, part_rev_no = partMaster.part_rev_no });
                var bomList = Mapper.Instance().QueryForList<ItfBomInfo>("Part.selPartBom", new ItfBomInfo { eo_idx = eo_idx });

                return Json(bomList);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetDistEoBomList(int? dist_idx, int? eo_idx)
        {
            try
            {  DistMasterModel dist = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel() { dist_idx = dist_idx });

                //ViewBag.dist_st = dist.dist_st;
                //ItfPartMaster partMaster = Mapper.Instance().QueryForObject<ItfPartMaster>("Part.selEoKeyPartInfo", eo_idx);
                var bomList = Mapper.Instance().QueryForList<ItfBomInfo>("Part.selPartBom", new ItfBomInfo { dist_idx = dist_idx, eo_idx = eo_idx, recv_us = Convert.ToInt32(Session["USER_IDX"]) });

                if(dist.dist_st == "DF" || dist.dist_st == "DD")
                {
                    foreach(ItfBomInfo bom in bomList)
                    { 
                        bom.part_file_cnt = 0;
                    }
                }

                return Json(bomList);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetPartFileList(string part_no, string part_rev_no)
        {
            try
            {
                var partFileList = Mapper.Instance().QueryForList<ItfFileInfo>("Part.selPartFileInfo", new ItfFileInfo { part_no = part_no, part_rev_no = part_rev_no });
                return Json(partFileList);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        public JsonResult GetDistPartFileList(string part_no, string part_rev_no, int? dist_idx = null)
        {
            try
            {
                var distPartFileList = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistRecvFile", new DistRecvFileModel { dist_idx = dist_idx, part_no = part_no, part_rev_no = part_rev_no, recv_us = Convert.ToInt32(Session["USER_IDX"]) });

                return Json(distPartFileList);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetPartList(ItfPartMaster searchArgs)
        {
            try
            {
                var PartList = Mapper.Instance().QueryForList<ItfPartMaster>("Part.selPartList", searchArgs);

                return Json(PartList);
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetPartToEoList(ItfPartMaster searchArgs)
        {
            try
            {
                var eoList = Mapper.Instance().QueryForList<ItfPartMaster>("Part.selPartToEo", searchArgs);

                return Json(eoList);
    }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString()
});
            }
        }
    }
}