namespace CoreTests;

public class UsersTests
{
    static readonly (string, string)[] TestUsers1 = new (string, string)[] {
        ("bducker0@ehow.com", "Boyd Ducker"),
        ("mwrinch1@umn.edu", "Mariquilla Wrinch"),
        ("smcmeanma2@time.com",	"Shandra McMeanma"),
        ("btoffano3@mapy.cz", "Bartie Toffano"),
        ("grawlins4@phpbb.com", "Gaelan Rawlins"),
        ("jdominicacci5@icio.us", "Jarrad Dominicacci"),
        ("tbruce6@vk.com", "Truman Bruce"),
        ("kkneel7@usda.gov", "Karita Kneel"),
        ("pwoonton8@t.co", "Patton Woonton"),
        ("skineton9@about.com", "Sofie Kineton"),
        ("lcaddya@shop-pro.jp", "Lazaro Caddy"),
        ("emeanyb@123-reg.co.uk", "Ev Meany"),
        ("dwixc@netscape.com", "Denis Wix"),
        ("efrained@reuters.com", "Eimile Fraine"),
        ("rtrebbette@uol.com.br", "Redd Trebbett"),
        ("dgrossierf@npr.org", "Douglas Grossier"),
        ("ucollisong@google.cn", "Ulrike Collison"),
        ("aodbyh@google.nl", "Alaster Odby"),
        ("jcomberbeachi@hatena.ne.jp", "Justen Comberbeach"),
        ("rtrevaskisj@si.edu", "Rianon Trevaskis")
    };

    static readonly (string, string)[] TestUsers2 = new (string, string)[] {
        ("bbosnell0@wired.com", "Bernadine Bosnell"),
        ("sshapiro1@time.com", "Shandra Shapiro"),
        ("sheinert2@apache.org", "Shell Heinert"),
        ("gmackeeg3@spotify.com", "Giffy MacKeeg"),
        ("shaddleston4@pbs.org", "Stefano Haddleston"),
        ("wsarvar5@taobao.com", "Willa Sarvar"),
        ("mhadaway6@e-recht24.de", "Mic Hadaway"),
        ("tbrownfield7@flickr.com", "Tobiah Brownfield"),
        ("cblacker8@cpanel.net", "Clevey Blacker"),
        ("citzchaky9@alibaba.com", "Cesya Itzchaky"),
        ("tchasmora@google.cn", "Travus Chasmor"),
        ("bsarginsonb@cafepress.com", "Bernadina Sarginson"),
        ("ksanbrookc@indiegogo.com", "Kipp Sanbrook"),
        ("dbullivantd@ft.com", "Davon Bullivant"),
        ("fcolemane@google.nl", "Forrester Coleman"),
        ("alaverenzf@scientificamerican.com", "Artemas Laverenz"),
        ("fgappg@state.gov", "Florie Gapp"),
        ("dstocktonh@addtoany.com", "Damaris Stockton"),
        ("mchaplaini@noaa.gov", "Merrill Chaplain"),
        ("aheeleyj@google.es", "Ashley Heeley")
    };

    public static Users CreateEmpty() {
        Database database = new Database();

        database.Open();
        database.Create(DatabaseSchema.Schema);

        return new Users(database);
    }

    public static Users CreatePopulated() {
        Users users = CreateEmpty();

        foreach (var user in TestUsers1) {
            users.AddUser(user.Item1, user.Item2);
        }

        return users;
    }

    [Fact]
    public void TestEmptyDbUserExists() {
        Users users = CreateEmpty();

        foreach (var user in TestUsers1) {
            Assert.False(users.UserExists(user.Item1));
        }

        Assert.False(users.UserExists(" "));
        Assert.False(users.UserExists("*"));
        Assert.False(users.UserExists("?"));
        Assert.False(users.UserExists(""));
    }

    [Fact]
    public void TestEmptyDbAddUserEmptyData() {
        Users users = CreateEmpty();

        Assert.False(users.AddUser("", ""));
        Assert.False(users.UserExists(""));
    }

    [Fact]
    public void TestEmptyDbAddUser() {
        Users users = CreateEmpty();

        foreach (var user in TestUsers1) {
            Assert.True(users.AddUser(user.Item1, user.Item2));
        }
    }

    [Fact]
    public void TestEmptyDbAddSameUser() {
        Users users = CreateEmpty();

        var testUser = TestUsers1.First();

        Assert.True(users.AddUser(testUser.Item1, testUser.Item2));
        Assert.False(users.AddUser(testUser.Item1, testUser.Item2));

        Assert.True(users.UserExists(testUser.Item1));
    }

    [Fact]
    public void TestPopulatedDbUserExists() {
        Users users = CreatePopulated();

        foreach (var user in TestUsers1) {
            Assert.True(users.UserExists(user.Item1));
        }

        foreach (var user in TestUsers2) {
            Assert.False(users.UserExists(user.Item1));
        }
    }

    [Fact]
    public void TestPopulatedDbFindNonExistingUsers() {
        Users users = CreatePopulated();

        foreach (var testUser in TestUsers2) {
            User? user = users.FindUser(testUser.Item1);

            Assert.True(user == null);
        }

        Assert.False(users.UserExists("*"));
        Assert.False(users.UserExists("?"));
        Assert.False(users.UserExists(""));
    }

    [Fact]
    public void TestPopulatedDbFindExistingUsers() {
        Users users = CreatePopulated();

        foreach (var testUser in TestUsers1) {
            User? user = users.FindUser(testUser.Item1);

            Assert.True(user != null);
            Assert.Equal(testUser.Item1, user.Email);
            Assert.Equal(testUser.Item2, user.Name);
        }
    }

    [Fact]
    public void TestPopulatedDbIdIsUnique() {
        Users users = CreatePopulated();

        List<int> ids = new List<int>();

        foreach (var testUser in TestUsers1) {
            User? user = users.FindUser(testUser.Item1);

            Assert.True(user != null);
            Assert.DoesNotContain(user.Id, ids);

            ids.Add(user.Id);
        }
    }

    [Fact]
    public void TestEmptyDbFindNonExistingUsersById() {
        Users users = CreateEmpty();

        for (int i = -10; i < 10; i++) {
            User? user = users.FindUser(i);

            Assert.True(user == null);
        }
    }

    [Fact]
    public void TestPopulatedDbFindUserById() {
        Users users = CreatePopulated();

        List<int> ids = new List<int>();

        foreach (var testUser in TestUsers1) {
            User? user = users.FindUser(testUser.Item1);

            Assert.True(user != null);

            ids.Add(user.Id);
        }

        Assert.Equal(TestUsers1.Length, ids.Count());

        for (int i = 0; i < ids.Count(); i++) {
            User? user = users.FindUser(ids[i]);

            Assert.True(user != null);

            Assert.Equal(TestUsers1[i].Item1, user.Email);
            Assert.Equal(TestUsers1[i].Item2, user.Name);
        }
    }
}
