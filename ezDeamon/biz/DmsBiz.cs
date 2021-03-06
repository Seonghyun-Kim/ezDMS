﻿using ezDeamon.cls;
using SmartDSP.Class;
using SmartDSP.Models.Dist;
using SmartDSP.Models.Interface;
using SmartDSP.Models.Log;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ezDeamon.biz
{
    public class DmsBiz
    {
        public PostgresqlDBControl dbCon;
        public DmsBiz()
        {
            /*
                // to get the location the assembly is executing from
                // (not necessarily where the it normally resides on disk)
                // in the case of the using shadow copies, for instance in NUnit tests, 
                // this will be in a temp directory.
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location
                //To get the location the assembly normally resides on disk or the install directory
                string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
             */
            string iniPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\ini\app.ini";
            string encrypPasword = CfgControl.IniReadValue("DESCRYPT", "KEY", iniPath);

            string DataSource = AESEncrypt.AESDecrypte256(CfgControl.IniReadValue("DATABASE", "DATA_SOURCE", iniPath), encrypPasword);
            string DataPort = AESEncrypt.AESDecrypte256(CfgControl.IniReadValue("DATABASE", "DATA_PORT", iniPath), encrypPasword);
            string DefaultDB = AESEncrypt.AESDecrypte256(CfgControl.IniReadValue("DATABASE", "DATABASE", iniPath), encrypPasword);
            string UserID = AESEncrypt.AESDecrypte256(CfgControl.IniReadValue("DATABASE", "DB_ID", iniPath), encrypPasword);
            string UserPW = AESEncrypt.AESDecrypte256(CfgControl.IniReadValue("DATABASE", "DB_PW", iniPath), encrypPasword);

            dbCon = new PostgresqlDBControl("POSTGRESQL", DataSource, UserID, UserPW, DefaultDB, DataPort);

        }

        public List<DistMasterModel> GetDistList()
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }

                string sQuery = @"
SELECT
A.dist_idx, A.dist_title, A.dist_msg, A.create_dt, A.dist_dt, A.edit_dt,
A.create_us, B.us_nm as create_us_nm, A.finish_dt, A.dist_st, C.kor_nm as dist_st_nm, A.eo_fl, D.eo_idx, D.eo_nm, D.eo_no  , grp_list.vender_list,
recvTotal.recvCnt as recv_cnt, recvTotal.recvFullDownCnt as recv_full_down_cnt
FROM dist_master A
LEFT JOIN sys_users B ON A.create_us = B.us_idx AND B.us_role > 10
LEFT JOIN v_sys_library C ON C.parent_comm_code = 'DIST_ST' AND A.dist_st = C.comm_code
LEFT JOIN v_dist_eo D ON A.dist_idx = D.dist_idx
LEFT JOIN (
SELECT dist_idx, array_to_string(array_agg(DISTINCT  grp_list.group_nm order by grp_list.group_nm), ', ') vender_list
FROM (
SELECT dist_idx, case when recvU.us_role = 10 then '협력사 : ' || vend.vend_nm else '자사 : ' || dept.dept_nm  end as group_nm
FROM dist_receiver recv
LEFT JOIN sys_users recvU on recv.recv_us = recvU.us_idx
LEFT JOIN sys_department dept on recvU.us_group = dept.dept_idx and recvU.us_role > 10
LEFT JOIN sys_vender vend on recvU.us_group = vend.vend_idx and recvU.us_role = 10
GROUP BY dist_idx, recvU.us_role, recvU.us_group,vend.vend_nm, dept.dept_nm ) grp_list
GROUP BY grp_list.dist_idx ) as grp_list on A.dist_idx = grp_list.dist_idx
LEFT JOIN (
select dist_idx, count(recv_idx) as recvCnt, sum(isFullRecv) as recvFullDownCnt from (
select dist_idx, recv_idx
, case when count(dist_file_idx) = sum(hiscnt) then 1 else 0 end as isFullRecv  from (
select
A.dist_idx, A.recv_idx, B.dist_file_idx, case when L.action_target_idx is null then 0 else 1 end hiscnt
from dist_receiver A
left join dist_recv_file B on A.recv_idx = B.recv_idx
left join log_action_history L on L.action_module = 10 and L.action_type = 90 and L.action_target_idx = B.dist_file_idx
group by A.dist_idx, A.recv_idx, B.dist_file_idx, L.action_target_idx) as g
group by dist_idx, recv_idx) t
group by dist_idx ) as recvTotal on A.dist_idx = recvTotal.dist_idx
WHERE 1=1  
AND A.dist_st = 'DS'";
                
                DataSet ds = dbCon.NewDataSet(sQuery);

                List<DistMasterModel> list = ConvertDataSetToList.ConvertTo<DistMasterModel>(ds.Tables[0]);

                return list;
            }
            catch(Exception ex)
            {
                throw ex;
            }         
        }

        public List<DistReceiverModel> GetRecvDistList()
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }

                string sQuery = @"
