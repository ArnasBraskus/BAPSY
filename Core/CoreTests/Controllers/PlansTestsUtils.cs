public class PlansTestsUtils
{
    private static readonly (int, string, bool[], string, int, string, string, int)[] TestPlans1 = new (int, string, bool[], string, int, string, string, int)[]
       {
        (3, "6-11-2024", [true, true, false, false, false, false, false], "1:28 PM", 3, "Touchy Feely", "Arlen Anmore", 45),
        (5, "9/2/2023", [true, false, true, false, true, false, true], "3:31 PM", -1, "Mantrap", "Devin Santos", 6),
        (2, "6/8/2023", [true, false, true, false, true, false, true], "4:56 PM", -1, "Cherish", "Blisse Wipfler", 99),
        (6, "11/3/2023", [true, false, true, false, true, false, true], "2:52 PM", -1, "For Pete's Sake", "Faith Cregeen", 78),
        (2, "8/28/2023", [true, false, true, false, true, false, true], "9:38 PM", -1, "The Story of Asya Klyachina", "Celisse Grossier", 21),
        (2, "5/26/2024", [false, false, false, false, false, false, true], "7:55 AM", 10, "Caddyshack II", "Stearn Jillings", 56),
        (6, "4/25/2024", [true, false, true, false, true, false, true], "8:42 PM", 9, "Bulletproof", "Lucila Raithmill", 90),
        (6, "9/21/2023", [true, false, true, false, true, false, true], "11:17 AM", -1, "A Thousand Times Goodnight", "Adrian Bingall", 17),
        (5, "7/3/2024", [false, false, false, false, false, false, false], "10:24 AM", -1, "À nous la liberté (Freedom for Us)", "Matthus Dohmer", 27),
        (2, "9/11/2024", [true, false, true, false, true, false, true], "1:50 PM", 1, "Punk's Dead: SLC Punk! 2", "Garner Cursons", 23),
        (6, "1/7/2024", [true, false, true, false, true, false, true], "10:06 PM", -1, "Not Another Happy Ending", "De witt Dooher", 3)
       };

    private static readonly (int, string, bool[], string, int, string, string, int)[] InvalidPlans = new (int, string, bool[], string, int, string, string, int)[]
{
        (5, "7/11/2023", [true, false, true, false, true, false, true], "5:49 PM", 3, "Shadows of Silence", "Liliana Harland", -10),
        (9, "8/9/2023", [true, false, true, false, true, false, true], "10:55 AM", 11, "", "Mara MacAlister", 14),
        (6, "7/11/2023", [true, false, true, false, true, false, true], "5:49 PM", 3, "Shadows of Silence", "", 10),
};

    private static readonly (int, string, bool[], string, int, string, string, int)[] TestPlans2 = new (int, string, bool[], string, int, string, string, int)[]
   {
        (7, "3/14/2024", [false, false, false, false, false, false, false], "8:12 AM", 14, "The Midnight Zone", "Jocelyn Doust", 11),
        (3, "12-25-2023", [true, false, true, false, true, false, true], "6:30 PM", 6, "The Last Journey", "Ryland Davison", 8),
        (12, "5/20/2024", [true, false, true, false, true, false, true], "2:17 PM", 12, "Whispers in the Dark", "Emmeline Stancliffe", 4),
        (5, "7/11/2023", [true, false, true, false, true, false, true], "5:49 PM", 3, "Shadows of Silence", "Liliana Harland", 10),
        (9, "8/9/2023", [true, false, true, false, true, false, true], "10:55 AM", 11, "Echoes of Eternity", "Mara MacAlister", 14),
        (10, "2/14/2023", [true, false, true, false, true, false, true], "3:05 AM", 8, "The Forgotten Castle", "Alessia Overfield", 13),
        (11, "10/3/2023", [true, false, true, false, true, false, true], "11:11 PM", 9, "The Secret Garden", "Alysia Shiel", 15),
        (8, "4/1/2023", [true, false, true, false, true, false, true], "9:23 AM", 5, "Dancing with Shadows", "Frederick Haylock", 7),
        (14, "11/22/2023", [true, false, true, false, true, false, true], "7:37 PM", 13, "Whispers of the Past", "Eloise Maddox", 6),
        (13, "6/15/2023", [true, false, true, false, true, false, true], "12:45 PM", 7, "The Enchanted Forest", "Savannah Northcote", 9),
   };

    private static readonly (string, int, string, bool[], string, int, string, string, int)[] TestIds = new (string, int, string, bool[], string, int, string, string, int)[]
    {
        ("bducker0@ehow.com", 1, "3/14/2024", [false, false, false, false, false, false, false], "8:12 AM", 14, "The Midnight Zone", "Jocelyn Doust", 11),
        ("mwrinch1@umn.edu",2, "12-25-2023", [true, false, true, false, true, false, true], "6:30 PM", 6, "The Last Journey", "Ryland Davison", 8),
        ("smcmeanma2@time.com",3, "5/20/2024", [true, false, true, false, true, false, true], "2:17 PM", 12, "Whispers in the Dark", "Emmeline Stancliffe", 4),
        ("btoffano3@mapy.cz",4, "7/11/2023", [true, false, true, false, true, false, true], "5:49 PM", 3, "Shadows of Silence", "Liliana Harland", 10)
    };

    public static Plans CreateEmpty()
    {
        Database database = TestUtils.CreateDatabase();

        Users users = UserTestsUtils.CreateEmpty(database);

        return new Plans(database);
    }

    public static Plans CreatePopulated(Database database, Users users)
    {
        Plans Plans = new Plans(database);

        foreach (var plan in TestPlans1)
        {
            User? usr = users.FindUser(plan.Item1);
            Plans.AddPlan(usr, plan.Item2, Weekdays.ToBitField(plan.Item3), plan.Item4, plan.Item5, plan.Item6, plan.Item7, plan.Item8);
        }
        return Plans;
    }

    public static Plans CreatePopulated(Database database)
    {
        return CreatePopulated(database, UserTestsUtils.CreatePopulated(database));
    }

    public static Plans CreatePopulated()
    {
        return CreatePopulated(TestUtils.CreateDatabase());
    }

    public static IEnumerable<object[]> GetTestPlansFromPopulatedDb()
    {
        foreach (var plan in TestPlans1)
        {
            yield return new object[] { plan.Item1, plan.Item2, plan.Item3, plan.Item4, plan.Item5, plan.Item6, plan.Item7, plan.Item8 };
        }
    }

    public static IEnumerable<object[]> GetTestPlans2FromPopulatedDb()
    {
        foreach (var plan in TestPlans2)
        {
            yield return new object[] { plan.Item1, plan.Item2, plan.Item3, plan.Item4, plan.Item5, plan.Item6, plan.Item7, plan.Item8 };
        }
    }

    public static IEnumerable<object[]> GetInvalidTestPlansFromPopulatedDb()
    {
        foreach (var plan in InvalidPlans)
        {
            yield return new object[] { plan.Item1, plan.Item2, plan.Item3, plan.Item4, plan.Item5, plan.Item6, plan.Item7, plan.Item8 };
        }
    }

    public static IEnumerable<object[]> GetTestIdsFromPopulatedDb()
    {
        foreach (var u in TestIds)
        {
            yield return new object[] { u.Item1, u.Item2, u.Item3, u.Item4, u.Item5, u.Item6, u.Item7, u.Item8, u.Item9 };
        }
    }
}
