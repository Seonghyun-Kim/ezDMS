using ezDMS.Models.Auth;
using ezDMS.Models.Dist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

namespace ezDMS.Class
{
    public class DistMail
    {
        SmtpClient smtpClient = null;

        HttpRequestBase Request = null;

        private string m_SmtpHost = string.Empty; 
        private int m_StmpPort = 25; // Default Port

        private string m_FromAddress = string.Empty;
        private string m_ToAddress = string.Empty;

        private bool m_IsAuth = false;
        private bool m_IsPassSecu = false;

        private string m_AuthUserID = string.Empty;
        private string m_AuthPassword = string.Empty;

        private string m_MailSubjet = string.Empty;
        private string m_mailBody = string.Empty;
        private string m_SendFromName = string.Empty;
        private string m_SendToName = string.Empty;
        private string m_SendCc = string.Empty; // 참조 

        private int m_SmtpTimeOut = 30;

        List<MailContent> MailList = new List<MailContent>();

        string MailTitle = System.Configuration.ConfigurationManager.AppSettings["MailTitle"];



        public DistMail(HttpRequestBase pRequest)
        {
            Request = pRequest;
            string smtpHost = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"];
            int smtpPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
            string authUserID = System.Configuration.ConfigurationManager.AppSettings["SmtpUserID"];
            string authPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
            bool isAuthLogin = System.Configuration.ConfigurationManager.AppSettings["IsAuthLogin"] == "Y" ? true : false;
            bool isPasswordSecurity = System.Configuration.ConfigurationManager.AppSettings["IsPasswordSecurity"] == "Y" ? true : false;

            SetSmtpClient(smtpHost, smtpPort, authUserID, authPassword, isAuthLogin, isPasswordSecurity);
        }

        public DistMail(HttpRequestBase pRequest, string smtpHost, int smtpPort, string authUserID, string authPassword, bool isAuthLogin = false, bool isPasswordSecurity = false)
        {
            Request = pRequest;
            SetSmtpClient(smtpHost, smtpPort, authUserID, authPassword, isAuthLogin, isPasswordSecurity);
        }      

        private void SetSmtpClient(string smtpHost, int smtpPort, string authUserID, string authPassword, bool isAuthLogin, bool isPasswordSecurity)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["smtpTimeOut"] != null)
            {
                m_SmtpTimeOut = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["smtpTimeOut"]);
            }

            smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Timeout = m_SmtpTimeOut * 1000;

            if (isAuthLogin)
            {
                var secPw = new SecureString();
                foreach (var i in authPassword) secPw.AppendChar(i);

                string PASSWORD = isPasswordSecurity ? secPw.ToString() : authPassword;
                smtpClient.Credentials = new NetworkCredential(authUserID, PASSWORD);
            }
        }

        public void SetMailInfo(DistMasterModel distMaster, UserModel fromUserModel, List<DistReceiverModel> toUserList)
        {
            try
            {
                foreach (DistReceiverModel recv in toUserList)
                {
                    MailContent content = new MailContent(distMaster, fromUserModel, recv, Request);

                    MailList.Add(content);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void SendMail()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["IsMailSend"] == "N")
            {
                return;
            }

            foreach(MailContent mailInfo in MailList)
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(mailInfo.FromUserAddress, mailInfo.FromUserName);

                mail.To.Add(new MailAddress(mailInfo.SendUserAddress, mailInfo.SendUserName));

                mail.Subject = string.Format(MailTitle, mailInfo.SendUserName);
                mail.Body = mailInfo.ToString();
                mail.SubjectEncoding = System.Text.Encoding.Default;
                mail.BodyEncoding = System.Text.Encoding.Default;
                mail.IsBodyHtml = true;

                smtpClient.Send(mail);
            }
        }
    }
}

public class MailContent
{
    DistMasterModel distMaster = null;
    UserModel fromUserModel = null;
    DistReceiverModel toUserModel = null;

