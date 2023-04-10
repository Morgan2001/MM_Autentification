using FluentResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Authentication.Api.Extensions;

public static class ResultToModelStateExtensions
{
    public static void AddToModelState<T>(this Result<T> result, ModelStateDictionary modelState)
    {
        if (result.IsSuccess) return;

        foreach (var reason in result.Reasons)
        {
            modelState.AddModelError("Validation", reason.Message);
        }
    }
    
    public static void AddToModelState(this Result result, ModelStateDictionary modelState)
    {
        if (result.IsSuccess) return;

        foreach (var reason in result.Reasons)
        {
            modelState.AddModelError("Validation", reason.Message);
        }
    }
}
