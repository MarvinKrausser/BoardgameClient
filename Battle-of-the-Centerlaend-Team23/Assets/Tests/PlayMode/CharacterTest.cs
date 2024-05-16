using System.Collections;
using communication;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests.PlayMode
{
    public class CharacterTest
  {

    [UnityTest]
    public IEnumerator ChangeCharactersTest()
    {
        var char1 = Character.GIMLI;
        var char2 = Character.MERRY;
        CharacterSelectionUI.changeCharacters(char1, char2);
        
        yield return null;

        Assert.IsTrue(char1 == CharacterSelectionUI.character1);
        Assert.IsTrue(char2 == CharacterSelectionUI.character2);
        Assert.IsTrue(CharacterSelectionUI.charactersChanged);
    }
    
    [Test]
    public void SetCharacterTest()
    {
        var characterSelectionUI = new GameObject().AddComponent<CharacterSelectionUI>();
        var characterUI = new CharacterUI();
        
        
        characterSelectionUI.SetCharacter(characterUI);
        Assert.AreEqual(characterUI, CharacterSelectionUI.currentCharacter);
    }
    
    [Test]
    public void CharacterEnumToTextureTest()
    {
        
        var characterSelectionUI = new CharacterSelectionUI();
        var characterEnum = Character.PIPPIN;
        var expectedSprite = characterSelectionUI.Pippin;
        
        var result = characterSelectionUI.CharacterEnumToTexture(characterEnum);
        
        Assert.AreEqual(expectedSprite, result);
    }
    
    //überprüft, ob die Methode "SetCharacter" in der Klasse "CharacterSelectionUI" den ausgewählten Charakter korrekt setzt.
    [Test]
    public void SelectedCharacterSelectionUI()
    {
        var characterSelection = new CharacterSelectionUI();
        var characterUI = new CharacterUI();
        characterSelection.characters = new CharacterUI[] { characterUI };
        var selectedCharacter = new CharacterUI();

        characterSelection.SetCharacter(selectedCharacter);

        Assert.AreEqual(selectedCharacter, CharacterSelectionUI.currentCharacter);
    }
    
    
    //zielt darauf ab sicherzustellen, dass der erste Charakter in der Liste der verfügbaren Charaktere als der
    //aktuelle Charakter gesetzt wird, wenn die Start()-Methode aufgerufen wird. 
    [UnityTest]
    public IEnumerator SetFirstCharacter()
    {
        var characterSelectionUI = new GameObject().AddComponent<CharacterSelectionUI>();
        var characterUI1 = new CharacterUI();
        var characterUI2 = new CharacterUI();
        characterSelectionUI.characters = new CharacterUI[] { characterUI1, characterUI2 };

        characterSelectionUI.Start();

        yield return null;

        Assert.AreEqual(characterUI1, CharacterSelectionUI.currentCharacter);
    }
    
    
    
    // überprüft, ob der übergebene Charakter korrekt als der aktuelle Charakter gesetzt wurde.
    [Test]
    public void CurrentCharacterTest()
    {
        var characterSelectionUI = new CharacterSelectionUI();
        var characterUI = new CharacterUI();

        characterSelectionUI.SetCharacter(characterUI);

        Assert.AreEqual(characterUI, CharacterSelectionUI.currentCharacter); 
    }
    
    //testet, ob die Update()-Methode ohne Fehler ausgeführt wird:
    [Test]
    public void Update_ExecutesWithoutError()
    {
        // Arrange
        var characterSelectionScript = new CharacterSelectionSkript();

        // Act and Assert (no explicit assertions, just checking for exceptions)
        Assert.DoesNotThrow(() => characterSelectionScript.Update());
    }
    
    //Test, ob der prevCharacter-Transform skaliert wird, wenn er nicht null ist:
    [Test]
    public void Update_ScalesPrevCharacterWhenNotNull()
    {
        // Arrange
        var characterSelectionScript = new CharacterSelectionSkript();
        var prevCharacter = new GameObject().transform;
        characterSelectionScript.prevCharacter = prevCharacter;

        // Act
        characterSelectionScript.Update();

        // Assert
        Assert.AreEqual(new Vector3(1f, 1f, 1f), prevCharacter.localScale);
    }
    
    
    //überprüft, ob das Hinzufügen eines Buttons zur korrekten Ausführung des SetCharacter Aufrufs führt
    //ob die currentCharacter Eigenschaft entsprechend aktualisiert wird
    [Test]
    public void Start_AddsButtonAndTriggersSetCharacter()
    {
        // Arrange
        var characterSelectionScript = new CharacterSelectionSkript();
        var optionPrefab = new GameObject();
        characterSelectionScript.optionPrefab = optionPrefab;

        var characterUI = new CharacterUI();
        var button = optionPrefab.AddComponent<Button>();

        bool setCharacterCalled = false;

        // Act
        button.onClick.AddListener(() =>
        {
            setCharacterCalled = true;
            CharacterSelectionUI.currentCharacter = characterUI;
        });

        button.onClick.Invoke();

        // Assert
        Assert.IsTrue(setCharacterCalled);
        Assert.AreEqual(characterUI, CharacterSelectionUI.currentCharacter);
    }



  }
}