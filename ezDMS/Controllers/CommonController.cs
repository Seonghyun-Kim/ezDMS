
using IBatisNet.DataMapper;
using IS_PODS.Class;
using IS_PODS.Filter;
using IS_PODS.Models.Auth;
using IS_PODS.Models.Common;
using IS_PODS.Models.Dist;
using IS_PODS.Models.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using static IS_PODS.Define.LogDefine;

namespace IS_PODS.Controllers
{
    public class CommonController : Controller
    { 
        log4net.ILog logger = log4net.LogManager.GetLogger("SystemLog");
        public PartialViewResult DialogUserList(string isVender = "N")
        {
            ViewBag.isVender = isVender;
            return PartialView("dlgUserList");
        }

        //[HttpGet]
        //public ActionResult ErrorView()
        //{
        //    return View();
        //}
      
        public ActionResult ErrorView(Exception ex)
        {
            return View(ex.Message);
        }

        public ActionResult PasswordChange()
        {
            return View("dlgPasswordChange");
        }

        public ActionResult PdfViewer(int? link_file_idx, string is_itf)
        {
            string fileOrgName = string.Empty;
            string fileConvName = string.Empty;
            string filePath = string.Empty;

            if (is_itf == "N")
            {
                DistTempFileModel fileInfo = Mapper.Instance().QueryForObject<DistTempFileModel>("DIST.selDistTempFile", new DistTempFileModel { temp_file_idx = link_file_idx });

                if (fileInfo == null)
                {
                    throw new Exception("잘못된 파일을 호출하셨습니다.");
                }

                fileOrgName = fileInfo.file_org_nm;
                fileConvName = fileInfo.file_conv_nm;

                filePath = System.Configuration.ConfigurationManager.AppSettings["LocalFilePath"].ToString() + "\\" + fileInfo.dist_idx;

                LogCtrl.SetLog(fileInfo, eActionType.FileView, this.HttpContext);
            }
            else
            {
                ItfFileInfo fileInfo = Mapper.Instance().QueryForObject<ItfFileInfo>("ITF.selItfFIleInfo", new ItfFileInfo { file_idx = link_file_idx });

                if (fileInfo == null)
                {
                    throw new Exception("잘못된 파일을 호출하셨습니다.");
                }

                fileOrgName = fileInfo.file_org_nm;
                fileConvName = fileInfo.file_conv_nm;

                filePath = System.Configuration.ConfigurationManager.AppSettings["EoFilePath"].ToString() + "\\" + fileInfo.part_no;

                LogCtrl.SetLog(fileInfo, eActionType.FileView, this.HttpContext);
            }

            if (!CommonUtil.IsFile(filePath, fileConvName))
            {
                ViewBag.Massage = "파일이 없습니다.";
                return View();
            }

            PdfWatermark watermark = new PdfWatermark();
            string watermarkFile = watermark.SetWaterMarkPdf(filePath, fileConvName, Convert.ToInt32(Session["USER_IDX"]), CommonUtil.GetRemoteIP(this.Request));

            string fPath =System.Configuration.ConfigurationManager.AppSettings["ViewTempFileUrl"].ToString() + "/" + Path.GetFileName(watermarkFile);
          
            ViewBag.FileName = fPath;
            return View();
            //return View("PdfFileViewer" + (System.Web.HttpUtility.UrlEncode(fPath, System.Text.Encoding.GetEncoding("utf-8"))).Replace("+", "%20"));
        }

        public ActionResult DistPdfViewer(int? dist_file_idx)
        {
            string fileOrgName = string.Empty;
            string fileConvName = string.Empty;
            string filePath = string.Empty;


            DistRecvFileModel distFile = Mapper.Instance().QueryForObject<DistRecvFileModel>("DIST.selDistRecvFile", new DistRecvFileModel { dist_file_idx = dist_file_idx, recv_us = Convert.ToInt32(Session["USER_IDX"]) });

            if (distFile == null)
            {
                throw new Exception("잘못된 파일을 호출하셨습니다.");
            }

            fileOrgName = distFile.file_org_nm;
            fileConvName = distFile.file_conv_nm;

            if (distFile.is_itf == "N")
            {
                filePath = System.Configuration.ConfigurationManager.AppSettings["LocalFilePath"].ToString() + "\\" + distFile.dist_idx;
            }
            else
            {
                filePath = System.Configuration.ConfigurationManager.AppSettings["EoFilePath"].ToString() + "\\" + distFile.part_no;
            }

            if (!CommonUtil.IsFile(filePath, fileConvName))
            {
                ViewBag.Massage = "파일이 없습니다.";
                return View();
            }

            PdfWatermark watermark = new PdfWatermark();
            string watermarkFile = watermark.SetWaterMarkPdf(filePath, fileConvName, Convert.ToInt32(Session["USER_IDX"]), CommonUtil.GetRemoteIP(this.Request));   

            LogCtrl.SetLog(distFile, eActionType.FileView, this.HttpContext);

            string fPath = System.Configuration.ConfigurationManager.AppSettings["ViewTempFileUrl"].ToString() + "/" + Path.GetFileName(watermarkFile);

            ViewBag.FileName = fPath;
            return View("PdfViewer");
        }

