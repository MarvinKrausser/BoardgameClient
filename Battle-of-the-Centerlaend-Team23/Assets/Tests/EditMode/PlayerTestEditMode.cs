using communication;
using entities;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class PlayerTestEditMode
    {
        [Test]
        public void Initialize_WithNameAndRole_SetsNameAndRole()
        {
            // Arrange
            GameObject playerObject = new GameObject();
            Player player = playerObject.AddComponent<Player>();

            string name = "TestPlayer";
            Role role = Role.PLAYER; // replace SomeRole with a valid Role
            // Act
            player.Initialize(name, role);

            // Assert
            Assert.AreEqual(name, player.name);
            Assert.AreEqual(role, player.role);
        }
    }
}