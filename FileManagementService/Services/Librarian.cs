using Comuns.Classes;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileManagementService.Services
{
    public static class Librarian
    {
        public static SurgeryResult? GetSurgery(int room, int year, int month, int day, int hour, int minute, int second)
        {
            string path = Path.Combine(
                Supervisor.GalleryPath,
                $"{room}",
                $"{year}",
                $"{month}",
                $"{day}",
                $"surgery-{room}-{year}{month:D2}{day:D2}{hour:D2}{minute:D2}{second:D2}.zip");

            string content = string.Empty;

            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                ZipArchiveEntry? entry = archive.GetEntry("results.json");
                if (entry is null) return null;

                using (StreamReader reader = new StreamReader(entry.Open()))
                {
                    content = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(content)) return null;

            try
            {
                return JsonSerializer.Deserialize<SurgeryResult>(content);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public static string[] ListSurgeriesAtGallery(int room, int year, int month, int day)
        {
            string path = Path.Combine(
                Supervisor.GalleryPath,
                $"{room}",
                $"{year}",
                $"{month}",
                $"{day}");

            return Directory.GetFiles(path);
        }

        public static string CreateSurgeryFolder(string root, int room, DateTime start)
        {
            string path = CreateFolder(root, room, start);
            string content = CreateMetadata(path, room, start);
            
            File.WriteAllText(Path.Combine(path, "metadata.json"), content);

            return path;
        }

        public static string CreateFolder(string root, int room, DateTime start)
        {
            string path = Path.Combine(root, $"surgery-{room}-{start:yyyyMMddHHmmss}");
            Directory.CreateDirectory(path);
            return path;
        }

        public static string CreateMetadata(string path, int room, DateTime start)
        {
            Surgery surgery = new Surgery
            {
                Room = room,
                Start = start
            };

            return JsonSerializer.Serialize(surgery);
        }

        public static void UpdateMetadata(string path, int shots, int seconds)
        {
            string content = File.ReadAllText(path);
            Surgery? surgery = JsonSerializer.Deserialize<Surgery>(content);

            if (surgery is null) return;

            surgery.Shots = (uint)shots;
            surgery.Seconds = (uint)seconds;
            
            string newContent = JsonSerializer.Serialize(surgery);
            File.WriteAllText(path, newContent);
        }

        public static void MoveToQueue(string path, string queuePath)
        {
            string name = Path.GetFileName(path);
            string destination = Path.Combine(queuePath, name);
            Directory.Move(path, destination);
        }

        public static void MoveToGallery(string entry, int room, DateTime date, string galleryPath)
        {
            string zipped = Librarian.ZipFolder(entry);
            string name = Path.GetFileName(zipped);
            string destinationFolder = Path.Combine(galleryPath, $"{room}", $"{date.Year}", $"{date.Month}", $"{date.Day}");
            Directory.CreateDirectory(destinationFolder);
            string destination = Path.Combine(destinationFolder, name);
            Directory.Move(zipped, destination);
        }

        public static string ZipFolder(string path)
        {
            string name = Path.GetFileName(path);
            string directory = Path.GetDirectoryName(path) ?? "";
            string destination = Path.Combine(directory, $"{name}.zip");
            ZipFile.CreateFromDirectory(path, destination);
            Directory.Delete(path, true);
            return destination;
        }

        public static IDictionary<string, int> GetSurgeryUsage(SurgeryResult surgeryResult)
        {
            PictureResult firstPicture = surgeryResult.Timeline.First();
            IDictionary<string, int> firstGrouping = new Dictionary<string, int>();
            if (firstPicture is not null)
            {
                ICollection<Prediction> detections = firstPicture.Detections;
                if (detections.Count() > 0)
                {
                    firstGrouping = detections.GroupBy(detection => detection.Name)
                                              .Select(group => (group.First().Name, group.Count()))
                                              .ToDictionary();
                }
            }

            PictureResult lastPicture = surgeryResult.Timeline.Last();
            IDictionary<string, int> lastGrouping = new Dictionary<string, int>();
            if (lastPicture is not null)
            {
                ICollection<Prediction> detections = lastPicture.Detections;
                if (detections.Count() > 0)
                {
                    lastGrouping = detections.GroupBy(detection => detection.Name)
                                             .Select(group => (group.First().Name, group.Count()))
                                             .ToDictionary();
                }
            }

            return CalculateDelta(firstGrouping, lastGrouping);
        }

        public static string SummarizeSurgeryResult(SurgeryResult surgeryResult)
        {
            StringBuilder summary = new StringBuilder();
            summary.AppendLine("Descrição da cirurgia");
            summary.AppendLine($"Sala: {surgeryResult.Surgery.Room}");
            summary.AppendLine($"Início: {surgeryResult.Surgery.Start}");
            summary.AppendLine($"Duração: {surgeryResult.Surgery.Seconds} segundos");
            summary.AppendLine($"Fotos: {surgeryResult.Surgery.Shots}");
            summary.AppendLine("");
            summary.AppendLine("Materiais detectados no início:");

            PictureResult firstPicture = surgeryResult.Timeline.First();
            IDictionary<string, int> firstGrouping = new Dictionary<string, int>();
            if (firstPicture is not null)
            {
                ICollection<Prediction> detections = firstPicture.Detections;
                if (detections.Count() > 0)
                {
                    firstGrouping = detections.GroupBy(detection => detection.Name)
                                              .Select(group => (group.First().Name, group.Count()))
                                              .ToDictionary();
                    foreach ((string name, int count) in firstGrouping)
                    {
                        summary.AppendLine($"{name}: ({count})");
                    }
                }
            }

            summary.AppendLine("");
            summary.AppendLine("Materiais detectados no final:");
            PictureResult lastPicture = surgeryResult.Timeline.Last();
            IDictionary<string, int> lastGrouping = new Dictionary<string, int>();
            if (lastPicture is not null)
            {
                ICollection<Prediction> detections = lastPicture.Detections;
                if (detections.Count() > 0)
                {
                    lastGrouping = detections.GroupBy(detection => detection.Name)
                                             .Select(group => (group.First().Name, group.Count()))
                                             .ToDictionary();
                    foreach ((string name, int count) in lastGrouping)
                    {
                        summary.AppendLine($"{name}: ({count})");
                    }
                }
            }

            summary.AppendLine("");
            summary.AppendLine("Diferença:");
            IDictionary<string, int> delta = CalculateDelta(firstGrouping, lastGrouping);
            foreach ((string name, int count) in delta)
            {
                if (count < 0)
                {
                    summary.AppendLine($"{count} {name} removidos da bandeja.");
                }
                else
                {
                    summary.AppendLine($"{count} {name} adicionados na bandeja.");
                }
            }

            return summary.ToString();
        }

        private static IDictionary<string, int> CalculateDelta(IDictionary<string, int> firstGrouping, IDictionary<string, int> lastGrouping)
        {
            IDictionary<string, int> result = new Dictionary<string, int>();

            IDictionary<string, int> onlyFirst = firstGrouping.Where(x => !lastGrouping.ContainsKey(x.Key))
                                                             .ToDictionary();
            foreach((string name, int count) in onlyFirst)
            {
                result.Add(name, -count);
            }

            IDictionary<string, int> onlyLast = lastGrouping.Where(x => !firstGrouping.ContainsKey(x.Key))
                                                            .ToDictionary();
            foreach((string name, int count) in onlyLast)
            {
                result.Add(name, count);
            }

            IDictionary<string, int> both = firstGrouping.Where(x => lastGrouping.ContainsKey(x.Key))
                                                         .ToDictionary();
            foreach((string name, int count) in both)
            {
                result.Add(name, lastGrouping[name] - count);
            }

            return result;
        }
    }
}
