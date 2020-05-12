using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Util : MonoBehaviour
{
    //Coroutine that can be used to execute any code with a delay
    //https://answers.unity.com/questions/796881/c-how-can-i-let-something-happen-after-a-small-del.html
    public static IEnumerator ExecuteAfterTime(float time, Action task)
    {
        Debug.Log($"Waiting for {time} seconds");
        yield return new WaitForSeconds(time);
        Debug.Log("Executing task");
        task();
    }

    public static IEnumerator FadeOutRoutine(Text text, float fadeOutTime)
    {
        Color originalColor = text.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }

        text.enabled = false;
        text.color = originalColor;
    }

    public static WeaponType getRandomWeaponTypeForEnemy(EnemyType type)
    {
        return Constants.enemyTypeToWeaponType[type][UnityEngine.Random.Range(0, Constants.enemyTypeToWeaponType[type].Count)];
    }

    public static bool CheckChance(float chance)
    {
        return chance > UnityEngine.Random.Range(0f, 100f);
    }

}
