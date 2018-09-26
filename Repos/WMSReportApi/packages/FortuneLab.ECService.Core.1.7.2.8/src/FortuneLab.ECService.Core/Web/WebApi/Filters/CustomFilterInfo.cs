using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Abp.Web.WebApi
{
    public class CustomFilterInfo : IComparable
    {
        public CustomFilterInfo(IFilter instance, FilterScope scope)
        {
            Instance = instance;
            Scope = scope;
        }

        public IFilter Instance { get; set; }
        public FilterScope Scope { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is CustomFilterInfo)
            {
                var item = obj as CustomFilterInfo;

                if (item.Instance is IBaseAttribute)
                {
                    var attr = item.Instance as IBaseAttribute;
                    if (Instance is IBaseAttribute)
                        return (Instance as IBaseAttribute).Order.CompareTo(attr.Order);
                    return 1;
                }
                return 1;
                //throw new ArgumentException("Object is of wrong type");
            }
            return 1;
            //throw new ArgumentException("Object is of wrong type");
        }

        public FilterInfo ConvertToFilterInfo()
        {
            return new FilterInfo(Instance, Scope);
        }
    }
}
