using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    AttItem,
    HpItem,
    SpeedItem,
    MoveItem,
    CriItem,
    GoldItem,
    Count
}

public enum ChaMode
{
    Idle,
    Attack,
    Move,
    Death
}

public enum MonsterType
{
    Slime,
    Beholder,
    ChestMonster,
    Boss,
    Count
}

public interface IMonster
{
    void MonsterSetting();
}

public interface ISerch
{
    void Serch();
}

public interface IMove
{
    void Move();
}

public interface IAttack
{
    void Attack();
}

public interface IHitDamage
{
    bool HitDamage(float[] damage_Point, Color a_Color);
}

public interface IDeath
{
    void Death();
}

public interface IRecover
{
    void Recover();
}

public interface IMoneyDrop
{
    void MoneyDrop();
}

public interface ITargetLost
{
    void TargetLost();
}

public class InterFaceData
{
}
