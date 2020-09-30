using MWL.Models;

namespace MWL.Services.Interface
{
    public interface IWeekendsLeftService
    {
        WeekendsLeftResponse GetWeekendsLeft(WeekendsLeftRequest weekendsLeftRequest);
    }
}
