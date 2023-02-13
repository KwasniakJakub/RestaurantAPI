using System.Linq;
using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(RestaurantDbContext dbContext)
        {

            RuleFor(x => x.Email)
                .NotEmpty()//Not empty - Pole będzie wymagane
                .EmailAddress(); //Musi byćw formacie adresu email
            RuleFor(x => x.Password)
                .MinimumLength(8); //Minimalna długość 
            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password); //Sprawdzenie czy pole ConfirmPassword jest takie same jak Password
            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    //Sprawdzenie czy istnieje już konto o podanym emailu w bazie danych 
                    var emailInUse = dbContext.Users.Any(u => u.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "This email is taken");
                    }
                });
        }
    }
}