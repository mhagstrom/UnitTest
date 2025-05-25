using NUnit.Framework;
using pjtUsernameChecker;

namespace TestUsernameChecker
{
    /*
     * NUGet's NUnit is the Unit Testing feature that JetBrains Rider uses,
     * it serves the same purpose and has the same features as Microsoft's Debug.Assert,
     * just with a slightly different syntax
     */
    
    public class Tests
    {
        private MainForm _mainForm;

        [SetUp]
        public void Setup()
        {
            _mainForm = new MainForm();
        }

        [Test]
        [TestCase("")]
        public void IsValidUsername_NothingTyped(string username)
        {
            //Access the method from the other project
            var method = typeof(MainForm).GetMethod("IsValidUsername", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)method.Invoke(_mainForm, new object[] { username });
            
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("no")]  //Too short
        [TestCase("violainviewahumblevaudevillianveteran")]  //Too long
        public void IsValidUsername_LengthProblem(string username)
        {
            var method = typeof(MainForm).GetMethod("IsValidUsername", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)method.Invoke(_mainForm, new object[] { username });
            
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("yep")]  //Minimum length
        [TestCase("L33tSp34k1sntC00l")]  //Alphanumeric
        [TestCase("under_score")]  //With underscore
        [TestCase("Kamakaleiimalamalamaiaik")]  //Maximum length (24 chars)
        public void IsValidUsername_GoodName(string username)
        {
            var method = typeof(MainForm).GetMethod("IsValidUsername", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)method.Invoke(_mainForm, new object[] { username });
            
            Assert.That(result, Is.True);
        }

        [Test] //Contains special characters
        [TestCase("not@your.email")]  //Contains @ and .
        [TestCase("Full Name")]  //Contains space
        [TestCase("VR-PC")]  //Contains hyphen
        [TestCase("local.net")]  //Contains period
        public void IsValidUsername_BadCharacters(string username)
        {
            var method = typeof(MainForm).GetMethod("IsValidUsername", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)method.Invoke(_mainForm, new object[] { username });
            
            Assert.That(result, Is.False);
        }

        [TearDown]
        public void Cleanup()
        {
            _mainForm.Dispose();
        }
    }
}