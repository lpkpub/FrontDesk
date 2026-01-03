using FrontDesk.Shared.Dto;
using FrontDesk.Shared.Enums;
using FrontDesk.Ui.Services;

namespace FrontDesk.UiTests;

public class MockApiClient : IApiClient
{
    public Task<VisitResponse> SendVisitRequestAsync(VisitRequest req)
        => Task.FromResult(VisitResponse.Accepted(VisitAction.Entry, "TestVisitor", "123456"));
}