        public ActionResult PdfFileViewer()
        {
            return View();
        }

        public JsonResult GetCommonGroup(CommLibraryModel searchModel)
        {
            try
            {
                var commCodeGroupList = Mapper.Instance().QueryForList<CommLibraryModel>("Common.selCommGroup", searchModel);
                return Json(commCodeGroupList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR CODE GROUP: "), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetCommonCode(CommLibraryModel searchModel)
        {
            try
            {
                var commCodeGroupList = Mapper.Instance().QueryForList<CommLibraryModel>("Common.selCommCode", searchModel);
                return Json(commCodeGroupList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR CODE GROUP: "), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetDeptList(DeptModel searchModel)
        {
            try
            {
                var deptList = Mapper.Instance().QueryForList<DeptModel>("User.selDepartment", searchModel);
                return Json(deptList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 부서정보"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetUserList(UserModel searchModel)
        {
            try
            {
                searchModel.isVender = "N";
                var userList = Mapper.Instance().QueryForList<UserModel>("User.selUser", searchModel);

                if(searchModel.isDist == "Y")
                {
                    userList = userList.Where(x => x.us_idx != Convert.ToInt32(Session["USER_IDX"])).ToList();
                }

                return Json(userList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 사용자정보"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetAllUserList(UserModel searchModel)
        {
            try
            {
                var userList = Mapper.Instance().QueryForList<UserModel>("User.selUser", searchModel);
                return Json(userList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 사용자정보"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetUser(UserModel searchModel)
        {
            try
            {
                searchModel.isVender = "N";
                var userList = Mapper.Instance().QueryForList<UserModel>("User.selUser", searchModel);
                return Json(userList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 사용자정보"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetVendList(VendModel searchModel)
        {
            try
            {
                var deptList = Mapper.Instance().QueryForList<VendModel>("User.selVender", searchModel);
                return Json(deptList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 협력업체 정보"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }


        public JsonResult GetVenderUserList(UserModel searchModel)
        {
            try
            {
                searchModel.isVender = "Y";
                var userList = Mapper.Instance().QueryForList<UserModel>("User.selUser", searchModel);
                return Json(userList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 협력사 사용자 정보"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetVenderUser(UserModel searchModel)
        {
            try
            {
                searchModel.isVender = "Y";
                var userList = Mapper.Instance().QueryForList<UserModel>("User.selUser", searchModel);
                return Json(userList);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 협력사 사용자 정보"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }


        /// <summary>
        /// 수신자가 다운로드 받는 함수
        /// </summary>
        /// <param name="dist_file_idx"></param>
        /// <returns></returns>
        public ActionResult DistFileDownload(int? dist_file_idx)
        {
            System.IO.Stream fStream = null;
            string fileOrgName = string.Empty;
            string fileConvName = string.Empty;
            string filePath = string.Empty;


            DistRecvFileModel distFile = Mapper.Instance().QueryForObject<DistRecvFileModel>("DIST.selDistRecvFile", new DistRecvFileModel { dist_file_idx = dist_file_idx, recv_us = Convert.ToInt32(Session["USER_IDX"]) });
            DistMasterModel dist = Mapper.Instance().QueryForObject<DistMasterModel>("DIST.selDistMaster", new DistMasterModel { dist_idx = distFile.dist_idx });

            if (distFile == null)
            {
                throw new Exception("잘못된 파일을 호출하셨습니다.");
            }

            if(dist.dist_st != "DS")
            {
                throw new Exception("배포가 만료된 파일입니다.");
            }

            fileOrgName = distFile.file_org_nm;
            fileConvName = distFile.file_conv_nm;
                
            if(distFile.is_itf == "N")
            {
                filePath = System.Configuration.ConfigurationManager.AppSettings["LocalFilePath"].ToString() + "\\" + distFile.dist_idx;
            }
            else
            {
                filePath = System.Configuration.ConfigurationManager.AppSettings["EoFilePath"].ToString() + "\\" + distFile.part_no;
            }

            if(!CommonUtil.IsFile(filePath, fileConvName))
            {
                ViewBag.Massage = "파일이 없습니다.";
                return View();
            }

            if (fileConvName.ToLower().Contains(".pdf"))
            {
                PdfWatermark watermark = new PdfWatermark();
                string watermarkFile = watermark.SetWaterMarkPdf(filePath, fileConvName, Convert.ToInt32(Session["USER_IDX"]), CommonUtil.GetRemoteIP(this.Request));
                fStream = CommonUtil.FileStream(watermarkFile);
            }
            else
            {
                fStream = CommonUtil.FileStream(filePath, fileConvName);
            }

            LogCtrl.SetLog(distFile, eActionType.FileDown, this.HttpContext);

            if (Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer")
            {
                return File(fStream, MediaTypeNames.Application.Octet, HttpUtility.UrlEncode(fileOrgName, System.Text.Encoding.UTF8));
            }
            else
            {
                return File(fStream, MediaTypeNames.Application.Octet, fileOrgName);
            }
        }

        /// <summary>
        /// 일반 사용자가 내부 파일 받을 때 쓰는 함수
        /// </summary>
        /// <param name="link_file_idx"></param>
        /// <param name="is_itf"></param>
        /// <returns></returns>
        public ActionResult FileDownload(int? link_file_idx, string is_itf)
        {
            System.IO.Stream fStream = null;
            string fileOrgName = string.Empty;
            string fileConvName = string.Empty;
            string filePath = string.Empty;

            if (is_itf == "N")
            {
                DistTempFileModel fileInfo = Mapper.Instance().QueryForObject<DistTempFileModel>("DIST.selDistTempFile", new DistTempFileModel { temp_file_idx = link_file_idx });

                if (fileInfo == null)
                {
                    throw new Exception("잘못된 파일을 호출하셨습니다.");
                }

                fileOrgName = fileInfo.file_org_nm;
                fileConvName = fileInfo.file_conv_nm;

                filePath = System.Configuration.ConfigurationManager.AppSettings["LocalFilePath"].ToString() + "\\" + fileInfo.dist_idx;

                LogCtrl.SetLog(fileInfo, eActionType.FileDown, this.HttpContext);
            }
            else
            {
                ItfFileInfo fileInfo = Mapper.Instance().QueryForObject<ItfFileInfo>("ITF.selItfFIleInfo", new ItfFileInfo { file_idx = link_file_idx });

                if (fileInfo == null)
                {
                    throw new Exception("잘못된 파일을 호출하셨습니다.");
                }

                fileOrgName = fileInfo.file_org_nm;
                fileConvName = fileInfo.file_conv_nm;

                filePath = System.Configuration.ConfigurationManager.AppSettings["EoFilePath"].ToString() + "\\" + fileInfo.part_no;

                LogCtrl.SetLog(fileInfo, eActionType.FileDown, this.HttpContext);
            }

            if (!CommonUtil.IsFile(filePath, fileConvName))
            {
                ViewBag.Massage = "파일이 없습니다.";
                return View();
            }

            if(fileConvName.ToLower().Contains(".pdf"))
            {
                PdfWatermark watermark = new PdfWatermark();
                string watermarkFile = watermark.SetWaterMarkPdf(filePath, fileConvName, Convert.ToInt32(Session["USER_IDX"]), CommonUtil.GetRemoteIP(this.Request));
                fStream = CommonUtil.FileStream(watermarkFile);
            } 
            else
            {
                fStream = CommonUtil.FileStream(filePath, fileConvName);
            }

            if (Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer")
            {
                return File(fStream, MediaTypeNames.Application.Octet, HttpUtility.UrlEncode(fileOrgName, System.Text.Encoding.UTF8));
            }
            else
            {
                return File(fStream, MediaTypeNames.Application.Octet, fileOrgName);
            }
        }


        [AuthFilter(limitRole = eRole.VENDER)]
        public JsonResult SetChangePassword(string nowPassword, string changePassword, string changeChkPassword)
        {
            try
            {
                UserModel chkUser = Mapper.Instance().QueryForObject<UserModel>("User.selUser", new UserModel { us_idx = Convert.ToInt32(Session["USER_IDX"]) });

                if (chkUser.login_pw != AESEncrypt.AESEncrypt256(nowPassword, Session["LOGIN_ID"].ToString()))
                {
                    throw new Exception("입력하신 비밀번호가 일치하지않습니다.");
                }

                if (changePassword == "" || changeChkPassword == "")
                {
                    throw new Exception("변경 될 비밀번호가 잘못되었습니다.");
                }

                if (changePassword != changeChkPassword)
                {
                    throw new Exception("입력 된 두개의 비밀번호가 다릅니다.");
                }

                // 만약 제약조건 있을경우 여기서 해결한다.

                

                Mapper.Instance().Update("User.udtUser", new UserModel { us_idx = Convert.ToInt32(Session["USER_IDX"]), login_pw = AESEncrypt.AESEncrypt256(changePassword, Session["LOGIN_ID"].ToString()) });

                return Json(1);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 시스템 설정 저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
    }
}