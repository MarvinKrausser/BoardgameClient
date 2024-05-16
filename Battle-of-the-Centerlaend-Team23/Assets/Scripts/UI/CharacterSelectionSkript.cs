using communication;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Increases the size of the chosen character in the CharacterSelectionScene so it is clear which one was selected.
/// </summary>
public class CharacterSelectionSkript : MonoBehaviour
{

    public GameObject optionPrefab;
    public Transform prevCharacter;
    public Transform selectedCharacter;

    private void Start()
    {
        foreach (CharacterUI c in CharacterSelectionUI.instance.characters)
        {
            GameObject option = Instantiate(optionPrefab, transform);
            Button button = option.GetComponent<Button>();
                
            button.onClick.AddListener(() =>
            {
                CharacterSelectionUI.instance.SetCharacter(c);
                if (selectedCharacter != null)
                {
                    prevCharacter = selectedCharacter;
                }

                selectedCharacter = option.transform;
            });

            Image image = option.GetComponentInChildren<Image>();
            image.sprite = c.icon;
        }
    }

    public void Update()
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.localScale = Vector3.Lerp(selectedCharacter.localScale, new Vector3(1.2f, 1.2f, 1.2f), Time.deltaTime * 10);
        }

        if (prevCharacter != null)
        {
            prevCharacter.localScale = Vector3.Lerp(prevCharacter.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 10);
        }

    }

   
}

