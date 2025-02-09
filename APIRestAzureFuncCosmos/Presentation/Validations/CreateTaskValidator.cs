using Application.DTOs;
using FluentValidation;
using Shared.Enums;

namespace Presentation.Validations;

public class CreateTaskValidator : AbstractValidator<TaskDTO>
{
    public CreateTaskValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty()
            .WithMessage(RestUtilityConsts.VALIDATION_TITLE_NOT_EMPTY);

        RuleFor(t => t.Description)
            .NotEmpty()
            .WithMessage(RestUtilityConsts.VALIDATION_DESCRIPTION_NOT_EMPTY);
    }
}