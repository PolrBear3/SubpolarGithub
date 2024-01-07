using UnityEngine;

[CreateAssetMenu]
public class BonusSpawner : ScriptableObject
{
    [System.Serializable]
    public class Bonus
    {
        public float probability;
        public GameObject prefab;
    }

    public Bonus[] Bonuses;

    public Bonus GenerateRandomBonus()
    {
        float r = Random.Range(0f, 100f);
        float total = 0f;

        foreach (var bonus in Bonuses)
        {
            total += bonus.probability;
            if (r <= total)
                return bonus;
        }

        return null;
    }

    public GameObject SpawnBonus(Vector3 where)
    {
        Bonus bonus = GenerateRandomBonus();
        GameObject result = null;

        if (bonus != null)
        {
            result = Instantiate(bonus.prefab, where, Quaternion.identity);
        }

        return result;
    }
}
