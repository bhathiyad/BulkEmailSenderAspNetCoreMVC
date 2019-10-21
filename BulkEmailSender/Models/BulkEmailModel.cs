using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BulkEmailSender.Models
{
    public sealed class BulkEmailModel
    {
        private static Queue<MailMessage> emailMsgQueue = GetEmailMessageQInstance();

        public static Queue<MailMessage> GetEmailMessageQInstance()
        {
            if (emailMsgQueue == null)
            {
                return new Queue<MailMessage>();
            }
            else
            {
                return emailMsgQueue;
            }

        }
    }
}
