namespace ManagerGame.Test.Domain;

public class EmailTest
{
    [Theory]
    [InlineData("jakob@jakob.se", "jakob@jakob.se", true)]
    [InlineData("jakob@jakob.se", "jakob@jakob.com", false)]
    [InlineData("JAKOB@jakob.se", "jakob@jakob.se", true)]
    [InlineData("JAKOB@jakob.se", "jakob@JAKOB.se", true)]
    [InlineData("jakob@jakob.se", "jakob@JAKOB.se", true)]
    [InlineData("jakob@jakob.SE", "jakob@JAKOB.SE", true)]
    [InlineData("jakob@jakob.SE", "jakob@JAKOB1.SE", false)]
    [InlineData("hej@hej.com", "hej@hej.com", true)]
    [InlineData("hej@hej.co.uk", "hej@hej.co.uk", true)]
    public void TestEquality(string first, string second, bool shouldEqual)
    {
        Assert.Equal(shouldEqual, new Email(first) == new Email(second));
    }

    [Theory]
    [InlineData("")]
    [InlineData("hej")]
    [InlineData("hej.com")]
    [InlineData("hej@")]
    [InlineData("@.com")]
    [InlineData("@.")]
    public void TestInvalidEmail(string email)
    {
        Assert.Throws<ArgumentException>(() => new Email(email));
    }
}
