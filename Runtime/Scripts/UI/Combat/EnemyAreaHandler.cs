using System.Collections.Generic;
using UnityEngine;

public class EnemyAreaHandler : MonoBehaviour, IViewUpdater
{
    public GameObject enemyPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateView() {
        // TODO: Need to support enemies that are added later on without repositioning existing enemies
        float myHeight = GetComponent<RectTransform>().rect.height;
        float x = 0;
        List<GameObject> createdEnemies = new List<GameObject>();
        this.updateChildrenViews<EnemyAreaHandler, EnemyHandler, Enemy>(
            CombatHandler.instance?.getEnemies() ?? new List<Enemy>(),
            (Enemy enemy) => {
                GameObject enemyObject = Instantiate(enemyPrefab);
                enemyObject.GetComponent<EnemyHandler>().SetEnemy(enemy);
                float objectWidth = myHeight * enemyObject.GetComponent<EnemyHandler>().GetAspectRatio();
                enemyObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x + objectWidth / 2, 0);
                x += objectWidth + 20;
                createdEnemies.Add(enemyObject);
                return enemyObject;
            },
            (enemyHandler) => enemyHandler.GetEnemy() ?? new Enemy()
        );
        foreach (GameObject enemyObject in createdEnemies) {
            RectTransform rectTransform = enemyObject.GetComponent<RectTransform>();
            Vector2 currentPosition = rectTransform.anchoredPosition;
            // Center enemies
            rectTransform.anchoredPosition = new Vector2(currentPosition.x - (x / 2), currentPosition.y);
        }
    }
}
