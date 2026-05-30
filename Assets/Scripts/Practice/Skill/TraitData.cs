using UnityEngine;

public class TraitData
{
    
}

public abstract class Trait
{
    protected CharacterScript _owner;
    public virtual void Equip(CharacterScript owner)
    {
        _owner = owner;
    }
    public virtual void UnEquip()
    {
        _owner = null;
    }
}

// 그냥 데이터 드리븐으로 얘도 조건과 함께 뺄 수 있을지도 모르지만...
// Ex. 조건:효과:양
// 일단 규모가 커지고 그때 중복되는게 많아지는 거 같으면 다듬는 걸로... 트레잇 1,2개 밖에 없고 중복되는 기능도 없으면
// 조합 / 데이터 드리븐을 해봤자 의미가 별로 없는거 같음...
// 또 특성/스킬마다 각자 유니크한 걸 만드려고 하면
// 조합이나 재활용 할 만한 구석이 안나오는 거 같음

public class HealOnKillTrait : Trait
{
    int spHealAmount = 3;
    int hpHealAmount = 5;

    public override void Equip(CharacterScript owner)
    {
        base.Equip(owner);
        _owner.OnKillEvent += HandleKill;
    }
    public override void UnEquip()
    {
        if (_owner != null)
        {
            _owner.OnKillEvent -= HandleKill;
        }
        base.UnEquip();
    }
    public void HandleKill(CharacterScript owner)
    {
        _owner.RecoverHP(hpHealAmount);
        _owner.RecoverSP(spHealAmount);
    }
}