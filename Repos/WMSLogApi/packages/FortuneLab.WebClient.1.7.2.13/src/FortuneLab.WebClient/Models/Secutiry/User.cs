
namespace FortuneLab.WebClient.Models
{
    public class User : ModelBase
    {
        public int SysId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SysNO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserStatus Status { get; set; }
    }

    public enum UserStatus
    {
        Disabled = 0,
        Enabled = 1
    }
}
