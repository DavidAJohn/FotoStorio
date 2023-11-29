using FotoStorio.Shared.Entities;
using System.Text.Json;

namespace FotoStorio.Server.Extensions;

public static class HttpContextExtensions
{
    public static void AddPaginationResponseHeaders(
            this HttpContext httpContext,
            int recordCount,
            int pageSize,
            int pageIndex
        )
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        // calculate total number of pages
        int totalPages = (int)Math.Ceiling(recordCount / (double)pageSize);

        // are there previous or next pages?
        bool hasPrevious = pageIndex > 1;
        bool hasNext = pageIndex < totalPages;

        var paginationHeader = new PagingMetadata
        {
            TotalCount = recordCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasPreviousPage = hasPrevious,
            HasNextPage = hasNext
        };

        httpContext.Response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader));
    }
}
