using UnityEngine;

namespace Tests.TestScripts
{
    public interface IAttackable
    {
        void ReceiveAttack(float attackDamage, Vector3 attackDirection, float attackForce);
    }
}