using IBatisNet.DataMapper;
using ezDMS.Class;
using ezDMS.Filter;
using ezDMS.Models.Auth;
using ezDMS.Models.Common;
using ezDMS.Models.Log;
using ezDMS.Models.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Controllers
{
    [AuthFilter][ActionFilter]
    public class AdminController : Controller
    { 
        log4net.ILog logger = log4net.LogManager.GetLogger("SystemLog");

        // GET: Admin
        public ActionResult index()
        {
            return View();
        }

        public ActionResult CommonManager()
        {
            return View();
        }

        public ActionResult ConfigManager()
        {
            var configList = Mapper.Instance().QueryForList<ConfigModel>("Common.selConfig", null);

            return View(configList);
        }

        public ActionResult SystemHistory()
        {
            return View();
        }

        public ActionResult LoginHistory()
        {
            return View();
        }

        public ActionResult UserManager()
        {
            return View();
        }

        public ActionResult UserActionHistory(int? action_us)
        {
            var ActionList = Mapper.Instance().QueryForList<ActionHistoryModel>("Log.selActionHis", new ActionHistoryModel { action_us = action_us });

            return View("Dialog/dlgUserActionHistory", ActionList);
        }
        public ActionResult VenderManager()
        {
            return View();
        }

        #region -- 다이얼로그 
        public PartialViewResult EditDepartment(DeptModel SearchDept)
        {
            DeptModel retModel = null;
            if (SearchDept.dept_idx != null) { retModel = Mapper.Instance().QueryForObject<DeptModel>("User.selDepartment", SearchDept); }
            else { retModel = SearchDept; }
            return PartialView("Dialog/dlgEditDepartment", retModel);
        }

        public PartialViewResult EditCommonCode(int? commCodeIDX, int? parentIdx)
        {
            CommLibraryModel commCode = null;
            if(commCodeIDX != null) { commCode = Mapper.Instance().QueryForObject<CommLibraryModel>("Common.selCommCode", new CommLibraryModel { idx = commCodeIDX }); }
            else { commCode = new CommLibraryModel(); }
            ViewBag.parentIdx = parentIdx;
            ViewBag.commCode = commCode;

            return PartialView("Dialog/dlgEditCommCode");
        }

        public PartialViewResult EditVender(VendModel searchModel)
        {            
            VendModel retModel = null;
            if (searchModel.vend_idx != null) { retModel = Mapper.Instance().QueryForObject<VendModel>("User.selVender", searchModel); }
            else { retModel = searchModel; }

            return PartialView("Dialog/dlgEditVend", retModel);
        }


        public PartialViewResult EditUser(UserModel searchUser)
        {
            UserModel retModel = null;
            if (searchUser.us_idx != null) { retModel = Mapper.Instance().QueryForObject<UserModel>("User.selUser", searchUser); }
            else {
                retModel = new UserModel();
                retModel.us_group = searchUser.us_group;
            }

            retModel.isVender = searchUser.isVender;

            return PartialView("Dialog/dlgEditUser", retModel);
        }

        #endregion

        public JsonResult SetDepartment(DeptModel SearchDept)
        {
            try
            {
                int resultInt = 0;

                if(SearchDept.dept_idx == null)
                {
                    SearchDept.create_us = Convert.ToInt32(Session["USER_IDX"]);
                    resultInt = (int)Mapper.Instance().Insert("User.insDepartment", SearchDept);
                    SearchDept.dept_idx = resultInt;
                    LogCtrl.SetLog(SearchDept, eActionType.DataInsert, this.HttpContext, SearchDept.dept_nm);
                }
                else
                {
                    resultInt = Mapper.Instance().Update("User.udtDepartment", SearchDept);
                    LogCtrl.SetLog(SearchDept, eActionType.DataUpdate, this.HttpContext, SearchDept.dept_nm);
                }
             
                return Json(resultInt);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 부서저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult DelDepartment(int deptIdx)
        {
            try
            {
                int resultInt = 0;

                UserModel SearchUser = new UserModel();
                SearchUser.isVender = "N";
                SearchUser.isLowDepartment = "Y";
                SearchUser.us_group = deptIdx;
                var userList = Mapper.Instance().QueryForList<UserModel>("User.selUser", SearchUser);

                if(userList.Count() > 0)
                {
                    throw new Exception("사용자가 있는 부서는 삭제 할 수 없습니다.");
                }

                DeptModel dept = Mapper.Instance().QueryForObject<DeptModel>("User.selDepartment", new DeptModel { dept_idx = deptIdx });

                var SonDepartment = Mapper.Instance().QueryForList<Hashtable>("User.selTargetSonDept", deptIdx);

                if(SonDepartment.Count() > 1)
                {
                    throw new Exception("하위 부서가 존재하여 삭제 할 수 없습니다.");
                }

                resultInt = (int)Mapper.Instance().Delete("User.delDepartment", deptIdx);

                if(resultInt < 1)
                {
                    throw new Exception("삭제되지 않았습니다. 관리자에게 문의해주세요.");
                }

                LogCtrl.SetLog(new DeptModel { dept_idx = deptIdx }, eActionType.DataDelete, this.HttpContext, dept.dept_nm);
                return Json(resultInt);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 부서저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetVender(VendModel argModel )
        {
            try
            {
                int resultInt = 0;

                if (argModel.vend_idx == null)
                {
                    argModel.create_us = Convert.ToInt32(Session["USER_IDX"]);

                    resultInt = (int)Mapper.Instance().Insert("User.insVender", argModel);
                    argModel.vend_idx = resultInt;
                    LogCtrl.SetLog(argModel, eActionType.DataInsert, this.HttpContext, argModel.vend_nm);
                }
                else
                {
                    resultInt = Mapper.Instance().Update("User.udtVender", argModel);
                    LogCtrl.SetLog(argModel, eActionType.DataUpdate, this.HttpContext, argModel.vend_nm);
                }

                return Json(resultInt);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 협력사 저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        public JsonResult SetCommCode(CommLibraryModel commcode)
        {
            try
            {
                int resultInt = 0;

                if (commcode.idx == null)
                {
                    commcode.create_us = Convert.ToInt32(Session["USER_IDX"]);

                    resultInt = (int)Mapper.Instance().Insert("Common.insCommCode", commcode);
                    commcode.idx = resultInt;
                    LogCtrl.SetLog(commcode, eActionType.DataInsert, this.HttpContext, commcode.kor_nm);
                }
                else
                {
                    resultInt = Mapper.Instance().Update("Common.udtCommCode", commcode);
                    LogCtrl.SetLog(commcode, eActionType.DataUpdate, this.HttpContext, commcode.kor_nm);
                }

                return Json(resultInt);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 공통코드 저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetUserEdit(UserModel user)
        {
            try
            {
                if(user.login_id.Trim() == "")
                {
                    throw new Exception("로그인 아이디를 입력해주세요.");
                }

                if (user.us_nm.Trim() == "")
                {
                    throw new Exception("사용자 이름을 입력해주세요.");
                }

                if (user.us_email.Trim() == "")
                {
                    throw new Exception("사용자 이메일을 입력해주세요.");
                }

                if (user.us_group == null)
                {
                    throw new Exception("사용자 부서를 입력해주세요.");
                }

                if (user.us_role == null)
                {
                    throw new Exception("사용자 권한을 입력해주세요.");
                }

                UserModel chkUser = Mapper.Instance().QueryForObject<UserModel>("User.selUser", new UserModel { login_id = user.login_id.Trim() });

                if(chkUser != null && chkUser.us_idx != user.us_idx)
                {
                    throw new Exception("동일한 로그인 계정이 존재합니다. 로그인 아이디를 확인해주세요.");
                }

                UserModel chkUserEmail = Mapper.Instance().QueryForObject<UserModel>("User.selUser", new UserModel { us_email = user.us_email.Trim() });

                if (chkUserEmail != null && chkUser.us_idx != user.us_idx)
                {
                    throw new Exception("동일한 메일 계정이 존재합니다. 메일을 확인해주세요.");
                }

                int resultInt = 0;
                user.login_id = user.login_id.Trim();

                if (user.us_idx == null)
                {
                    user.create_us = Convert.ToInt32(Session["USER_IDX"]);                
                    user.login_pw = AESEncrypt.AESEncrypt256(CommonUtil.DefaultPassword(), user.login_id.Trim());

                    resultInt = (int)Mapper.Instance().Insert("User.insUser", user);
                    user.us_idx = resultInt;
                    LogCtrl.SetLog(user, eActionType.DataInsert, this.HttpContext, user.us_nm);
                }
                else
                {
                    resultInt = Mapper.Instance().Update("User.udtUser", user);
                    LogCtrl.SetLog(user, eActionType.DataUpdate, this.HttpContext, user.us_nm);
                }

                return Json(resultInt);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 사용자 저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetVendUserEdit(UserModel user)
        {
            try
            {
                if (user.login_id.Trim() == "")
                {
                    throw new Exception("로그인 아이디를 입력해주세요.");
                }

                if (user.us_nm.Trim() == "")
                {
                    throw new Exception("사용자 이름을 입력해주세요.");
                }

                if (user.us_email.Trim() == "")
                {
                    throw new Exception("사용자 이메일을 입력해주세요.");
                }

                UserModel chkUser = Mapper.Instance().QueryForObject<UserModel>("User.selUser", new UserModel { login_id = user.login_id.Trim() });

                if (chkUser != null && chkUser.us_idx != user.us_idx)
                {
                    throw new Exception("동일한 로그인 계정이 존재합니다. 로그인 아이디를 확인해주세요.");
                }

                UserModel chkUserEmail = Mapper.Instance().QueryForObject<UserModel>("User.selUser", new UserModel { us_email = user.us_email.Trim() });

                if (chkUserEmail != null && chkUser.us_idx != user.us_idx)
                {
                    throw new Exception("동일한 메일 계정이 존재합니다. 메일을 확인해주세요.");
                }

                user.us_role = 10;

                int resultInt = 0;

                user.login_id = user.login_id.Trim();

                if (user.us_idx == null)
                {
                    user.create_us = Convert.ToInt32(Session["USER_IDX"]);
                    user.login_pw = AESEncrypt.AESEncrypt256(CommonUtil.DefaultPassword(), user.login_id.Trim());

                    resultInt = (int)Mapper.Instance().Insert("User.insUser", user);
                    user.us_idx = resultInt;
                    LogCtrl.SetLog(user, eActionType.DataInsert, this.HttpContext, user.us_nm);
                }
                else
                {
                    resultInt = Mapper.Instance().Update("User.udtUser", user);
                    LogCtrl.SetLog(user, eActionType.DataUpdate, this.HttpContext, user.us_nm);
                }

                return Json(resultInt);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ERROR : 사용자 저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetResetPassword (int? us_idx)
        {
            try
            {
                if(us_idx == null)
                {
                    throw new Exception("잘못된 호출입니다.");
                }

                UserModel chkUser = Mapper.Instance().QueryForObject<UserModel>("User.selUser", new UserModel { us_idx = us_idx });

                Mapper.Instance().Update("User.udtUser", new UserModel { us_idx = us_idx, login_pw = AESEncrypt.AESEncrypt256(CommonUtil.DefaultPassword(), chkUser.login_id.Trim()) });

                return Json(1);
            }
            catch (Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                logger.Error(string.Format("ERROR : 시스템 설정 저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult SetConfig(List<ConfigModel> argModel)
        {
            try
            {
                Mapper.Instance().BeginTransaction();

                foreach (ConfigModel conf in argModel)
                {
                    Mapper.Instance().Update("Common.udtConfig", conf);
                }

                Mapper.Instance().CommitTransaction();
                return Json(1);
            }
            catch(Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                logger.Error(string.Format("ERROR : 시스템 설정 저장"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult GetLoginHistory(LoginHistoryModel searchParam)
        {
            var LoginList = Mapper.Instance().QueryForList<LoginHistoryModel>("Log.selLoginHis", searchParam);

            return Json(LoginList);
        }

        public JsonResult GetActionHistory(ActionHistoryModel searchParam)
        {
            var ActionList = Mapper.Instance().QueryForList<ActionHistoryModel>("Log.selActionHis", searchParam);

            return Json(ActionList);
        }

        public JsonResult GetLoginCntStats(LoginDateStats searchParam)
        {
            var cntList = Mapper.Instance().QueryForList<LoginDateStats>("Log.selLoginDateCnt", searchParam);

            return Json(cntList);
        }

        public JsonResult GetLoginTopUserStats(LoginTopUserStats searchParam)
        {
            var cntList = Mapper.Instance().QueryForList<LoginTopUserStats>("Log.selLoginTopUser", searchParam);

            return Json(cntList);
        }

        
    }
}