using IBatisNet.DataMapper;
using ezDMS.Class;
using ezDMS.Filter;
using ezDMS.Models.Auth;
using ezDMS.Models.Common;
using ezDMS.Models.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ezDMS.Controllers
{
    public class LoginController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("SystemLog");
        
        // GET: Login
        public ActionResult Index()
        {           
            Response.Expires = -1;
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-cache, must-revalidate");

            return View();
        }

        [AuthFilter]
        public ActionResult logout()
        {
            Response.Expires = -1;
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-cache, must-revalidate");

            if (Session != null)
            {
                Session.Clear();
                FormsAuthentication.SignOut();
            }


            return View();
        }

        [HttpPost]
        public ActionResult TryLogin(UserModel loginInfo)
        {
            LoginHistoryModel loginHis = new LoginHistoryModel();
            try
            {
                /*
                N	잘못된 ID	
                P	잘못된 PW	
                O	OPT 발행	
                X	잘못된 OPT 입력	
                S	정상 로그인	
                */
                string tryID = loginInfo.login_id.Trim();
                string tryPW = AESEncrypt.AESEncrypt256(loginInfo.login_pw, tryID);

                loginHis.try_id = tryID;
                loginHis.try_pw = tryPW;
                loginHis.try_ip = CommonUtil.GetRemoteIP(this.Request);
                loginHis.session_id = Session.SessionID;

                if (tryID == "")
                {
                    loginHis.login_st = "N";
                    throw new Exception("아이디가 입력되지 않았습니다.");
                }

                if(loginInfo.login_pw.Trim() == "")
                {
                    loginHis.login_st = "P";
                    throw new Exception("비밀번호가 입력되지 않았습니다.");
                }

                UserModel user = Mapper.Instance().QueryForObject<UserModel>("User.selUser", loginInfo);

                if(user == null || user.us_idx == null)
                {
                    loginHis.login_st = "N";
                    throw new Exception("잘못된 접속 정보입니다.");
                }

                if (user.login_pw != tryPW)
                {
                    loginHis.login_st = "P";
                    throw new Exception("잘못된 접속 정보입니다.");
                }

                loginHis.login_us = user.us_idx;
                
                if (user.use_fl != "Y")
                {
                    loginHis.login_st = "A";                   
                    throw new Exception("잘못된 접속 정보입니다.");
                }

                loginHis.login_st = "S";

                Session["USER_IDX"] = user.us_idx;
                Session["LOGIN_ID"] = user.login_id;
                Session["USER_NM"] = user.us_nm;
                Session["USER_POS"] = user.us_pos;
                Session["USER_POS_NM"] = user.us_pos_nm;
                Session["USER_GROUP_IDX"] = user.us_group;
                Session["USER_GROUP_NM"] = user.us_group_nm;
                Session["USER_ROLE"] = user.us_role;
                Session["USER_ROLE_NM"] = user.us_role_nm;
                Session["SESSION_ID"] = Session.SessionID;
                Session["LANG_CD"] = loginInfo.langCd;
                var userAuth = Mapper.Instance().QueryForList<AuthModel>("Auth.selAuthInfo", new AuthModel { target_type = "U", target_idx = user.us_idx });
                var deptAuth = Mapper.Instance().QueryForList<AuthModel>("Auth.selAuthInfo", new AuthModel { target_type = "D", target_idx = user.us_group });
                var roleAuth = Mapper.Instance().QueryForList<AuthModel>("Auth.selAuthInfo", new AuthModel { target_type = "R", target_idx = (int)user.us_role });
                               
                Hashtable authList = new Hashtable();

                authList.Add("USER", userAuth);
                authList.Add("DEPT", deptAuth);
                authList.Add("ROLE", roleAuth);

                Session["AUTH"] = authList;


                return Json(new { result = "Redirect", url = Url.Action("index", "Main") });
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("LOGIN ERROR -", loginHis.login_st), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            finally
            {
                loginHis.SetLoginHistory();
            }
        }



        public ActionResult TryLogOut()
        {
            return RedirectToAction("logout", "Login");
        }
    }
}