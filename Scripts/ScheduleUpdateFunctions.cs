using AngleSharp.Html.Parser;

namespace German_Project;

public class ScheduleElement
{
    public int Index { get; set; }
    public DateTime Beginning { get; set; }
    public DateTime Ending { get; set; }
    public TimeSpan Duration { get; set; }
}

public class Lesson : ScheduleElement
{
    public string Name { get; set; }
    public string Teacher { get; set; }
    public string Room { get; set; }
}

public class Break : ScheduleElement
{
    public Break(int index)
    {
        Index = index;
    }
}

public class LessonsForDay
{
    public List<ScheduleElement> LessonsAndBreaks { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    public void AddTimeForLessons(DayOfWeek dayOfWeek)
    {
        DayOfWeek = dayOfWeek;
        if (DayOfWeek == DayOfWeek.Monday)
        {
            LessonsAndBreaks[0].Beginning = new DateTime(1, 1, 1, 8, 30, 0);
            LessonsAndBreaks[0].Ending = new DateTime(1, 1, 1, 9, 15, 0);
            LessonsAndBreaks[1].Beginning = new DateTime(1, 1, 1, 9, 15, 0);
            LessonsAndBreaks[1].Ending = new DateTime(1, 1, 1, 9, 20, 0);
            LessonsAndBreaks[2].Beginning = new DateTime(1, 1, 1, 9, 20, 0);
            LessonsAndBreaks[2].Ending = new DateTime(1, 1, 1, 10, 50, 0);
            LessonsAndBreaks[3].Beginning = new DateTime(1, 1, 1, 10, 50, 0);
            LessonsAndBreaks[3].Ending = new DateTime(1, 1, 1, 11, 10, 0);
            LessonsAndBreaks[4].Beginning = new DateTime(1, 1, 11, 10, 0, 0);
            LessonsAndBreaks[4].Ending = new DateTime(1, 1, 1, 12, 40, 0);
            LessonsAndBreaks[5].Beginning = new DateTime(1, 1, 1, 12, 40, 0);
            LessonsAndBreaks[5].Ending = new DateTime(1, 1, 1, 12, 50, 0);
            LessonsAndBreaks[6].Beginning = new DateTime(1, 1, 1, 12, 50, 0);
            LessonsAndBreaks[6].Ending = new DateTime(1, 1, 1, 14, 20, 0);
            LessonsAndBreaks[7].Beginning = new DateTime(1, 1, 1, 14, 20, 0);
            LessonsAndBreaks[7].Ending = new DateTime(1, 1, 1, 14, 35, 0);
            LessonsAndBreaks[8].Beginning = new DateTime(1, 1, 1, 14, 35, 0);
            LessonsAndBreaks[8].Ending = new DateTime(1, 1, 1, 16, 05, 0);
            LessonsAndBreaks[9].Beginning = new DateTime(1, 1, 1, 16, 05, 0);
            LessonsAndBreaks[9].Ending = new DateTime(1, 1, 1, 16, 10, 0);
            LessonsAndBreaks[10].Beginning = new DateTime(1, 1, 1, 16, 10, 0);
            LessonsAndBreaks[10].Ending = new DateTime(1, 1, 1, 17, 40, 0);
            LessonsAndBreaks[11].Beginning = new DateTime(1, 1, 1, 17, 40, 0);
            LessonsAndBreaks[11].Ending = new DateTime(1, 1, 1, 17, 45, 0);
            LessonsAndBreaks[12].Beginning = new DateTime(1, 1, 1, 17, 45, 0);
            LessonsAndBreaks[12].Ending = new DateTime(1, 1, 1, 19, 15, 0);
        }

        else
        {
            LessonsAndBreaks[0].Beginning = new DateTime(1, 1, 1, 8, 30, 0);
            LessonsAndBreaks[0].Ending = new DateTime(1, 1, 1, 10, 0, 0);
            LessonsAndBreaks[1].Beginning = new DateTime(1, 1, 1, 10, 0, 0);
            LessonsAndBreaks[1].Ending = new DateTime(1, 1, 1, 10, 10, 0);
            LessonsAndBreaks[2].Beginning = new DateTime(1, 1, 1, 10, 10, 0);
            LessonsAndBreaks[2].Ending = new DateTime(1, 1, 1, 11, 40, 0);
            LessonsAndBreaks[3].Beginning = new DateTime(1, 1, 1, 11, 40, 0);
            LessonsAndBreaks[3].Ending = new DateTime(1, 1, 1, 12, 0, 0);
            LessonsAndBreaks[4].Beginning = new DateTime(1, 1, 1, 12, 0, 0);
            LessonsAndBreaks[4].Ending = new DateTime(1, 1, 1, 13, 30, 0);
            LessonsAndBreaks[5].Beginning = new DateTime(1, 1, 1, 13, 30, 0);
            LessonsAndBreaks[5].Ending = new DateTime(1, 1, 1, 14, 0, 0);
            LessonsAndBreaks[6].Beginning = new DateTime(1, 1, 1, 14, 0, 0);
            LessonsAndBreaks[6].Ending = new DateTime(1, 1, 1, 15, 30, 0);
            LessonsAndBreaks[7].Beginning = new DateTime(1, 1, 1, 15, 30, 0);
            LessonsAndBreaks[7].Ending = new DateTime(1, 1, 1, 15, 40, 0);
            LessonsAndBreaks[8].Beginning = new DateTime(1, 1, 1, 15, 40, 0);
            LessonsAndBreaks[8].Ending = new DateTime(1, 1, 1, 17, 10, 0);
            LessonsAndBreaks[9].Beginning = new DateTime(1, 1, 1, 17, 10, 0);
            LessonsAndBreaks[9].Ending = new DateTime(1, 1, 1, 17, 15, 0);
            LessonsAndBreaks[10].Beginning = new DateTime(1, 1, 1, 17, 15, 0);
            LessonsAndBreaks[10].Ending = new DateTime(1, 1, 1, 18, 45, 0);

        }
        foreach (var lesson in LessonsAndBreaks)
        {
            lesson.Duration = lesson.Ending.TimeOfDay - lesson.Beginning.TimeOfDay;
        }
    }
}


public class ScheduleUpdateFunctions
{
    private static DayOfWeek[] DaysOfWorkingWeek = { DayOfWeek.Monday,
                                                     DayOfWeek.Tuesday,
                                                     DayOfWeek.Wednesday,
                                                     DayOfWeek.Thursday,
                                                     DayOfWeek.Friday,
                                                     DayOfWeek.Saturday };

