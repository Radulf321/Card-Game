using System.Collections.Generic;
using System.Threading.Tasks;
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
        List<GameObject> createdEnemies = new List<GameObject>();
        List<Enemy> enemies = CombatHandler.instance?.getEnemies() ?? new List<Enemy>();
        this.updateChildrenViews<EnemyAreaHandler, EnemyHandler, Enemy>(
            enemies,
            (Enemy enemy) => {
                GameObject enemyObject = Instantiate(enemyPrefab);
                enemyObject.GetComponent<EnemyHandler>().SetEnemy(enemy);
                createdEnemies.Add(enemyObject);
                return enemyObject;
            },
            (enemyHandler) => enemyHandler.GetEnemy() ?? new Enemy()
        );

        _ = ConfigureEnemyHandlers(createdEnemies);
    }

    private async Task ConfigureEnemyHandlers(List<GameObject> createdEnemies)
    {
        float myHeight = GetComponent<RectTransform>().rect.height;
        float x = 0;
        for (int i = 0; i < createdEnemies.Count; i++)
        {
            EnemyHandler enemyHandler = createdEnemies[i].GetComponent<EnemyHandler>();

            await enemyHandler.SetHeight(myHeight);

            float objectWidth = enemyHandler.GetWidth();
            UnityEngine.Debug.Log("Setting enemy in EnemyAreaHandler: " + myHeight + "x" + objectWidth);
            createdEnemies[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x + objectWidth / 2, 0);
            x += objectWidth + 20;
        }
        foreach (GameObject enemyObject in createdEnemies) {
            RectTransform rectTransform = enemyObject.GetComponent<RectTransform>();
            Vector2 currentPosition = rectTransform.anchoredPosition;
            // Center enemies
            rectTransform.anchoredPosition = new Vector2(currentPosition.x - (x / 2), currentPosition.y);
        }
    }
}
