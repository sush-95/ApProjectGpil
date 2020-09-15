using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;


namespace Common.App_Code
{
    public class EMailManager : IDisposable
    {
        #region Private Variables

        MailMessage message;
        SmtpClient client;
        ActiveUp.Net.Mail.MessageCollection messageCollection;
        string imap; string userId; string password; string rcpUserId; string rcpPassword; string mailBox; string CCMailIDs;
        string fromMailid; string toMailID; string subject; string body; string hostName; string networkuname; string networkpassword; string Port; string sharedPath;
        string APfromMailid; string APPassword;
        #endregion

        public EMailManager()
        {
            this.imap = (ConfigurationManager.AppSettings["IMAP"]).ToString();
            this.userId = (ConfigurationManager.AppSettings["UserID"]).ToString();
            this.password = ConfigurationManager.AppSettings["Password"];
            this.mailBox = ConfigurationManager.AppSettings["MailBox"];
            this.fromMailid = ConfigurationManager.AppSettings["FromMailID"];
            this.toMailID = ConfigurationManager.AppSettings["ToMailID"];
            this.CCMailIDs = ConfigurationManager.AppSettings["CCMailIDs"];
            //this.subject = ConfigurationManager.AppSettings["Subject"];
            //this.body = ConfigurationManager.AppSettings["Body"];
            this.hostName = ConfigurationManager.AppSettings["HostName"];
            this.networkuname = ConfigurationManager.AppSettings["NetworkUserName"];
            this.networkpassword = ConfigurationManager.AppSettings["NetworkPassword"];
            this.sharedPath = ConfigurationManager.AppSettings["ProcessedInvoicePath"];
            this.Port = ConfigurationManager.AppSettings["Port"];
            GetApCredential();

        }

        public void GetApCredential()
        {
            string path = (ConfigurationManager.AppSettings["ApCredentialPath"]).ToString();
            string gettext = System.IO.File.ReadAllText(path);
            var objList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ViewModelClass.GET_GP_MailIDcls>>(gettext);
            this.APfromMailid = objList[0].AP_MAIL_BOX_ID.Trim();
            this.APPassword = objList[0].MAIL_PASSWORD.Trim();
            

        }


        public string ZipAttachment(string startPath)
        {
            string zipPath = string.Empty;
            try
            {
                zipPath = startPath + ".zip";
                if (System.IO.File.Exists(zipPath))
                {
                    System.IO.File.Delete(zipPath);
                }
                ZipFile.CreateFromDirectory(startPath, zipPath);
            }
            catch (Exception ex)
            {
            }
            return zipPath;
        }

        public void SendMail(string attachmentFilename, string toEmailId)
        {
            try
            {
                client = new SmtpClient(hostName, Convert.ToInt32(Port));
                message = new MailMessage(fromMailid, toEmailId, subject, body);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(fromMailid, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                message.From = new MailAddress(fromMailid);
                message.Subject = subject;
                message.Body = body;

                if (attachmentFilename != null)
                    message.Attachments.Add(new Attachment(attachmentFilename));

                client.Send(message);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }

        public void SendMail(string attachmentFilename, string toEmailId, string subject, string body)
        {
            try
            {
                client = new SmtpClient(hostName, Convert.ToInt32(Port));
                message = new MailMessage(APfromMailid, toEmailId, subject, body);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(APfromMailid, APPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                message.From = new MailAddress(APfromMailid);
                message.Subject = subject;
                message.Body = body;
            
                if (!string.IsNullOrEmpty(attachmentFilename))
                    message.Attachments.Add(new Attachment(attachmentFilename));

                client.Send(message);



            }
            catch (Exception ex)
            {
                //throw;
            }
            finally
            {

            }
        }

        public void SendMail(string toEmailId, string startPath, string gstNumber)
        {
            try
            {
                client = new SmtpClient(hostName, Convert.ToInt32(Port));
                message = new MailMessage(fromMailid, toEmailId, subject, body);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(fromMailid, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                message.From = new MailAddress(fromMailid);
                message.Subject = "Attachment Size is More Than 12 MB";
                message.Body = "Attachment size is more than 12 MB for GSTNumber: " + gstNumber + " @ " + startPath;

                client.Send(message);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }

        public void Dispose()
        {

        }

        public ActiveUp.Net.Mail.Mailbox GetMailBox()
        {
            ActiveUp.Net.Mail.Imap4Client imapClient = new ActiveUp.Net.Mail.Imap4Client();
            //System.Threading.Thread.Sleep(180000);
            System.Threading.Thread.Sleep(3000);
            imapClient.ConnectSsl(imap);
            imapClient.Login(APfromMailid, APPassword);

            ActiveUp.Net.Mail.Mailbox inbox = imapClient.SelectMailbox(mailBox);
            //ActiveUp.Net.Mail.Mailbox inbox = imapClient.SelectMailbox("Processed_BackUp");
            return inbox;
        }

        ActiveUp.Net.Mail.Mailbox mailBoxObj;
        public ActiveUp.Net.Mail.Mailbox DownloadInvoiceFromMail()
        {
            try
            {
                mailBoxObj = GetMailBox();
            }
            catch (Exception ex)
            {
                throw;
                //SendMail();
            }
            return mailBoxObj;
        }
    }
}
