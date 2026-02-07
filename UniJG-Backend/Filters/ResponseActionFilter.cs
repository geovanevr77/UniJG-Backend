using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UniJG.Application.Abstractions.Data;

namespace UniJG_Backend.Filters
{
    /// <summary>
    /// Classe responsável por verificar se a resposta de uma Action
    /// é um objeto Response e, caso afirmativo, define o StatusCode
    /// da responsa o mesmo Status do objeto response.
    /// Caso a resposta seja OK, somente retorna o objeto Result.
    /// </summary>
    public class ResponseActionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // [SONAR] Não há implementação, pois atualmente não é necessário.
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult
                && objectResult.Value is Response response)
            {
                object result = response.Status switch
                {
                    ResponseStatus.Ok => response.Result,
                    ResponseStatus.NoContent => null,
                    _ => response,
                };

                context.Result = new ObjectResult(result)
                {
                    StatusCode = (int)response.Status,
                };
            }
        }
    }
}