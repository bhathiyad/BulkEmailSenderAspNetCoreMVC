using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Timers;
using BulkEmailSender.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkEmailSender.Controllers
{
    public class BulkEmailController : Controller
    {
        Queue<MailMessage> emailMessageQ = BulkEmailModel.GetEmailMessageQInstance();
        BackgroundWorker backgroundWorker;
        int emailCountPerRound = 0;

        public IActionResult Index()
        {
            return View();
        }

        private List<MailMessage> CreateEmails()
        {
            List<string> receipientList = new List<string>
            {
                "xxx@gmail.com",
                "yyy@gmail.co",
                "zzz@gmail.com",
                "aaa@gmail.com",
                "bbb@gmail.com",
                "ccc@gmail.com",
                "ddd@gmail.com"
            };

            List<MailMessage> mailMessages = new List<MailMessage>();

            foreach (var item in receipientList)
            {
                MailMessage mailMessage = new MailMessage(
                new MailAddress("youremail@gmail.com", "Sender"),
                new MailAddress(item, "Receiver"))
                {
                    Subject = "Hello there!",
                    Body = "Welcome to the Bulk Email Sender."
                };
                mailMessage.IsBodyHtml = true;
                mailMessages.Add(mailMessage);
            }

            return mailMessages;
        }

        public IActionResult AddEmailsToQueue(int emailCount)
        {
            try
            {
                emailCountPerRound = emailCount;
                //Create emails 
                var emails = CreateEmails();

                //add to queue
                for (int i = 0; i < emails.Count; i++)
                    emailMessageQ.Enqueue(emails[i]);

                //Start sending
                EmailWorker();
                
            }
            catch (Exception ex)
            {

                throw;
            }

            TempData["SuccessMsg"] = "Emails queued successfully";

            return RedirectToAction("Index", "Home");
        }

        public void DequeueEmails()
        {
            try
            {
                //DeQueue
                //Consider the email server send limitations(Per minute how many Emails can be send) 
                List<MailMessage> mailmsgs = new List<MailMessage>();

                if (emailMessageQ.Count > 0)
                {
                    Debug.WriteLine("SendEmailsWorker, MessageQueueSize = " + emailMessageQ.Count);
                    
                    for (int i = 0; i < emailCountPerRound; i++)
                    {
                        if (emailMessageQ.Count > 0)
                            mailmsgs.Add(emailMessageQ.Dequeue());
                    }

                }

                #region Sender

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 30000;
                    client.Credentials = new System.Net.NetworkCredential("youremail@gmail.com", "password");

                    //Send Emails
                    foreach (var msg in mailmsgs)
                        client.Send(msg);

                    client.Dispose();
                }

                #endregion
                

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void EmailWorker()
        {
            try
            {
                if (backgroundWorker == null)
                    backgroundWorker = new BackgroundWorker();

                backgroundWorker.DoWork += SendEmails;

                //NOTE: Run for the first time. 2nd run will initiate after the "interval" value
                backgroundWorker.RunWorkerAsync();

                var interval = new TimeSpan(0, 0, 60);
                Timer timer = new Timer(interval.TotalMilliseconds);
                timer.Elapsed += TimerElapsed;
                timer.Start();
            }
            catch (Exception)
            {

                throw;
            }
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!backgroundWorker.IsBusy)
                backgroundWorker.RunWorkerAsync();

        }

        public void SendEmails(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine("MessageWorker, SendEmails");

            DequeueEmails();

            Debug.WriteLine("Success, MessageWorker, SendEmails");
        }

        
    }
}