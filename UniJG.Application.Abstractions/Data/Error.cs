using FluentValidation.Results;

namespace UniJG.Application.Abstractions.Data
{
    public class Error
    {
        public Error() { }

        public Error(string message)
        {
            Message = message;
        }

        public Error(string field, string message)
        {
            Field = field;
            Message = message;
        }

        public Error(ValidationFailure validationFailure)
        {
            Field = validationFailure.PropertyName;
            Message = validationFailure.ErrorMessage;
        }

        public string Field { get; set; }

        public string Message { get; set; }
    }
}