using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using BulkEmailSender.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkEmailSender.Controllers
{
    public class BulkEmailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddEmailsToQueue(int emailCount)
        {
            try
            {
                Queue<MailMessage> surveyMessageQ = BulkEmailModel.GetEmailMessageQInstance();

                //TODO : Create emails and add to queue
            }
            catch (Exception ex)
            {

                throw;
            }

            TempData["SuccessMsg"] = "Emails queued successfully";

            return RedirectToAction("Index", "Home");
        }

        public IActionResult SendEmails()
        {
            try
            {
                Queue<MailMessage> surveyMessageQ = BulkEmailModel.GetEmailMessageQInstance();

                //TODO : Send Emails, DeQueue, Consider the email server send limitations(Per minute how many Emails can be send) 
            }
            catch (Exception ex)
            {

                throw;
            }

            return View();
        }
    }
}