    private static DateTime[] TimeSchedule;

    // Парсинг расписания с сайта шараги
    public async static Task<List<LessonsForDay>> ParseScheduleForWeek(string group)
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
        var scheduleListForWeek = new List<LessonsForDay>();

        // Итерация по строкам таблицы
        for (int day = 0; day < 6; day++)
        {
            var lessonsOfDayString = rows[day].QuerySelectorAll("td > ol > li");
            LessonsForDay lessonsForDay = new LessonsForDay();
            lessonsForDay.LessonsAndBreaks = new();
            var currentDay = DaysOfWorkingWeek[day];


            if (currentDay == DayOfWeek.Monday)
            {
                Lesson shittyLesson = new Lesson();
                shittyLesson.Name = "Разговры о важном";
                shittyLesson.Room = string.Join(" ", lessonsOfDayString[0].TextContent.Trim().Split(' ').Skip(lessonsOfDayString[0].TextContent.Trim().Split(' ').Length - 3)).Replace("Аудитория", "");
                shittyLesson.Index = 0;
                lessonsForDay.LessonsAndBreaks.Add(shittyLesson);
                lessonsForDay.LessonsAndBreaks.Add(new Break(-1));
            }

            for (int lessonIndex = 0; lessonIndex < lessonsOfDayString.Length; lessonIndex++)
            {
                Lesson lesson = new Lesson();
                // Разделяем строку на слова
                string[] words = lessonsOfDayString[lessonIndex].TextContent.Trim().Split(' ');

                lesson.Index = lessonIndex + 1;

                // Получаем название пары
                lesson.Name = string.Join(" ", words.Take(words.Length - 5));

                // Получаем преподавателя
                lesson.Teacher = string.Join(" ", words.Skip(words.Length - 5).Take(2));

                // Получаем аудиторию
                lesson.Room = string.Join(" ", words.Skip(words.Length - 3)).Replace("Аудитория", "");

                lesson.Duration = new TimeSpan();

                lessonsForDay.LessonsAndBreaks.Add(lesson);
                if (lessonIndex < lessonsOfDayString.Length - 1)
                    lessonsForDay.LessonsAndBreaks.Add(new Break((lessonIndex+1)*-1));
            }
            lessonsForDay.AddTimeForLessons(currentDay);

            for (int i = lessonsForDay.LessonsAndBreaks.Count - 1; i >= 0; i--)
            {
                if (lessonsForDay.LessonsAndBreaks[i] is Lesson lesson && lesson.Room.Length < 6)
                {
                    lessonsForDay.LessonsAndBreaks.RemoveAt(i); // Удаляем урок

                    // Проверяем, что после удаления урока есть элемент с индексом i
                    // И что этот элемент - это перемена (Break)
                    if (i < lessonsForDay.LessonsAndBreaks.Count && lessonsForDay.LessonsAndBreaks[i] is Break)
                    {
                        lessonsForDay.LessonsAndBreaks.RemoveAt(i); // Удаляем перемену
                    }
                }
            }

            if (lessonsForDay.LessonsAndBreaks.Last() is Break)
            {
                lessonsForDay.LessonsAndBreaks.RemoveAt(lessonsForDay.LessonsAndBreaks.Count - 1);
            }

            scheduleListForWeek.Add(lessonsForDay);
        }



        // Возвращение списка расписания
        return scheduleListForWeek;

    }
}
