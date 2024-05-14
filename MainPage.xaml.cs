namespace German_Project;

public partial class MainPage : ContentPage
{

    private Timer _timer;

    public DayOfWeek today = DayOfWeek.Monday;
    public DateTime now = DateTime.MaxValue;

    public List<LessonsForDay> schduleForWeek;
    public LessonsForDay todayLessons;
    public ScheduleElement lessonOrBreakNow;

    public bool LessonsEndToday = false;


    private void updateNowLesson()
    {
        if (schduleForWeek != null)
        {
            todayLessons = schduleForWeek[(int)today - 1];
            foreach (var lessonOrBreak in todayLessons.LessonsAndBreaks)
            {
                if (now.TimeOfDay < lessonOrBreak.Ending.TimeOfDay && now.TimeOfDay >= lessonOrBreak.Beginning.TimeOfDay)
                {
                    lessonOrBreakNow = lessonOrBreak;
                    break;
                }
                else
                {
                    lessonOrBreakNow = new Lesson() { Index = todayLessons.LessonsAndBreaks[0].Index,
                                                      Room = ((Lesson)todayLessons.LessonsAndBreaks[0]).Room,
                                                      Ending = todayLessons.LessonsAndBreaks[0].Beginning,
                                                      Name = "Пары еще не начались"};
                }  
            }

            LessonsEndToday = todayLessons.LessonsAndBreaks.Last().Ending.TimeOfDay < now.TimeOfDay;

        }
        
    }

    [Obsolete]
    private async void TimerCallback(object state)
    {
        // Здесь можно разместить код, который нужно выполнить каждую секунду

        today = DateTimeOffset.Now.DayOfWeek;
        now = DateTimeOffset.Now.LocalDateTime;
        
        updateNowLesson();
        


        await Device.InvokeOnMainThreadAsync(() =>
        {
            //lessonName.Text = $"{lessonOrBreakNow.Index}";
            if (lessonOrBreakNow != null)
            {
                if (!LessonsEndToday)
                {
                    endsIn.Text = $"Осталось {lessonOrBreakNow.Ending.TimeOfDay - now.TimeOfDay:hh\\:mm\\:ss}";
                    if (lessonOrBreakNow is Break)
                        lessonName.Text = $"Перемена {((Lesson)todayLessons.LessonsAndBreaks[todayLessons.LessonsAndBreaks.IndexOf(lessonOrBreakNow) + 1]).Room}";
                    else 
                        lessonName.Text = $"{lessonOrBreakNow.Index}. {((Lesson)lessonOrBreakNow).Name}. {((Lesson)lessonOrBreakNow).Room}";
                }
                else lessonName.Text = "Пары на сегодня закончились";

                foreach (var lesson in MainStackForSchedule.Children)
                {
                    ((Frame)lesson).BackgroundColor = Color.FromHex("#502bd4");
                }
                
                if (todayLessons.LessonsAndBreaks.Contains(lessonOrBreakNow))
                    ((Frame)MainStackForSchedule.Children[todayLessons.LessonsAndBreaks.IndexOf(lessonOrBreakNow)]).BackgroundColor = Color.FromHex("#802bd4");

            }
        });
    }

    public MainPage()
    {
        InitializeComponent();
        // Создаем таймер с интервалом в 1 секунду
        _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        
        string group = Preferences.Get("group", "no");
        if (group != "no")
            updateSchedule(group);
    }


    private async void updateButton(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Введите название группы", "Название группы:");
        if (!string.IsNullOrWhiteSpace(result))
        {
            //в кеш
            Preferences.Set("group", result);
            updateSchedule(result);
        }

    }


    private async void updateSchedule(string group)
    {
        MainStackForSchedule.Clear();

        schduleForWeek = await ScheduleUpdateFunctions.ParseScheduleForWeek(group);
        updateNowLesson();

        renderSchedule();

    }

    public void renderSchedule()
    {
        foreach (var lesson in todayLessons.LessonsAndBreaks)
        {
            // Получение времени начала и окончания пары
            DateTime lessonStart = lesson.Beginning;
            DateTime lessonEnd = lesson.Ending;

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

            if (lesson.Index < 0) // Перемена
            {
                lessonStack.Children.Add(new Label
                {
                    Text = $"Перемена {lesson.Duration:mm} мин",
                    FontFamily = "Open Sans MS",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#f0f8ff"),
                    Margin = new Thickness(0, 0, 0, 0)
                });
            }

            else if (lesson.Index >= 0)
            {
                // Добавление индекса и названия урока
                lessonStack.Children.Add(new Label
                {
                    Text = $"{lesson.Index}. {((Lesson)lesson).Name}", // Индекс и название
                    FontFamily = "Open Sans MS",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#f0f8ff"),
                    Margin = new Thickness(0, 0, 0, 0)
                });

                // Добавление преподавателя и аудитории
                lessonStack.Children.Add(new Label
                {
                    Text = ((Lesson)lesson).Teacher + ", " + ((Lesson)lesson).Room, // Преподаватель и аудитория
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


            }

            // Добавление рамки в основной стек расписания
            lessonFrame.Content = lessonStack;
            MainStackForSchedule.Children.Add(lessonFrame);

        }
    }
}