SELECT * FROM dist_receiver
WHERE recv_dist_st = 'DS'";

                DataSet ds = dbCon.NewDataSet(sQuery);

                List<DistReceiverModel> list = ConvertDataSetToList.ConvertTo<DistReceiverModel>(ds.Tables[0]);

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateDistStatus(NpgsqlTransaction tran, DistMasterModel dist)
        {
            try
            {
                if(!dbCon.IsDBConnected) 
                { 
                    dbCon.DBConnect();
                }
                string sQuery = "UPDATE dist_master SET dist_st = '{0}' WHERE dist_idx = {1}";

                sQuery = string.Format(sQuery, dist.dist_st, dist.dist_idx);

                int res = dbCon.DBExecuteQuery(tran, sQuery);

                return res;

            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public int UpdateRecvDistStatus(NpgsqlTransaction tran, DistReceiverModel recv)
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }
                string sQuery = "UPDATE dist_receiver SET recv_dist_st = '{0}' WHERE recv_idx = {1}";

                sQuery = string.Format(sQuery, recv.recv_dist_st, recv.recv_idx);

                int res = dbCon.DBExecuteQuery(tran, sQuery);

                return res;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateLog(ActionHistoryModel log)
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }

                string sQuery = "SELECT COALESCE(MAX(action_idx) +1, 1) AS action_idx FROM log_action_history";

                DataSet ds = dbCon.NewDataSet(sQuery);

                int action_idx = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());

                string sInsQuery = @" INSERT INTO log_action_history
      (action_idx, action_module, action_module_idx, action_type, action_target_idx, action_ip, action_dt, action_us, action_url, session_id, description)
      VALUES({0}, {1}, {2}, {3}, {4}, '192.0.0.1', now(), -1, 'ezDeamon', null, null)";

                sInsQuery = string.Format(sInsQuery, action_idx, (int)log.action_module, log.action_module_idx == null ? "null" : log.action_module_idx.ToString(), (int)log.action_type, log.action_target_idx);

                int res = dbCon.DBExecuteQuery(sInsQuery);

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SysConfigValue(string section, string code)
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }

                string sQuery = "SELECT comm_value FROM sys_config where comm_section = '{0}' and comm_code = '{1}'";

                sQuery = string.Format(sQuery, section, code);

                DataSet ds = dbCon.NewDataSet(sQuery);

                return ds.Tables[0].Rows[0][0].ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ItfFileInfo> TermFileList(int Term)
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }

                string sQuery = @"
select itf_dt, part_no, part_rev_no, file_idx, file_org_nm, file_conv_nm from (
select max(c.itf_dt) as itf_dt, a.part_no, a.part_rev_no, a.file_idx, a.file_org_nm, a.file_conv_nm from itf_file_info A
left join itf_part_master B on A.part_no = B.part_no and A.part_rev_no = B.part_rev_no
left join itf_eo_info C on B.eo_idx = C.eo_idx
where is_itf='Y' and is_del ='N'
group by a.part_no, a.part_rev_no, a.file_idx, a.file_org_nm, a.file_conv_nm) f
where itf_dt ::date < now() - interval '{0} day'";

                sQuery = string.Format(sQuery, Term);

                DataSet ds = dbCon.NewDataSet(sQuery);

                List<ItfFileInfo> list = ConvertDataSetToList.ConvertTo<ItfFileInfo>(ds.Tables[0]);

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateFileStatus(NpgsqlTransaction tran, int? file_idx)
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }

                string sQuery = "UPDATE itf_file_info SET is_del = 'Y' WHERE file_idx = {0}";

                sQuery = string.Format(sQuery, file_idx);

                int res = dbCon.DBExecuteQuery(tran, sQuery);

                return res;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ItfEoInfo> TermEOList(int Term)
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }

                string sQuery = @"
select * from itf_eo_info
where itf_dt ::date < now() - interval '{0} day'";

                sQuery = string.Format(sQuery, Term);

                DataSet ds = dbCon.NewDataSet(sQuery);

                List<ItfEoInfo> list = ConvertDataSetToList.ConvertTo<ItfEoInfo>(ds.Tables[0]);

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateEOStatus(NpgsqlTransaction tran, int? eo_idx)
        {
            try
            {
                if (!dbCon.IsDBConnected)
                {
                    dbCon.DBConnect();
                }

                string sQuery = "UPDATE itf_eo_info SET notuse_fl = 'Y' WHERE eo_idx = {0}";

                sQuery = string.Format(sQuery, eo_idx);

                int res = dbCon.DBExecuteQuery(tran, sQuery);

                return res;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
