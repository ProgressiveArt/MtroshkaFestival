using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace MetroshkaFestival.Application.Services
{
    public static class ErrorService
    {
        public static IActionResult Error(ViewDataDictionary viewData, Func<string, IActionResult> statusCodeFunc)
        {
            var errors = (from modelState in viewData.ModelState.Values
                          from error in modelState.Errors
                          select error.ErrorMessage)
               .ToList();

            return statusCodeFunc(JsonConvert.SerializeObject(errors));
        }
    }
}