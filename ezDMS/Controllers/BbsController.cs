using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ezDMS.Models.Bbs;
using IBatisNet.DataMapper;
using System.Collections.Generic;
using static ezDMS.Define.LogDefine;
using ezDMS.Models.Common;
using ezDMS.Class;
using System.IO;

namespace ezDMS.Controllers
{
    public class BbsController : Controller
    {   //view는 그냥 이름으로 해주고 기능은 select = get 나머지 =set 붙여주기
       
        
        public ActionResult BoardWrite(BbsContentsModel bbsContents)
        {
            return View();
        }
        public ActionResult BoardModify(int? bbs_idx)
        {
            //내용
            var bbsContentsModel = Mapper.Instance().QueryForObject<BbsContentsModel>("Bbs.selBbsContent", new BbsContentsModel { bbs_idx = bbs_idx });
            //파일
            var bbsFileModel = Mapper.Instance().QueryForList<BbsFileModel>("Bbs.selBbsFile", new BbsFileModel { bbs_idx = bbs_idx });
           
            ViewBag.bbsContentsModel = bbsContentsModel;
            ViewBag.bbsFileModel = bbsFileModel;
           
            LogCtrl.SetLog(bbsContentsModel, eActionType.DataSelect, this.HttpContext);
            return View();
        }

        public ActionResult BoardList()  
        {
            return View("BoardList");
        }
        public ActionResult BoardView(int? bbs_idx)
        {//쓰고-view-list로
        
            //내용
            var bbsContentsModel = Mapper.Instance().QueryForObject<BbsContentsModel>("Bbs.selBbsContent", new BbsContentsModel { bbs_idx = bbs_idx });
            var bbsFileModel = Mapper.Instance().QueryForList<BbsFileModel>("Bbs.selBbsFile", new BbsFileModel { bbs_idx = bbs_idx });

            ViewBag.bbsContentsModel = bbsContentsModel;
            ViewBag.bbsFileModel = bbsFileModel;
           

            //ViewBag.bbsReplyModel = bbsReplyModel;ajax로 보내야함.

            LogCtrl.SetLog(bbsContentsModel, eActionType.DataSelect, this.HttpContext);

            return View();
        }

