using System.Collections.Generic;

namespace MarketApi.Errors
{
    public class ApiValidationErrorResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public bool HasError { get; set; } = false;

    }
}
