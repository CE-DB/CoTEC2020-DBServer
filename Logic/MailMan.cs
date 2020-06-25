using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic
{
    public class MailMan
    {

        public static void Email(string msg, string sub)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("diaza913@gmail.com");
                message.To.Add(new MailAddress("diaza913@gmail.com"));
                message.Subject = sub;
                message.IsBodyHtml = false; //to make message body as html  
                message.Body = msg;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("diaza913@gmail.com", "@cer_2599");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception) { }
        }

    }
}
