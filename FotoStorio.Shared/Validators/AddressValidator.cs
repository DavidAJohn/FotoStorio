using FluentValidation;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Shared.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(address => address.FirstName).NotEmpty().WithMessage("Your First Name is required");
            RuleFor(address => address.LastName).NotEmpty().WithMessage("Your Last Name is required");
            RuleFor(address => address.Street).NotEmpty().WithMessage("Your Street is required");
            RuleFor(address => address.City).NotEmpty().WithMessage("Your Town/City is required");
            RuleFor(address => address.County).NotEmpty().WithMessage("Your County is required");
            RuleFor(address => address.PostCode).NotEmpty().WithMessage("Your Post Code is required");
        }
    }
}