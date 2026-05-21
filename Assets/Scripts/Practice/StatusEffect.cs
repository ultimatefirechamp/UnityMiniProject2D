using UnityEngine;

public abstract class StatusEffect
{
    public string Id { get; private set; }
    public string Name { get; protected set;  }
    public int Duration { get; protected set; }
    public int Stack { get; protected set; }
    public int MaxStack { get; protected set; }
    protected CharacterScript _target;

    public StatusEffect(string id, string name, int duration, int maxStack, CharacterScript target)
    {
        Id = id;
        Name = name;
        Duration = duration;
        MaxStack = maxStack;
        _target = target;
    }
    public virtual void AddStack(int duration)
    {
        Stack++;
        if (Stack > MaxStack)
        {
            Stack = MaxStack;
        }
        Duration = duration;
    }
    public abstract void OnTurnTick();
}

public class BombShuriken : StatusEffect
{
    public int _damage;
    public BombShuriken(int duration, int damage, CharacterScript target) : base("Status_BombShuriken","폭탄수리검",duration, 3,target)
    {
        _damage = damage;
    }
    public override void OnTurnTick()
    {
        Duration--;
        if (Duration <= 0)
        {
            _target.TakeDamage(_damage * Stack);
        }
    }
}
public class Poison : StatusEffect
{
    public Poison(int duration, CharacterScript target) : base("Status_Poison", "독", duration, 99, target) { }
    public override void OnTurnTick()
    {
        _target.TakeDamage(Stack);   
        Duration--;
    }
}
