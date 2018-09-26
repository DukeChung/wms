using System.Collections.Generic;
using Abp.Configuration;

namespace NBK.ECService.WMSLog.Application
{
    public class WMSLogSettingProvider : SettingProvider
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