using Comuns.Classes;
using Comuns.Extension;
using Comuns.Interfaces;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private const string _folderNamePattern = @"\bsurgery-(?<room>[0-9]{1,11})-(?<start>[0-9]{14})\b";
        private const string _imageFileNamePattern = @"\b(?<hours>[0-9]{2})-(?<minutes>[0-9]{2})-(?<seconds>[0-9]{2})\b";

        public static Match MatchSurgeryFolderNamePattern(string name) => Regex.Match(name, _folderNamePattern);
        public static Match MatchImageFileNamePattern(string name) => Regex.Match(name, _imageFileNamePattern);

        public static bool IsValidSurgeryFolderName(string name)
        {
            if (!Regex.IsMatch(name, _folderNamePattern)) return false;
            if (!TryGetSurgeryRoom(name, out int room)) return false;
            if (!TryGetSurgeryStart(name, out DateTime start)) return false;
            return true;
        }

        public static bool IsValidImageFileName(string fileName)
        {
            return Regex.IsMatch(fileName, _imageFileNamePattern);
        }

        public static IResult IsValidMetadataFile(ZipArchiveEntry metadata, string zipPath)
        {
            ICollection<string> messages = [];

            if (!TryGetSurgeryRoom(zipPath, out int room))
            {
                messages.Add($"Invalid room in surgery folder name");
                return new Result(false, null, messages);
            }

            if (!TryGetSurgeryStart(zipPath, out DateTime start))
            {
                messages.Add($"Invalid start in surgery folder name");
                return new Result(false, null, messages);
            }

            using Stream file = metadata.Open();
            using StreamReader reader = new(file);
            string json = reader.ReadToEnd();

            return IsValidSurgeryJson(json, room, start, messages);
        }

        public static IResult IsValidMetadataFile(string metadata, string folderPath)
        {
            ICollection<string> messages = [];

            if (!TryGetSurgeryRoom(folderPath, out int room))
            {
                messages.Add($"Invalid room in surgery folder name");
                return new Result(false, null, messages);
            }

            if (!TryGetSurgeryStart(folderPath, out DateTime start))
            {
                messages.Add($"Invalid start in surgery folder name");
                return new Result(false, null, messages);
            }

            string json = File.ReadAllText(metadata);

            return IsValidSurgeryJson(json, room, start, messages);
        }

        public static bool TryGetSurgeryRoom(string folderPath, out int room)
        {
            string folderName = Path.GetFileNameWithoutExtension(folderPath);
            string roomText = Regex.Match(folderName, _folderNamePattern).Groups["room"].Value;

            return int.TryParse(roomText, out room);
        }

        public static bool TryGetSurgeryStart(string folderPath, out DateTime start)
        {
            string folderName = Path.GetFileNameWithoutExtension(folderPath);
            string startText = Regex.Match(folderName, _folderNamePattern).Groups["start"].Value;

            return DateTime.TryParseExact(startText, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out start);
        }

        private static IResult IsValidSurgeryJson(string json, int room, DateTime start, ICollection<string> messages)
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

            if (surgery.Room <= 0)
            {
                messages.Add("The metadata.json contains a surgery id that is different from the folder id.");
                return new Result(false, null, messages);
            }

            if (surgery.Room != room)
            {
                messages.Add("The metadata.json contains a surgery room that is different from the folder name.");
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

            if (surgery.Start != start)
            {
                messages.Add("The metadata.json contains a start date that is different from the folder name.");
                return new Result(false, null, messages);
            }

            return Result.Successfull;
        }

        public static IResult IsValidImageFile(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            if (!IsValidImageFileName(fileName))
            {
                return new Result(false, null, ["Invalid image filename."]);
            }

            Match match = Regex.Match(fileName, _imageFileNamePattern);

            if (!match.Success)
            {
                return new Result(false, null, ["Invalid image filename."]);
            }

            string hoursText = match.Groups["hours"].Value;
            string minutesText = match.Groups["minutes"].Value;
            string secondsText = match.Groups["seconds"].Value;

            if (!uint.TryParse(hoursText, out uint hours))
            {
                return new Result(false, null, ["The image filename does not contain a valid hour indicator."]);
            }

            if (hours < 0)
            {
                return new Result(false, null, ["The image filename does not contain a valid hour indicator."]);
            }

            if (!uint.TryParse(minutesText, out uint minutes))
            {
                return new Result(false, null, ["The image filename does not contain a valid minute indicator."]);
            }

            if (minutes < 0 || minutes > 59)
            {
                return new Result(false, null, ["The image filename does not contain a valid minute indicator."]);
            }

            if (!uint.TryParse(secondsText, out uint seconds))
            {
                return new Result(false, null, ["The image filename does not contain a valid second indicator."]);
            }

            if (seconds < 0 || seconds > 59)
            {
                return new Result(false, null, ["The image filename does not contain a valid second indicator."]);
            }

            return Result.Successfull;
        }

        public static DateTime GetSurgeryDateFromFolderName(string entry)
        {
            string entryName = Path.GetFileNameWithoutExtension(entry);
            Match match = MatchSurgeryFolderNamePattern(entryName);

            string startText = match.Groups["start"].Value;

            return DateTime.ParseExact(startText, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        internal static int GetSurgeryRoomFromFolderName(string entry)
        {
            string entryName = Path.GetFileNameWithoutExtension(entry);
            Match match = MatchSurgeryFolderNamePattern(entryName);

            string roomText = match.Groups["room"].Value;

            return int.Parse(roomText);
        }
    }
}