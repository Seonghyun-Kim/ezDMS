using IBatisNet.DataMapper;
using IS_PODS.Models.Auth;
using IS_PODS.Models.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace IS_PODS.Class
{
    public class PdfWatermark
    {
        public UserModel userInfo = null;

        string _IP = string.Empty;
        BaseFont bf = null;

        float pageWidth = 0; 
        float pageHeight = 0;

        ConfigModel wPosition = null;
        ConfigModel wMsg1 = null; 
        ConfigModel wMsg2 = null;
        ConfigModel wMsg3 = null;
        ConfigModel wColor = null;
        ConfigModel isOver = null;
        ConfigModel wFontSize = null;

        public string SetWaterMarkPdf(string strFilePath, string strFileName, int? us_idx, string remoteIP)
        {
            ConfigModel isWatermark = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "WMARK", comm_code = "IS_WMARK" });

            if(isWatermark.comm_value != "Y")
            {
                return Path.Combine(strFilePath, strFileName);
            }

            try
            {
                /*
                { value: "TR", text: "상단 우측" },
                { value: "TC", text: "상단 중앙" },
                { value: "TL", text: "상단 좌측" },
                { value: "CC", text: "중앙" },
                { value: "BR", text: "하단 우측" },
                { value: "BC", text: "하단 중앙" },
                { value: "BL", text: "하단 좌측" }];
                 */
                _IP = remoteIP;
                userInfo = UserModel.GetUserInfo(us_idx);

                wPosition = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "WMARK", comm_code = "WMARK_POS" });
                wMsg1 = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "WMARK", comm_code = "WMARK_MSG1" });
                wMsg2 = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "WMARK", comm_code = "WMARK_MSG2" });
                wMsg3 = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "WMARK", comm_code = "WMARK_MSG3" });
                wColor = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "WMARK", comm_code = "WMARK_COLOR" });
                isOver = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "WMARK", comm_code = "IS_OVER" });
                wFontSize = Mapper.Instance().QueryForObject<ConfigModel>("Common.selConfig", new ConfigModel { comm_section = "WMARK", comm_code = "FONT_SIZE" });

                string wFont = System.Configuration.ConfigurationManager.AppSettings["WatermarkFont"];

                string strCopyFileName = CommonUtil.FileCopyToTemp(strFilePath, strFileName);

                PdfReader pdf = new PdfReader(System.IO.File.ReadAllBytes(strCopyFileName));

                using (FileStream stream = new FileStream(strCopyFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    PdfStamper stamp = new PdfStamper(pdf, stream);

                    bf = BaseFont.CreateFont(wFont, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Color textColor = new Color(CommonUtil.CodeToColor(wColor.comm_value));
                    Rectangle pagesize = pdf.GetPageSize(1);
                    pageWidth = pagesize.Width;
                    pageHeight = pagesize.Height;
                    //StartPoint st = new StartPoint(pagesize, bf);

                    string sMsg1 = GetFormattingText(wMsg1.comm_value);
                    string sMsg2 = GetFormattingText(wMsg2.comm_value);
                    string sMsg3 = GetFormattingText(wMsg3.comm_value);

                    StartPoint msg1Point = GetStartPoint(sMsg1, 1);
                    StartPoint msg2Point = GetStartPoint(sMsg2, 2);
                    StartPoint msg3Point = GetStartPoint(sMsg3, 3);

                    for (int page = 1; page <= pdf.NumberOfPages; page++)
                    {
                        PdfContentByte waterMark;
                        if(isOver.comm_value == "Y")
                        {
                            waterMark = stamp.GetOverContent(page);
                        }
                        else
                        {
                            waterMark = stamp.GetUnderContent(page);
                        }

                        var gstate = new PdfGState { FillOpacity = 0.5f, StrokeOpacity = 0.3f };
                        waterMark.SetGState(gstate);
                        waterMark.SetColorFill(textColor);
                        waterMark.SetFontAndSize(bf, 14);

                        if(wMsg1.comm_value.Trim() != "")
                        {
                            waterMark.BeginText();
                            waterMark.ShowTextAligned(msg1Point.element, sMsg1, msg1Point.x, msg1Point.y, 0);
                            waterMark.EndText();
                        }

                        if (wMsg2.comm_value.Trim() != "")
                        {
                            waterMark.BeginText();
                            waterMark.ShowTextAligned(msg2Point.element, sMsg2, msg2Point.x, msg2Point.y, 0);
                            waterMark.EndText();
                        }

                        if (wMsg3.comm_value.Trim() != "")
                        {
                            waterMark.BeginText();
                            waterMark.ShowTextAligned(msg3Point.element, sMsg3, msg3Point.x, msg3Point.y, 0);
                            waterMark.EndText();
                        }
                    }

                    stamp.FormFlattening = true;

                    stamp.Close();
                }

                return strCopyFileName;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string GetFormattingText(string _SaveMsg)
        {
            if(_SaveMsg.Trim() == "")
            {
                return "";
            }

            if(_SaveMsg.IndexOf("{") < 0)
            {
                return _SaveMsg;
            }

            try
            {
                string retMsg = string.Empty;

                string[] sarFormatList = _SaveMsg.Replace("{", "").Replace("}", "").Split(new char[] { '[', ']' });

                foreach(string s in sarFormatList)
                {
                    if(s == "")
                    {
                        continue;
                    }
                    else if (s == "IP")
                    {
                        retMsg += _IP;
                    }
                    else if(s == "USER_NAME")
                    {
                        retMsg += userInfo.us_nm;
                    }
                    else if (s == "USER_GROUP")
                    {
                        retMsg += userInfo.us_group_nm;
                    }
                    else if (s == "USER_TITLE")
                    {
                        retMsg += userInfo.us_pos_nm;
                    }
                    else if (s.IndexOf("DATE") >= 0)
                    {
                        string[] sarDate = s.Split('=');

                        retMsg += DateTime.Now.ToString(sarDate[1]);
                    } 
                    else
                    {
                        retMsg += s;
                    }
                }
                return retMsg;
            }

            catch(Exception ex)
            {
                throw ex;
            }
        }

        public StartPoint GetStartPoint(string sMsg, int num)
        {
            StartPoint sp = new StartPoint();

            string position = wPosition.comm_value;
            float fontSize = Convert.ToSingle(wFontSize.comm_value);

            float _x = 0;
            float _y = 0;

            switch (position.Substring(0, 1))
            {
                case "T":
                    _y = (pageHeight - 20) - ((num - 1) * (fontSize + 5));
                    break;
                case "B":
                    _y = 15;
                    if (num == 1)
                    {
                        _y = _y + ((fontSize + 5) * 2);
                    }
                    else if (num == 2)
                    {
                        _y = _y + ((fontSize + 5) * 1);
                    }
                    break;
                case "C":
                    _y = (pageHeight / 2);

                    if (num == 3)
                    {
                        _y = _y - (fontSize + 10);
                    }
                    else if (num == 1)
                    {
                        _y = _y + (fontSize + 10);
                    }

                    break;
            }

            switch (position.Substring(1, 1))
            {
                case "R":
                    //_x = pageWidth - bf.GetWidthPoint(sMsg.Trim(), fontSize) + 10;
                    _x = pageWidth - 10;
                    sp.element = Element.ALIGN_RIGHT;
                    break;
                case "C":
                    sp.element = Element.ALIGN_CENTER;
                    _x = (pageWidth / 2) - ((bf.GetWidthPoint(sMsg.Trim(), fontSize) * 0.06f) / 2);
                    break;
                case "L":
                    sp.element = Element.ALIGN_LEFT;
                    _x = 10;
                    break;
            }

            sp.x = _x;
            sp.y = _y;

            return sp;
        }
    }

    public class StartPoint
    {
        public float x { get; set; }
        public float y { get; set; }
        public int element { get; set; }
    }
}