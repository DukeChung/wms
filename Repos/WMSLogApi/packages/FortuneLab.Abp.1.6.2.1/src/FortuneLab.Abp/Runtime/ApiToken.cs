using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Runtime
{
    public class ApiToken : IDto
    {
        public string Token { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
