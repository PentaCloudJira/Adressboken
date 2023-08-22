using System.Net.Mail;

namespace Adressboken.Data

{
    public class EmailSettings
    {
        public string Host { get; set; } = "smtp.office365.com";
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = "Bengts.BilService@hotmail.com";
        public string Password { get; set; } = "BengtsService1";
        public string SenderEmail { get; set; } = "Bengts.BilService@hotmail.com";
        public string SenderName { get; set; } = "Bengts BilService";
    }
}
