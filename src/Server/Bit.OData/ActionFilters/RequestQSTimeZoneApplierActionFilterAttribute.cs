﻿using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Bit.Core.Contracts;
using Bit.Owin.Contracts;
using Microsoft.Owin;

namespace Bit.OData.ActionFilters
{
    public class RequestQSTimeZoneApplierActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Uri url = actionContext.Request.RequestUri;

            if (!string.IsNullOrEmpty(url.Query))
            {
                IDependencyResolver dependencyResolver = actionContext.Request.GetOwinContext()
                        .GetDependencyResolver();

                ITimeZoneManager timeZoneManager = dependencyResolver.Resolve<ITimeZoneManager>();

                string urlAsTextToFix = url.ToString();

                const string isoDateRegExp = "(\\d{4}-[01]\\d-[0-3]\\dT[0-2]\\d:[0-5]\\d:[0-5]\\d\\.\\d+([+-][0-2]\\d:[0-5]\\d|Z))|(\\d{4}-[01]\\d-[0-3]\\dT[0-2]\\d:[0-5]\\d:[0-5]\\d([+-][0-2]\\d:[0-5]\\d|Z))|(\\d{4}-[01]\\d-[0-3]\\dT[0-2]\\d:[0-5]\\d([+-][0-2]\\d:[0-5]\\d|Z))";

                urlAsTextToFix = Regex.Replace(urlAsTextToFix, "\'date\"(.*?)\"\'", (match) =>
                {
                    string result = null;

                    Regex.Replace(match.Value, isoDateRegExp, (innerMatch) =>
                    {
                        DateTimeOffset baseDate = DateTimeOffset.Parse(innerMatch.Value);

                        DateTimeOffset date = baseDate;

                        date = timeZoneManager.MapFromClientToServer(date);

                        result = date.ToString("yyyy-MM-dd");

                        return result;

                    }, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                    return result;

                }, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                urlAsTextToFix = Regex.Replace(urlAsTextToFix, isoDateRegExp, (match) =>
                {
                    DateTimeOffset date = DateTimeOffset.Parse(match.Value);

                    date = timeZoneManager.MapFromClientToServer(date);

                    return date.ToString("yyyy-MM-ddTHH:mm:ss.00Z");

                }, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                actionContext.Request.RequestUri = new Uri(urlAsTextToFix, UriKind.Absolute);
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
