#region Using

using System.ComponentModel.DataAnnotations;

#endregion

namespace WMS.Report.Portal.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "登陆用户名")]
        public string LoginName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    //public class AccountLoginModel
    //{
    //    //[Required]
    //    [EmailAddress]
    //    public string Email { get; set; }

    //    public string UserName { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    public string Password { get; set; }

    //    public string ReturnUrl { get; set; }
    //    public bool RememberMe { get; set; }
    //}

    //public class AccountForgotPasswordModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    public string Email { get; set; }
    //}

    //public class AccountResetPasswordModel
    //{
    //    [Required]
    //    [DataType(DataType.Password)]
    //    public string Password { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [Compare("Password")]
    //    public string PasswordConfirm { get; set; }
    //}

    //public class AccountRegistrationModel
    //{
    //    public string Username { get; set; }

    //    [Required]
    //    [EmailAddress]
    //    public string Email { get; set; }

    //    [Required]
    //    [EmailAddress]
    //    [Compare("Email")]
    //    public string EmailConfirm { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    public string Password { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [Compare("Password")]
    //    public string PasswordConfirm { get; set; }
    //}
}