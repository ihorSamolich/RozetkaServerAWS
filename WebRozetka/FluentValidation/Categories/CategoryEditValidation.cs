using FluentValidation;
using WebRozetka.Data;
using WebRozetka.Models.Category;

namespace WebRozetka.FluentValidation.Categories
{
    public class CategoryEditValidation : AbstractValidator<CategoryEditViewModel>
    {
        private readonly AppEFContext _appEFContext;
        public CategoryEditValidation(AppEFContext appEFContext)
        {
            _appEFContext = appEFContext;

            RuleFor(x => x.Name)
                .NotNull().WithMessage("Назва категорії не може бути порожньою")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Name).MinimumLength(3).WithMessage("Назва категорії повиннa містити принаймні 3 символів!")
                            .DependentRules(() =>
                            {
                                RuleFor(x => x).Must(BeUniqueName).WithMessage("Категорія з такою назвою вже існує!");
                            });
                    });

            RuleFor(x => x.Description)
                .NotNull().WithMessage("Опис категорії не може бути порожнім")
                .MinimumLength(10).WithMessage("Опис категорії повинен містити принаймні 10 символів");
        }
        private bool BeUniqueName(CategoryEditViewModel model)
        {
            if (String.IsNullOrEmpty(model.Name))
                return false;
            return !_appEFContext.Categories
                .Where(x => !x.IsDeleted)
                .Where(x => x.Id != model.Id)
                .Any(x => x.Name.ToLower().Equals(model.Name.ToLower()));
        }
    }
}
