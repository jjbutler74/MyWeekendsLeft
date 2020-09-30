using System.Collections.Generic;
using FluentValidation;
using MWL.Models.Entities;

namespace MWL.Models.Validators
{
    public class WeekendsLeftRequestValidator : AbstractValidator<WeekendsLeftRequest>
    {
        public WeekendsLeftRequestValidator()
        {
            RuleFor(wlr => wlr.Age).InclusiveBetween(1, 120);

            RuleFor(wlr => wlr.Gender).IsInEnum()
                .NotEqual(Gender.Unknown);

            RuleFor(wlr => wlr.Country).NotEmpty().Length(3);
        }
    }
}