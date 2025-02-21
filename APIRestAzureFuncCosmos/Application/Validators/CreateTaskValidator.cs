using Application.DTOs;
using Domain.Entities;
using FluentValidation;
using Shared.Exceptions;

namespace Application.Validators;

public class CreateTaskValidator : AbstractValidator<TaskDTO>
{
    public CreateTaskValidator()
    {
        RuleFor(t => t)
            .Custom((task, context) =>
            {
                try
                {
                    TaskItem.ValidateCreation(task.Title, task.Description, task.Deadline);
                }
                catch (DomainException ex)
                {
                    context.AddFailure(ex.Message);
                }
            });
    }
}