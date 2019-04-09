using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Helpers
{
    public static class UrlHelper
    {
        public static string ToUrlArray(this IEnumerable<int> array,string prompt,bool isStart=true)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder(120);
            if (!isStart)
            {
                str.Append("&");
            }
            int[] elementsArray = array.ToArray();
            for(var i=0;i<elementsArray.Length;i++)
            {
                str.AppendFormat("{0}={1}&", prompt, elementsArray[i]);
            }
            if(str.Length>=1)
            {
                str.Length -= 1;
            }
            return Uri.UnescapeDataString(str.ToString());
        }
    }
}