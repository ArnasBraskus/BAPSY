namespace Core
{
    public class BookPlan
    {
        public int Id { get;}
        public int UserId { get;}
        public DateTime DeadLine { get;}
        public int DayOfWeek { get;}
        public int PagesPerDay { get;}

        public BookPlan(int id, int userId, DateTime deadLine, int dayOfWeek)
        {
            Id = id;
            UserId = userId;
            DeadLine = deadLine;
            DayOfWeek = dayOfWeek;
        }

        public BookPlan(int id, int userId, DateTime deadLine, int dayOfWeek, int pagesPerDay) : this(id, userId, deadLine, dayOfWeek)
        {
            PagesPerDay = pagesPerDay;
        }
    }
}
