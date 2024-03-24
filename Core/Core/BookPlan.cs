    public class BookPlan
    {
        public int Id { get;}
        public int UserId { get;}
        public string DeadLine { get;}
        public int DayOfWeek { get;}    //ne viena ?
        public string timeOfDay {  get;}    
        public int PagesPerDay { get; set;}
        public string Title { get; }
        public string Author { get; }
        public int PageCount { get; }
        public int Size { get; }

        public BookPlan(int id, int userId, string deadLine, int dayOfWeek, string timeOfDay, int pagesPerDay, 
            string title, string author, int pageCount, int size)
        {
            Id = id;
            UserId = userId;
            DeadLine = deadLine;
            DayOfWeek = dayOfWeek;
            this.timeOfDay = timeOfDay;
            PagesPerDay = pagesPerDay;
            Title = title;
            Author = author;
            PageCount = pageCount;
            Size = size;
        }

        public void PagesToReadBeforeDeadline()
    {
        DateTime deadline = DateTime.Parse(DeadLine);
        TimeSpan timeLeft = deadline - DateTime.Now;

        if (timeLeft.Days < 0)
        {
            PagesPerDay = 0;
            return;
        }
        int daysLeft = 0;
        for (int i = 0; i < timeLeft.Days; i++)
        {
            int dayOfWeek = (int)deadline.AddDays(i).DayOfWeek;
            bool[] weekdays = Weekdays.FromBitField(DayOfWeek);
            if (weekdays[dayOfWeek])
            {
                daysLeft++;
            }
        }
        if (daysLeft > 0)
        {
            PagesPerDay = PageCount / daysLeft;
        }
        else
        {
            PagesPerDay = 0;
        }
    }
        
    }
