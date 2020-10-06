using System.Threading.Tasks;
using MWL.Models;
using MWL.Models.Entities;

namespace MWL.Services.Interface
{
    public interface IWeekendsLeftService
    {
        Task<WeekendsLeftResponse> GetWeekendsLeftAsync(WeekendsLeftRequest weekendsLeftRequest);

        VersionInfo GetVersion();
    }
}
