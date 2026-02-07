using FluentValidation.Results;
using System.Text.Json.Serialization;

namespace UniJG.Application.Abstractions.Data
{
    public class Response
    {
        public Response()
        {
            TraceId = Elastic.Apm.Agent.Tracer?.CurrentTransaction?.TraceId;
        }

        public Response(object result)
            : this()
        {
            Result = result;
        }

        public Response(Exception exception)
            : this()
        {
            Status = ResponseStatus.InternalServerError;
            Message = exception.Message;
        }

        public Response(ValidationFailure validationFailure)
            : this()
        {
            Status = ResponseStatus.BadRequest;
            Message = validationFailure.ErrorMessage;
            Errors =
            [
                new Error(validationFailure)
            ];
        }

        public Response(IEnumerable<ValidationFailure> validationFailures)
            : this()
        {
            Status = ResponseStatus.BadRequest;
            Message = "Erro na validação da requisição.";
            Errors = validationFailures
                .Select(failure => new Error(failure))
                .ToArray();
        }

        public Response(ResponseStatus responseStatus, string message)
            : this()
        {
            Status = responseStatus;
            Message = message;
        }

        public ResponseStatus Status { get; set; } = ResponseStatus.Ok;

        public string Message { get; set; } = string.Empty;

        public Error[] Errors { get; set; } = [];

        [JsonIgnore]
        public object Result { get; set; }

        [JsonIgnore]
        public bool IsSuccess =>
            Status == ResponseStatus.Ok &&
            Errors.Length == 0;

        public string TraceId { get; set; }
    }

    public class Response<TResult> : Response
    {
        public Response() { }

        public Response(TResult result)
            : base(result) { }

        public Response(ValidationFailure validationFailure)
            : base(validationFailure) { }

        public Response(IEnumerable<ValidationFailure> validationFailures)
            : base(validationFailures) { }

        public Response(Exception exception)
            : base(exception) { }

        public Response(ResponseStatus responseStatus, string message)
            : base(responseStatus, message) { }

        public new TResult Result
        {
            get => (TResult)base.Result;
            set => base.Result = value;
        }
    }
}