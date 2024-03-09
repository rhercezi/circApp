using Core.Messages;

namespace Circles.Query.Application.Queries
{
    public class SearchQuery : BaseQuery
    {
        public string? QWord { get; set; }
    }
}