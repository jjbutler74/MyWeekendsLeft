using System.Threading.Tasks;
using MWL.Models;

namespace MWL.Services.Interface
{
    public interface IWeekendsLeftService
    {
        Task<WeekendsLeftResponse> GetWeekendsLeftAsync(WeekendsLeftRequest weekendsLeftRequest);

        string GetVersion();
    }
}