        public JsonResult getBoardView(int? bbs_idx)
        {
            try
            {
                return Json(bbs_idx);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }

        }
        //view단 
        //grid밑단 list Data뿌리기
        public ActionResult getBoardList(BbsContentsModel BbsContentsModel)//콜백함수
        {
            try
            {
                return Json(Mapper.Instance().QueryForList<BbsContentsModel>("Bbs.selBbsContent", BbsContentsModel));//xml.id값
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            
        }
       
        //댓글 갱신
        public JsonResult getBbsReply(int? bbs_idx)// 무엇이 기준 ? bbs_idx , bbs_reply_idx 
        {
            try
            {
                return Json(Mapper.Instance().QueryForList<BbsReplyModel>("Bbs.selBbsReply", new BbsContentsModel { bbs_idx = bbs_idx }));//xml.id값
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }

        }

        [ValidateInput(false)]
        public JsonResult SetBoardContents()
        {
            try
            {
                FormCollection collection = new FormCollection(Request.Unvalidated.Form);

                BbsContentsModel bbsContents = new BbsContentsModel();

                //dropzone = name, value
                //formData = formdata, file , filename / 보내는 형식이 달라 formdata 를 자르고 사용.
                bbsContents.bbs_idx = collection["bbs_idx"] == null ? null : (int?)Convert.ToInt32(collection["bbs_idx"]);
                bbsContents.bbs_category = collection["bbs_category"] == null ? "" : collection["bbs_category"].Trim() == "" ? "" : collection["bbs_category"].Split(',')[1];
                bbsContents.bbs_title = collection["bbs_title"] == null ? "" : collection["bbs_title"].Trim() == "" ? "" : collection["bbs_title"].Split(',')[1];
                bbsContents.bbs_content = collection["bbs_content"] == null ? "" : collection["bbs_content"].Trim() == "" ? "" : collection["bbs_content"].Split(',')[1];
                
                bool isNew = bbsContents.bbs_idx == null ? true : false;

               
                if (isNew)
                {
                    bbsContents.create_us = Convert.ToInt32(Session["USER_IDX"]);
                    
                    int bbs_idx = (int)Mapper.Instance().Insert("Bbs.insBbsContent", bbsContents);

                    Mapper.Instance().BeginTransaction();
                    foreach (string f in Request.Files)
                    {   //xhr에 있.
                        HttpPostedFileBase file = Request.Files[f];
                        
                        //파일명으로 찾아서 serlec
                        BbsFileModel BbsFIleList = Mapper.Instance().QueryForObject<BbsFileModel>("Bbs.selBbsFile", new BbsFileModel() { bbs_idx = bbs_idx , file_org_nm = file.FileName });
                       
                        string fileOrgName = file.FileName;
                        string fileExtension = Path.GetExtension(file.FileName);//확장자
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);//without확장자
                        string fileConvNm = AESEncrypt.AESEncrypt256(fileName, bbs_idx.ToString());

                        string valutPath = System.Configuration.ConfigurationManager.AppSettings["BbsFilePath"].ToString();

                        int? BbsFIleIdx = (int)Mapper.Instance().Insert("Bbs.insBbsFile", new BbsFileModel { bbs_idx = bbs_idx, file_org_nm = file.FileName, file_conv_nm = fileConvNm + fileExtension });
                        
                        CommonUtil.FileSave(valutPath + "\\" + bbs_idx, file, fileConvNm + fileExtension);//\은 두개써야함 인식못함. 

                    }

                    Mapper.Instance().CommitTransaction();
                    
                    return Json(bbsContents);
                }
                else //Mapper.Instance().Update("Bbs.udtBbsContent", bbsContents);
                {
                    //bbsContents.bbs_idx = (int)Convert.ToInt32(collection["bbs_idx"]);
                    //bbsContents.bbs_category = collection["bbs_category"] == null ? "" : collection["bbs_category"].Trim() == "" ? "" : collection["bbs_category"].Split(',')[1];
                    //bbsContents.bbs_title = collection["bbs_title"] == null ? "" : collection["bbs_title"].Trim() == "" ? "" : collection["bbs_title"].Split(',')[1];
                    //bbsContents.bbs_content = collection["bbs_content"] == null ? "" : collection["bbs_content"].Trim() == "" ? "" : collection["bbs_content"].Split(',')[1];

                    //Mapper.Instance().BeginTransaction();
                    //foreach (string f in Request.Files)
                    //{   //xhr에 있.
                    //    HttpPostedFileBase file = Request.Files[f];

                       
                    //    BbsFileModel BbsFIleList = Mapper.Instance().QueryForObject<BbsFileModel>("Bbs.selBbsFile", new BbsFileModel() { bbs_idx = bbsContents.bbs_idx });

                    //    string fileOrgName = file.FileName;
                    //    string fileExtension = Path.GetExtension(file.FileName);//확장자
                    //    string fileName = Path.GetFileNameWithoutExtension(file.FileName);//without확장자
                    //    string fileConvNm = AESEncrypt.AESEncrypt256(fileName, bbsContents.bbs_idx.ToString());

                    //    string valutPath = System.Configuration.ConfigurationManager.AppSettings["BbsFilePath"].ToString();

                    //    int? BbsFIleIdx = (int)Mapper.Instance().Insert("Bbs.insBbsFile", new BbsFileModel { bbs_idx = bbsContents.bbs_idx, file_org_nm = file.FileName, file_conv_nm = fileConvNm + fileExtension });

                    //    CommonUtil.FileSave(valutPath + "\\" + bbsContents.bbs_idx, file, fileConvNm + fileExtension);

                    //}

                    //Mapper.Instance().CommitTransaction();
                    return Json(bbsContents.bbs_idx);
                }

            }
            catch (Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() }); 

            }
        }
       
        public ActionResult setBbsContentDelete(int? bbs_idx)
        {
            try
            {
                Mapper.Instance().BeginTransaction();

                Mapper.Instance().Delete("Bbs.delBbsContent", new BbsContentsModel { bbs_idx = bbs_idx });
                Mapper.Instance().Delete("Bbs.delBbsReply", new BbsContentsModel { bbs_idx = bbs_idx });
                Mapper.Instance().Delete("Bbs.delBbsFile", new BbsFileModel { bbs_idx = bbs_idx});
                
                Mapper.Instance().CommitTransaction();

                return Redirect("BoardList");
            }
            catch (Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        public JsonResult setBbsReply(BbsReplyModel bbsReply)
        {//댓글insert

            try
            {
                bbsReply.create_us = Convert.ToInt32(Session["USER_IDX"]);
                //빼오기
                int bbsIDX = (int)Mapper.Instance().Insert("Bbs.insBbsReply", bbsReply);

                return Json(bbsReply.bbs_idx);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult setReplyDelete(int? bbs_reply_idx, int? bbs_idx) 
        {   
            try 
            { 
                Mapper.Instance().Delete("Bbs.delBbsReply" , new BbsReplyModel{ bbs_reply_idx = bbs_reply_idx });
                    
                return Json(bbs_idx);
            }

            catch (Exception ex) 
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ToString() });
            }

        }
        public JsonResult setFileDelete(int? bbs_file_idx) {
            try 
            {
                Mapper.Instance().BeginTransaction();
                Mapper.Instance().Delete("Bbs.delBbsFile", new BbsFileModel { bbs_file_idx = bbs_file_idx });
                Mapper.Instance().CommitTransaction();
                return Json(1);
            }
            catch (Exception ex) 
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        
        }

    }
}