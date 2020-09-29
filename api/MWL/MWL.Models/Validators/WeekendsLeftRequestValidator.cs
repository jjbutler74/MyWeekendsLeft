using FluentValidation;

namespace MWL.Models.Validators
{
    public class WeekendsLeftRequestValidator : AbstractValidator<WeekendsLeftRequest>
    {
        public WeekendsLeftRequestValidator()
        {
            RuleFor(wlr => wlr.Age).NotEmpty()
                                                           .InclusiveBetween(1, 120);
        }
    }
}
