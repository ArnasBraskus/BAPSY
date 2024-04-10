using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Interfaces;
using System.Numerics;


namespace CoreTests
{
    public class PlansTests
    {

        [Theory]
        [MemberData(nameof(PlansTestsUtils.GetTestPlansFromPopulatedDb), MemberType = typeof(PlansTestsUtils))]
        public void TestAddPlan_ValidInput_ReturnsTrue(int userid, string deadline, bool[] days, string time, int pagerPerDay, string title, string author, int pgcount, int size)
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);

            Plans Plans = new Plans(database);
            User? usr = users.FindUser(userid);
            Assert.True(
            Plans.AddPlan(usr, deadline, Weekdays.ToBitField(days), time, pagerPerDay, title, author, pgcount, size));
        }


        [Theory]
        [MemberData(nameof(PlansTestsUtils.GetInvalidTestPlansFromPopulatedDb), MemberType = typeof(PlansTestsUtils))]
        public void TestAddPlan_InvalidInput_ThrowsException(int userid, string deadline, bool[] days, string time, int pagerPerDay, string title, string author, int pgcount, int size)
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);

            Plans Plans = new Plans(database);
            User? usr = users.FindUser(userid);

            var exception = Record.Exception(() => Plans.AddPlan(usr, deadline, Weekdays.ToBitField(days), time, pagerPerDay, title, author, pgcount, size));
            Assert.NotNull(exception);
        }

        [Theory]
        [MemberData(nameof(PlansTestsUtils.GetTestPlansFromPopulatedDb), MemberType = typeof(PlansTestsUtils))]
        public void TestFindPlan_ExistingPlanID_ReturnsCorrectBookPlan(int userid, string deadline, bool[] days, string time, int pagesPerDay, string title, string author, int pgcount, int size)
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);

            Plans Plans = new Plans(database);
            int i = 1;
            User? usr = users.FindUser(userid);
            Plans.AddPlan(usr, deadline, Weekdays.ToBitField(days), time, pagesPerDay, title, author, pgcount, size);
            BookPlan? foundPlan = Plans.FindPlan(i);

            Assert.NotNull(foundPlan);
            Assert.Equal(i, foundPlan.Id);
            Assert.Equal(userid, foundPlan.UserId);
            Assert.Equal(deadline, foundPlan.DeadLine);
            Assert.Equal(Weekdays.ToBitField(days), foundPlan.DayOfWeek);
            Assert.Equal(time, foundPlan.timeOfDay);
            Assert.Equal(pagesPerDay, foundPlan.PagesPerDay);
            Assert.Equal(title, foundPlan.Title);
            Assert.Equal(author, foundPlan.Author);
            Assert.Equal(pgcount, foundPlan.PageCount);
            Assert.Equal(size, foundPlan.Size);
            i++;
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(100)]

        public void TestFindPlan_NonExistentPlanID_ReturnsNull(int id)
        {
            Plans plans = PlansTestsUtils.CreatePopulated();
            Assert.Null(plans.FindPlan(id));
        }

        [Fact]
        public void TestFindPlanByUser_ExistingUserID_ReturnsCorrectIDList()
        {
            Database database = TestUtils.CreateDatabase();
            Plans Plans = PlansTestsUtils.CreatePopulated(database);
            List<int> planIDs = new List<int>();


            var expectedIDs = new List<int>() { 1 };
            planIDs = Plans.FindPlanByUser(3);
            Assert.Equal(expectedIDs, planIDs);

            var expectedIDs2 = new List<int>() { 3, 5, 6, 10 };
            List<int> planIDs2 = Plans.FindPlanByUser(2);
            Assert.Equal(expectedIDs2, planIDs2);

            var expectedIDs3 = new List<int>() { 2, 9 };
            List<int> planIDs3 = Plans.FindPlanByUser(5);
            Assert.Equal(expectedIDs3, planIDs3);

            var expectedIDs4 = new List<int>() { 4, 7, 8, 11 };
            List<int> planIDs4 = Plans.FindPlanByUser(6);
            Assert.Equal(expectedIDs4, planIDs4);

        }

        [Fact]
        public void TestFindPlanByUser_UserWithoutPlans_ReturnsNull()
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);
            Plans Plans = new Plans(database);

            List<int> planIDs = new List<int>();
            for (int i = 10; i < 15; i++)
            {
                planIDs = Plans.FindPlanByUser(i);
                Assert.Null(planIDs);
            }
        }


        [Fact]
        public void TestDeletePlan_ExistingPlanID_ReturnsTrue()
        {
            Plans Plans = PlansTestsUtils.CreatePopulated();

            for (int i = 0; i < 10; i++)
            {
                Assert.True(Plans.DeletePlan(i + 1));
                BookPlan? deleted = Plans.FindPlan(i + 1);
                Assert.True(deleted == null);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(100)]

        public void TestDeletePlan_NonExistentPlanID_ReturnsFalse(int id)
        {
            Plans Plans = PlansTestsUtils.CreatePopulated();
            var exception = Record.Exception(() => Plans.DeletePlan(id));
            Assert.NotNull(exception);
        }

        [Theory]
        [MemberData(nameof(PlansTestsUtils.GetTestPlans2FromPopulatedDb), MemberType = typeof(PlansTestsUtils))]
        public void TestUpdatePlan_ValidInputs_ReturnsTrue(int userid, string deadline, bool[] days, string time, int pagesPerDay, string title, string author, int pgcount, int size)
        {
            Plans Plans = PlansTestsUtils.CreatePopulated();
            Assert.True(Plans.UpdatePlan(1, deadline, Weekdays.ToBitField(days), time, title, author, pgcount, size));
        }

        [Theory]
        [MemberData(nameof(PlansTestsUtils.GetInvalidTestPlansFromPopulatedDb), MemberType = typeof(PlansTestsUtils))]
        public void TestUpdatePlan_InvalidInputs_ThrowsException(int userid, string deadline, bool[] days, string time, int pagesPerDay, string title, string author, int pgcount, int size)
        {
            Plans Plans = PlansTestsUtils.CreatePopulated();
            var exception = Record.Exception(() => Plans.UpdatePlan(1, deadline, Weekdays.ToBitField(days), time, title, author, pgcount, size));
            Assert.NotNull(exception);

        }

        [Fact]
        public void Test_UpdatePagesRead_NumberIsNegative_ThrowsException()
        {
            var ID = 1;
            var PAGES_READ = -1;

            Plans plans = PlansTestsUtils.CreatePopulated();

            Assert.Throws<ArgumentException>(() =>
            {
                plans.UpdatePagesRead(ID, PAGES_READ);
            });
        }

        [Theory]
        [InlineData(1, 50)]
        [InlineData(2, 78)]
        [InlineData(2, 0)]
        public void TestUpdatePagesRead(int id, int pagesRead)
        {
            Plans plans = PlansTestsUtils.CreatePopulated();

            plans.UpdatePagesRead(id, pagesRead);

            BookPlan? plan = plans.FindPlan(id);

            Assert.NotNull(plan);
            Assert.Equal(pagesRead, plan.PagesRead);
        }

        [Fact]
        public void TestPageCountPerDayUsingPlanBook()
        {
            Plans Plans = PlansTestsUtils.CreatePopulated();
            for(int i = 1; i < 12; i++)
            {
                BookPlan? bookPlan = Plans.FindPlan(i);
                int pagesPerDay = bookPlan.PagesPerDay;
                bookPlan.PagesToReadBeforeDeadline(new DateTime(2024, 4, 8, 20, 51, 14));
                Assert.Equal(pagesPerDay, bookPlan.PagesPerDay);
            }        
        }
    }
}
