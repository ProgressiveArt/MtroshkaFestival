using System.Collections.Generic;
using System.Linq;
using MetroshkaFestival.Core.Extensions;

namespace MetroshkaFestival.Core.Models.Common
{
    public class ReturnQueryModel
    {
        public Dictionary<string, string> GetAllQueryData()
        {
            return this.GetEnumerableObjectPenetration(objectInfo => new
                        {
                            Name = objectInfo.Name,
                            Value = objectInfo.Value?.ToString()
                        })
                       .ToDictionary(x => x.Name, x => x.Value);
        }
    }
}