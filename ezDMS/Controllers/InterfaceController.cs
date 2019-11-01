using IBatisNet.DataMapper;
using IS_PODS.Class;
using IS_PODS.Models.Common;
using IS_PODS.Models.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IS_PODS.Controllers
{
    public class InterfaceController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("SystemLog");
        // GET: Interface

        public JsonResult IF_EO_INFO(InterfaceEoInfo itfEo)
        {
            List<interfaceResult> itfResult = new List<interfaceResult>();

            try
            {
                if (itfEo == null)
                {
                    throw new Exception("EO 정보가 없는 데이터입니다.");
                }

                #region -- 입력된 데이터를 우선 temp table 에 저장한다. 검증을 위해.
                try
                {
                    string argsJson = new JavaScriptSerializer().Serialize(itfEo);
                    int jSonIdx = (int)Mapper.Instance().Insert("ITF.insTempJson", new TempItfJson { itf_json = argsJson });
                    itfEo.json_idx = jSonIdx;
                    logger.Info(string.Format("ITF : IF_EO_INFO\r\nID:{0}\r\n{1}", jSonIdx, argsJson));

                    //itfEo.temp_idx = temp_idx;
                    //Mapper.Instance().Insert("ITF.insTempEo", itfEo.temp_idx);
                    //foreach (interfacePartInfo partInfo in itfEo.partList)
                    //{
                    //    partInfo.temp_idx = temp_idx;
                    //    Mapper.Instance().Insert("ITF.insTempPart", partInfo);

                    //    if (partInfo.fileList == null) 
                    //    {
                    //        continue;
                    //    }

                    //    foreach (ItfFileInfo fileInfo in partInfo.fileList)
                    //    {
                    //        fileInfo.temp_idx = temp_idx;
                    //        Mapper.Instance().Insert("ITF.insTempFile", fileInfo);
                    //    }
                    //}
                }
                catch(Exception ex)
                {
                    logger.Error(string.Format("ITF : IF_EO_INFO"), ex);
                    return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
                }
                #endregion

                Mapper.Instance().BeginTransaction();

                ItfEoInfo chkEo = Mapper.Instance().QueryForObject<ItfEoInfo>("ITF.selItfEoInfo", new ItfEoInfo { eo_no = itfEo.eo_no });

                if(chkEo != null)
                {
                    throw new Exception("인터페이스가 완료된 EO 데이터입니다.");
                }
               
                int eoIdx = (int)Mapper.Instance().Insert("ITF.insItfEoInfo", itfEo);
                logger.Info(string.Format("ITF : IF_EO_INFO\r\nEO 저장 : {0}", eoIdx));
                if (itfEo.partList == null)
                {
                    throw new Exception("부품정보가 없는 EO입니다.");
                }

                foreach (interfacePartInfo partInfo in itfEo.partList)
                {
                    ItfPartMaster partMaster = partInfo.GetPartMaster();
                    partMaster.eo_idx = eoIdx;
                    int partIdx = (int)Mapper.Instance().Insert("ITF.insItfPartMaster", partMaster);
                    logger.Info(string.Format("ITF : IF_EO_INFO\r\nPART 저장 : {0} - {1}", partIdx, partMaster.part_no));
                    partInfo.part_idx = partIdx;
                
                    var childPartList = from pl in itfEo.partList
                                        where pl.parent_part_no == partInfo.part_no && pl.parent_part_rev_no == partInfo.part_rev_no
                                        select pl;

                    foreach(interfacePartInfo childPart in childPartList)
                    {
                        childPart.parent_part_idx = partIdx;
                    }

                    int FileCnt = 0;
                    if (partInfo.fileList == null)
                    {
                        logger.Info(string.Format("ITF : IF_EO_INFO\r\nPART FILE 없음 : {0} - {1} -----------------------------", partIdx, partMaster.part_no));
                        continue;
                    }

                    logger.Info(string.Format("ITF : IF_EO_INFO\r\nPART FILE 갯수 : {0} - {1} {2}개 -----------------------------", partIdx, partMaster.part_no, partInfo.fileList.Count));

                    foreach (ItfFileInfo fileInfo in partInfo.fileList)
                    {
                        var getFile = Mapper.Instance().QueryForObject<ItfFileInfo>("ITF.selItfFIleInfo", new ItfFileInfo()
                        {
                            file_org_nm = fileInfo.file_org_nm,
                            part_no = partInfo.part_no,
                            part_rev_no = partInfo.part_rev_no
                        });

                        if(getFile != null)
                        {
                            continue;
                        }

                        string valutPath = System.Configuration.ConfigurationManager.AppSettings["EoFilePath"].ToString();

                        CommonUtil.FolderCreate(valutPath + "\\" + partInfo.part_no);

                        string fileName = fileInfo.file_org_nm;
                        string fileExtension = Path.GetExtension(fileName);
                        string fileConvName = string.Empty; ;

                        fileConvName = AESEncrypt.AESEncrypt256(string.Format("{0}_{1}_{2}", eoIdx, partIdx, ++FileCnt), partInfo.part_no);

                        fileInfo.file_conv_nm = fileConvName + fileExtension;
                        // 파일 경로 작업 끝

                        int file_idx = (int)Mapper.Instance().Insert("ITF.insItfFileInfo", fileInfo);
                        logger.Info(string.Format("ITF : IF_EO_INFO\r\nPART FILE 저장 : {0} - {1} {2} - {3}", partIdx, partMaster.part_no, file_idx, fileName));
                        interfaceResult resFileInfo = new interfaceResult();

                        resFileInfo.file_idx = file_idx;
                        resFileInfo.partNo = partInfo.part_no;
                        resFileInfo.partRevNo = partInfo.part_rev_no;
                        resFileInfo.plmFIleName = fileInfo.file_org_nm;
                        resFileInfo.plmFIleRevNo = fileInfo.file_rev_no;
                        resFileInfo.ftpFilePath = partInfo.part_no;
                        resFileInfo.ftpFileConvName = fileInfo.file_conv_nm;
                        resFileInfo.plmFileOid = fileInfo.file_oid;
                        resFileInfo.file_format = fileInfo.file_format;

                        itfResult.Add(resFileInfo);
                    }

                    logger.Info(string.Format("ITF : IF_EO_INFO\r\nPART FILE 저장 완료 : {0} - {1} {2}개 -----------------------------", partIdx, partMaster.part_no, FileCnt));

                }

                foreach (interfacePartInfo partInfo in itfEo.partList)
                {
                    ItfBomInfo bomInfo = partInfo.GetBomInfo();
                    logger.Info(string.Format("ITF : IF_EO_INFO\r\nPART BOM 저장 완료 : [대상]{0} - {1} [부모]{2} - {3} -----------------------------", partInfo.part_no, partInfo.part_rev_no, partInfo.parent_part_no, partInfo.parent_part_rev_no));
                    Mapper.Instance().Insert("ITF.insItfBom", bomInfo);
                }

                Mapper.Instance().CommitTransaction();

                //string retJson = new JavaScriptSerializer().Serialize(itfResult);
                return Json(itfResult);
            }
            catch(Exception ex)
            {
                logger.Error(string.Format("ITF : IF_EO_INFO"), ex);
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        } 

        //[HttpGet]
        //public JsonResult IF_FILE_COMPLEATE()
        //{
        //    try
        //    {                 
        //        return Json("SUCCESS", JsonRequestBehavior.AllowGet);
        //    }
        //    catch(Exception ex)
        //    {
        //        return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public JsonResult IF_FILE_COMPLEATE(int? file_idx)
        {
            try
            {
                logger.Info(string.Format("ITF : IF_FILE_COMPLEATE\r\nfile_idx : {0}", file_idx));
                if (file_idx == null)
                {
                    throw new Exception("잘못된 호출입니다.");
                }

                Mapper.Instance().Update("ITF.udtItfFileCompleate", file_idx);

                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("ITF : IF_FILE_COMPLEATE"), ex);
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
    }
}