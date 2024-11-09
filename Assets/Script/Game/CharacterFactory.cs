using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    [SerializeField] private Character playerCharacterPrefab;
    [SerializeField] private Character enemyCharacterPrefab;

    private Dictionary<CharacterType, Queue<Character>> disabledCharacters = new Dictionary<CharacterType, Queue<Character>>();
    private List<Character> activeCharacters = new List<Character>();

    public Character Player { get; private set; }
    public List<Character> ActiveCharacters => activeCharacters;

    public Character GetCharacter(CharacterType type)
    {
        Character character = null;

        if (type == CharacterType.Player)
        {
            // Создание игрока только один раз
            if (Player == null)
            {
                Player = InstantiateCharacter(type); // Создаем игрока
                Player.Initialize(); // Инициализируем объект сразу после создания
                activeCharacters.Add(Player);
            }
            return Player;
        }

        // Логика для врагов и других типов персонажей
        if (!disabledCharacters.ContainsKey(type))
        {
            disabledCharacters[type] = new Queue<Character>();
        }

        if (disabledCharacters[type].Count > 0)
        {
            // Получаем врага из пула
            character = disabledCharacters[type].Dequeue();
            character.gameObject.SetActive(true);
        }
        else
        {
            // Создаем новый объект врага, если пул пуст
            character = InstantiateCharacter(type);
            character.Initialize(); // Инициализируем объект после создания
        }

        if (character != null)
        {
            activeCharacters.Add(character);
        }

        return character;
    }

    public void ReturnCharacter(Character character)
    {
        if (character == Player)
        {
            // Если это игрок, не добавляем его в пул и не деактивируем
            return;
        }

        if (!disabledCharacters.ContainsKey(character.CharacterType))
        {
            disabledCharacters[character.CharacterType] = new Queue<Character>();
        }

        // Возвращаем врага в пул
        Queue<Character> characters = disabledCharacters[character.CharacterType];
        characters.Enqueue(character);
        activeCharacters.Remove(character);
        character.gameObject.SetActive(false);
    }

    private Character InstantiateCharacter(CharacterType type)
    {
        Character character = null;

        switch (type)
        {
            case CharacterType.Player:
                character = Instantiate(playerCharacterPrefab);
                Player = character;
                break;
            case CharacterType.DefaultEnemy:
                character = Instantiate(enemyCharacterPrefab);
                break;
            default:
                Debug.LogError("Character type not found: " + type);
                break;
        }

        return character;
    }
}
