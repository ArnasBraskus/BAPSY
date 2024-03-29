﻿using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Interfaces;
using System.Numerics;


namespace CoreTests
{
    public class PlansTests
    {
        private static readonly (int, string, bool[], string, int, string, string, int, int)[] TestPlans = new (int, string, bool[], string, int, string, string, int, int)[]
        {
        (3, "6-11-2024", [true, true, false, false, false, false, false], "1:28 PM", 74, "Touchy Feely", "Arlen Anmore", 45, 4),
        (5, "9/2/2023", [true, false, true, false, true, false, true], "3:31 PM", 59, "Mantrap", "Devin Santos", 6, 1),
        (2, "6/8/2023", [true, false, true, false, true, false, true], "4:56 PM", 44, "Cherish", "Blisse Wipfler", 99, 2),
        (6, "11/3/2023", [true, false, true, false, true, false, true], "2:52 PM", 73, "For Pete's Sake", "Faith Cregeen", 78, 4),
        (2, "8/28/2023", [true, false, true, false, true, false, true], "9:38 PM", 66, "The Story of Asya Klyachina", "Celisse Grossier", 21, 2),
        (2, "3/25/2024", [true, false, true, false, true, false, true], "7:55 AM", 18, "Caddyshack II", "Stearn Jillings", 56, 1),
        (6, "3/5/2024", [true, false, true, false, true, false, true], "8:42 PM", 67, "Bulletproof", "Lucila Raithmill", 90, 4),
        (6, "9/21/2023", [true, false, true, false, true, false, true], "11:17 AM", 39, "A Thousand Times Goodnight", "Adrian Bingall", 17, 4),
        (5, "7/3/2023", [true, false, true, false, true, false, true], "10:24 AM", 70, "À nous la liberté (Freedom for Us)", "Matthus Dohmer", 27, 1),
        (2, "9/11/2023", [true, false, true, false, true, false, true], "1:50 PM", 80, "Punk's Dead: SLC Punk! 2", "Garner Cursons", 23, 4),
        (6, "1/7/2024", [true, false, true, false, true, false, true], "10:06 PM", 10, "Not Another Happy Ending", "De witt Dooher", 3, 1)
        };

        private static readonly (int, string, bool[], string, int, string, string, int, int)[] InvalidPlans = new (int, string, bool[], string, int, string, string, int, int)[]
{
        (5, "7/11/2023", [true, false, true, false, true, false, true], "5:49 PM", 3, "Shadows of Silence", "Liliana Harland", -10, 12),
        (9, "8/9/2023", [true, false, true, false, true, false, true], "10:55 AM", 11, "Echoes of Eternity", "Mara MacAlister", 14, -6),
};

        private static readonly (int, string, bool[], string, int, string, string, int, int)[] TestPlans2 = new (int, string, bool[], string, int, string, string, int, int)[]
       {
        (7, "3/14/2024", [false, false, false, false, false, false, false], "8:12 AM", 14, "The Midnight Zone", "Jocelyn Doust", 11, 9),
        (3, "12-25-2023", [true, false, true, false, true, false, true], "6:30 PM", 6, "The Last Journey", "Ryland Davison", 8, 3),
        (12, "5/20/2024", [true, false, true, false, true, false, true], "2:17 PM", 12, "Whispers in the Dark", "Emmeline Stancliffe", 4, 7),
        (5, "7/11/2023", [true, false, true, false, true, false, true], "5:49 PM", 3, "Shadows of Silence", "Liliana Harland", 10, 12),
        (9, "8/9/2023", [true, false, true, false, true, false, true], "10:55 AM", 11, "Echoes of Eternity", "Mara MacAlister", 14, 6),
        (10, "2/14/2023", [true, false, true, false, true, false, true], "3:05 AM", 8, "The Forgotten Castle", "Alessia Overfield", 13, 8),
        (11, "10/3/2023", [true, false, true, false, true, false, true], "11:11 PM", 9, "The Secret Garden", "Alysia Shiel", 15, 5),
        (8, "4/1/2023", [true, false, true, false, true, false, true], "9:23 AM", 5, "Dancing with Shadows", "Frederick Haylock", 7, 10),
        (14, "11/22/2023", [true, false, true, false, true, false, true], "7:37 PM", 13, "Whispers of the Past", "Eloise Maddox", 6, 11),
        (13, "6/15/2023", [true, false, true, false, true, false, true], "12:45 PM", 7, "The Enchanted Forest", "Savannah Northcote", 9, 13),
       };

