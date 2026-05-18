using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HPBarGroupScript : MonoBehaviour
{
    List<CharacterScript> _characterList;
    [SerializeField] GameObject _sliderPrefab;
    Dictionary<CharacterScript, HPSlideScript> _hpBarDic;

    private void Awake()
    {
        _hpBarDic = new Dictionary<CharacterScript, HPSlideScript>();
    }

    public void RegisterCharacter(CharacterScript character)
    {
        if(character == null)
        {
            return;
        }
        GameObject createdSlide = Instantiate(_sliderPrefab, this.transform);
        createdSlide.name = $"HPBar";
        HPSlideScript slider = createdSlide.GetComponent<HPSlideScript>();
        _hpBarDic[character] = slider;
        character.OnMove += slider.MoveSliderPos;
        character.OnDamaged += slider.SetHpSlideRatio;
        slider.MoveSliderPos(character.transform);
        slider.SetHpSlideRatio(character.MaxHp, character.Hp);
    }
    public void UnRegisterCharacter(CharacterScript character)
    {
        if( _hpBarDic.ContainsKey(character))
        {
            character.OnMove -= _hpBarDic[character].MoveSliderPos;
            character.OnDamaged -= _hpBarDic[character].SetHpSlideRatio;
            Destroy(_hpBarDic[character].gameObject);
            _hpBarDic.Remove(character);
        }
    }
    
}