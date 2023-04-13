using Microsoft.AspNetCore.Http;
using Myrmec;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MicroBase.Share.Models;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Extensions
{
    public static class FileExtensions
    {
        public static byte[] GetBytes(this IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static async Task SaveAsAsync(this IFormFile formFile, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }
        }

        public static void SaveAs(this IFormFile formFile, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
        }

        public static byte[] GetBytes(this FileInfo file)
        {
            var fs = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            //create byte array of same size as FileStream length
            byte[] fileBytes = new byte[fs.Length];

            //define counter to check how much bytes to read. Decrease the counter as you read each byte
            int numBytesToRead = (int)fileBytes.Length;

            //Counter to indicate number of bytes already read
            int numBytesRead = 0;

            //iterate till all the bytes read from FileStream
            while (numBytesToRead > 0)
            {
                int n = fs.Read(fileBytes, numBytesRead, numBytesToRead);

                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }

            return fileBytes;
        }

        public static byte[] ReadFileHead(IFormFile file)
        {
            using var fs = new BinaryReader(file.OpenReadStream());
            var bytes = new byte[20];
            fs.Read(bytes, 0, 20);
            return bytes;
        }

        public static BaseResponse<string> ValidateImageFile(IFormFile image, int size)
        {
            if (image.Length > size)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = CommonMessage.MAX_FILE_LENGTH,
                    MessageCode = nameof(CommonMessage.MAX_FILE_LENGTH)
                }; ;
            }

            Regex reg = new Regex("[*'\",<>&#^@]");
            if (reg.IsMatch(image.FileName))
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = CommonMessage.INVALID_FILE_NAME,
                    MessageCode = nameof(CommonMessage.MAX_FILE_LENGTH)
                };
            }

            string fileExtension = Path.GetExtension(image.FileName);
            if (fileExtension.Contains("gif") || fileExtension.Contains("mp4"))
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = CommonMessage.CANT_UPLOAD_GIF_FILE,
                    MessageCode = nameof(CommonMessage.CANT_UPLOAD_GIF_FILE)
                };
            }

            var fileTypeCheck = ValidateImageFile(image);
            if (!fileTypeCheck)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = CommonMessage.MODEL_STATE_INVALID,
                    MessageCode = nameof(CommonMessage.MODEL_STATE_INVALID)
                };
            }

            return new BaseResponse<string>
            {
                Success = true,
            };
        }

        private static List<Record> DOCS_TYPES = new List<Record>
        {
            new Record("doc xls ppt msg", "D0 CF 11 E0 A1 B1 1A E1"),
            new Record("xlsx", "50 4B 03 04")
        };

        private static List<Record> PDF_TYPES = new List<Record>
        {
            new Record("pdf", "25 50 44 46")
        };

        private static List<Record> IMAGE_TYPES = new List<Record>
        {
            new Record("jpg,jpeg", "ff,d8,ff,db"),
            new Record("jpg,jpeg", "ff,d8,ff,e2"),
            new Record("jpg,jpeg", "ff,d8,ff,e3"),
            new Record("jpg", "ff,d8,ff,e8"),
            new Record("png", "89,50,4e,47,0d,0a,1a,0a"),
            new Record("gif", "47 49 46 38 39 61"),
            new Record("jpg,jpeg","FF D8 FF E0 ?? ?? 4A 46 49 46 00 01"),
            new Record("jpg,jpeg","FF D8 FF E1 ?? ?? 45 78 69 66 00 00")
        };

        public static bool ValidateImageFile(IFormFile file)
        {
            var sniffer = new Sniffer();
            sniffer.Populate(IMAGE_TYPES);

            byte[] fileHead = ReadFileHead(file);
            var results = sniffer.Match(fileHead);

            if (results.Count > 0)
            {
                return true;
            }

            return false;
        }
        
        public static bool ValidateExcelFile(IFormFile file)
        {
            var sniffer = new Sniffer();
            sniffer.Populate(DOCS_TYPES);

            byte[] fileHead = ReadFileHead(file);
            var results = sniffer.Match(fileHead);

            if (results.Count > 0)
            {
                return true;
            }

            return false;
        }

        public static byte[] ConvertImageToByteArray(IFormFile inputImage, int? quality)
        {
            byte[] result = null;
            using (var memoryStream = new MemoryStream())
            {
                using (var image = Image.Load(inputImage.OpenReadStream())) // IFormFile inputImage
                {
                    var maxWidth = 600;
                    var thumbnailPath = new BaseResponse<string>();
                    var imagePath = new BaseResponse<string>();

                    IResampler sampler = KnownResamplers.Lanczos3;
                    bool compand = true;
                    ResizeMode mode = ResizeMode.Stretch;

                    if (image.Width > maxWidth)
                    {
                        float thumbScaleFactor = (float)maxWidth / (float)image.Width;
                        int maxHeight = (int)(image.Height * thumbScaleFactor);
                        var resizeOptions = new ResizeOptions
                        {
                            Size = new Size(maxWidth, maxHeight),
                            Sampler = sampler,
                            Compand = compand,
                            Mode = mode
                        };

                        image.Mutate(x => x.Resize(resizeOptions).AutoOrient());
                    }

                    var afterMutations = image.Size();
                    var encoder = new JpegEncoder()
                    {
                        Quality = quality //Use variable to set between 5-30 based on your requirements
                    };

                    //This saves to the memoryStream with encoder
                    image.Save(memoryStream, encoder);
                    memoryStream.Position = 0;

                    result = memoryStream.ToArray();
                }
            }

            return result;
        }

        public static string CombinePaths(List<string> others)
        {
            string path = string.Empty;
            foreach (string section in others)
            {
                path = Path.Combine(path, section);
            }

            return path;
        }
    }
}