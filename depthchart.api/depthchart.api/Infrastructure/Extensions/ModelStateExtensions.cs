using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace depthchart.api.Infrastructure.Extensions
{
    public static class ModelStateExtensions
    {
        public static bool ModelStateInvalidOrIsModelNull(this ModelStateDictionary modelState, object model, out IEnumerable<string> errors)
        {
            if (model is null)
            {
                errors = ["Model must cannot not be null"];
                return true;
            }
            if (!modelState.IsValid)
            {
                errors = modelState.GetModelErrors();
                return true;
            }

            errors = Enumerable.Empty<string>();
            return false;
        }

        public static IEnumerable<string> GetModelErrors(this ModelStateDictionary modelState) =>
            modelState.Values.Where(v => v.ValidationState.Equals(ModelValidationState.Invalid)).SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
    }
}