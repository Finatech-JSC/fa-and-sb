using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MicroBase.BaseMvc
{
    public static class ModelStateService
    {
        public static List<string> GetModelStateErros(ModelStateDictionary state)
        {
            var erros = new List<string>();
            foreach (var modelState in state.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    erros.Add(error.ErrorMessage);
                }
            }

            return erros;
        }
    }
}