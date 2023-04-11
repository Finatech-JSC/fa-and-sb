using MicroBase.Share.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MicroBase.Share.Extensions
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Send GET request
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static async Task<BaseResponse<TResponse>> GetRequestAsync<TResponse>(this HttpClient httpClient,
            string requestUrl)
        {
            if (requestUrl.StartsWith("/"))
            {
                requestUrl = requestUrl.Remove(0, 1);
            }

            var requestPath = httpClient.BaseAddress + requestUrl;
            try
            {
                var response = await httpClient.GetAsync(requestPath);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponse<TResponse>
                    {
                        Success = false,
                        Code = (int)response.StatusCode,
                        Message = content
                    };
                }

                response.EnsureSuccessStatusCode();
                var res = JsonExtensions.JsonDeserialize<TResponse>(content);

                return new BaseResponse<TResponse>
                {
                    Success = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = res
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TResponse>
                {
                    Success = false,
                    Message = $"UN_DETECTED_ERROR {Environment.NewLine} URL: {requestPath} {Environment.NewLine} {ex}",
                    MessageCode = "UN_DETECTED_ERROR"
                };
            }
        }

        /// <summary>
        /// POST as application/json
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="requestUrl"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<BaseResponse<TResponse>> PostRequestAsync<TRequest, TResponse>(this HttpClient httpClient,
            string requestUrl,
            TRequest request)
        {
            var requestPath = httpClient.BaseAddress + requestUrl;
            try
            {
                var postData = string.Empty;
                if (typeof(TRequest) != typeof(string))
                {
                    postData = request.JsonSerialize();
                }
                else
                {
                    postData = request.ToString();
                }

                var response = await httpClient.PostAsync(requestPath, new StringContent(postData, Encoding.UTF8, "application/json"));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponse<TResponse>
                    {
                        Success = false,
                        Code = (int)response.StatusCode,
                        Message = content
                    };
                }

                response.EnsureSuccessStatusCode();
                var res = JsonExtensions.JsonDeserialize<TResponse>(content);

                return new BaseResponse<TResponse>
                {
                    Success = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = res
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TResponse>
                {
                    Success = false,
                    Message = $"UN_DETECTED_ERROR {Environment.NewLine} URL: {requestPath} {Environment.NewLine} {ex}",
                    MessageCode = "UN_DETECTED_ERROR"
                };
            }
        }

        /// <summary>
        /// POST as FormUrlEncoded
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="requestUrl"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        public static async Task<BaseResponse<TResponse>> PostRequestAsync<TRequest, TResponse>(this HttpClient httpClient,
            string requestUrl,
            Dictionary<string, string> requests)
        {
            var requestPath = httpClient.BaseAddress + requestUrl;
            try
            {
                var response = await httpClient.PostAsync(requestPath, new FormUrlEncodedContent(requests));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponse<TResponse>
                    {
                        Success = false,
                        Code = (int)response.StatusCode,
                        Message = content
                    };
                }

                response.EnsureSuccessStatusCode();
                var res = JsonExtensions.JsonDeserialize<TResponse>(content);

                return new BaseResponse<TResponse>
                {
                    Success = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = res
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TResponse>
                {
                    Success = false,
                    Message = $"UN_DETECTED_ERROR {Environment.NewLine} URL: {requestPath} {Environment.NewLine} {ex}",
                    MessageCode = "UN_DETECTED_ERROR"
                };
            }
        }

        /// <summary>
        /// POST with files
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="requestUrl"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        public static async Task<BaseResponse<TResponse>> PostFileRequestAsync<TResponse>(this HttpClient httpClient,
            string requestUrl,
            IFormFile files)
        {
            HttpResponseMessage response;
            string resContent = string.Empty;
            var requestPath = httpClient.BaseAddress + requestUrl;
            try
            {
                using (var contentx = new MultipartFormDataContent())
                {
                    using (var fileStream = files.OpenReadStream())
                    {
                        var fileContent = new StreamContent(fileStream);

                        contentx.Add(fileContent, "file", files.Name);

                        response = await httpClient.PostAsync(requestPath, contentx);
                        resContent = await response.Content.ReadAsStringAsync();
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponse<TResponse>
                    {
                        Success = false,
                        Code = (int)response.StatusCode,
                        Message = resContent
                    };
                }

                response.EnsureSuccessStatusCode();
                var res = JsonExtensions.JsonDeserialize<TResponse>(resContent);

                return new BaseResponse<TResponse>
                {
                    Success = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = res
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TResponse>
                {
                    Success = false,
                    Message = $"UN_DETECTED_ERROR {Environment.NewLine} URL: {requestPath} {Environment.NewLine} {ex}",
                    MessageCode = "UN_DETECTED_ERROR"
                };
            }
        }

        /// <summary>
        /// PUT application/json
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="requestUrl"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<BaseResponse<TResponse>> PutRequestAsync<TRequest, TResponse>(this HttpClient httpClient,
            string requestUrl,
            TRequest request)
        {
            var requestPath = httpClient.BaseAddress + requestUrl;
            try
            {
                var postData = string.Empty;
                if (typeof(TRequest) != typeof(string))
                {
                    postData = request.JsonSerialize();
                }
                else
                {
                    postData = request.ToString();
                }

                var response = await httpClient.PutAsync(requestPath, new StringContent(postData, Encoding.UTF8, "application/json"));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponse<TResponse>
                    {
                        Success = false,
                        Code = (int)response.StatusCode,
                        Message = content
                    };
                }

                response.EnsureSuccessStatusCode();
                var res = JsonExtensions.JsonDeserialize<TResponse>(content);

                return new BaseResponse<TResponse>
                {
                    Success = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = res
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TResponse>
                {
                    Success = false,
                    Message = $"UN_DETECTED_ERROR {Environment.NewLine} URL: {requestPath} {Environment.NewLine} {ex}",
                    MessageCode = "UN_DETECTED_ERROR"
                };
            }
        }

        /// <summary>
        /// DELETE
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static async Task<BaseResponse<TResponse>> DeleteRequestAsync<TResponse>(this HttpClient httpClient,
            string requestUrl)
        {
            var requestPath = httpClient.BaseAddress + requestUrl;
            try
            {
                var response = await httpClient.DeleteAsync(requestPath);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponse<TResponse>
                    {
                        Success = false,
                        Code = (int)response.StatusCode,
                        Message = content
                    };
                }

                response.EnsureSuccessStatusCode();
                var res = JsonExtensions.JsonDeserialize<TResponse>(content);

                return new BaseResponse<TResponse>
                {
                    Success = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = res
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TResponse>
                {
                    Success = false,
                    Message = $"UN_DETECTED_ERROR {Environment.NewLine} URL: {requestPath} {Environment.NewLine} {ex}",
                    MessageCode = "UN_DETECTED_ERROR"
                };
            }
        }
    }
}