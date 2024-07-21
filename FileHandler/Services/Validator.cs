using Comuns.Classes;
using Comuns.Extension;
using Comuns.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileHandler.Services
{
    internal static class Validator
    {
        private const string _folderNamePattern = @"\bsurgery-(?<id>[0-9]{1,11})\b";
        private const string _imageFileNamePattern = @"\b(?<hours>[0-9]{2})-(?<minutes>[0-9]{2})-(?<seconds>[0-9]{2})\b";

        public static bool IsValidSurgeryFolderName(string name)
        {
            return Regex.IsMatch(name, _folderNamePattern);
        }

        public static bool IsValidImageFileName(string fileName)
        {
            return Regex.IsMatch(fileName, _imageFileNamePattern);
        }

        public static IResult IsValidMetadataFile(ZipArchiveEntry metadata, string zipPath)
        {
            ICollection<string> messages = [];

            if (!TryGetSurgeryId(zipPath, out int surgeryId))
            {
                messages.Add($"Invalid surgery folder name");
                return new Result(false, null, messages);
            }

            using Stream file = metadata.Open();
            using StreamReader reader = new(file);
            string json = reader.ReadToEnd();

            return IsValidSurgeryJson(json, surgeryId, messages);
        }

        public static IResult IsValidMetadataFile(string metadata, string folderPath)
        {
            ICollection<string> messages = [];

            if (!TryGetSurgeryId(folderPath, out int surgeryId))
            {
                messages.Add($"Invalid surgery folder name");
                return new Result(false, null, messages);
            }

            string json = File.ReadAllText(metadata);

            return IsValidSurgeryJson(json, surgeryId, messages);
        }

        public static bool TryGetSurgeryId(string folderPath, out int surgeryId)
        {
            string folderName = Path.GetFileNameWithoutExtension(folderPath);
            string idText = Regex.Match(folderName, _folderNamePattern).Groups["id"].Value;

            if (!int.TryParse(idText, out surgeryId))
            {
                return false;
            }

            return true;
        }

        private static IResult IsValidSurgeryJson(string json, int surgeryId, ICollection<string> messages)
        {
            Surgery? surgery;

            try
            {
                surgery = JsonSerializer.Deserialize<Surgery>(json);
            }
            catch (JsonException exception)
            {
                messages.Add($"Exception deserializing metadata.json: {exception.Message}");
                return new Result(false, exception, messages);
            }

            if (surgery is null)
            {
                messages.Add("Error deserializing metadata.json.");
                return new Result(false, null, messages);
            }

            if (surgery.Id <= 0 || surgery.Id != surgeryId)
            {
                messages.Add("The metadata.json contains a surgery id that is different from the folder id.");
                return new Result(false, null, messages);
            }

            if (surgery.Type == Comuns.Enums.SurgeryType.None)
            {
                messages.Add("The metadata.json contains an invalid surgery type.");
                return new Result(false, null, messages);
            }

            if (!surgery.Type.IsDefined())
            {
                messages.Add("The metadata.json contains an invalid surgery type.");
                return new Result(false, null, messages);
            }

            if (surgery.Shots <= 0)
            {
                messages.Add("The metadata.json contains an invalid quantity of shots.");
                return new Result(false, null, messages);
            }

            if (surgery.Seconds <= 0)
            {
                messages.Add("The metadata.json contains an invalid quantity of seconds.");
                return new Result(false, null, messages);
            }

            if (surgery.Start == default)
            {
                messages.Add("The metadata.json contains an invalid start date.");
                return new Result(false, null, messages);
            }

            return Result.Successfull;
        }
    }
}
