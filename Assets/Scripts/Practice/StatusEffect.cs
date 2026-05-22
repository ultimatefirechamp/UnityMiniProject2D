using JetBrains.Annotations;
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
    public virtual void OnApply() { }
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
    public virtual int ModifyDamage(int originalDamage, CharacterScript attacker = null) { return originalDamage; }
    public virtual void OnRemove() { }
}

// 상태이상 좀 더 가까운...

// 스탯에 영향을 주는 유형
// 특정 조건을 만족하면 효과를 유발하는 유형

// ex. 10스택이 쌓이면 터지는 뭔가. -> Stack형이라는 유형을 만들어요
// 컨디션 배열이 있고. 그 배열이 Condition이라는 테이블 참조
// Condition이 Stack형이면 10레벨 됐을 때 빵 터진다.


// 다 깎이면 발동하는 뭔가가.

// triggerChecker -> 보유한 객체가 다른 상태가 될때

// ex. 공격을 할 때. 다른 상태가 될 때. 이런 조건들을 만족 시키는 무언가.
// 보유한 자가 어떤 애들에게 피해를 입힐 때.

// 상태이상이.. 이제 효과를 여러개로 모듈화 시켜서
// 상태이상은 그 효과들의 조합으로 생각하는게 어떠한가?

// 상태이상도 상속처럼 되어있고. 컴포넌트화도 가능하다
// 모듈화가 필요하다.

public class BombShuriken : StatusEffect
{
    public int _damage;
    // 일단 지금 하드코딩으로 다 때려박고 있는데... 이거 나중에 데이터 드리븐으로 처리하는 방식으로 할 수는 없나?
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
public class Invincible : StatusEffect
{
    public Invincible(int duration, CharacterScript target) : base("Status_Invincible", "무적", duration, 1, target) { }
    public override void OnTurnTick()
    {
        Duration--;
    }
    void InvalidAttack(AttackData attackData)
    {
        attackData.Damage = 0;
    }
    public override void OnApply()
    {
        _target.OnDamageStep += InvalidAttack;
    }
    public override void OnRemove()
    {
        _target.OnDamageStep -= InvalidAttack;
    }
    public override int ModifyDamage(int originalDamage, CharacterScript attacker = null)
    {
        return 0;
    }
    
}

public class BlockingMode : StatusEffect
{
    public BlockingMode(int duration, CharacterScript target) : base("Status_BlockingMode","방어태세",duration,1,target) { }
    public override void OnApply()
    {
        _target.OnBeforeDamage += TryEvade;
    }
    public override void OnTurnTick()
    {
        Duration--;
    }
    public void TryEvade(AttackData data)
    {
        if(data.Attacker == null)
        {
            // 상태이상이나 기타 공격자가 없는 데미지로는 피할 수 없음. Invincible은 아예 데미지 자체를 0으로 만듬.
            return;
        }
        if(UnityEngine.Random.value <= 0.5f)
        {
            data.IsEvaded = true;
        }
    }
    public override void OnRemove()
    {
        if(_target != null)
        {
            _target.OnBeforeDamage -= TryEvade;
        }
    }
}
 