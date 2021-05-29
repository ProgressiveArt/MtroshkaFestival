using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroshkaFestival.Core.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Today73.Web.TagHelpers
{
    [HtmlTargetElement("sort")]
    public class SortTagHelper : TagHelper
    {
        private const string NameFieldSortState = "Sort.SortState";
        private const string NameFieldSortName = "Sort.NumSort";

        private readonly string[] _sortStateNames;
        private readonly IDictionary<string, string> _sortStateDesignationsDictionary;

        private IUrlHelperFactory _urlHelperFactory;

        [HtmlAttributeName("asp-area")]
        public string AspArea { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string AspController { get; set; }

        [HtmlAttributeName("asp-action")]
        public string AspAction { get; set; }

        [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
        public Dictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [HtmlAttributeName("sort-name")]
        public string SortName { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public SortTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;

            _sortStateDesignationsDictionary = new Dictionary<string, string>
            {
                {SortState.Asc.ToString("F"), "↓"},
                {SortState.Desc.ToString("F"), "↑"}
            };
            _sortStateNames = _sortStateDesignationsDictionary.Keys.ToArray();
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (SortName == null)
            {
                return;
            }

            var oldContent = (await output.GetChildContentAsync()).GetContent();
            output.Content.SetContent($"{oldContent}{GetSortStateDesignation()}");

            SetNextSort();
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "a";
            output.Attributes.Add("href", GenerateSortUrl(urlHelper));
        }

        private string GenerateSortUrl(IUrlHelper urlHelper)
        {
            var routeValueDictionary = new RouteValueDictionary(RouteValues);

            if (AspArea != null)
            {
                routeValueDictionary["area"] = AspArea;
            }

            return urlHelper.Action(AspAction, AspController, routeValueDictionary);
        }

        private void SetNextSort()
        {
            if (IsCurrentSort())
            {
                if (RouteValues[NameFieldSortState] == _sortStateNames.Last())
                {
                    RouteValues.Remove(NameFieldSortName);
                    RouteValues.Remove(NameFieldSortState);
                }
                else
                {
                    RouteValues[NameFieldSortState] = _sortStateNames.SkipWhile(x => x != RouteValues[NameFieldSortState])
                                                                     .Skip(1)
                                                                     .First();
                }
            }
            else
            {
                RouteValues[NameFieldSortName] = SortName;
                RouteValues[NameFieldSortState] = _sortStateNames.First();
            }
        }

        private string GetSortStateDesignation()
        {
            return IsCurrentSort()
                       ? _sortStateDesignationsDictionary[RouteValues[NameFieldSortState]]
                       : null;
        }

        private bool IsCurrentSort()
        {
            return RouteValues.ContainsKey(NameFieldSortState) &&
                   RouteValues.ContainsKey(NameFieldSortName) &&
                   RouteValues[NameFieldSortName] == SortName;
        }
    }
}