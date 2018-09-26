using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Abp.Web.WebApi
{
    public class CustomFilterProvider : IFilterProvider
    {
        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("Configuration is null");
            }

            if (actionDescriptor == null)
            {
                throw new ArgumentNullException("ActionDescriptor is null");
            }


            IEnumerable<CustomFilterInfo> customActionFilters = actionDescriptor.GetFilters().Select(i => new CustomFilterInfo(i, FilterScope.Action));
            IEnumerable<CustomFilterInfo> customControllerFilters = actionDescriptor.ControllerDescriptor.GetFilters().Select(i => new CustomFilterInfo(i, FilterScope.Action));
            //IEnumerable<CustomFilterInfo> globalFilters = configuration.Filters.ToList().Select(i => new CustomFilterInfo(i.Instance, FilterScope.Global));

            return customControllerFilters.Concat(customActionFilters).OrderBy(i => i).Select(i => i.ConvertToFilterInfo());
        }
    }
}
