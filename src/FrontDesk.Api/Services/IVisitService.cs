
using FrontDesk.Shared.Dto;

namespace FrontDesk.Api.Services;

public interface IVisitService
{
    Task<VisitResponse> ProcessVisitAsync(VisitRequest req);
}