        private static readonly (int, string, bool[], string, int, string, string, int, int)[] TestPlans3 = new (int, string, bool[], string, int, string, string, int, int)[]
        {
        (2, "3/26/2024", [true, false, true, false, true, false, true], "7:55 AM", 18, "Caddyshack II", "Stearn Jillings", 56, 1),
        (10, "4/14/2024", [true, false, false, false, false, false, false], "3:05 AM", 8, "The Forgotten Castle", "Alessia Overfield", 100, 8),
        (11, "10/3/2024", [true, false, false, false, false, false, true], "11:11 PM", 9, "The Secret Garden", "Alysia Shiel", 15, 5),
        };

        private Plans CreateEmpty()
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreateEmpty(database);

            return new Plans(database);
        }

        private Plans CreatePopulated()
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);

            Plans Plans = new Plans(database);

            foreach (var plan in TestPlans)
            {
                User? usr = users.FindUser(plan.Item1);
                Plans.AddPlan(usr, plan.Item2, Weekdays.ToBitField(plan.Item3), plan.Item4, plan.Item5, plan.Item6, plan.Item7, plan.Item8, plan.Item9);
            }
            return Plans;
        }

        [Fact]
        public void TestAddPlan_ValidInput_ReturnsTrue()
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);

            Plans Plans = new Plans(database);

            foreach (var plan in TestPlans)
            {
                User? usr = users.FindUser(plan.Item1);
                Assert.True(
                Plans.AddPlan(usr, plan.Item2, Weekdays.ToBitField(plan.Item3), plan.Item4, plan.Item5, plan.Item6, plan.Item7, plan.Item8, plan.Item9));
            }
        }


        [Fact]
        public void TestAddPlan_InvalidInput_ReturnsFalse()
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);

            Plans Plans = new Plans(database);

            foreach (var plan in InvalidPlans)
            {
                User? usr = users.FindUser(plan.Item1);
                Assert.False(
                Plans.AddPlan(usr, plan.Item2, Weekdays.ToBitField(plan.Item3), plan.Item4, plan.Item5, plan.Item6, plan.Item7, plan.Item8, plan.Item9));
            }
        }

        [Fact]
        public void TestFindPlan_ExistingPlanID_ReturnsCorrectBookPlan()
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);

            Plans Plans = new Plans(database);
            int i = 1;
            foreach (var plan in TestPlans)
            {
                User? usr = users.FindUser(plan.Item1);
                Plans.AddPlan(usr, plan.Item2, Weekdays.ToBitField(plan.Item3), plan.Item4, plan.Item5, plan.Item6, plan.Item7, plan.Item8, plan.Item9);
                BookPlan ? foundPlan = Plans.FindPlan(i);

                Assert.NotNull(foundPlan); 
                Assert.Equal(i, foundPlan.Id); 
                Assert.Equal(plan.Item1, foundPlan.UserId); 
                Assert.Equal(plan.Item2, foundPlan.DeadLine);
                Assert.Equal(Weekdays.ToBitField(plan.Item3), foundPlan.DayOfWeek);
                Assert.Equal(plan.Item4, foundPlan.timeOfDay);
                Assert.Equal(plan.Item5, foundPlan.PagesPerDay);
                Assert.Equal(plan.Item6, foundPlan.Title);
                Assert.Equal(plan.Item7, foundPlan.Author);
                Assert.Equal(plan.Item8, foundPlan.PageCount);
                Assert.Equal(plan.Item9, foundPlan.Size);

                i++;
            }
        }

        [Fact]
        public void TestFindPlan_NonExistentPlanID_ReturnsNull()
        {
            Plans plans = CreatePopulated();
            Assert.Null(plans.FindPlan(-1));
            Assert.Null(plans.FindPlan(TestPlans.Length + 5));
            Assert.Null(plans.FindPlan(0));

        }

        [Fact]
        public void TestFindPlanByUser_ExistingUserID_ReturnsCorrectIDList()
        {
            /*
            Users users = UsersTests.CreatePopulated();
            Plans Plans = new Plans(users.DB);

            List<int> planIDs = new List<int>();
           
            var expectedIDs = new List<int>() { 1 };
            planIDs = Plans.FindPlanByUser(3);
           // Assert.Equivalent(planIDs, expectedIDs);

            var expectedIDs2 = new List<int>() { 3, 5, 6, 10};
            planIDs = Plans.FindPlanByUser(2);
            Assert.Equivalent(planIDs, expectedIDs2);

            var expectedIDs3 = new List<int>() { 2, 9 };
            planIDs = Plans.FindPlanByUser(5);
            Assert.Equivalent(planIDs, expectedIDs3);

            var expectedIDs4 = new List<int>() { 4, 7, 8, 11 };
            planIDs = Plans.FindPlanByUser(6);
            Assert.Equivalent(planIDs, expectedIDs4);
            */
        }

        [Fact]
        public void TestFindPlanByUser_UserWithoutPlans_ReturnsNull()
        {
            Database database = TestUtils.CreateDatabase();

            Users users = UserTestsUtils.CreatePopulated(database);
            Plans Plans = new Plans(database);

            List<int> planIDs = new List<int> ();
            for (int i =10; i<15; i++)
            {
            planIDs = Plans.FindPlanByUser(i);
            Assert.Null(planIDs);
            }      
        }


        [Fact]
        public void TestDeletePlan_ExistingPlanID_ReturnsTrue()
        {
            Plans Plans = CreatePopulated();

            for (int i = 0; i < TestPlans.Count(); i++)
            {
                Assert.True(Plans.DeletePlan(i + 1));
                BookPlan? deleted = Plans.FindPlan(i + 1);
                Assert.True(deleted == null);
            }
        }

        [Fact]
        public void TestDeletePlan_NonExistentPlanID_ReturnsFalse()
        {
            Plans Plans = CreatePopulated();
            Assert.False(Plans.DeletePlan(-1));
            Assert.False(Plans.DeletePlan(TestPlans.Length+5));
        }

        [Fact]
        public void TestUpdatePlan_ValidInputs_ReturnsTrue()
        {
            Plans Plans = CreatePopulated();
            int i = 1;
            foreach (var plan in TestPlans2)
            {
                Assert.True(Plans.UpdatePlan(i, plan.Item2, Weekdays.ToBitField(plan.Item3), plan.Item4, plan.Item6, plan.Item7, plan.Item8, plan.Item9));
                i++;
            }

        }

        [Fact]
        public void TestUpdatePlan_InvalidInputs_ReturnsFalse()
        {
            Plans Plans = CreatePopulated();
            int i = 1;
            foreach (var plan in InvalidPlans)
            {
                Assert.False(Plans.UpdatePlan(i, plan.Item2, Weekdays.ToBitField(plan.Item3), plan.Item4, plan.Item6, plan.Item7, plan.Item8, plan.Item9));
                i++;
            }
        }

        [Fact]
        public void TestDbUpdatePlanDate()
        {
            Plans Plans = CreatePopulated();
            int i = 1;
            foreach (var plan in TestPlans2)
            {
                Plans.UpdatePlanDates(i, plan.Item2, Weekdays.ToBitField(plan.Item3), plan.Item4, plan.Item5);
                BookPlan updated = Plans.FindPlan(i);
                Assert.Equal(updated.DeadLine, plan.Item2);
                Assert.Equal(Weekdays.FromBitField(updated.DayOfWeek), plan.Item3);
                Assert.Equal(updated.timeOfDay, plan.Item4);
                Assert.Equal(updated.PagesPerDay, plan.Item5);

                i++;
            }
        }

        [Fact]
        public void TestDbUpdatePlanBook()
        {
            Plans Plans = CreatePopulated();
            int i = 1;
            foreach (var plan in TestPlans2)
            {
                Assert.True(Plans.UpdatePlanBook(i, plan.Item5, plan.Item6, plan.Item7, plan.Item8, plan.Item9));
                BookPlan updated = Plans.FindPlan(i);
                Assert.Equal(updated.PagesPerDay, plan.Item5);
                Assert.Equal(updated.Title, plan.Item6);
                Assert.Equal(updated.Author, plan.Item7);
                Assert.Equal(updated.PageCount, plan.Item8);
                Assert.Equal(updated.Size, plan.Item9);
                i++;
            }
        }

        /*[Fact]
        public void TestPageCountPerDayUsingPlanBook()
        {
            Plans Plans = CreatePopulated();
            int i = 1;
            foreach (var plan in TestPlans)
            {
                Assert.True(Plans.UpdatePlanBook(i, plan.Item5, plan.Item6, plan.Item7, plan.Item8, plan.Item9));
                BookPlan updated = Plans.FindPlan(i);
                Assert.Equal(updated.PagesPerDay, plan.Item5);
                Assert.Equal(updated.Title, plan.Item6);
                Assert.Equal(updated.Author, plan.Item7);
                updated.PagesToReadBeforeDeadline();
                Assert.Equal(updated.PageCount, plan.Item8);
                Assert.Equal(updated.Size, plan.Item9);
                i++;
            }
        }*/
    }
}
