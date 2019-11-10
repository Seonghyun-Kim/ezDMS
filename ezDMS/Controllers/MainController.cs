using System;
using System.Collections.Generic;
using IBatisNet.DataMapper;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using ezDMS.Models.Common;
using ezDMS.Class;
using ezDMS.Models.Auth;
using ezDMS.Filter;
using ezDMS.Models.Stats;
using ezDMS.Models.Dist;

namespace ezDMS.Controllers
{
    [AuthFilter(limitRole = eRole.VENDER)]
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            if(Convert.ToInt32(Session["USER_ROLE"]) == 10)
            {
                return RedirectToAction("RecvList", "Recv");
            }

            return View();
        }

        public ActionResult Menu()
        {
            var roleGroup = Mapper.Instance().QueryForList<CommLibraryModel>("Common.selCommCode", new CommLibraryModel { parent_code = "ROLE", use_fl = "Y" });
            var positionGroup = Mapper.Instance().QueryForList<CommLibraryModel>("Common.selCommCode", new CommLibraryModel { parent_code = "POSITION", use_fl = "Y" });
            var eoTypeGroup = Mapper.Instance().QueryForList<Hashtable>("ITF.selEoType", null);
            var distStatusGroup = Mapper.Instance().QueryForList<CommLibraryModel>("Common.selCommCode", new CommLibraryModel { parent_code = "DIST_ST", use_fl = "Y" });
            var actionModuleGroup = Mapper.Instance().QueryForList<CommLibraryModel>("Common.selCommCode", new CommLibraryModel { parent_code = "MODULE", use_fl = "Y" });
            var actionTypeGroup = Mapper.Instance().QueryForList<CommLibraryModel>("Common.selCommCode", new CommLibraryModel { parent_code = "ACTION", use_fl = "Y" });

            ViewBag.roleGrp = roleGroup;
            ViewBag.posGrp = positionGroup;
            ViewBag.eoTypeGroup = eoTypeGroup; 
            ViewBag.distStatusGroup = distStatusGroup;
            ViewBag.actionModuleGroup = actionModuleGroup;
            ViewBag.actionTypeGroup = actionTypeGroup;

            var menu = Mapper.Instance().QueryForList<MenuModel>("Main.selMenuList", null);

            //AuthModel menuAth = CommonUtil.CheckMenuAuth((Hashtable)Session["AUTH"], 4);

            ViewBag.menuList = CommonUtil.CheckMenusAuth((Hashtable)Session["AUTH"], menu);

            return View();
        }

        public JsonResult GetDistRecvStats()
        {
            ConfigModel distRecvDay = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "MAIN", comm_code = "MAIN_DIST_RECV_DATE" });
            return Json(Mapper.Instance().QueryForList<DistRecvStats>("Main.selDistRecvStatsList", new DistRecvStats { search_dt = DateTime.Now.AddDays((Convert.ToInt32(distRecvDay.comm_value) * -1)) }));
        }

        public JsonResult GetDistCntStats()
        {
            ConfigModel distRecvDay = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "MAIN", comm_code = "MAIN_DIST_CNT_DATE" });
            return Json(Mapper.Instance().QueryForList<DistCntStats>("Main.selDistCntList", new DistCntStats { search_dt = DateTime.Now.AddDays((Convert.ToInt32(distRecvDay.comm_value) * -1)) }));
        }

        public JsonResult GetLatestDistList()
        {
            ConfigModel distRecvDay = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "MAIN", comm_code = "MAIN_DIST_LIST_DATE" });

            DistMasterModel dist = new DistMasterModel();
            dist.dist_sdt = DateTime.Now.AddDays((Convert.ToInt32(distRecvDay.comm_value) * -1)).ToString("yyyy-MM-dd");
            dist.dist_edt = DateTime.Now.ToString("yyyy-MM-dd");
            var retList = Mapper.Instance().QueryForList<DistMasterModel>("DIST.selDistMaster", dist);
            return Json(retList);
        }

        public JsonResult GetFinishDistList()
        {
            ConfigModel distRecvDay = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "MAIN", comm_code = "MAIN_FINISH_DIST_DATE" });

            DistMasterModel dist = new DistMasterModel();
            dist.finish_dt = DateTime.Now.AddDays((Convert.ToInt32(distRecvDay.comm_value)));

            return Json(Mapper.Instance().QueryForList<DistMasterModel>("DIST.selDistMaster", dist));
        }


    }
}