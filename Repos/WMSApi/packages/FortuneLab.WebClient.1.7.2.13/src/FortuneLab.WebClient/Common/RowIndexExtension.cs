using FortuneLab.Models;
using FortuneLab.WebApiClient;
using FortuneLab.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Common
{
    public static class RowIndexExtensions
    {
        public static IList<TResult> AttachRowIndex<TResult>(this IList<TResult> resultList, int startIndex = 1)
            where TResult : class, IRowIndex
        {
            foreach (var item in resultList)
            {
                item.RowIndex = startIndex++;
            }
            return resultList;
        }

        public static Page<TResult> AttachRowIndex<TResult>(this Page<TResult> resultList, int startIndex = 1)
           where TResult : class, IRowIndex
        {
            resultList.Records.AttachRowIndex((resultList.Paging.PageIndex - 1) * resultList.Paging.PageSize + 1);
            return resultList;
        }
    }
}
