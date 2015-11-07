using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EasyOn.Utilities.Test {
    [TestClass]
    public class ObjectUtilitiesTests {

        [TestMethod]
        public void CopyProperties_WithCompatibleObjects_ShouldHaveEqualPropertyValues() {
            // Arrange
            TestClass object1 = new TestClass {
                Property1 = 1,
                Property2 = "abc",
                Property4 = false
            };

            TestClass object2 = new TestClass();

            // Act
            object1.CopyProperties(object2);

            // Assert
            Assert.AreEqual(object1.Property1, object1.Property1);
            Assert.AreEqual(object1.Property2, object2.Property2);
        }
    }


    class TestClass {
        public int Property1 { get; set; }
        public String Property2 { get; set; }
        public Boolean Property3 { get { return false; } }
        public Object Property4 { set { } }
    }
}
