using System.Collections.Generic;
using Abp.Configuration;

namespace NBK.ECService.WMS.Application
{
    public class WMSSettingProvider: SettingProvider
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