using AutoMapper;
using MicroBase.Entity;
using MicroBase.NoDependencyService;
using MicroBase.Service.Foundations;
using MicroBase.Share.Constants;
using MicroBase.Share.Extensions;
using MicroBase.Share.Linqkit;
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
    public abstract class CrudImExportBaseApiController<TEntity, TKey, TEntityDto, TResponse, TFileGirdResponse>
        : CrudBaseApiController<TEntity, TKey, TEntityDto, TResponse>
        where TEntity : class, IBaseEntity<TKey>
        where TEntityDto : BaseModel
        where TFileGirdResponse : BaseFileGirdModel
        where TResponse : class
    {
        private readonly IDataGridService dataGridService;
        private readonly ICrudAppService<TEntity, TKey, TEntityDto, TResponse> crudAppService;
        private readonly IMapper mapper;

        protected CrudImExportBaseApiController(IHttpContextAccessor httpContextAccessor,
            ICrudAppService<TEntity, TKey, TEntityDto, TResponse> crudAppService,
            IDataGridService dataGridService,
            IMapper mapper)
            : base(httpContextAccessor, crudAppService)
        {
            this.dataGridService = dataGridService;
            this.crudAppService = crudAppService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Đọc dữ liệu từ file excel và mapping vào model validate dữ liệu sau đó trả về cho client
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <param name="fieldMappings"></param>
        /// <returns></returns>
        [HttpPost("read-from-excel-file")]
        public virtual BaseResponse<IEnumerable<TFileGirdResponse>> ReadFromExcelFile(IFormFile fileUpload,
            [FromForm] IEnumerable<ExcelFieldMapping> fieldMappings)
        {
            try
            {
                var dataRecordRes = ReadDataFromExcelFile(fileUpload, fieldMappings);
                return dataRecordRes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Đọc dữ liệu từ file excel và mapping vào model validate dữ liệu sau đó lưu vào database
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <param name="fieldMappings"></param>
        /// <returns></returns>
        [HttpPost("import-from-excel-file")]
        public virtual async Task<BaseResponse<IEnumerable<TKey>>> ImportFromExcelFile(IFormFile fileUpload,
            [FromForm] IEnumerable<ExcelFieldMapping> fieldMappings)
        {
            try
            {
                var dataRecordRes = ReadDataFromExcelFile(fileUpload, fieldMappings);
                if (!dataRecordRes.Success)
                {
                    return new BaseResponse<IEnumerable<TKey>>
                    {
                        Success = dataRecordRes.Success,
                        Code = dataRecordRes.Code,
                        Message = dataRecordRes.Message,
                        MessageCode = dataRecordRes.MessageCode
                    };
                }

                var entityDtos = new List<TEntityDto>();
                foreach (var excelRecord in dataRecordRes.Data)
                {
                    if (!excelRecord.IsValidData)
                    {
                        continue;
                    }

                    var entityDto = mapper.Map<TEntityDto>(excelRecord);
                    entityDtos.Add(entityDto);
                }

                var insertRes = await crudAppService.AddManyRecordsAsync(entityDtos);
                if (!insertRes.Success)
                {
                    return new BaseResponse<IEnumerable<TKey>>
                    {
                        Success = insertRes.Success,
                        Message = insertRes.Message,
                        MessageCode = insertRes.MessageCode
                    };
                }

                return new BaseResponse<IEnumerable<TKey>>
                {
                    Success = true,
                    Message = string.Format(CommonMessage.IMPORT_FILE_SUCESSFULLY_WITH_COUNT, $"{insertRes.Data.Count()}/{dataRecordRes.Data.Count()}"),
                    MessageCode = nameof(CommonMessage.IMPORT_FILE_SUCESSFULLY_WITH_COUNT)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("export-to-csv-file")]
        public virtual async Task<TPaging<TResponse>> ExportToCsv([FromQuery] List<SearchTermModel> searchTerms,
            string? fieldOrderBy,
            bool isDescending,
            int pageIndex = 1,
            int pageSize = 1000000)
        {
            try
            {
                var records = await crudAppService.FindAsync(searchTerms, fieldOrderBy, isDescending, pageIndex, pageSize);
                return records;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Đọc dữ liệu từ file excel và mapping vào model validate dữ liệu
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <param name="fieldMappings"></param>
        /// <returns></returns>
        private BaseResponse<IEnumerable<TFileGirdResponse>> ReadDataFromExcelFile(IFormFile fileUpload,
            IEnumerable<ExcelFieldMapping> fieldMappings)
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
    }
}