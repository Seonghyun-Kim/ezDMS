using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IS_PODS.Models.Bbs;
using IBatisNet.DataMapper;
using System.Collections.Generic;
using static IS_PODS.Define.LogDefine;
using IS_PODS.Models.Common;
using IS_PODS.Class;

namespace IS_PODS.Controllers
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
            //ViewBag.bbsReplyModel = bbsReplyModel;ajax로 보내야함.

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
        //파일본명 가져오기 클릭시 링크타고 다운로드
        public ActionResult getBbsFileUpload(int? bbs_idx)
        {
            try
            {
                Mapper.Instance().BeginTransaction();
                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[f];
                    //파일 명가져와 넢
                    BbsFileModel BbsFIleList = Mapper.Instance().QueryForObject<BbsFileModel>("Bbs.selBbsFile", new BbsFileModel() { bbs_idx = bbs_idx, file_org_nm = file.FileName });

                    return Json(bbs_idx);
                }
                return Json(1);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });

            }

        }
        /*
        //파일 insert 
        public JsonResult setBbsFile(BbsFileModel bbsFile)
        {
            try
            {
                int bbsIDX = (int)Mapper.Instance().Insert("Bbs.insBbsFile", bbsFile);
                return Json(bbsIDX);
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        */
        public JsonResult setBoardWrite(BbsContentsModel bbsContents)
        {
            try
            {
                //섹션에 있는 idx값 가져와 강제넣
                bbsContents.create_us = Convert.ToInt32(Session["USER_IDX"]);
                
                int bbsIDX = (int)Mapper.Instance().Insert("Bbs.insBbsContent", bbsContents);//xml

                return Json(bbsIDX);
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true , resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
           
        }
        public JsonResult SetBoardContents(BbsContentsModel bbsContents)
        {
            try
            {
                bool isNew = bbsContents.bbs_idx == null ? true : false;

                if (isNew)
                {
                    bbsContents.create_us = Convert.ToInt32(Session["USER_IDX"]);
                    int bbsIDX = (int)Mapper.Instance().Insert("Bbs.insBbsContent", bbsContents);

                    return Json(bbsIDX);
                }
                else
                {
                    Mapper.Instance().Update("Bbs.udtBbsContent", bbsContents);
                    //int  bbsIDX = (int)Mapper.Instance().Update("Bbs.udtBbsContent", bbsContents.bbs_idx);
                    return Json(bbsContents);
                   //return Json(bbsIDX);
                }

            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() }); ;

            }
        }
        public JsonResult setBbsReply(BbsReplyModel bbsReply)
        {//댓글insert

            try
            {
                //bbsReply.bbs_idx = bbs_idx;
                //강넣
                bbsReply.create_us = Convert.ToInt32(Session["USER_IDX"]);
                //빼오기
                int bbsIDX = (int)Mapper.Instance().Insert("Bbs.insBbsReply", bbsReply);
                return Json(bbsIDX);
            }
            catch (Exception ex)
            {
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

                Mapper.Instance().CommitTransaction();
               
                return Redirect("BoardList");
            }
            catch (Exception ex)
            {
                Mapper.Instance().RollBackTransaction();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
       
       

       
        
        
        
    }
}