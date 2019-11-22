using IBatisNet.DataMapper;
using ezDMS.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ezDMS.Define.LogDefine;
using ezDMS.Class;

namespace ezDMS.Models.Auth
{
    public class UserModel : IAction
    {
        public int? us_idx { get; set; }
        public string login_id { get; set; }
        public string login_pw { get; set; }
        public string us_nm { get; set; }

        public string us_email { get; set; }

        public int? us_pos { get; set; }
        public string us_pos_kor_nm { get; set; }
        //public string us_pos_eng_nm { get; set; }
        //public string us_pos_chn_nm { get; set; }
        //public string us_pos_etc1_nm { get; set; }
        //public string us_pos_etc2_nm { get; set; }

        public string us_pos_nm 
        { 
            get 
            {
                return us_pos_kor_nm;
                //return CommonUtil.GetLngNM(us_pos_kor_nm, us_pos_eng_nm, us_pos_chn_nm, us_pos_etc1_nm, us_pos_etc2_nm);
            } 
        }
        public int? us_group { get; set; }
        public string us_group_nm { get; set; }

        public string user_type
        {
            get
            {
                return us_role == 10 ? "협력사" : "자사";
            }
        }

        public int? us_role { get; set; }

        public string us_role_kor_nm { get; set; }
        public string us_role_eng_nm { get; set; }
        public string us_role_chn_nm { get; set; }
        public string us_role_etc1_nm { get; set; }
        public string us_role_etc2_nm { get; set; }


        public string us_role_nm
        {
            get
            {
                return CommonUtil.GetLngNM(us_role_kor_nm, us_role_eng_nm, us_role_chn_nm, us_role_etc1_nm, us_role_etc2_nm);
            }
        }

        public string use_fl { get; set; }

        public DateTime create_dt { get; set; }
        public string create_date
        {
            get
            {
                return create_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }

        public string create_datetime
        {
            get
            {
                return create_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }
        public int? create_us { get; set; }
        public string create_us_nm { get; set; }




        public DateTime change_pw_dt { get; set; }
        public DateTime secu_oath_dt { get; set; }


        // 검색용
        public string isVender { get; set; }
        public string isLowDepartment { get; set; }

        public int? target_idx { get { return us_idx; } }

        public string isDist { get; set; }

        public eModule action_module
        {
            get
            {
                if(us_role == 10 || isVender == "Y") { return eModule.Admin_Vend_User; }
                else { return eModule.Admin_User; }
                
            }
        }
        public int? action_module_idx { get { return null; } }

        public static UserModel GetUserInfo(int? _us_idx)
        {
            return Mapper.Instance().QueryForObject<UserModel>("User.selUser", new UserModel { us_idx = _us_idx });
        }

        public string langCd { get; set; }
    }
}