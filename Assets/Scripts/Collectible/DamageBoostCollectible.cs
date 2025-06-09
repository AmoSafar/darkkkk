using UnityEngine;
using System.Collections;

public class DamageBoostCollectible : MonoBehaviour
{
    [SerializeField] private float boostDuration = 5f;
    [SerializeField] private int damageIncreaseAmount = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        bool affected = false;

        MeleeAttack melee = collision.GetComponent<MeleeAttack>();
        if (melee != null)
        {
            StartCoroutine(BoostMeleeDamage(melee));
            affected = true;
        }

        FirstPlayerAttack ranged = collision.GetComponent<FirstPlayerAttack>();
        if (ranged != null)
        {
            StartCoroutine(BoostRangedDamage(ranged));
            affected = true;
        }

        if (affected)
            gameObject.SetActive(false);
    }

    private IEnumerator BoostMeleeDamage(MeleeAttack melee)
    {
        int original = melee.AttackDamage;
        melee.AttackDamage += damageIncreaseAmount;
        yield return new WaitForSeconds(boostDuration);
        melee.AttackDamage = original;
    }

    private IEnumerator BoostRangedDamage(FirstPlayerAttack attack)
    {
        int original = attack.arrowDamage;
        attack.arrowDamage += damageIncreaseAmount;
        yield return new WaitForSeconds(boostDuration);
        attack.arrowDamage = original;
    }
}
