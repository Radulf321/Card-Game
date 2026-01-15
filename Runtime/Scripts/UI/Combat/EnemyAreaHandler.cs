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
        this.updateChildrenViews<EnemyAreaHandler, EnemyHandler, Enemy>(
            CombatHandler.instance?.getEnemies() ?? new List<Enemy>(),
            (Enemy enemy) => {
                GameObject enemyObject = Instantiate(enemyPrefab);
                enemyObject.GetComponent<EnemyHandler>().SetEnemy(enemy);
                return enemyObject;
            },
            (enemyHandler) => enemyHandler.GetEnemy() ?? new Enemy()
        );
    }
}
