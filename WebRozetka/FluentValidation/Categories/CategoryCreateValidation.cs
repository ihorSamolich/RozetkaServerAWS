using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using WebRozetka.Data;
using WebRozetka.Models.Category;

namespace WebRozetka.FluentValidation.Categories
{
    public class CategoryCreateValidation : AbstractValidator<CategoryCreateViewModel>
    {
        private readonly AppEFContext _appEFContext;

        public CategoryCreateValidation(AppEFContext appEFContext)
        {
            _appEFContext = appEFContext;

            RuleFor(x => x.Name)
                .NotNull().WithMessage("Назва категорії не може бути порожньою")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Name).MinimumLength(3).WithMessage("Назва категорії повиннa містити принаймні 3 символів!")
                            .DependentRules(() =>
                            {
                                RuleFor(x => x.Name).Must(BeUniqueName).WithMessage("Категорія з такою назвою вже існує!");
                            });
                    });

            RuleFor(x => x.Description)
                .NotNull().WithMessage("Опис категорії не може бути порожнім")
                .MinimumLength(10).WithMessage("Опис категорії повинен містити принаймні 10 символів");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Зображення категорії не може бути порожнім");
        }

        private bool BeUniqueName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return !_appEFContext.Categories
                .Where(c => !c.IsDeleted)
                .Any(x => x.Name.ToLower().Equals(name.ToLower()));
        }
    }

}
