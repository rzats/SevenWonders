using System;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Owin.Security;
using System.Security.Claims;
using SevenWonders.WebAPI.Models;

namespace SevenWonders.WebAPI.DTO.Account
{
    public class Utils
    {

        public string GetEncodedHash(string password, string salt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            return base64digest.Substring(0, base64digest.Length - 2);
        }

        public bool SendEmail(MailMessage message)
        {
            if (message == null)
            {
                throw new NullReferenceException("message is empty!!");
            }
            bool result = false;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("7wondersedvantis", "1q2w3eASD");
            smtp.EnableSsl = true;
            smtp.Send(message);
            result = true;
            return result;
        }

        public MailMessage GenerateMessage(string email, string title, string subject, string body, bool isBodyHtml = false)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new NullReferenceException("Email is empty!!");
            }
            MailAddress from = new MailAddress("7wondersedvantis@gmail.com", title);
            MailAddress to = new MailAddress(email);
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;
            return message;
        }


        public void AddUserInCookies(User user, IAuthenticationManager authenticationManager)
        {
            ClaimsIdentity claim = new ClaimsIdentity("ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            claim.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String));
            claim.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email, ClaimValueTypes.String));
            claim.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                "OWIN Provider", ClaimValueTypes.String));
            if (user.Role != null)
                claim.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name, ClaimValueTypes.String));
            authenticationManager.SignOut();
            authenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = true
            }, claim);
        }

    }
}