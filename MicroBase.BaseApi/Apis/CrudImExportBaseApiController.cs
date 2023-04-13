using MicroBase.Entity;
using MicroBase.NoDependencyService;
using MicroBase.Service.Foundations;
using MicroBase.Share.Constants;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using MicroBase.Share.Models.DataGirds;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.BaseApi.Apis
{
    /// <summary>
    /// Base API Controller xử lý các nghiệp vụ về CRUD và Import Export file
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntityDto"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TFileGirdResponse"></typeparam>
    [ApiController]
    public abstract class CrudImExportBaseApiController<TEntity, TKey, TEntityDto, TResponse, TFileGirdResponse> : CrudBaseApiController<TEntity, TKey, TEntityDto, TResponse>
        where TEntity : class, IBaseEntity<TKey>
        where TEntityDto : BaseModel
        where TFileGirdResponse : BaseFileGirdModel
        where TResponse : class
    {
        private readonly IDataGridService dataGridService;

        protected CrudImExportBaseApiController(IHttpContextAccessor httpContextAccessor,
            ICrudAppService<TEntity, TKey, TEntityDto, TResponse> crudAppService,
            IDataGridService dataGridService)
            : base(httpContextAccessor, crudAppService)
        {
            this.dataGridService = dataGridService;
        }

        [HttpPost("read-from-file")]
        public virtual BaseResponse<IEnumerable<TFileGirdResponse>> ReadFromFile(IFormFile fileUpload,
            [FromForm] IEnumerable<ExcelFieldMapping> fieldMappings)
        {
            try
            {
                var isFileValid = FileExtensions.ValidateExcelFile(fileUpload);
                if (!isFileValid)
                {
                    return new BaseResponse<IEnumerable<TFileGirdResponse>>
                    {
                        Success = false,
                        Message = CommonMessage.EXCEL_FILE_INVALID,
                        MessageCode = nameof(CommonMessage.EXCEL_FILE_INVALID)
                    };
                }

                var dataRecords = dataGridService.GetDataFromExcelFile<TFileGirdResponse>(fileUpload, fieldMappings);
                foreach (var item in dataRecords)
                {
                    var context = new ValidationContext(item, serviceProvider: null, items: null);
                    var errorResults = new List<ValidationResult>();

                    var isValid = Validator.TryValidateObject(item, context, errorResults, validateAllProperties: true);
                    item.IsValidData = isValid;

                    if (!isValid)
                    {
                        item.ErrorMessages = new List<string>();
                        for (var i = 0; i < errorResults.Count(); i++)
                        {
                            item.ErrorMessages.Add(errorResults.ElementAt(i).ErrorMessage);
                        }
                    }
                }

                return new BaseResponse<IEnumerable<TFileGirdResponse>>
                {
                    Success = true,
                    Data = dataRecords,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}