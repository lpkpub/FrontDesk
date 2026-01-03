using FrontDesk.Shared.Dto;
using System.Threading.Tasks;

namespace FrontDesk.Ui.Services;

public interface IApiClient
{
    Task<VisitResponse> SendVisitRequestAsync(VisitRequest req);
}