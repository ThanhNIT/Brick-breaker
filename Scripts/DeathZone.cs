using UnityEngine;
using System.Collections.Generic;

public class DeathZone : MonoBehaviour
{
    private HashSet<int> fallenBalls = new HashSet<int>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        int id = other.gameObject.GetInstanceID();
        if (fallenBalls.Contains(id)) return;
        fallenBalls.Add(id);

        Debug.Log("DeathZone hit by: " + other.gameObject.name);
        GameManager.Instance.BallFell();
        Destroy(other.gameObject);
    }
}