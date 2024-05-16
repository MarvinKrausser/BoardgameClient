using UnityEngine;

namespace entities
{
    /// <summary>
    /// ScriptableObject for each character to control their respective animations and weapon sprites. 
    /// </summary>
    [CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/CharacterObject", order = 1)]
    public class CharacterObject : ScriptableObject
    {
        public AnimatorOverrideController overrideController;

        public Sprite up;
        public Sprite down;
        public Sprite left;
        public Sprite right;
        public Sprite weapon;

    }
}