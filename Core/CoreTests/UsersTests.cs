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

    [Fact]
    public void Test_EmptyDb_AddUserWithEmptyData_AddingFails() {
        Users users = UserTestsUtils.CreateEmpty();

        var actual = users.AddUser("", "");

        Assert.False(actual);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_EmptyDb_AddUserWithValidData_AddsUser(string email, string name) {
        Users users = UserTestsUtils.CreateEmpty();

        var actual = users.AddUser(email, name);

        Assert.True(actual);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_EmptyDb_AddAndFindUser_UserHasCorrectInfo(string email, string name) {
        Users users = UserTestsUtils.CreateEmpty();

        var status = users.AddUser(email, name);
        User? user = users.FindUser(email);

        Assert.True(status);
        Assert.True(user is not null);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
    }

    [Theory]
    [InlineData("Boyd Ducker")]
    [InlineData("           ")]
    [InlineData("@")]
    public void Test_EmptyDb_AddUserWithInvalidEmail_AddingFails(string email) {
        var NAME = "Boyd Ducker";

        Users users = UserTestsUtils.CreateEmpty();

        var actual = users.AddUser(email, NAME);

        Assert.False(actual);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_AddAlreadyExistingUser_AddingFails(string email, string name) {
        Users users = UserTestsUtils.CreatePopulated();

        var actual = users.AddUser(email, name);

        Assert.False(actual);
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
    public void Test_PopulatedDb_FindNonExistingUser_UserDoesntExist(string email) {
        Users users = UserTestsUtils.CreatePopulated();

        User? user = users.FindUser(email);

        Assert.True(user is null);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_FindExistingUser_UserHasCorrectInfo(string email, string name) {
        Users users = UserTestsUtils.CreatePopulated();

        User? user = users.FindUser(email);

        Assert.True(user is not null);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Test_EmptyDb_FindUserById_UserDoesntExist(int id) {
        Users users = UserTestsUtils.CreateEmpty();

        User? user = users.FindUser(id);

        Assert.True(user == null);
    }

    [Theory]
    [MemberData(nameof(UserTestsUtils.GetTestUsersWithIdsFromPopulatedDb), MemberType = typeof(UserTestsUtils))]
    public void Test_PopulatedDb_FindExistingUserById_FindsUserWithCorrectInfo(string email, string name, int id) {
        Users users = UserTestsUtils.CreatePopulated();

        User? user = users.FindUser(id);

        Assert.True(user is not null);
        Assert.Equal(id, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
    }

    [Fact]
    public void Test_PopulatedDb_UserIdIsUnique_IdIsUnique() {
        Users users = UserTestsUtils.CreatePopulated();

        List<int> ids = new List<int>();

        foreach (var testUser in UserTestsUtils.TestUsers1) {
            User? user = users.FindUser(testUser.Item1);

            Assert.True(user != null);
            Assert.DoesNotContain(user.Id, ids);

            ids.Add(user.Id);
        }
    }
}
