namespace CoreTests;

public class UsersTests
{
    [Theory]
    [InlineData(" ")]
    [InlineData("*")]
    [InlineData("?")]
    [InlineData("")]
    [MemberData(nameof(UserTestsUtils.GetTestUsers1Emails), MemberType = typeof(UserTestsUtils))]
    public void Test_EmptyDb_CheckUserExistsByEmail_UserDoesntExist(string email) {
        Users users = UserTestsUtils.CreateEmpty();

        var actual = users.UserExists(email);

        Assert.False(actual);
    }

    public static IEnumerable<object[]> GetEmptyDetails() {
        return new List<object[]>
        {
            new object[] {UserTestsUtils.GetFirstUserEmail(), ""},
            new object[] {"", UserTestsUtils.GetFirstUserName()},
            new object[] {"", ""}
        };
    }

    [Theory]
    [MemberData(nameof(GetEmptyDetails))]
    public void Test_EmptyDb_AddUserWithEmptyDetails_ThrowsArgumentException(string email, string name) {
        Users users = UserTestsUtils.CreateEmpty();

        Action action = () => users.AddUser(email, name);

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_EmptyDb_AddAndFindUser_UserHasCorrectInfo(string email, string name) {
        Users users = UserTestsUtils.CreateEmpty();

        users.AddUser(email, name);

        User? actual = users.FindUser(email);

        Assert.NotNull(actual);
        Assert.Equal(email, actual.Email);
        Assert.Equal(name, actual.Name);
    }

    [Theory]
    [InlineData("Boyd Ducker")]
    [InlineData("           ")]
    [InlineData("@")]
    public void Test_EmptyDb_AddUserWithInvalidEmail_ThrowsFormatException(string email) {
        var NAME = "Boyd Ducker";

        Users users = UserTestsUtils.CreateEmpty();

        Action action = () => users.AddUser(email, NAME);

        Assert.Throws<FormatException>(action);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_AddAlreadyExistingUser_ThrowsException(string email, string name) {
        Users users = UserTestsUtils.CreatePopulated();

        var exception = Record.Exception(() => users.AddUser(email, name));

        Assert.NotNull(exception);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsers1Emails), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_ExistingUserExists_UserExists(string email) {
        Users users = UserTestsUtils.CreatePopulated();

        var actual = users.UserExists(email);

        Assert.True(actual);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsers2Emails), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_NonExistingUserExists_UserDoesntExist(string email) {
        Users users = UserTestsUtils.CreatePopulated();

        var actual = users.UserExists(email);

        Assert.False(actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("*")]
    [InlineData(" ")]
    [MemberData(nameof(UserTestsUtils.GetTestUsers2Emails), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_FindNonExistingUser_UserIsNull(string email) {
        Users users = UserTestsUtils.CreatePopulated();

        User? user = users.FindUser(email);

        Assert.Null(user);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_FindExistingUser_UserHasCorrectInfo(string email, string name) {
        Users users = UserTestsUtils.CreatePopulated();

        User? user = users.FindUser(email);

        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Test_EmptyDb_FindUserById_UserIsNull(int id) {
        Users users = UserTestsUtils.CreateEmpty();

        User? user = users.FindUser(id);

        Assert.Null(user);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersWithIdsFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_FindExistingUserById_FindsUserWithCorrectInfo(string email, string name, int id) {
        Users users = UserTestsUtils.CreatePopulated();

        User? user = users.FindUser(id);

        Assert.NotNull(user);
        Assert.Equal(id, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
    }

    [Fact]
    public void Test_EmptyDb_UpdateName_ShouldFail() {
        var ID = UserTestsUtils.FirstUserId;
        var NAME = "New Name";

        Users users = UserTestsUtils.CreateEmpty();

        Action action = () => users.UpdateName(ID, NAME);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Test_PopulatedDb_UpdateNameWhenNewNameIsEmpty_ShouldFail() {
        var ID = UserTestsUtils.FirstUserId;
        var NAME = String.Empty;

        Users users = UserTestsUtils.CreatePopulated();

        Action action = () => users.UpdateName(ID, NAME);

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsers2WithIds), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_UpdateName_NameShouldChange(int id, string newName) {
        Users users = UserTestsUtils.CreatePopulated();

        users.UpdateName(id, newName);

        User? user = users.FindUser(id);

        Assert.NotNull(user);
        Assert.Equal(newName, user.Name);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsers2WithIds), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_UpdateNameWithSetterMethod_NameShouldChange(int id, string newName) {
        Users users = UserTestsUtils.CreatePopulated();

        User? user = users.FindUser(id);

        Assert.NotNull(user);

        user.Name = newName;
        user = users.FindUser(id);

        Assert.NotNull(user);
        Assert.Equal(newName, user.Name);
    }

    [Fact]
    public void Test_PopulatedDb_UserIdIsUnique_IdIsUnique() {
        Users users = UserTestsUtils.CreatePopulated();

        List<int> ids = new List<int>();

        foreach (var testUser in UserTestsUtils.TestUsers1) {
            User? user = users.FindUser(testUser.Item1);

            Assert.NotNull(user);
            Assert.DoesNotContain(user.Id, ids);

            ids.Add(user.Id);
        }
    }
}
