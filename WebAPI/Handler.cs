using Comuns.Classes;
using Comuns.Extension;
using System.Globalization;
using System.Text.RegularExpressions;
using FileHandler.Services;
using FileManagementService.Services;

namespace WebAPI
{
    public static class Handler
    {
        public static SurgeryResult? GetSurgeryJson(string surgeryName)
        {
            Match match = Validator.MatchSurgeryFolderNamePattern(surgeryName);

            if (!match.Success)
            {
                return null;
            }

            int room = int.Parse(match.Groups["room"].Value);
            string startText = match.Groups["start"].Value;
            DateTime start = startText.GetStartFromSurgeryName();
            int year = start.Year;
            int month = start.Month;
            int day = start.Day;
            int hour = start.Hour;
            int minute = start.Minute;
            int second = start.Second;

            return GetSurgeryJson(room, year, month, day, hour, minute, second);
        }

        public static IEnumerable<SurgeryResult?> GetSurgeryJson(int room, int year, int month, int day)
        {
            string[] pathes = Librarian.ListSurgeriesAtGallery(room, year, month, day);

            foreach (string path in pathes)
            {
                string filename = Path.GetFileName(path);
                SurgeryResult? surgery = GetSurgeryJson(filename);
                yield return surgery;
            }
        }

        private static SurgeryResult? GetSurgeryJson(int room, int year, int month, int day, int hour, int minute, int second)
        {
            SurgeryResult? surgery = Librarian.GetSurgery(room, year, month, day, hour, minute, second);

            return surgery;
        }

        public static IDictionary<string, int>? GetSurgeryUsage(string surgeryName)
        {
            SurgeryResult? surgery = GetSurgeryJson(surgeryName);

            if (surgery == null)
            {
                return null;
            }

            return Librarian.GetSurgeryUsage(surgery);
        }

        internal static IEnumerable<IDictionary<string, int>>? GetSurgeryUsage(int room, int year, int month, int day)
        {
            string[] pathes = Librarian.ListSurgeriesAtGallery(room, year, month, day);

            foreach (string path in pathes)
            {
                string filename = Path.GetFileName(path);
                SurgeryResult? surgery = GetSurgeryJson(filename);

                if (surgery == null)
                {
                    continue;
                }

                IDictionary<string, int>? usage = Librarian.GetSurgeryUsage(surgery);

                if (usage == null)
                {
                    continue;
                }

                yield return usage;
            }
        }

        public static IDictionary<string, int>? GetSurgeryUsage(int room, int year, int month, int day, int hour, int minute, int second)
        {
            SurgeryResult? surgery = GetSurgeryJson(room, year, month, day, hour, minute, second);

            if (surgery == null)
            {
                return null;
            }

            return Librarian.GetSurgeryUsage(surgery);
        }
    }
}
