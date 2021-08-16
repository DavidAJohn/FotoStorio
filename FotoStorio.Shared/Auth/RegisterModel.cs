namespace FotoStorio.Shared.Auth
{
    public class RegisterModel
    {
        // Fluent Validation provided by FotoStorio.Shared.Validators.RegistrationValidator
        
        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}