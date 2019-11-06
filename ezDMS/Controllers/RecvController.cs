using IBatisNet.DataMapper;
using IS_PODS.Class;
using IS_PODS.Models.Common;
using IS_PODS.Models.Dist;
using IS_PODS.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static IS_PODS.Define.LogDefine;

namespace IS_PODS.Controllers
{
    public class RecvController : Controller
    {
        // GET: Recv
        public ActionResult RecvList()
        {
            return View();
        }

        public JsonResult GetRecvList(DistMasterModel searchModel) 
        {
            try
            {
                searchModel.recv_us = Convert.ToInt32(Session["USER_IDX"]);
                return Json(Mapper.Instance().QueryForList<DistMasterModel>("Recv.selReciverDistInfo", searchModel));
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public ActionResult RecvView(int? dist_idx)
        {
            if (dist_idx == null)
            {
                return RedirectToAction("ErrorView", "Error", "잘못된 페이지를 호출하셨습니다.");
            }

            var distModel = Mapper.Instance().QueryForObject<DistMasterModel>("Recv.selReciverDistInfo", new DistMasterModel { dist_idx = dist_idx, recv_us = Convert.ToInt32(Session["USER_IDX"]) });
            var distEoModel = Mapper.Instance().QueryForObject<DistEoModel>("DIST.selDistEo", new DistEoModel { dist_idx = dist_idx });
            var distRecvFile = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistRecvFile", new DistRecvFileModel { dist_idx = dist_idx, recv_us = Convert.ToInt32(Session["USER_IDX"]), use_fl = "Y" });
            var distRecieverInfo = Mapper.Instance().QueryForObject<DistReceiverModel>("DIST.selDistReceiver", new DistReceiverModel { dist_idx = dist_idx, recv_us = Convert.ToInt32(Session["USER_IDX"]) });
            
            var eoBom = Mapper.Instance().QueryForList<ItfBomInfo>("Part.selPartBom", new ItfBomInfo { eo_idx = distModel.eo_idx });
           
            ViewBag.distModel = distModel;
            ViewBag.distEoModel = distEoModel;
            ViewBag.distRecvFile = distRecvFile;
            ViewBag.eoBomList = eoBom;
            ViewBag.distRecieverInfo = distRecieverInfo;
            LogCtrl.SetLog(distModel, eActionType.RecvView, this.HttpContext);

            return View();
        }

        public JsonResult GetRecvFileList(int? dist_idx)
        {
            var distRecvFile = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistRecvFile", new DistRecvFileModel { dist_idx = dist_idx, recv_us = Convert.ToInt32(Session["USER_IDX"]) });

            return Json(distRecvFile);
        }


    }
}