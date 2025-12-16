//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ModelBinding;

//namespace UniJG_Backend.Filters
//{
//    public static class InvalidModelStateResponseFactory
//    {
//        public static IActionResult Handle(ActionContext context)
//        {
//            return null;
//        }

//        private static Error[] GetErrorsFromModelState(
//            ModelStateDictionary modelState)
//        {
//            return modelState.SelectMany(modelState => GetErrorsFromModelStateEntry).ToArray();
//        }

//        private static IEnumerable<Error> GetErrorsFromModelStateEntry(KeyValuePair<string, ModelStateEntry> modelState)
//        {
//            string property = string.Concat(modelState.Key[..1].ToLowerInvariant(), modelState.Key.AsSpan(1));

//            foreach(ModelError error in modelState.Value.Errors)
//            {
//                yield return new Error(property, error.ErrorMessage);
//            }
//        }
//    }
//}