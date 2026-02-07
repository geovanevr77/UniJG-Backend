using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Mime;
using UniJG.Application.Abstractions.Data;

namespace UniJG_Backend.Filters
{
    /// <summary>
    /// Classe responsável por tratar todas as requisições que retornarem
    /// InvalidModelStateResponse e transformá-las em uma resposta padrão
    /// com a classe Response.
    /// </summary>
    public static class InvalidModelStateResponseFactory
    {
        /// <summary>
        /// Tratar todas as requisições que retornam InvalidModelStateResponse 
        /// e transforma em uma resposta padrão com a classe Response.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IActionResult Handle(ActionContext context)
        {
            Response response = new(ResponseStatus.BadRequest, "A requisição tem alguns dados inválidos.")
            {
                Errors = GetErrorsFromModelState(context.ModelState)
            };

            ObjectResult result = new(response)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            result.ContentTypes.Add(MediaTypeNames.Application.Json);
            return result;
        }

        /// <summary>
        /// Transforma os erros do modelState em um array de Error.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private static Error[] GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            return modelState
                .SelectMany(modelState => GetErrorsFromModelStateEntry(modelState))
                .ToArray();
        }

        /// <summary>
        /// Transforma os erros do modelState em um array de Error.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private static IEnumerable<Error> GetErrorsFromModelStateEntry(KeyValuePair<string, ModelStateEntry> modelState)
        {
            string property = string.Concat(modelState.Key[..1].ToLowerInvariant(), modelState.Key.AsSpan(1));

            foreach (ModelError error in modelState.Value.Errors)
            {
                yield return new Error(property, error.ErrorMessage);
            }
        }
    }
}