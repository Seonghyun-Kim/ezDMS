using IBatisNet.DataMapper;
using SmartDSP.Class;
using SmartDSP.Filter;
using SmartDSP.Models.Auth;
using SmartDSP.Models.Common;
using SmartDSP.Models.Dist;
using SmartDSP.Models.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static SmartDSP.Define.LogDefine;

namespace SmartDSP.Controllers
{
    [AuthFilter(limitRole = Models.Common.eRole.USER)]
    [ActionFilter]
    public class DistController : Controller
    {
        public ActionResult TempDistList()
        {
            return View();
        }

        public ActionResult CreateDist(int? dist_idx)
        {
            DistMasterModel distMaster = null;
            if(dist_idx != null)
            {
                distMaster = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel() { dist_idx = dist_idx });
            } else
            {
                distMaster = new DistMasterModel();
            }

            if(distMaster.finish_date == "")
            {
                ConfigModel finishTerm = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "DIST", comm_code = "EXPIRE_TERM" });
                distMaster.finish_dt = DateTime.Now.AddDays(Convert.ToInt32(finishTerm.comm_value));
            }

            ViewBag.distMaster = distMaster;
            return View();
        }

        public ActionResult HistoryDistList()
        {
            return View("HistoryList");
        }

        public ActionResult DistView(int? dist_idx)
        {
            if(dist_idx == null)
            {
                return RedirectToAction("ErrorView", "Error", Resources.Resource.res0270);//"잘못된 페이지를 호출하셨습니다."                
            }

            var distModel = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel { dist_idx = dist_idx });
            var distEoModel = Mapper.Instance().QueryForObject<DistEoModel>("DIST.selDistEo", new DistEoModel { dist_idx = dist_idx });
            var distRecvFile = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistRecvFile", new DistRecvFileModel { dist_idx = dist_idx, recv_us = Convert.ToInt32(Session["USER_IDX"]) });
            var eoBom = Mapper.Instance().QueryForList<ItfBomInfo>("Part.selPartBom", new ItfBomInfo { eo_idx = distModel.eo_idx });


            ViewBag.distModel = distModel;
            ViewBag.distEoModel = distEoModel;
            ViewBag.distRecvFile = distRecvFile;
            ViewBag.eoBomList = eoBom;

            return View();
        }

        public PartialViewResult EoSelect()
        {
            return PartialView("Dialog/dlgEoList");
        }

        public PartialViewResult DistRecvLiet(int? dist_idx)
        {
            ViewBag.dist_idx = dist_idx;
            return PartialView("Dialog/dlgDistRecvList");
        }
        public PartialViewResult DateChange(string type, int? target_idx)
        {
            ViewBag.type = type;
            ViewBag.target_idx = target_idx;

            if(type == "dist")
            {
                var distMaster = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel { dist_idx = target_idx });
                ViewBag.date = distMaster.finish_date;
            }
            else
            {
                var distReciever = Mapper.Instance().QueryForObject<DistReceiverModel>("DIST.selDistReceiver", new DistReceiverModel { recv_idx = target_idx });
                ViewBag.date = distReciever.recv_finish_date;
            }

            return PartialView("Dialog/dlgDateChange");
        }

        public JsonResult GetTempDistList(DistMasterModel searchModel)
        {
            searchModel.create_us = Convert.ToInt32(Session["USER_IDX"]);
            return Json(Mapper.Instance().QueryForList<DistMasterModel>("DIST.selDistMaster", searchModel));
        }

        public JsonResult GetDistList(DistMasterModel searchModel)
        {
            return Json(Mapper.Instance().QueryForList<DistMasterModel>("DIST.selDistMaster", searchModel));
        }

        public JsonResult DelDistList(List<DistMasterModel> delList)
        {
            try
            {
                Mapper.Instance().BeginTransaction();

                foreach(DistMasterModel dist in delList)
                {
                    Mapper.Instance().Delete("DIST.delDistRecvFileForDist", dist.dist_idx);
                    int? tempFileDel = Mapper.Instance().Delete("DIST.delDistTempFileForDist", dist.dist_idx);
                    Mapper.Instance().Delete("DIST.delDistReceiverForDist", dist.dist_idx);
                    Mapper.Instance().Delete("DIST.delDistEo", dist.dist_idx);
                    Mapper.Instance().Delete("DIST.delDistMaster", dist.dist_idx);

                    if(tempFileDel > 0)
                    {
                        string valutPath = System.Configuration.ConfigurationManager.AppSettings["LocalFilePath"].ToString();
                        CommonUtil.FolderDelete(valutPath + "\\" + dist.dist_idx);
                    }
                    
                }

                Mapper.Instance().CommitTransaction();

                return Json(1);
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }


        public JsonResult GetItfEoList(ItfEoInfo searchModel)
        {
            searchModel.notuse_fl = "N";
            var eoList = Mapper.Instance().QueryForList<ItfEoInfo>("ITF.selItfEoInfo", searchModel);

            return Json(eoList);
        }

        public int SetDistMaster(DistMasterModel distMaster)
        {
            int distIDX = (int)Mapper.Instance().Insert("DIST.insDistMaster", distMaster);

            LogCtrl.SetLog(new DistMasterModel { dist_idx = distIDX }, eActionType.DistInsert, this.HttpContext, distMaster.dist_title);
            return distIDX;
        }

        public int SetUdtDistMaster(DistMasterModel distMaster)
        {
            LogCtrl.SetLog(distMaster, eActionType.DistSave, this.HttpContext, distMaster.dist_title);
            return (int)Mapper.Instance().Update("DIST.udtDistMaster", distMaster);
        }

        public JsonResult SetTempSaveDistMaster(DistMasterModel saveModel, List<DistReceiverModel> recvList)
        {
            try
            {
                bool isNew = saveModel.dist_idx == null ? true : false;

                Mapper.Instance().BeginTransaction();

                if (isNew)
                {
                    saveModel.dist_idx = SetDistMaster(new DistMasterModel() { eo_fl = "Y", dist_st = "CR", create_us = Convert.ToInt32(Session["USER_IDX"]) });  
                }
               
                SetUdtDistMaster(saveModel);

                if(recvList != null)
                {
                    foreach (DistReceiverModel recv in recvList)
                    {
                        recv.dist_idx = saveModel.dist_idx;
                        Mapper.Instance().Update("DIST.udtDistReceiver", recv);
                    }
                }

                Mapper.Instance().CommitTransaction();
                return Json(saveModel.dist_idx);
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetDistEoInfo(DistEoModel distEo)
        {
            try
            {
                bool isNew = distEo.dist_idx == null ? true : false;
                Mapper.Instance().BeginTransaction();
                // 신규
                if (isNew)
                {
                    distEo.dist_idx = SetDistMaster(new DistMasterModel() { eo_fl = "Y", dist_st = "CR", create_us = Convert.ToInt32(Session["USER_IDX"])});
                    Mapper.Instance().Insert("DIST.insDistEo", distEo);
                }
                else
                {
                    DistEoModel compareEo = Mapper.Instance().QueryForObject<DistEoModel>("DIST.selDistEo", new DistEoModel() { dist_idx = distEo.dist_idx });

                    // 기존 EO에 속한 파일이 있을 경우 지워버린다.
                    if (compareEo != null)
                    {
                        Mapper.Instance().Delete("DIST.delDistRecvEoFile", new DistRecvFileModel { dist_idx = distEo.dist_idx, eo_idx = compareEo.eo_idx });
                        Mapper.Instance().Update("DIST.udtDistEo", distEo);
                    }
                    else
                    {
                        Mapper.Instance().Insert("DIST.insDistEo", distEo);
                    }

                    SetUdtDistMaster(new DistMasterModel { dist_idx = distEo.dist_idx });
                }

                SetRecieverLinkFile(distEo.dist_idx);

                DistMasterModel dist = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel() { dist_idx = distEo.dist_idx });

                LogCtrl.SetLog(distEo, eActionType.DistEoSave, this.HttpContext, dist.dist_title);
                Mapper.Instance().CommitTransaction();
                return Json(distEo.dist_idx);
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }           
        }

        /// <summary>
        /// 배포 데이터중 파일과 수신자를 연결 (처음 수신자 OR 파일 등록 시 모든 수신자가 해당 파일을 받을 수 있도록 설정함.)
        /// 호출시 Transaction 필수 임
        /// </summary>
        /// <param name="dist_idx">배포 IDX</param>
        /// <returns></returns>
        public void SetRecieverLinkFile(int? dist_idx)
        {
            try
            {                
                // 수신자가 없을 경우 retrun
                var distRecv = Mapper.Instance().QueryForList<DistReceiverModel>("DIST.selDistReceiver", new DistReceiverModel() { dist_idx = dist_idx });
                if(distRecv.Count == 0) return;

                // 모든 수신자의 파일들 
                List<DistRecvFileModel> distRecvFileList = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistRecvFile", new DistRecvFileModel { dist_idx = dist_idx }).Cast<DistRecvFileModel>().ToList();

                // EO 정보
                DistEoModel distEo = Mapper.Instance().QueryForObject<DistEoModel>("DIST.selDistEo", new DistEoModel() { dist_idx = dist_idx });

                int? eo_idx = distEo == null ? null : distEo.eo_idx;

                // 배당 배포에 속한 모든 파일 
                List<DistRecvFileModel> distAllFileList = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistFileList", new DistRecvFileModel() { dist_idx = dist_idx , eo_idx = eo_idx }).Cast<DistRecvFileModel>().ToList();
                
                // EO 가 존재 할 경우에 EO에 속한 파일들을 수신자에 입력한다.
                if (distEo != null)
                {
                    var eoFileList = from eoFile in distAllFileList
                                     where eoFile.is_itf == "Y"
                                     select eoFile;

                    foreach(DistReceiverModel recv in distRecv)
                    {
                        foreach(DistRecvFileModel eoFIle in eoFileList)
                        {
                            var receiverEoFile = from f in distRecvFileList
                                                 where f.recv_idx == recv.recv_idx && f.link_file_idx == eoFIle.link_file_idx && f.is_itf == "Y"
                                                 select f;

                            // 해당 수신자에게 파일이 미리 등록되어 있을 경우는 패스함
                            if(receiverEoFile.Count() > 0)
                            {
                                continue;
                            }

                            // 파일이 없는 PART 가 넘어올수 있음(BOM)
                            if(eoFIle.link_file_idx == null)
                            {
                                continue;
                            }

                            Mapper.Instance().Insert("DIST.insDistRecvFile", new DistRecvFileModel { dist_idx = dist_idx, recv_idx = recv.recv_idx, is_itf = "Y", link_file_idx = eoFIle.link_file_idx });

                        }
                    }
                }

                // LOCAL 파일이 존재 할 경우에 수신자에게 LOCAL 파일을 입력한다.
                var localFileList = from localFile in distAllFileList
                                 where localFile.is_itf == "N"
                                 select localFile;

                foreach (DistReceiverModel recv in distRecv)
                {
                    foreach (DistRecvFileModel localFile in localFileList)
                    {
                        var receiverlocalFile = from f in distRecvFileList
                                             where f.recv_idx == recv.recv_idx && f.link_file_idx == localFile.link_file_idx && f.is_itf == "N"
                                             select f;

                        // 해당 수신자에게 파일이 미리 등록되어 있을 경우는 패스함
                        if (receiverlocalFile.Count() > 0)
                        {
                            continue;
                        }

                        Mapper.Instance().Insert("DIST.insDistRecvFile", new DistRecvFileModel { dist_idx = dist_idx, recv_idx = recv.recv_idx, is_itf = "N", link_file_idx = localFile.link_file_idx });
                    }
                }
                
                SetUdtDistMaster(new DistMasterModel { dist_idx = dist_idx });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDistEoList(DistEoModel searchModel)
        {
            var eoList = Mapper.Instance().QueryForList<DistEoModel>("DIST.selDistEo", searchModel);

            return Json(eoList);
        }

        public JsonResult SetDistReceiver(int? dist_idx, List<DistReceiverModel> receiverModel)
        {
            try
            {
                Mapper.Instance().BeginTransaction();
                DistMasterModel dist = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel() { dist_idx = dist_idx });

                if (dist_idx == null)
                {
                    dist_idx = SetDistMaster(new DistMasterModel() { eo_fl = "Y", dist_st = "CR", create_us = Convert.ToInt32(Session["USER_IDX"]) });
                }
                else
                {
                    SetUdtDistMaster(new DistMasterModel { dist_idx = dist_idx });
                }
              
                var prevRecvList = Mapper.Instance().QueryForList<DistReceiverModel>("DIST.selDistReceiver", new DistReceiverModel { dist_idx = dist_idx });

                foreach (DistReceiverModel recv in receiverModel)
                {
                    var chkRecv = from rcv in prevRecvList
                                    where rcv.recv_us == recv.recv_us
                                    select rcv;

                    if(chkRecv.Count() > 0)
                    {
                        continue;
                    }
                    else
                    {
                        recv.dist_idx = dist_idx;
                        Mapper.Instance().Insert("DIST.insDistReceiver", recv);

                        LogCtrl.SetLog(recv, eActionType.DistReceiverSave, this.HttpContext, dist.dist_title);
                    }
                }

                SetRecieverLinkFile(dist_idx);

                Mapper.Instance().CommitTransaction();

                return Json(dist_idx);
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetDistReciverInfo(DistReceiverModel searchModel)
        {
            try
            {
                if (searchModel.dist_idx == null) return Json(null);

                var resList = Mapper.Instance().QueryForList<DistReceiverModel>("DIST.selDistReceiver", searchModel);

                return Json(resList);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetDelDistReceiver (List<DistReceiverModel> receiverModel)
        {
            try
            {
                Mapper.Instance().BeginTransaction();
                DistMasterModel dist = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel() { dist_idx = receiverModel[0].dist_idx });

                SetUdtDistMaster(new DistMasterModel { dist_idx = receiverModel[0].dist_idx });
                foreach (DistReceiverModel distRecv in receiverModel)
                {
                    // 1. 수신자에 속한 배포 파일 삭제 
                   Mapper.Instance().Delete("DIST.delDistRecvFile", new DistRecvFileModel { dist_idx = distRecv.dist_idx, recv_idx = distRecv.recv_idx });

                    // 2. 수신자 삭제
                    int i = (int)Mapper.Instance().Delete("DIST.delDistReceiver", new DistReceiverModel { dist_idx = distRecv.dist_idx, recv_idx = distRecv.recv_idx });

                    LogCtrl.SetLog(distRecv, eActionType.DistReceiverDelete, this.HttpContext, dist.dist_title);
                    if (i == 0)
                    {
                        throw new Exception(Resources.Resource.res0312);//수신자 삭제가 실패했습니다
                    }
                }

                Mapper.Instance().CommitTransaction();

                return Json(1);
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetDistFileList(int? dist_idx)
        {
            if (dist_idx == null)
            {
                return Json(null);
            }
            try
            {
                DistEoModel distEo = Mapper.Instance().QueryForObject<DistEoModel>("DIST.selDistEo", new DistEoModel { dist_idx = dist_idx });

                int? eoIDX = distEo == null ? null : distEo.eo_idx;
                var distFileList = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistFileList", new DistRecvFileModel { eo_idx = eoIDX, dist_idx = dist_idx });

                return Json(distFileList);
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public ActionResult DistTempFileUpload(int? dist_idx)
        {
            try
            {
                if (Request.Files == null) { throw new Exception(Resources.Resource.res0221);/*업로드할 파일이 존재하지 않습니다*/ }

                try
                {
                    Mapper.Instance().BeginTransaction();

                    if (dist_idx == null)
                    {
                        dist_idx = SetDistMaster(new DistMasterModel() { eo_fl = "Y", dist_st = "CR", create_us = Convert.ToInt32(Session["USER_IDX"]) });
                    }

                    foreach (string f in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[f];

                       DistTempFileModel distTempFileList = Mapper.Instance().QueryForObject<DistTempFileModel>("DIST.selDistTempFile", new DistTempFileModel() { dist_idx = dist_idx, file_org_nm = file.FileName });

                        if (distTempFileList != null)
                        {
                            throw new Exception(Resources.Resource.res0327);//한 배포 건 안에 동일한 이름의 파일을 업로드 할 수 없습니다
                        }

                        string fileOrgName = file.FileName;
                        string fileExtension = Path.GetExtension(file.FileName);
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string fileConvNm = AESEncrypt.AESEncrypt256(fileName, dist_idx.ToString());

                        int? tempFileIdx = (int)Mapper.Instance().Insert("DIST.insDistTempFile", new DistTempFileModel { dist_idx = dist_idx, file_org_nm = file.FileName, file_conv_nm = fileConvNm + fileExtension });

                        string valutPath = System.Configuration.ConfigurationManager.AppSettings["LocalFilePath"].ToString();
                        CommonUtil.FileSave(valutPath + "\\" + dist_idx, file, fileConvNm + fileExtension);

                        LogCtrl.SetLog(new DistTempFileModel { dist_idx = dist_idx, temp_file_idx = tempFileIdx }, eActionType.DistLocalFileSave, this.HttpContext, fileOrgName);

                    }

                    SetRecieverLinkFile(dist_idx);
                    Mapper.Instance().CommitTransaction();
                    return Json(dist_idx);
                }
                catch(Exception ex)
                {
                    Mapper.Instance().RollBackTransaction();
                    throw ex;
                }
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetDistRecvFileList(DistRecvFileModel searchModel)
        {
            var distFileList = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistRecvFile", searchModel);

            return Json(distFileList);

        }

        public JsonResult SetRecvFile(List<DistRecvFileModel> updateModel)
        {
            try
            {
                Mapper.Instance().BeginTransaction();

                foreach(DistRecvFileModel recvFile in updateModel)
                {
                    recvFile.use_fl = "Y";
                    Mapper.Instance().Update("DIST.udtDistRecvFile", recvFile);
                }

                SetUdtDistMaster(new DistMasterModel { dist_idx = updateModel[0].dist_idx });
                Mapper.Instance().CommitTransaction();

                return Json(updateModel[0].dist_idx);
            }
            catch (Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetDeleteRecvFile(List<DistRecvFileModel> searchModel)
        {
            try
            {
                Mapper.Instance().BeginTransaction();

                foreach(DistRecvFileModel rcvFile in searchModel)
                {
                    if (rcvFile.recv_idx == null || rcvFile.is_itf == null || rcvFile.link_file_idx == null)
                    {
                        throw new Exception(Resources.Resource.res0321);//잘못된 형식입니다. 삭제에 실패했습니다
                    }

                    rcvFile.use_fl = "N";
                    Mapper.Instance().Update("DIST.udtDistRecvFile", rcvFile);
                }

                SetUdtDistMaster(new DistMasterModel { dist_idx = searchModel[0].dist_idx });
                Mapper.Instance().CommitTransaction();

                return Json(1);
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            
            
        }

        public JsonResult StartDist(DistMasterModel distModel, List<DistReceiverModel> recvList)
        {
            try
            {
                Mapper.Instance().BeginTransaction();

                if(recvList == null)
                {
                    throw new Exception(Resources.Resource.res0311);// 선택 된 수신자가 없습니다
                }

                var distFileList = Mapper.Instance().QueryForList<DistRecvFileModel>("DIST.selDistRecvFile", new DistRecvFileModel { dist_idx = distModel.dist_idx, use_fl = "Y" });

                if (distFileList.Count == 0)
                {
                    throw new Exception(Resources.Resource.res0296);//"배포 할 파일이 없습니다."
                }
                
                TimeSpan TS = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")) - DateTime.Parse(distModel.finish_date);

                int diffDay = TS.Days;

                if(diffDay >= 0)
                {
                    throw new Exception(Resources.Resource.res0294);//만료일은 금일 보다 이후여야 합니다
                }

                //selConfig
                //DIST	EXPIRE_TERM	10
                //DIST DISCARD_TERM    20
                //
                ConfigModel disuseTerm = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "DIST", comm_code = "DISCARD_TERM" });

                DateTime now = DateTime.Now;

                distModel.dist_st = "DS";
                distModel.dist_dt = now;

                SetUdtDistMaster(distModel);

                foreach (DistReceiverModel recv in recvList)
                {
                    recv.recv_finish_dt = distModel.finish_dt;
                    recv.recv_dist_st = "DS";

                    Mapper.Instance().Update("DIST.udtDistReceiver", recv);
                }

                DistMasterModel distMaster = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel { dist_idx = distModel.dist_idx });
                UserModel distUsr = Mapper.Instance().QueryForObject<UserModel>("User.selUser", new UserModel { us_idx = distMaster.create_us });
                List<DistReceiverModel> distRecvList = Mapper.Instance().QueryForList<DistReceiverModel>("DIST.selDistReceiver", new DistReceiverModel { dist_idx = distModel.dist_idx }).Cast<DistReceiverModel>().ToList();

                // Transaction 이 걸려 있어 임시로 데이터 대입
                distMaster.dist_dt = now;
                distMaster.finish_dt = distModel.finish_dt;
                distMaster.dist_title = distModel.dist_title;
                distMaster.dist_msg = distModel.dist_msg;

                DistMail smtpMail = new DistMail(Request);

                smtpMail.SetMailInfo(distMaster, distUsr, distRecvList);
                smtpMail.SendMail();

                LogCtrl.SetLog(distModel, eActionType.DistStart, this.HttpContext, distModel.dist_title);
                Mapper.Instance().CommitTransaction();
                return Json(1);
            }
            catch (Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetReDist(DistMasterModel distModel)
        {
            try
            {
                if(distModel == null)
                {
                    throw new Exception(Resources.Resource.res0219);//잘못된 호출입니다
                }

                if (distModel.dist_idx == null)
                {
                    throw new Exception(Resources.Resource.res0219);//잘못된 호출입니다
                }

                Mapper.Instance().BeginTransaction();

                var recvList = Mapper.Instance().QueryForList<DistReceiverModel>("DIST.selDistReceiver", new DistReceiverModel { dist_idx = distModel.dist_idx });

                ConfigModel disuseTerm = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "DIST", comm_code = "EXPIRE_TERM" });

                DateTime now = DateTime.Now;
                DateTime finishDate = now.AddDays(Convert.ToInt32(disuseTerm.comm_value));

                distModel.dist_st = "DS";
                distModel.finish_dt = finishDate;



                int recvUpdate = Mapper.Instance().Update("DIST.udtDistReceiverFinishdate", new DistReceiverModel { dist_idx = distModel.dist_idx, recv_finish_dt = finishDate, recv_dist_st = "DS" });

                if(recvUpdate <= 0)
                {
                    throw new Exception(Resources.Resource.res0313);//수신자 상태를 수정하지 못했습니다
                }

                int distIdx = Mapper.Instance().Update("DIST.udtDistMaster", new DistMasterModel { dist_idx = distModel.dist_idx, finish_dt = finishDate, dist_st = "DS" });

                if (distIdx <= 0)
                {
                    throw new Exception(Resources.Resource.res0295);//"배포 상태를 수정하지 못했습니다."
                }

                Mapper.Instance().CommitTransaction();

                return Json("1");
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetReDistReceiver(DistReceiverModel reciverModel)
        {
            try
            {
                if (reciverModel == null)
                {
                    throw new Exception(Resources.Resource.res0219);//잘못된 호출입니다
                }

                if (reciverModel.dist_idx == null)
                {
                    throw new Exception(Resources.Resource.res0219);//잘못된 호출입니다
                }

                Mapper.Instance().BeginTransaction();

                ConfigModel disuseTerm = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "DIST", comm_code = "EXPIRE_TERM" });

                DateTime now = DateTime.Now;
                DateTime finishDate = now.AddDays(Convert.ToInt32(disuseTerm.comm_value));

                int recvUpdate = Mapper.Instance().Update("DIST.udtDistReceiverFinishdate", new DistReceiverModel { dist_idx = reciverModel.dist_idx, recv_idx = reciverModel.recv_idx, recv_finish_dt = finishDate, recv_dist_st = "DS" });

                if (recvUpdate <= 0)
                {
                    throw new Exception(Resources.Resource.res0313);//수신자 상태를 수정하지 못했습니다
                }

                Mapper.Instance().CommitTransaction();

                return Json("1");
            }
            catch (Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetDistFinish(DistMasterModel distModel)
        {
            try
            {
                if (distModel.dist_idx == null) { throw new Exception(Resources.Resource.res0219);/*잘못된 호출입니다*/ }

                    distModel.dist_st = "DF";

                Mapper.Instance().Update("DIST.udtDistMaster", distModel);

                Mapper.Instance().Update("DIST.udtDistReceiverStatus", new DistReceiverModel { dist_idx = distModel.dist_idx, recv_dist_st = "DF" });

                DistMasterModel distMaster = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel { dist_idx = distModel.dist_idx });

                LogCtrl.SetLog(distModel, eActionType.CompulsionExpire, this.HttpContext, distMaster.dist_title);

                return Json("1");
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetChangeDistFinishDate(DistMasterModel distModel)
        {
            try
            {
                if(distModel.finish_date == "") { throw new Exception(Resources.Resource.res0220); }//"날짜가 설정되지 않았습니다."

                Mapper.Instance().Update("DIST.udtDistMaster", distModel);
                Mapper.Instance().Update("DIST.udtDistReceiverFinishdate", new DistReceiverModel { dist_idx = distModel.dist_idx, recv_finish_dt = distModel.finish_dt });

                return Json(distModel.finish_date);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetChangeRecieverFinishDate(DistReceiverModel recvModel)
        {
            try
            {
                if (recvModel.recv_finish_date == "") { throw new Exception(Resources.Resource.res0220); }//"날짜가 설정되지 않았습니다."
                Mapper.Instance().Update("DIST.udtDistReceiverFinishdate", recvModel);

                return Json(recvModel.recv_finish_date);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public ActionResult SetBookmark()
        {
            return View("Dialog/dlgSetBookmark");
        }

        public ActionResult GetBookmark ()
        {
            return View("Dialog/dlgGetBookmark");
        }

        /// <summary>
        /// 배포에 등록된 수신자를 이용하여 즐겨찾기 추가
        /// </summary>
        /// <param name="dist_idx"></param>
        /// <returns></returns>
        public JsonResult SetBookmarkGroup (int? dist_idx, string grp_nm)
        {
            try
            {
                Mapper.Instance().BeginTransaction();

                if (dist_idx == null)
                {
                    throw new Exception(Resources.Resource.res0597);//잘못된 호출입니다. NO DIST ID
                }

                if(grp_nm.Trim() == "")
                {
                    throw new Exception(Resources.Resource.res0290);//그룹명이 입력되지않았습니다
                }

                var recvList = Mapper.Instance().QueryForList<DistReceiverModel>("DIST.selDistReceiver", new DistReceiverModel { dist_idx = dist_idx });

                if(recvList == null || recvList.Count <= 0)
                {
                    throw new Exception(Resources.Resource.res0314);//수신자가 지정되지 않은 배포입니다. 수신자를 먼저 지정해주세요.
                }

                int grpRes = (int)Mapper.Instance().Insert("DIST.intBookmarkGroup", new BookmarkGroup { grp_nm = grp_nm, create_us = Convert.ToInt32(Session["USER_IDX"]) });

                if(grpRes <= 0)
                {
                    throw new Exception(Resources.Resource.res0301);//북마크 등록이 실패했습니다. GROUP
                }

                foreach(DistReceiverModel recv in recvList)
                {
                    BookmarkUser bUsr = new BookmarkUser();
                    bUsr.grp_idx = grpRes;
                    bUsr.grp_us_idx = recv.recv_us;

                    Mapper.Instance().Insert("DIST.intBookmarkUser", bUsr);
                }

                Mapper.Instance().CommitTransaction();

                return Json("1");
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetBookmarkGroup()
        {
            try
            {
                var bookmarkGrpList = Mapper.Instance().QueryForList<BookmarkGroup>("DIST.selBookmarkGroup",  new BookmarkGroup { create_us = Convert.ToInt32(Session["USER_IDX"]) });
                return Json(bookmarkGrpList);
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetBookmarkUser(int? grp_idx)
        {
            try
            {
                if (grp_idx <= 0)
                {
                    throw new Exception(Resources.Resource.res0322);//잘못된 호출입니다. GROUP IDX
                }

                var bookmarkUserList = Mapper.Instance().QueryForList<BookmarkUser>("DIST.selBookmarkUser", new BookmarkUser { grp_idx = grp_idx, create_us = Convert.ToInt32(Session["USER_IDX"]) });
                return Json(bookmarkUserList);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult DelBookmark(int? grp_idx)
        {
            try
            {
                Mapper.Instance().BeginTransaction();

                int grpRes = (int)Mapper.Instance().Delete("DIST.delBookmarkGroup", new BookmarkGroup { grp_idx = grp_idx, create_us = Convert.ToInt32(Session["USER_IDX"]) });

                if (grpRes <= 0)
                {
                    throw new Exception(Resources.Resource.res0302);//북마크 삭제가 실패했습니다. GROUP
                }

                int usrRes = (int)Mapper.Instance().Delete("DIST.delBookmarkUser", new BookmarkUser { grp_idx = grp_idx });

                if (grpRes <= 0)
                {
                    throw new Exception(Resources.Resource.res0303);//북마크에 등록된 수신자 삭제가 실패했습니다. USER
                }

                Mapper.Instance().CommitTransaction();

                return Json(1);
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetBookmarkForDistReceiver(int? dist_idx, int? grp_idx)
        {
            try
            {
                var bookmarkUserList = Mapper.Instance().QueryForList<BookmarkUser>("DIST.selBookmarkUser", new BookmarkUser { grp_idx = grp_idx, create_us = Convert.ToInt32(Session["USER_IDX"]) });

                List<DistReceiverModel> list = new List<DistReceiverModel>();
                foreach(BookmarkUser usr in bookmarkUserList)
                {
                    DistReceiverModel recv = new DistReceiverModel();
                    recv.dist_idx = dist_idx;
                    recv.recv_us = usr.grp_us_idx;
                    recv.isVender = usr.us_role == 10 ? "Y" : "N";

                    list.Add(recv);
                }

                return SetDistReceiver(dist_idx, list);
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            
        }
    }
}