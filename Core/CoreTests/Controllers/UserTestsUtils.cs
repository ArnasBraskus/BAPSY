public class UserTestsUtils
{
    public static readonly (string, string)[] TestUsers1 = new (string, string)[] {
        ("bducker0@ehow.com", "Boyd Ducker"),
        ("mwrinch1@umn.edu", "Mariquilla Wrinch"),
        ("smcmeanma2@time.com",	"Shandra McMeanma"),
        ("btoffano3@mapy.cz", "Bartie Toffano"),
        ("grawlins4@phpbb.com", "Gaelan Rawlins"),
        ("jdominicacci5@icio.us", "Jarrad Dominicacci"),
        ("tbruce6@vk.com", "Truman Bruce"),
        ("kkneel7@usda.gov", "Karita Kneel"),
        ("pwoonton8@t.co", "Patton Woonton"),
        ("skineton9@about.com", "Sofie Kineton")
    };

    public static readonly (string, string)[] TestUsers2 = new (string, string)[] {
        ("bbosnell0@wired.com", "Bernadine Bosnell"),
        ("sshapiro1@time.com", "Shandra Shapiro"),
        ("sheinert2@apache.org", "Shell Heinert"),
        ("gmackeeg3@spotify.com", "Giffy MacKeeg"),
        ("shaddleston4@pbs.org", "Stefano Haddleston"),
        ("wsarvar5@taobao.com", "Willa Sarvar"),
        ("mhadaway6@e-recht24.de", "Mic Hadaway"),
        ("tbrownfield7@flickr.com", "Tobiah Brownfield"),
        ("cblacker8@cpanel.net", "Clevey Blacker"),
        ("citzchaky9@alibaba.com", "Cesya Itzchaky")
    };

    public static readonly int FirstUserId = 1;

    public static string GetFirstUserEmail() {
        return TestUsers1.First().Item1;
    }

    public static string GetFirstUserName() {
        return TestUsers1.First().Item2;
    }

    public static string GetUserEmail(int id)
    {
        return TestUsers1[id - 1].Item1;
    }

    public static Users CreateEmpty() {
        return new Users(TestUtils.CreateDatabase());
    }

    public static Users CreateEmpty(Database database) {
        return new Users(database);
    }


    public static void Fill(Users users) {
        foreach (var user in TestUsers1) {
            users.AddUser(user.Item1, user.Item2);
        }
    }

    public static Users CreatePopulated() {
        Users users = CreateEmpty();

        Fill(users);

        return users;
    }

    public static Users CreatePopulated(Database database) {
        Users users = new Users(database);

        Fill(users);

        return users;
    }

    public static IEnumerable<object[]> GetTestUsersFromPopulatedDb() {
        foreach (var user in TestUsers1) {
            yield return new object[] { user.Item1, user.Item2 };
        }
    }

    public static IEnumerable<object[]> GetTestUsersWithIdsFromPopulatedDb() {
        int id = FirstUserId;

        foreach (var user in TestUsers1) {
            yield return new object[] { user.Item1, user.Item2, id++ };
        }
    }

    public static IEnumerable<object[]> GetTestUsers2WithIds() {
        int id = FirstUserId;

        foreach (var user in TestUsers2) {
            yield return new object[] { id++, user.Item2 };
        }
    }

    public static IEnumerable<object[]> GetTestUsers1Emails() {
        foreach (var user in TestUsers1) {
            yield return new object[] { user.Item1 };
        }
    }

    public static IEnumerable<object[]> GetTestUsers2Emails() {
        foreach (var user in TestUsers2) {
            yield return new object[] { user.Item1 };
        }
    }
}
