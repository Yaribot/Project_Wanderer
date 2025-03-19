using UnityEngine;

public interface ICommand
{
    public void Execute()
    {
        Debug.Log("Command Executed " + GetType().Name);
    }

    public static ICommand Null { get; } = new NullCommand();
    class NullCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("Doing NOTHING");
        }
    }

    public static T Create<T>() where T : ICommand, new()
    {
        return new T();
    }

    public class SpellCommand : ICommand { }
    public class ItemCommand : ICommand { }
}
