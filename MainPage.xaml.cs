using System.Net.Http;
using AngleSharp.Html.Parser;

namespace German_Project
{
    public partial class MainPage : ContentPage
    {



        public Dictionary<DayOfWeek, List<DateTime>> ScheduleList;

        public MainPage()
        {
            ScheduleList = new();
            List<DateTime> mondaySchedule = new List<DateTime>
            {
                new DateTime(1, 1, 1, 8, 30, 0),
                new DateTime(1, 1, 1, 9, 15, 0),
                new DateTime(1, 1, 1, 9, 20, 0),
                new DateTime(1, 1, 1, 10, 50, 0),
                new DateTime(1, 1, 1, 11, 10, 0),
                new DateTime(1, 1, 1, 12, 40, 0),
                new DateTime(1, 1, 1, 12, 50, 0),
                new DateTime(1, 1, 1, 14, 20, 0),
                new DateTime(1, 1, 1, 14, 35, 0),
                new DateTime(1, 1, 1, 16, 5, 0),
                new DateTime(1, 1, 1, 16, 10, 0),
                new DateTime(1, 1, 1, 17, 40, 0),
                new DateTime(1, 1, 1, 17, 45, 0),
                new DateTime(1, 1, 1, 19, 15, 0)
            };
            ScheduleList.Add(DayOfWeek.Monday, mondaySchedule);

            for (DayOfWeek day = DayOfWeek.Tuesday; day <= DayOfWeek.Saturday; day++)
            {
                if (day != DayOfWeek.Sunday)
                {
                    List<DateTime> schedule = new List<DateTime>
                {
                    new DateTime(1, 1, 1, 8, 30, 0),
                    new DateTime(1, 1, 1, 10, 00, 0),
                    new DateTime(1, 1, 1, 10, 10, 0),
                    new DateTime(1, 1, 1, 11, 40, 0),
                    new DateTime(1, 1, 1, 12, 0, 0),
                    new DateTime(1, 1, 1, 13, 30, 0),
                    new DateTime(1, 1, 1, 14, 00, 0),
                    new DateTime(1, 1, 1, 15, 30, 0),
                    new DateTime(1, 1, 1, 15, 40, 0),
                    new DateTime(1, 1, 1, 17, 10, 0),
                    new DateTime(1, 1, 1, 17, 15, 0),
                    new DateTime(1, 1, 1, 18, 45, 0),
                };
                    ScheduleList.Add(day, schedule);
                }
            }

            InitializeComponent();
        }

        private void updateButton(object sender, EventArgs e)
        {
            DayOfWeek today = DateTimeOffset.Now.DayOfWeek;
            DateTime now = DateTimeOffset.Now.LocalDateTime;
            endsIn.Text = TimeUntilEndOfClass(today, now);
            //LoadAndParseData("3са3");
        }

        private string[] CustomSplit(string input)
        {
            // Находим индекс начала имени преподавателя
            int teacherIndex = input.LastIndexOf(' ') + 1;

            // Находим индекс начала названия аудитории
            int roomIndex = input.LastIndexOf('-', teacherIndex - 1) + 1;

            // Извлекаем информацию о предмете, преподавателе и аудитории
            string subject = input.Substring(0, roomIndex - 1).Trim();
            string teacher = input.Substring(roomIndex, teacherIndex - roomIndex).Trim();
            string room = input.Substring(teacherIndex).Trim();

            return new string[] { subject, teacher, room };
        }

        private async Task<List<List<string[]>>> LoadAndParseData(string group)
        {
            try
            {
                // URL сайта для парсинга
                string url = $"https://oksei.ru/studentu/raspisanie_uchebnykh_zanyatij?group={group}";

                // Создание HTTP-клиента
                using var httpClient = new HttpClient();

                // Получение HTML-страницы
                var response = await httpClient.GetAsync(url);
                var htmlContent = await response.Content.ReadAsStringAsync();

                // Создание парсера AngleSharp
                var parser = new HtmlParser();

                // Парсинг HTML-кода
                var document = parser.ParseDocument(htmlContent);
                // Извлечение данных из таблицы расписания
                var scheduleTable = document.QuerySelector("table.table");
                var rows = scheduleTable.QuerySelectorAll("tbody > tr");

                // Создание списка для хранения расписания
                var scheduleList = new List<List<string[]>>();

                // Итерация по строкам таблицы
                foreach (var row in rows)
                {
                    var lessonsOfDay = row.QuerySelectorAll("td > ol > li");

                    foreach (var lesson in lessonsOfDay)
                    {
                        Console.Write(lesson.TextContent, "///");
                    }
                    Console.WriteLine();
                }


                // Возвращение списка расписания
                return scheduleList;
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если таковые возникнут
                Console.WriteLine("Ошибка при загрузке и парсинге данных: " + ex.Message);
                return null;
            }
        }


        public string TimeUntilEndOfClass(DayOfWeek today, DateTime now)
        {

            if (ScheduleList.ContainsKey(today))
            {
                List<DateTime> schedule = ScheduleList[today];

                foreach (var classStartTime in schedule)
                {
                    if (classStartTime.TimeOfDay > now.TimeOfDay)
                    {
                        if (schedule.IndexOf(classStartTime) % 2 == 0) lessonName.Text = "Перемена";
                        TimeSpan timeUntilNextClass = classStartTime.TimeOfDay - now.TimeOfDay;
                        return $"{timeUntilNextClass.ToString(@"hh\:mm\:ss")} осталось";
                    }
                }

                lessonName.Text = "Пары закончились";
                return "";

            }
            lessonName.Text = "Сегодня пар нет";
            return "";

        }
    }
}
