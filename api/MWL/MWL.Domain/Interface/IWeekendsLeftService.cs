using MWL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MWL.Domain.Interface
{
    public interface IWeekendsLeftService
    {
        WeekendsLeftResponse GetWeekendsLeft(WeekendsLeftRequest weekendsLeftRequest);
    }
}
