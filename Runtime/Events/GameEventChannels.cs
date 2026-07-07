using UnityEngine;

namespace CatAnnaDev.Events
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Events/Int Game Event", fileName = "IntGameEvent")]
    public sealed class IntGameEvent : GameEvent<int>
    {
    }

    [CreateAssetMenu(menuName = "CatAnnaDev/Events/Float Game Event", fileName = "FloatGameEvent")]
    public sealed class FloatGameEvent : GameEvent<float>
    {
    }

    [CreateAssetMenu(menuName = "CatAnnaDev/Events/Bool Game Event", fileName = "BoolGameEvent")]
    public sealed class BoolGameEvent : GameEvent<bool>
    {
    }

    [CreateAssetMenu(menuName = "CatAnnaDev/Events/String Game Event", fileName = "StringGameEvent")]
    public sealed class StringGameEvent : GameEvent<string>
    {
    }

    [CreateAssetMenu(menuName = "CatAnnaDev/Events/Vector3 Game Event", fileName = "Vector3GameEvent")]
    public sealed class Vector3GameEvent : GameEvent<Vector3>
    {
    }

    [CreateAssetMenu(menuName = "CatAnnaDev/Events/GameObject Game Event", fileName = "GameObjectGameEvent")]
    public sealed class GameObjectGameEvent : GameEvent<GameObject>
    {
    }
}
