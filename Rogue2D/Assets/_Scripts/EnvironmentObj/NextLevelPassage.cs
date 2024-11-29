using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelPassage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer text_sprite;


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            text_sprite.enabled = true;
            KeyInputEventManager.interactionEvent.AddListener(StartNextLevel);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            text_sprite.enabled = false;
            KeyInputEventManager.interactionEvent.RemoveListener(StartNextLevel);
        }
    }


    private void StartNextLevel()
    {
        DungeonEventManager.SendStartGenerateDungeon();
    }
}
