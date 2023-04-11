using MicroBase.Share.Models;

namespace MicroBase.Service.Localizations
{
    public interface IGenericLocalizationService
    {
        BaseResponse<T> LocalizationBaseResponse<T>(BaseResponse<T> model, string prefix = null);
    }

    public class GenericLocalizationService : IGenericLocalizationService
    {
        private readonly ILocalizationService localizationService;

        public GenericLocalizationService(ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        public BaseResponse<T> LocalizationBaseResponse<T>(BaseResponse<T> model, string prefix = null)
        {
            var message = string.Empty;
            if (!string.IsNullOrWhiteSpace(model.MessageCode))
            {
                message = localizationService.GetLocalizationString(model.MessageCode, model.MsgParams, prefix, model.Message);
            }
            else if (!string.IsNullOrWhiteSpace(model.Message))
            {
                message = localizationService.GetLocalizationString(model.Message, model.MsgParams, prefix, model.MessageCode);
                if (!string.IsNullOrWhiteSpace(model.MessageCode) && message.Equals(model.MessageCode))
                {
                    message = localizationService.GetLocalizationString(model.MessageCode, model.MsgParams, prefix, model.Message);
                }
            }

            model.Message = message;
            return model;
        }
    }
}