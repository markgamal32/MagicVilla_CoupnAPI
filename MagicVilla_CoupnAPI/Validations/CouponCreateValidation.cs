using FluentValidation;
using MagicVilla_CoupnAPI.Models.DTO;

namespace MagicVilla_CoupnAPI.Validations
{
	public class CouponCreateValidation :AbstractValidator<CouponCreateDTO>
	{
        public CouponCreateValidation()
        {
			RuleFor(model => model.Name).NotEmpty();
			RuleFor(model => model.Percent).InclusiveBetween(1, 100);

		}
	}
}