    private string CompanyName = System.Configuration.ConfigurationManager.AppSettings["CompanyName"];
    private string mailContentLink = @"<br/><br/><a href='http://{0}' style='color:blue;font-family:gulim;'>※ 자료배포시스템 바로가기</a><br/><br/>";

    private string mailContentCss = @"
<style>
	table {
	width:960px; cellpadding:0; cellspacing=0; border-collapse:collapse; border : 2px solid #C4C2C2; text-align:center;table-layout:fixed;font-family:gulim;
}

table th {
	font-size:13px; font-weight:bolder;border-right : 1px solid #C4C2C2;width:100px;background-color:#82878A;
}

	
table tr {
	height:30px;border-bottom : 1px solid #C4C2C2;
}

table tbody tr td {        
	border-right : 1px solid #C4C2C2;font-size:13px;font-weight:bolder;
}

label {    
	font-family:dotum;    font-weight:bold;    font-size:13px;
}

a {
	color : #2E2D2D;
}
</style>";

    public MailContent(DistMasterModel pDistMaster, UserModel pFromUserModel, DistReceiverModel pToUserModel, HttpRequestBase Request)
    {
        distMaster = pDistMaster;
        fromUserModel = pFromUserModel;
        toUserModel = pToUserModel;

        mailContentLink = string.Format(mailContentLink, Request.Url.Authority);
    }

    public string FromUserAddress
    {
        get
        {
            return fromUserModel.us_email;
        }
    }

    public string FromUserName
    {
        get
        {
            return fromUserModel.us_nm;
        }
    }

    public string SendUserAddress
    {
        get
        {
            return toUserModel.us_email;
        }
    }

    public string SendUserName
    {
        get
        {
            return toUserModel.us_nm;
        }
    }

    public override string ToString()
    {
        StringBuilder MailMessage = new StringBuilder();

        MailMessage.Append(mailContentCss);
        MailMessage.Append("<div style='margin:10px'>");
        MailMessage.AppendFormat("<label>안녕하세요. {0} - {1} {2}님<br/><br/> {3} 에서 자료가 배포되었습니다. 하기 내역을 확인해주세요.</label><br/><br/>", toUserModel.us_group_nm, toUserModel.us_nm, toUserModel.us_pos_nm, CompanyName);

        MailMessage.Append("<table>");
        MailMessage.Append("<tr>");
        MailMessage.Append("<th>배포자</th>");
        MailMessage.AppendFormat("<td colspan='2'>{0} - {1} {2}</td>", fromUserModel.us_group_nm, fromUserModel.us_nm, fromUserModel.us_pos_nm);
        MailMessage.Append("<th>배포일시</th>");
        MailMessage.AppendFormat("<td>{0}</td>", distMaster.dist_datetime);
        MailMessage.Append("<th>만료일자</th>");
        MailMessage.AppendFormat("<td>{0}</td>", distMaster.finish_date);
        MailMessage.Append("</tr>");
        MailMessage.Append("<tr>");
        MailMessage.Append("<th>배포 제목</th>");
        MailMessage.AppendFormat("<td colspan='6' style='text-align:left;padding:0px 10px;'>{0}</td>", distMaster.dist_title);
        MailMessage.Append("</tr>");
        MailMessage.Append("<tr style='height:80px'>");
        MailMessage.Append("<th>배포 내용</th>");
        MailMessage.AppendFormat("<td colspan='6' style='text-align:left;vertical-align:top;padding:10px;'>{0}</td>", distMaster.dist_msg);
        MailMessage.Append("</tr>");

        if (toUserModel.recv_msg != null && toUserModel.recv_msg.Trim() != "")
        {
            MailMessage.Append("<tr style='height:80px'>");
            MailMessage.Append("<th>개별 배포 메세지</th>");
            MailMessage.AppendFormat("<td colspan='6' style='text-align:left;vertical-align:top;padding:10px;'>{0}</td>", toUserModel.recv_msg);
            MailMessage.Append("</tr>");
        }

        MailMessage.Append("</table><br/><br/>");
        MailMessage.Append(mailContentLink);
        MailMessage.Append("</ div>");
      
        return MailMessage.ToString();
    }


}