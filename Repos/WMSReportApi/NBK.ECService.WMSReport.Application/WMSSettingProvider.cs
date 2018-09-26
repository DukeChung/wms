using System.Collections.Generic;
using Abp.Configuration;

namespace NBK.ECService.WMSReport.Application
{
    public class WMSReportSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new List<SettingDefinition>
                   {
                       new SettingDefinition("Deposit", "99", scopes: SettingScopes.Application | SettingScopes.Tenant)
                   };
        }
    }
}