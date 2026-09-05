using cbt.be.Models.RequestModels.Admin.Dashboard;
using FluentValidation;

namespace cbt.be.Validator.Admin
{
    public class GetActivityLogsValidator : AbstractValidator<GetActivityLogsRequest>
    {
        public GetActivityLogsValidator() 
        {
            RuleFor(x => x.limit)
                .GreaterThan(0).WithMessage("Limit must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Limit must be less than or equal to 100.");
        }
    }
}
