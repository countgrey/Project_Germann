using AngleSharp.Html.Parser;

namespace German_Project;

public partial class MainPage : ContentPage
{
    private int lessonTimeIndexNow = 0;
    public List<String[]> todayLessons;

    private Timer _timer;

    public DayOfWeek today;
    public DateTime now;

    public Dictionary<DayOfWeek, List<DateTime>> ScheduleList;

    [Obsolete]
    private async void TimerCallback(object state)
    {
        // Здесь можно разместить код, который нужно выполнить каждую секунду
        today = DateTimeOffset.Now.DayOfWeek;
        now = DateTimeOffset.Now.LocalDateTime;
        await Device.InvokeOnMainThreadAsync(() =>
        {
            endsIn.Text = TimeUntilEndOfClass(today, now);

            for (int i = 0; i < MainStackForSchedule.Count; i++)
            {
                ((Frame)MainStackForSchedule.Children[i]).BackgroundColor = Color.FromHex("#502bd4");
            }

            if (lessonTimeIndexNow/2 - 2 >= 0 && lessonTimeIndexNow / 2 - 2 < MainStackForSchedule.Children.Count)
                ((Frame)MainStackForSchedule.Children[lessonTimeIndexNow/2 - 2]).BackgroundColor = Color.FromHex("#802bd4");
            //Console.WriteLine(lessonTimeIndexNow);
        });
    }

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
        // Создаем таймер с интервалом в 1 секунду
        _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));


        string group = Preferences.Get("group", "no");

        if (group != "no")
            UpdateSchedule(group);
    }


    

    private async void updateButton(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Введите название группы", "Название группы:");
        if (!string.IsNullOrWhiteSpace(result))
        {
            //в кеш
            Preferences.Set("group", result);

            MainStackForSchedule.Clear();
            UpdateSchedule(result);
        }
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
                var lessonsOfDayString = row.QuerySelectorAll("td > ol > li");
                List<string[]> lessonsOfDayList = new List<string[]>();
                
                for (int lessonIndex = 0;  lessonIndex < lessonsOfDayString.Length; lessonIndex++)
                {
                    // Разделяем строку на слова
                    string[] words =lessonsOfDayString[lessonIndex].TextContent.Trim().Split(' ');

                    // Получаем название пары
                    string lessonName = string.Join(" ", words.Take(words.Length - 5));

                    // Получаем преподавателя
                    string teacher = string.Join(" ", words.Skip(words.Length - 5).Take(2));

                    // Получаем аудиторию
                    string classroom = string.Join(" ", words.Skip(words.Length - 3)).Replace("Аудитория", "");

                    

                    string[] lesson = [(lessonIndex+1).ToString(),
                        lessonName, teacher, classroom];
                    lessonsOfDayList.Add(lesson);
                }
                
                scheduleList.Add(lessonsOfDayList);
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
                    lessonTimeIndexNow = schedule.IndexOf(classStartTime);

                    if (lessonTimeIndexNow % 2 == 0) lessonName.Text = "Перемена";
                    else if (todayLessons != null)
                    {
                        if (lessonTimeIndexNow / 2 < todayLessons.Count)
                        {
                            lessonName.Text = $"{todayLessons[lessonTimeIndexNow/2][1]}";
                        }

                        else lessonName.Text = "bimbimbambam";
                    }
                    TimeSpan timeUntilNextClass = classStartTime.TimeOfDay - now.TimeOfDay;
                    return $"{timeUntilNextClass:hh\\:mm\\:ss} осталось";
                }

            }

            lessonName.Text = "Пары закончились";
            return "";

        }
        lessonName.Text = "Сегодня пар нет";
        return "";

    }

    public async void UpdateSchedule(string group)
    {
        List<List<string[]>> lessonsOfWeek = await LoadAndParseData(group);
        todayLessons = lessonsOfWeek[(int)today-1];
        // Удаление пустых элементов с конца списка
        while (todayLessons.Last()[1] == "")
        {
            todayLessons.RemoveAt(todayLessons.Count - 1);
        }

        // Удаление пустых элементов с начала

        for (int les = 0; les < todayLessons.Count; les++)
        {
            if (todayLessons[les][1] == "") todayLessons.RemoveAt(les);
        }

        //var tomorrowLessons = lessonsOfWeek[1];

        foreach (var lesson in todayLessons)
        {
            if (lesson[1] != "")
            {
                // Получение времени начала и окончания пары
                DateTime lessonStart = ScheduleList[today][(int.Parse(lesson[0]) * 2) - 2];
                DateTime lessonEnd = ScheduleList[today][(int.Parse(lesson[0]) * 2) - 1];

                // Форматирование времени в строку в нужном формате
                string lessonTime = lessonStart.ToString("HH:mm") + " - " + lessonEnd.ToString("HH:mm");

                // Создание рамки для отдельного урока
                Frame lessonFrame = new Frame
                {
                    CornerRadius = 15,
                    Margin = new Thickness(0, 0, 0, 0),
                    BackgroundColor = Color.FromHex("#502bd4"),
                    BorderColor = Color.FromHex("#502bd4")
                };

                // Создание стека для урока
                StackLayout lessonStack = new StackLayout();

                // Добавление индекса и названия урока
                lessonStack.Children.Add(new Label
                {
                    Text = lesson[0] + ". " + lesson[1], // Индекс и название
                    FontFamily = "Open Sans MS",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#f0f8ff"),
                    Margin = new Thickness(0, 0, 0, 0)
                });

                // Добавление преподавателя и аудитории
                lessonStack.Children.Add(new Label
                {
                    Text = lesson[2] + ", " + lesson[3], // Преподаватель и аудитория
                    FontFamily = "Open Sans MS",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#a9a9a9")
                });

                // Добавление времени пары
                lessonStack.Children.Add(new Label
                {
                    Text = lessonTime, // Время пары
                    FontFamily = "Open Sans MS",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#a9a9a9")
                });

                // Установка содержимого рамки
                lessonFrame.Content = lessonStack;

                // Добавление рамки в основной стек расписания
                MainStackForSchedule.Children.Add(lessonFrame);

                // Проверяем, не последний ли это урок для группы
                if (lesson != todayLessons.Last())
                {
                    // Добавление рамки с информацией о перерыве только если это не последний урок для группы

                    // Добавление рамки с информацией о перерыве
                    Frame breakFrame = new Frame
                    {
                        BackgroundColor = Color.FromHex("#502bd4"),
                        CornerRadius = 15,
                        BorderColor = Color.FromHex("#502bd4"),
                        Padding = new Thickness(20, 0, 0, 0),
                        Margin = new Thickness(0, 0, 5, 0)
                    };

                    // Получение времени начала и окончания перемены
                    DateTime breakStart = ScheduleList[today][(int.Parse(lesson[0]) * 2) - 1];
                    DateTime breakEnd = ScheduleList[today][(int.Parse(lesson[0]) * 2)];

                    // Вычисление продолжительности перемена в минутах
                    int breakDurationMinutes = (int)(breakEnd - breakStart).TotalMinutes;

                    // Создание строки с использованием интерполяции строк
                    string breakText = $"Перемена {breakDurationMinutes} мин";

                    breakFrame.Content = new Label
                    {
                        Text = breakText, // Информация о перерыве
                        FontFamily = "Open Sans MS",
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromHex("#a9a9a9")
                    };

                    // Добавление рамки с перерывом в основной стек расписания
                    MainStackForSchedule.Children.Add(breakFrame);
                }
            }
        }
    }
}
